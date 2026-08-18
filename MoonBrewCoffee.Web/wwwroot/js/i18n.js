(() => {
    "use strict";

    const culture = document.documentElement.lang === "en" ? "en" : "es";
    let selectedTranslations = {};
    window.moonbrewI18n = {
        culture,
        t: (key, fallback = key) => selectedTranslations[key] ?? fallback
    };
    const translatableAttributes = [
        "alt",
        "placeholder",
        "title",
        "aria-label",
        "data-val-required",
        "data-val-length",
        "data-val-range",
        "data-val-regex",
        "data-val-email",
        "data-val-equalto"
    ];

    const flatten = (value, prefix = "", result = {}) => {
        Object.entries(value).forEach(([key, item]) => {
            const path = prefix ? `${prefix}.${key}` : key;
            if (item && typeof item === "object") {
                flatten(item, path, result);
            } else {
                result[path] = String(item);
            }
        });
        return result;
    };

    const escapeExpression = value =>
        value.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");

    const buildPattern = source => {
        let expression = escapeExpression(source);
        const placeholders = [...source.matchAll(/\{(\d+)\}/g)].map(match => match[1]);
        placeholders.forEach(index => {
            expression = expression.replace(`\\{${index}\\}`, "(.+?)");
        });
        return { regex: new RegExp(`^${expression}$`, "u"), placeholders };
    };

    const normalize = value => value.trim().replace(/\s+/g, " ");

    const initialize = async () => {
        const [spanishResponse, selectedResponse] = await Promise.all([
            fetch("/i18n/es.json", { cache: "no-cache" }),
            fetch(`/i18n/${culture}.json`, { cache: "no-cache" })
        ]);

        if (!spanishResponse.ok || !selectedResponse.ok) {
            throw new Error("No fue posible cargar los recursos de idioma.");
        }

        const spanish = flatten(await spanishResponse.json());
        const selected = flatten(await selectedResponse.json());
        selectedTranslations = selected;
        const exactTranslations = new Map();
        const patternTranslations = [];

        Object.entries(spanish).forEach(([key, source]) => {
            const target = selected[key] ?? source;
            if (source.includes("{0}")) {
                patternTranslations.push({
                    ...buildPattern(source),
                    target,
                    // Una frase con más texto fijo es más específica. Por ejemplo,
                    // "Pedido #{0} registrado..." debe evaluarse antes que
                    // "Pedido #{0}" para no traducir solamente el inicio.
                    specificity: source.replace(/\{\d+\}/g, "").length
                });
            } else if (!exactTranslations.has(source)) {
                exactTranslations.set(source, target);
            }
        });

        patternTranslations.sort((left, right) => right.specificity - left.specificity);

        const translateValue = value => {
            const normalizedValue = normalize(value);
            const exact = exactTranslations.get(normalizedValue);
            if (exact) return exact;

            for (const pattern of patternTranslations) {
                const match = normalizedValue.match(pattern.regex);
                if (!match) continue;

                let translated = pattern.target;
                pattern.placeholders.forEach((placeholder, index) => {
                    const capturedValue = match[index + 1];
                    const translatedCapture = translateValue(capturedValue) ?? capturedValue;
                    translated = translated.replace(`{${placeholder}}`, translatedCapture);
                });
                return translated;
            }

            return null;
        };

        const translateTextNode = node => {
            const original = node.nodeValue ?? "";
            if (!original.trim()) return;

            const translated = translateValue(original);
            if (!translated) return;

            const leading = original.match(/^\s*/)?.[0] ?? "";
            const trailing = original.match(/\s*$/)?.[0] ?? "";
            node.nodeValue = `${leading}${translated}${trailing}`;
        };

        const translateElement = element => {
            if (!(element instanceof HTMLElement)) return;
            if (["SCRIPT", "STYLE", "CODE"].includes(element.tagName)) return;

            const explicitKey = element.dataset.i18nKey;
            if (explicitKey && selected[explicitKey]) {
                element.textContent = selected[explicitKey];
            }

            const valueKey = element.dataset.i18nValueKey;
            if (valueKey && selected[valueKey] && "value" in element) {
                element.value = selected[valueKey];
            }

            const placeholderKey = element.dataset.i18nPlaceholderKey;
            if (placeholderKey && selected[placeholderKey]) {
                element.setAttribute("placeholder", selected[placeholderKey]);
            }

            const ariaKey = element.dataset.i18nAriaKey;
            if (ariaKey && selected[ariaKey]) {
                element.setAttribute("aria-label", selected[ariaKey]);
            }

            const titleKey = element.dataset.i18nTitleKey;
            if (titleKey && selected[titleKey]) {
                document.title = `${selected[titleKey]} - MoonBrew Coffee`;
            }

            translatableAttributes.forEach(attribute => {
                if (!element.hasAttribute(attribute)) return;
                const translated = translateValue(element.getAttribute(attribute) ?? "");
                if (translated) element.setAttribute(attribute, translated);
            });

            if ((element instanceof HTMLInputElement || element instanceof HTMLButtonElement) && element.value) {
                const translated = translateValue(element.value);
                if (translated) element.value = translated;
            }

            const walker = document.createTreeWalker(element, NodeFilter.SHOW_TEXT, {
                acceptNode: node => {
                    const parent = node.parentElement;
                    return parent && !["SCRIPT", "STYLE", "CODE"].includes(parent.tagName)
                        ? NodeFilter.FILTER_ACCEPT
                        : NodeFilter.FILTER_REJECT;
                }
            });

            const nodes = [];
            while (walker.nextNode()) nodes.push(walker.currentNode);
            nodes.forEach(translateTextNode);
        };

        translateElement(document.documentElement);

        const observer = new MutationObserver(mutations => {
            mutations.forEach(mutation => {
                mutation.addedNodes.forEach(node => {
                    if (node.nodeType === Node.TEXT_NODE) translateTextNode(node);
                    if (node.nodeType === Node.ELEMENT_NODE) translateElement(node);
                });
            });
        });

        observer.observe(document.body, { childList: true, subtree: true });
        localStorage.setItem("moonbrew-culture", culture);
        document.documentElement.dataset.i18nReady = "true";
    };

    document.querySelector(".language-selector select")?.addEventListener("change", event => {
        localStorage.setItem("moonbrew-culture", event.target.value.startsWith("en") ? "en" : "es");
    });

    initialize().catch(error => {
        document.documentElement.dataset.i18nReady = "error";
        console.error(error);
    });
})();
