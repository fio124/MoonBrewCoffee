(() => {
    const sidebar = document.getElementById("appSidebar");
    const toggle = document.querySelector("[data-sidebar-toggle]");
    const closeControls = document.querySelectorAll("[data-sidebar-close]");
    const mobileSidebar = window.matchMedia("(max-width: 1100px)");

    if (!sidebar || !toggle) return;

    const setSidebarOpen = open => {
        document.body.classList.toggle("sidebar-open", open);
        toggle.setAttribute("aria-expanded", String(open));
        toggle.setAttribute("aria-label", open ? "Cerrar menú principal" : "Abrir menú principal");

        if (open) {
            sidebar.querySelector("a")?.focus();
        }
    };

    toggle.addEventListener("click", () => {
        setSidebarOpen(!document.body.classList.contains("sidebar-open"));
    });

    closeControls.forEach(control => {
        control.addEventListener("click", () => setSidebarOpen(false));
    });

    sidebar.querySelectorAll("a").forEach(link => {
        link.addEventListener("click", () => {
            if (mobileSidebar.matches) setSidebarOpen(false);
        });
    });

    document.addEventListener("keydown", event => {
        if (event.key === "Escape" && document.body.classList.contains("sidebar-open")) {
            setSidebarOpen(false);
            toggle.focus();
        }
    });

    mobileSidebar.addEventListener("change", event => {
        if (!event.matches) setSidebarOpen(false);
    });
})();
