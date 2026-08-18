(() => {
    const product = document.querySelector("[data-process-product]");
    const order = document.querySelector("#Orden");
    if (!product || !order) return;

    product.addEventListener("change", async () => {
        if (!product.value) {
            order.value = 1;
            return;
        }
        try {
            const response = await fetch(`${product.dataset.orderUrl}?idProducto=${encodeURIComponent(product.value)}`);
            if (!response.ok) return;
            const result = await response.json();
            order.value = result.order;
            order.dispatchEvent(new Event("input", { bubbles: true }));
        } catch {
            // El formulario sigue siendo utilizable si falla la sugerencia.
        }
    });
})();
