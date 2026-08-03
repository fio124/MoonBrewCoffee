(() => {
    const form = document.querySelector("[data-checkout]");
    if (!form) return;

    const baseTotal = Number(form.dataset.cartTotal);
    const delivery = form.querySelector("[data-delivery-type]");
    const address = form.querySelector("[data-address-field]");
    const cardFields = form.querySelector("[data-card-fields]");
    const cashFields = form.querySelector("[data-cash-fields]");
    const cashInput = form.querySelector("[data-cash-received]");
    const formatter = new Intl.NumberFormat(document.documentElement.lang || "es-CR", { style: "currency", currency: "CRC", maximumFractionDigits: 0 });

    const grandTotal = () => baseTotal + (delivery.value === "domicilio" ? 2500 : 0);
    const refresh = () => {
        const homeDelivery = delivery.value === "domicilio";
        address.hidden = !homeDelivery;
        form.querySelector("[data-checkout-shipping]").textContent = formatter.format(homeDelivery ? 2500 : 0);
        form.querySelector("[data-checkout-total]").textContent = formatter.format(grandTotal());

        const payment = form.querySelector("input[name='PaymentMethod']:checked")?.value || "credito";
        const cash = payment === "efectivo";
        cardFields.hidden = cash;
        cashFields.hidden = !cash;
        if (cash) {
            const change = Math.max(0, Number(cashInput.value || 0) - grandTotal());
            form.querySelector("[data-cash-change]").textContent = formatter.format(change);
        }
    };

    delivery.addEventListener("change", refresh);
    form.querySelectorAll("input[name='PaymentMethod']").forEach(input => input.addEventListener("change", refresh));
    cashInput.addEventListener("input", refresh);
    form.querySelector("#CardNumber")?.addEventListener("input", event => {
        event.target.value = event.target.value.replace(/\D/g, "").slice(0, 19).replace(/(.{4})/g, "$1 ").trim();
    });
    form.querySelector("#CardExpiry")?.addEventListener("input", event => {
        const value = event.target.value.replace(/\D/g, "").slice(0, 4);
        event.target.value = value.length > 2 ? `${value.slice(0, 2)}/${value.slice(2)}` : value;
    });
    refresh();
})();
