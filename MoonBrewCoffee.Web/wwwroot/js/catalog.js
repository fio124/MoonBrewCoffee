(() => {
    "use strict";

    const initializeCatalog = () => {
        const catalog = document.getElementById("catalogoProductos");
        if (!catalog) return;

        const search = document.getElementById("buscarProducto");
        const emptyState = document.getElementById("catalogoVacio");
        const cards = [...catalog.querySelectorAll(".producto-card")];
        const filters = [...document.querySelectorAll(".filter-btn[data-filter]")];
        let activeFilter = "todos";

        const normalize = value => String(value ?? "")
            .trim()
            .toLocaleLowerCase("es")
            .normalize("NFD")
            .replace(/[\u0300-\u036f]/g, "");

        const updateCards = () => {
            const term = normalize(search?.value);
            let visibleCards = 0;

            cards.forEach(card => {
                const matchesSearch = normalize(card.dataset.name).includes(term);
                const matchesCategory = activeFilter === "todos" ||
                    normalize(card.dataset.category) === normalize(activeFilter);
                const visible = matchesSearch && matchesCategory;

                card.hidden = !visible;
                card.classList.toggle("catalog-card-hidden", !visible);
                if (visible) visibleCards++;
            });

            if (emptyState) {
                emptyState.hidden = visibleCards > 0;
            }
        };

        filters.forEach(button => {
            button.addEventListener("click", () => {
                activeFilter = button.dataset.filter || "todos";

                filters.forEach(filter => {
                    const selected = filter === button;
                    filter.classList.toggle("active", selected);
                    filter.setAttribute("aria-pressed", String(selected));
                });

                updateCards();
            });
        });

        search?.addEventListener("input", updateCards);
        updateCards();
    };

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", initializeCatalog, { once: true });
    } else {
        initializeCatalog();
    }
})();
