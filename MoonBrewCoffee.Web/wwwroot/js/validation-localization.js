(() => {
    if (!window.jQuery?.validator) return;

    const english = document.documentElement.lang?.startsWith("en");
    const messages = english
        ? {
            required: "This field is required.",
            number: "Please enter a valid number.",
            min: "Please enter a value greater than or equal to {0}.",
            max: "Please enter a value less than or equal to {0}.",
            range: "Please enter a value between {0} and {1}."
        }
        : {
            required: "Este campo es obligatorio.",
            number: "Ingresa un número válido.",
            min: "Ingresa un valor mayor o igual a {0}.",
            max: "Ingresa un valor menor o igual a {0}.",
            range: "Ingresa un valor entre {0} y {1}."
        };

    jQuery.extend(jQuery.validator.messages, {
        required: messages.required,
        number: messages.number,
        min: jQuery.validator.format(messages.min),
        max: jQuery.validator.format(messages.max),
        range: jQuery.validator.format(messages.range)
    });

    document.querySelectorAll("[data-val-required]").forEach(input => {
        const current = input.getAttribute("data-val-required") || "";
        if (/^The .+ field is required\.$/i.test(current))
            input.setAttribute("data-val-required", messages.required);
    });
})();
