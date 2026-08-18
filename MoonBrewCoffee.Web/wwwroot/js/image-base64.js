(() => {
    "use strict";

    const loadImage = file => new Promise((resolve, reject) => {
        const image = new Image();
        const objectUrl = URL.createObjectURL(file);

        image.onload = () => {
            URL.revokeObjectURL(objectUrl);
            resolve(image);
        };

        image.onerror = () => {
            URL.revokeObjectURL(objectUrl);
            reject(new Error("No fue posible leer la imagen seleccionada."));
        };

        image.src = objectUrl;
    });

    const toBase64 = async (file, options = {}) => {
        if (!file?.type?.startsWith("image/")) {
            throw new Error("Seleccione un archivo de imagen válido.");
        }

        const maximumInputSize = options.maximumInputSize ?? 12 * 1024 * 1024;
        if (file.size > maximumInputSize) {
            throw new Error("La imagen no puede superar los 12 MB.");
        }

        const image = await loadImage(file);
        const maxWidth = options.maxWidth ?? 1400;
        const maxHeight = options.maxHeight ?? 1000;
        const scale = Math.min(1, maxWidth / image.naturalWidth, maxHeight / image.naturalHeight);
        const width = Math.max(1, Math.round(image.naturalWidth * scale));
        const height = Math.max(1, Math.round(image.naturalHeight * scale));
        const canvas = document.createElement("canvas");

        canvas.width = width;
        canvas.height = height;

        const context = canvas.getContext("2d", { alpha: true });
        if (!context) {
            throw new Error("El navegador no pudo procesar la imagen.");
        }

        context.imageSmoothingEnabled = true;
        context.imageSmoothingQuality = "high";
        context.drawImage(image, 0, 0, width, height);

        return canvas.toDataURL("image/webp", options.quality ?? 0.82);
    };

    window.MoonBrewImage = { toBase64 };
})();
