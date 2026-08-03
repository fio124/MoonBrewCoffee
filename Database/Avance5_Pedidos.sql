USE MoonBrewCoffee;
GO

SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF COL_LENGTH('dbo.Pedidos', 'IdEncargado') IS NULL
BEGIN
    ALTER TABLE dbo.Pedidos ADD IdEncargado INT NULL;
END;

IF COL_LENGTH('dbo.DetallePedidos', 'Impuesto') IS NULL
BEGIN
    ALTER TABLE dbo.DetallePedidos
        ADD Impuesto DECIMAL(10, 2) NOT NULL
            CONSTRAINT DF_DetallePedidos_Impuesto DEFAULT (0);
END;

IF NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys
    WHERE name = 'FK_Pedidos_Usuarios_IdEncargado'
)
BEGIN
    ALTER TABLE dbo.Pedidos WITH CHECK
        ADD CONSTRAINT FK_Pedidos_Usuarios_IdEncargado
        FOREIGN KEY (IdEncargado) REFERENCES dbo.Usuarios(IdUsuario);
END;

IF NOT EXISTS (SELECT 1 FROM dbo.EstadosPedido WHERE Nombre = N'Pendiente de pago')
    INSERT INTO dbo.EstadosPedido (Nombre, ColorHex) VALUES (N'Pendiente de pago', '#B7791F');
IF NOT EXISTS (SELECT 1 FROM dbo.EstadosPedido WHERE Nombre = N'Aceptada')
    INSERT INTO dbo.EstadosPedido (Nombre, ColorHex) VALUES (N'Aceptada', '#2F855A');
IF NOT EXISTS (SELECT 1 FROM dbo.EstadosPedido WHERE Nombre = N'Preparación')
    INSERT INTO dbo.EstadosPedido (Nombre, ColorHex) VALUES (N'Preparación', '#805AD5');
IF NOT EXISTS (SELECT 1 FROM dbo.EstadosPedido WHERE Nombre = N'Procesando')
    INSERT INTO dbo.EstadosPedido (Nombre, ColorHex) VALUES (N'Procesando', '#2B6CB0');
IF NOT EXISTS (SELECT 1 FROM dbo.EstadosPedido WHERE Nombre = N'Entregada')
    INSERT INTO dbo.EstadosPedido (Nombre, ColorHex) VALUES (N'Entregada', '#276749');

DECLARE @ClienteId INT = (
    SELECT TOP (1) u.IdUsuario
    FROM dbo.Usuarios u
    INNER JOIN dbo.Roles r ON r.IdRol = u.IdRol
    WHERE u.Activo = 1 AND LOWER(r.Nombre) LIKE '%cliente%'
    ORDER BY u.IdUsuario
);
DECLARE @ProductoId INT = (
    SELECT TOP (1) IdProducto FROM dbo.Productos WHERE Activo = 1 ORDER BY IdProducto
);
DECLARE @Precio DECIMAL(10, 2) = (
    SELECT Precio FROM dbo.Productos WHERE IdProducto = @ProductoId
);
DECLARE @EstadoId INT = (
    SELECT TOP (1) IdEstado FROM dbo.EstadosPedido WHERE Nombre = N'Entregada'
);

IF @ClienteId IS NOT NULL AND @ProductoId IS NOT NULL
   AND (SELECT COUNT(*) FROM dbo.Pedidos) < 4
BEGIN
    DECLARE @Indice INT = 1;
    WHILE @Indice <= 4
    BEGIN
        DECLARE @Cantidad INT = @Indice;
        DECLARE @Subtotal DECIMAL(10, 2) = @Precio * @Cantidad;
        DECLARE @Impuesto DECIMAL(10, 2) = ROUND(@Subtotal * 0.13, 2);
        DECLARE @PedidoId INT;

        INSERT INTO dbo.Pedidos
            (IdCliente, IdEncargado, IdEstado, FechaPedido, TipoEntrega,
             DireccionEntrega, CostoEnvio, Subtotal, Impuesto, Total,
             Observaciones, Activo)
        VALUES
            (@ClienteId, NULL, @EstadoId, DATEADD(DAY, -@Indice, GETDATE()),
             N'Recogida en tienda', NULL, 0, @Subtotal, @Impuesto,
             @Subtotal + @Impuesto, N'Pedido precargado para pruebas del Avance 5', 1);

        SET @PedidoId = SCOPE_IDENTITY();

        INSERT INTO dbo.DetallePedidos
            (IdPedido, IdProducto, IdCombo, Cantidad, PrecioUnitario,
             Subtotal, Impuesto, Observaciones)
        VALUES
            (@PedidoId, @ProductoId, NULL, @Cantidad, @Precio,
             @Subtotal, @Impuesto, N'Preparación estándar');

        INSERT INTO dbo.Pagos
            (IdPedido, MetodoPago, FechaPago, TotalPagado, EstadoPago)
        VALUES
            (@PedidoId, N'Efectivo', DATEADD(DAY, -@Indice, GETDATE()),
             @Subtotal + @Impuesto, N'Aprobado');

        SET @Indice += 1;
    END;
END;

COMMIT TRANSACTION;
GO
