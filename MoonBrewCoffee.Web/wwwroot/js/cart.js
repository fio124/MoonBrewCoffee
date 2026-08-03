(() => {
    const apiUrl = "/api/carrito";
    const currency = new Intl.NumberFormat(document.documentElement.lang || "es-CR", {
        style: "currency",
        currency: "CRC",
        maximumFractionDigits: 0
    });
    let updateTimer;
    const t = (key, fallback) => window.moonbrewI18n?.t(key, fallback) ?? fallback;

    const setCount = count => {
        document.querySelectorAll("[data-cart-count]").forEach(element => {
            element.textContent = count;
            element.hidden = count === 0;
        });
    };

    const showNotice = (message, isError = false) => {
        let notice = document.querySelector("[data-cart-notice]");
        if (!notice) {
            notice = document.createElement("div");
            notice.dataset.cartNotice = "";
            notice.className = "cart-toast";
            notice.setAttribute("role", "status");
            document.body.appendChild(notice);
        }

        notice.classList.toggle("is-error", isError);
        notice.innerHTML = `<i class="bi ${isError ? "bi-exclamation-circle" : "bi-check-circle"}"></i><span></span>`;
        notice.querySelector("span").textContent = message;
        notice.classList.add("show");
        window.setTimeout(() => notice.classList.remove("show"), 2600);
    };

    const request = async (url, options = {}) => {
        const response = await fetch(url, {
            headers: { "Content-Type": "application/json", ...(options.headers || {}) },
            ...options
        });
        const data = await response.json().catch(() => ({}));
        if (!response.ok)
            throw new Error(t("orders.updateError", "No fue posible actualizar el carrito."));
        setCount(data.itemCount || 0);
        return data;
    };

    const refreshSummary = data => {
        const values = {
            "[data-summary-count]": data.itemCount,
            "[data-summary-subtotal]": currency.format(data.subtotal),
            "[data-summary-tax]": currency.format(data.tax),
            "[data-summary-total]": currency.format(data.total)
        };
        Object.entries(values).forEach(([selector, value]) => {
            const element = document.querySelector(selector);
            if (element) element.textContent = value;
        });
        if (data.isEmpty && document.querySelector("[data-cart-page]"))
            window.location.reload();
    };

    document.addEventListener("click", async event => {
        const addButton = event.target.closest("[data-cart-add]");
        if (addButton) {
            addButton.disabled = true;
            try {
                const data = await request(`${apiUrl}/items`, {
                    method: "POST",
                    body: JSON.stringify({
                        itemType: addButton.dataset.itemType,
                        itemId: Number(addButton.dataset.itemId),
                        quantity: 1
                    })
                });
                showNotice(t("orders.added", "Artículo agregado al carrito."));
            } catch (error) {
                showNotice(error.message, true);
            } finally {
                addButton.disabled = false;
            }
            return;
        }

        const line = event.target.closest("[data-cart-line]");
        if (!line) return;

        const removeButton = event.target.closest("[data-cart-remove]");
        if (removeButton) {
            try {
                const data = await request(`${apiUrl}/items/${line.dataset.key}`, { method: "DELETE" });
                line.remove();
                refreshSummary(data);
                showNotice(t("orders.removed", "Artículo eliminado."));
            } catch (error) {
                showNotice(error.message, true);
            }
            return;
        }

        const stepButton = event.target.closest("[data-cart-step]");
        if (stepButton) {
            const input = line.querySelector("[data-cart-quantity]");
            input.value = Math.max(0, Math.min(99, Number(input.value || 0) + Number(stepButton.dataset.cartStep)));
            input.dispatchEvent(new Event("change", { bubbles: true }));
        }
    });

    const updateLine = async line => {
        const quantityInput = line.querySelector("[data-cart-quantity]");
        if (quantityInput.value === "") return;

        const quantity = Number(quantityInput.value);
        if (!Number.isInteger(quantity) || quantity < 0 || quantity > 99) {
            showNotice(t("orders.quantityError", "La cantidad debe ser un número entre 0 y 99."), true);
            return;
        }

        try {
            const data = await request(`${apiUrl}/items/${line.dataset.key}`, {
                method: "PATCH",
                body: JSON.stringify({ quantity, notes: line.querySelector("[data-cart-notes]")?.value || "" })
            });
            if (quantity === 0) line.remove();
            else {
                const subtotal = Number(line.dataset.unitPrice) * quantity;
                const tax = Math.round(subtotal * 0.13 * 100) / 100;
                line.querySelector("[data-line-subtotal]").textContent = currency.format(subtotal);
                line.querySelector("[data-line-tax]").textContent = currency.format(tax);
            }
            refreshSummary(data);
        } catch (error) {
            showNotice(error.message, true);
        }
    };

    document.addEventListener("change", event => {
        if (!event.target.matches("[data-cart-quantity]")) return;
        updateLine(event.target.closest("[data-cart-line]"));
    });

    document.addEventListener("input", event => {
        if (!event.target.matches("[data-cart-notes]")) return;
        window.clearTimeout(updateTimer);
        updateTimer = window.setTimeout(() => updateLine(event.target.closest("[data-cart-line]")), 500);
    });

    request(apiUrl).catch(() => setCount(0));
})();
