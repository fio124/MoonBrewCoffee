IF DB_ID(N'MoonBrewCoffee') IS NULL CREATE DATABASE MoonBrewCoffee;
GO
USE MoonBrewCoffee;
GO

CREATE TABLE [Categorias] (
    [IdCategoria] int NOT NULL IDENTITY,
    [Nombre] nvarchar(max) NOT NULL,
    [Descripcion] nvarchar(max) NULL,
    [Activo] bit NOT NULL,
    CONSTRAINT [PK_Categorias] PRIMARY KEY ([IdCategoria])
);
GO


CREATE TABLE [Combos] (
    [IdCombo] int NOT NULL IDENTITY,
    [Nombre] nvarchar(max) NOT NULL,
    [Descripcion] nvarchar(max) NULL,
    [PrecioCombo] decimal(10,2) NOT NULL,
    [ImagenURL] nvarchar(max) NULL,
    [Activo] bit NOT NULL,
    CONSTRAINT [PK_Combos] PRIMARY KEY ([IdCombo])
);
GO


CREATE TABLE [EstacionesCocina] (
    [IdEstacion] int NOT NULL IDENTITY,
    [Nombre] nvarchar(max) NOT NULL,
    [Descripcion] nvarchar(max) NULL,
    [ColorHex] nvarchar(max) NULL,
    [Activo] bit NOT NULL,
    CONSTRAINT [PK_EstacionesCocina] PRIMARY KEY ([IdEstacion])
);
GO


CREATE TABLE [EstadosPedido] (
    [IdEstado] int NOT NULL IDENTITY,
    [Nombre] nvarchar(max) NOT NULL,
    [ColorHex] nvarchar(max) NULL,
    CONSTRAINT [PK_EstadosPedido] PRIMARY KEY ([IdEstado])
);
GO


CREATE TABLE [Ingredientes] (
    [IdIngrediente] int NOT NULL IDENTITY,
    [Nombre] nvarchar(max) NOT NULL,
    [Activo] bit NOT NULL,
    CONSTRAINT [PK_Ingredientes] PRIMARY KEY ([IdIngrediente])
);
GO


CREATE TABLE [Menus] (
    [IdMenu] int NOT NULL IDENTITY,
    [Nombre] nvarchar(max) NOT NULL,
    [FechaInicio] datetime2 NOT NULL,
    [FechaFin] datetime2 NOT NULL,
    [HoraInicio] time NOT NULL,
    [HoraFin] time NOT NULL,
    [ImagenURL] nvarchar(max) NULL,
    [Disponible] bit NOT NULL,
    [Activo] bit NOT NULL,
    CONSTRAINT [PK_Menus] PRIMARY KEY ([IdMenu])
);
GO


CREATE TABLE [Roles] (
    [IdRol] int NOT NULL IDENTITY,
    [Nombre] nvarchar(max) NOT NULL,
    [Descripcion] nvarchar(max) NULL,
    [Activo] bit NOT NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY ([IdRol])
);
GO


CREATE TABLE [Productos] (
    [IdProducto] int NOT NULL IDENTITY,
    [IdCategoria] int NOT NULL,
    [Nombre] nvarchar(max) NOT NULL,
    [Descripcion] nvarchar(max) NULL,
    [Precio] decimal(10,2) NOT NULL,
    [Image64] nvarchar(max) NULL,
    [TiempoPreparacion] int NOT NULL,
    [Activo] bit NOT NULL,
    CONSTRAINT [PK_Productos] PRIMARY KEY ([IdProducto]),
    CONSTRAINT [FK_Productos_Categorias_IdCategoria] FOREIGN KEY ([IdCategoria]) REFERENCES [Categorias] ([IdCategoria]) ON DELETE CASCADE
);
GO


CREATE TABLE [MenuCombos] (
    [IdMenu] int NOT NULL,
    [IdCombo] int NOT NULL,
    CONSTRAINT [PK_MenuCombos] PRIMARY KEY ([IdMenu], [IdCombo]),
    CONSTRAINT [FK_MenuCombos_Combos_IdCombo] FOREIGN KEY ([IdCombo]) REFERENCES [Combos] ([IdCombo]) ON DELETE CASCADE,
    CONSTRAINT [FK_MenuCombos_Menus_IdMenu] FOREIGN KEY ([IdMenu]) REFERENCES [Menus] ([IdMenu]) ON DELETE CASCADE
);
GO


CREATE TABLE [Usuarios] (
    [IdUsuario] int NOT NULL IDENTITY,
    [IdRol] int NOT NULL,
    [Nombre] nvarchar(max) NOT NULL,
    [Apellido] nvarchar(max) NOT NULL,
    [Correo] nvarchar(256) NOT NULL,
    [Telefono] nvarchar(max) NOT NULL,
    [PasswordHash] nvarchar(max) NOT NULL,
    [FechaRegistro] datetime2 NOT NULL,
    [Activo] bit NOT NULL,
    CONSTRAINT [PK_Usuarios] PRIMARY KEY ([IdUsuario]),
    CONSTRAINT [FK_Usuarios_Roles_IdRol] FOREIGN KEY ([IdRol]) REFERENCES [Roles] ([IdRol]) ON DELETE CASCADE
);
GO


CREATE TABLE [ComboProductos] (
    [IdCombo] int NOT NULL,
    [IdProducto] int NOT NULL,
    [Cantidad] int NOT NULL,
    CONSTRAINT [PK_ComboProductos] PRIMARY KEY ([IdCombo], [IdProducto]),
    CONSTRAINT [FK_ComboProductos_Combos_IdCombo] FOREIGN KEY ([IdCombo]) REFERENCES [Combos] ([IdCombo]) ON DELETE CASCADE,
    CONSTRAINT [FK_ComboProductos_Productos_IdProducto] FOREIGN KEY ([IdProducto]) REFERENCES [Productos] ([IdProducto]) ON DELETE CASCADE
);
GO


CREATE TABLE [MenuProductos] (
    [IdMenu] int NOT NULL,
    [IdProducto] int NOT NULL,
    CONSTRAINT [PK_MenuProductos] PRIMARY KEY ([IdMenu], [IdProducto]),
    CONSTRAINT [FK_MenuProductos_Menus_IdMenu] FOREIGN KEY ([IdMenu]) REFERENCES [Menus] ([IdMenu]) ON DELETE CASCADE,
    CONSTRAINT [FK_MenuProductos_Productos_IdProducto] FOREIGN KEY ([IdProducto]) REFERENCES [Productos] ([IdProducto]) ON DELETE CASCADE
);
GO


CREATE TABLE [ProcesosPreparacion] (
    [IdProceso] int NOT NULL IDENTITY,
    [IdProducto] int NOT NULL,
    [IdEstacion] int NOT NULL,
    [TiempoPreparacionMin] int NOT NULL,
    [Orden] int NOT NULL,
    CONSTRAINT [PK_ProcesosPreparacion] PRIMARY KEY ([IdProceso]),
    CONSTRAINT [FK_ProcesosPreparacion_EstacionesCocina_IdEstacion] FOREIGN KEY ([IdEstacion]) REFERENCES [EstacionesCocina] ([IdEstacion]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ProcesosPreparacion_Productos_IdProducto] FOREIGN KEY ([IdProducto]) REFERENCES [Productos] ([IdProducto]) ON DELETE NO ACTION
);
GO


CREATE TABLE [ProductoIngredientes] (
    [IdProducto] int NOT NULL,
    [IdIngrediente] int NOT NULL,
    CONSTRAINT [PK_ProductoIngredientes] PRIMARY KEY ([IdProducto], [IdIngrediente]),
    CONSTRAINT [FK_ProductoIngredientes_Ingredientes_IdIngrediente] FOREIGN KEY ([IdIngrediente]) REFERENCES [Ingredientes] ([IdIngrediente]) ON DELETE CASCADE,
    CONSTRAINT [FK_ProductoIngredientes_Productos_IdProducto] FOREIGN KEY ([IdProducto]) REFERENCES [Productos] ([IdProducto]) ON DELETE CASCADE
);
GO


CREATE TABLE [Carritos] (
    [IdCarrito] int NOT NULL IDENTITY,
    [IdUsuario] int NOT NULL,
    [FechaCreacion] datetime2 NOT NULL,
    [UltimaActualizacion] datetime2 NOT NULL,
    [Activo] bit NOT NULL,
    [UsuarioIdUsuario] int NULL,
    CONSTRAINT [PK_Carritos] PRIMARY KEY ([IdCarrito]),
    CONSTRAINT [FK_Carritos_Usuarios_UsuarioIdUsuario] FOREIGN KEY ([UsuarioIdUsuario]) REFERENCES [Usuarios] ([IdUsuario])
);
GO


CREATE TABLE [Pedidos] (
    [IdPedido] int NOT NULL IDENTITY,
    [IdCliente] int NOT NULL,
    [IdEncargado] int NULL,
    [IdEstado] int NOT NULL,
    [ClaveOperacion] nvarchar(64) NOT NULL,
    [FechaPedido] datetime2 NOT NULL,
    [TipoEntrega] nvarchar(max) NOT NULL,
    [DireccionEntrega] nvarchar(max) NULL,
    [CostoEnvio] decimal(10,2) NOT NULL,
    [Subtotal] decimal(10,2) NOT NULL,
    [Impuesto] decimal(10,2) NOT NULL,
    [Total] decimal(10,2) NOT NULL,
    [Observaciones] nvarchar(max) NULL,
    [Activo] bit NOT NULL,
    CONSTRAINT [PK_Pedidos] PRIMARY KEY ([IdPedido]),
    CONSTRAINT [FK_Pedidos_EstadosPedido_IdEstado] FOREIGN KEY ([IdEstado]) REFERENCES [EstadosPedido] ([IdEstado]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Pedidos_Usuarios_IdCliente] FOREIGN KEY ([IdCliente]) REFERENCES [Usuarios] ([IdUsuario]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Pedidos_Usuarios_IdEncargado] FOREIGN KEY ([IdEncargado]) REFERENCES [Usuarios] ([IdUsuario]) ON DELETE NO ACTION
);
GO


CREATE TABLE [CarritoDetalles] (
    [IdCarritoDetalle] int NOT NULL IDENTITY,
    [IdCarrito] int NOT NULL,
    [IdProducto] int NULL,
    [IdCombo] int NULL,
    [Cantidad] int NOT NULL,
    [Observaciones] nvarchar(max) NULL,
    [CarritoIdCarrito] int NULL,
    [ProductoIdProducto] int NULL,
    [ComboIdCombo] int NULL,
    CONSTRAINT [PK_CarritoDetalles] PRIMARY KEY ([IdCarritoDetalle]),
    CONSTRAINT [FK_CarritoDetalles_Carritos_CarritoIdCarrito] FOREIGN KEY ([CarritoIdCarrito]) REFERENCES [Carritos] ([IdCarrito]),
    CONSTRAINT [FK_CarritoDetalles_Combos_ComboIdCombo] FOREIGN KEY ([ComboIdCombo]) REFERENCES [Combos] ([IdCombo]),
    CONSTRAINT [FK_CarritoDetalles_Productos_ProductoIdProducto] FOREIGN KEY ([ProductoIdProducto]) REFERENCES [Productos] ([IdProducto])
);
GO


CREATE TABLE [DetallePedidos] (
    [IdDetallePedido] int NOT NULL IDENTITY,
    [IdPedido] int NOT NULL,
    [IdProducto] int NULL,
    [IdCombo] int NULL,
    [Cantidad] int NOT NULL,
    [PrecioUnitario] decimal(10,2) NOT NULL,
    [Subtotal] decimal(10,2) NOT NULL,
    [Impuesto] decimal(10,2) NOT NULL,
    [Observaciones] nvarchar(max) NULL,
    [ProductoIdProducto] int NULL,
    [ComboIdCombo] int NULL,
    CONSTRAINT [PK_DetallePedidos] PRIMARY KEY ([IdDetallePedido]),
    CONSTRAINT [FK_DetallePedidos_Combos_ComboIdCombo] FOREIGN KEY ([ComboIdCombo]) REFERENCES [Combos] ([IdCombo]),
    CONSTRAINT [FK_DetallePedidos_Pedidos_IdPedido] FOREIGN KEY ([IdPedido]) REFERENCES [Pedidos] ([IdPedido]) ON DELETE CASCADE,
    CONSTRAINT [FK_DetallePedidos_Productos_ProductoIdProducto] FOREIGN KEY ([ProductoIdProducto]) REFERENCES [Productos] ([IdProducto])
);
GO


CREATE TABLE [Pagos] (
    [IdPago] int NOT NULL IDENTITY,
    [IdPedido] int NOT NULL,
    [MetodoPago] nvarchar(max) NOT NULL,
    [FechaPago] datetime2 NOT NULL,
    [TotalPagado] decimal(10,2) NOT NULL,
    [EstadoPago] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Pagos] PRIMARY KEY ([IdPago]),
    CONSTRAINT [FK_Pagos_Pedidos_IdPedido] FOREIGN KEY ([IdPedido]) REFERENCES [Pedidos] ([IdPedido]) ON DELETE CASCADE
);
GO


CREATE TABLE [PedidoEstadoHistorial] (
    [IdHistorial] int NOT NULL IDENTITY,
    [IdPedido] int NOT NULL,
    [IdEstado] int NOT NULL,
    [FechaCambio] datetime2 NOT NULL,
    [IdUsuario] int NULL,
    CONSTRAINT [PK_PedidoEstadoHistorial] PRIMARY KEY ([IdHistorial]),
    CONSTRAINT [FK_PedidoEstadoHistorial_EstadosPedido_IdEstado] FOREIGN KEY ([IdEstado]) REFERENCES [EstadosPedido] ([IdEstado]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PedidoEstadoHistorial_Pedidos_IdPedido] FOREIGN KEY ([IdPedido]) REFERENCES [Pedidos] ([IdPedido]) ON DELETE CASCADE,
    CONSTRAINT [FK_PedidoEstadoHistorial_Usuarios_IdUsuario] FOREIGN KEY ([IdUsuario]) REFERENCES [Usuarios] ([IdUsuario]) ON DELETE NO ACTION
);
GO


CREATE TABLE [PedidoProcesos] (
    [IdPedidoProceso] int NOT NULL IDENTITY,
    [IdPedido] int NOT NULL,
    [IdEstacion] int NOT NULL,
    [Orden] int NOT NULL,
    [Descripcion] nvarchar(180) NOT NULL,
    [Estado] nvarchar(30) NOT NULL,
    [FechaInicio] datetime2 NULL,
    [FechaFin] datetime2 NULL,
    [IdEncargado] int NULL,
    CONSTRAINT [PK_PedidoProcesos] PRIMARY KEY ([IdPedidoProceso]),
    CONSTRAINT [FK_PedidoProcesos_EstacionesCocina_IdEstacion] FOREIGN KEY ([IdEstacion]) REFERENCES [EstacionesCocina] ([IdEstacion]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PedidoProcesos_Pedidos_IdPedido] FOREIGN KEY ([IdPedido]) REFERENCES [Pedidos] ([IdPedido]) ON DELETE CASCADE,
    CONSTRAINT [FK_PedidoProcesos_Usuarios_IdEncargado] FOREIGN KEY ([IdEncargado]) REFERENCES [Usuarios] ([IdUsuario]) ON DELETE NO ACTION
);
GO


CREATE INDEX [IX_CarritoDetalles_CarritoIdCarrito] ON [CarritoDetalles] ([CarritoIdCarrito]);
GO


CREATE INDEX [IX_CarritoDetalles_ComboIdCombo] ON [CarritoDetalles] ([ComboIdCombo]);
GO


CREATE INDEX [IX_CarritoDetalles_ProductoIdProducto] ON [CarritoDetalles] ([ProductoIdProducto]);
GO


CREATE INDEX [IX_Carritos_UsuarioIdUsuario] ON [Carritos] ([UsuarioIdUsuario]);
GO


CREATE INDEX [IX_ComboProductos_IdProducto] ON [ComboProductos] ([IdProducto]);
GO


CREATE INDEX [IX_DetallePedidos_ComboIdCombo] ON [DetallePedidos] ([ComboIdCombo]);
GO


CREATE INDEX [IX_DetallePedidos_IdPedido] ON [DetallePedidos] ([IdPedido]);
GO


CREATE INDEX [IX_DetallePedidos_ProductoIdProducto] ON [DetallePedidos] ([ProductoIdProducto]);
GO


CREATE INDEX [IX_MenuCombos_IdCombo] ON [MenuCombos] ([IdCombo]);
GO


CREATE INDEX [IX_MenuProductos_IdProducto] ON [MenuProductos] ([IdProducto]);
GO


CREATE UNIQUE INDEX [IX_Pagos_IdPedido] ON [Pagos] ([IdPedido]);
GO


CREATE INDEX [IX_PedidoEstadoHistorial_IdEstado] ON [PedidoEstadoHistorial] ([IdEstado]);
GO


CREATE INDEX [IX_PedidoEstadoHistorial_IdPedido] ON [PedidoEstadoHistorial] ([IdPedido]);
GO


CREATE INDEX [IX_PedidoEstadoHistorial_IdUsuario] ON [PedidoEstadoHistorial] ([IdUsuario]);
GO


CREATE INDEX [IX_PedidoProcesos_IdEncargado] ON [PedidoProcesos] ([IdEncargado]);
GO


CREATE INDEX [IX_PedidoProcesos_IdEstacion] ON [PedidoProcesos] ([IdEstacion]);
GO


CREATE INDEX [IX_PedidoProcesos_IdPedido_Orden] ON [PedidoProcesos] ([IdPedido], [Orden]);
GO


CREATE UNIQUE INDEX [IX_Pedidos_ClaveOperacion] ON [Pedidos] ([ClaveOperacion]);
GO


CREATE INDEX [IX_Pedidos_IdCliente] ON [Pedidos] ([IdCliente]);
GO


CREATE INDEX [IX_Pedidos_IdEncargado] ON [Pedidos] ([IdEncargado]);
GO


CREATE INDEX [IX_Pedidos_IdEstado] ON [Pedidos] ([IdEstado]);
GO


CREATE UNIQUE INDEX [IX_Usuarios_Correo] ON [Usuarios] ([Correo]);
GO


CREATE INDEX [IX_ProcesosPreparacion_IdEstacion] ON [ProcesosPreparacion] ([IdEstacion]);
GO


CREATE UNIQUE INDEX [IX_ProcesosPreparacion_IdProducto_Orden] ON [ProcesosPreparacion] ([IdProducto], [Orden]);
GO


CREATE INDEX [IX_ProductoIngredientes_IdIngrediente] ON [ProductoIngredientes] ([IdIngrediente]);
GO


CREATE INDEX [IX_Productos_IdCategoria] ON [Productos] ([IdCategoria]);
GO


CREATE INDEX [IX_Usuarios_IdRol] ON [Usuarios] ([IdRol]);
GO



SET XACT_ABORT ON;
BEGIN TRANSACTION;

INSERT INTO Roles (Nombre, Descripcion, Activo) VALUES
(N'Administrador',N'Administración completa',1),(N'Encargado',N'Gestión de pedidos',1),
(N'Cliente',N'Compras en línea',1),(N'Preparación',N'Operación de estaciones',1);

DECLARE @RolAdmin INT=(SELECT IdRol FROM Roles WHERE Nombre=N'Administrador');
DECLARE @RolEncargado INT=(SELECT IdRol FROM Roles WHERE Nombre=N'Encargado');
DECLARE @RolCliente INT=(SELECT IdRol FROM Roles WHERE Nombre=N'Cliente');
INSERT INTO Usuarios (IdRol,Nombre,Apellido,Correo,Telefono,PasswordHash,FechaRegistro,Activo) VALUES
(@RolAdmin,N'Admin',N'MoonBrew',N'admin@moonbrew.cr',N'8888-0001',N'MoonBrew123!',GETDATE(),1),
(@RolEncargado,N'Yendry',N'Cisneros',N'encargado@moonbrew.cr',N'8888-0002',N'MoonBrew123!',GETDATE(),1),
(@RolCliente,N'Cliente',N'Demo',N'cliente@moonbrew.cr',N'8888-0003',N'MoonBrew123!',GETDATE(),1),
(@RolCliente,N'Cliente',N'Invitado',N'invitado@moonbrew.cr',N'8888-0004',N'MoonBrew123!',GETDATE(),1);

INSERT INTO Categorias (Nombre,Descripcion,Activo) VALUES
(N'Cafés',N'Bebidas de café',1),(N'Bebidas Frías',N'Bebidas refrescantes',1),
(N'Postres',N'Repostería artesanal',1),(N'Té Caliente',N'Infusiones calientes',1);
INSERT INTO Ingredientes (Nombre,Activo) VALUES (N'Café',1),(N'Leche',1),(N'Chocolate',1),(N'Vainilla',1);

DECLARE @CatCafe INT=(SELECT IdCategoria FROM Categorias WHERE Nombre=N'Cafés');
DECLARE @CatFria INT=(SELECT IdCategoria FROM Categorias WHERE Nombre=N'Bebidas Frías');
DECLARE @CatPostre INT=(SELECT IdCategoria FROM Categorias WHERE Nombre=N'Postres');
DECLARE @CatTe INT=(SELECT IdCategoria FROM Categorias WHERE Nombre=N'Té Caliente');
INSERT INTO Productos (IdCategoria,Nombre,Descripcion,Precio,Image64,TiempoPreparacion,Activo) VALUES
(@CatCafe,N'Latte Clásico',N'Café espresso con leche cremosa',2200,N'/Imagenes/Latte Clasico.webp',8,1),
(@CatFria,N'Frappé de Chocolate',N'Bebida fría con chocolate',2800,N'/Imagenes/Frappe Caramelo.webp',10,1),
(@CatPostre,N'Brownie',N'Brownie artesanal de chocolate',1800,N'/Imagenes/Brownie.webp',12,1),
(@CatTe,N'Té Chai',N'Infusión especiada con leche',2100,N'/Imagenes/Te verde.webp',7,1);

INSERT INTO ProductoIngredientes (IdProducto,IdIngrediente)
SELECT p.IdProducto,i.IdIngrediente FROM Productos p CROSS JOIN Ingredientes i
WHERE (p.Nombre=N'Latte Clásico' AND i.Nombre IN(N'Café',N'Leche'))
   OR (p.Nombre=N'Frappé de Chocolate' AND i.Nombre IN(N'Café',N'Leche',N'Chocolate'))
   OR (p.Nombre=N'Brownie' AND i.Nombre IN(N'Chocolate',N'Vainilla'))
   OR (p.Nombre=N'Té Chai' AND i.Nombre IN(N'Leche',N'Vainilla'));

INSERT INTO Combos (Nombre,Descripcion,PrecioCombo,ImagenURL,Activo) VALUES
(N'Combo Desayuno',N'Latte y brownie',3500,NULL,1),(N'Combo Dulce',N'Frappé y brownie',4200,NULL,1),
(N'Combo Tarde',N'Té y brownie',3300,NULL,1),(N'Combo Premium',N'Latte, frappé y brownie',5900,NULL,1);
INSERT INTO ComboProductos (IdCombo,IdProducto,Cantidad)
SELECT c.IdCombo,p.IdProducto,1 FROM Combos c CROSS JOIN Productos p WHERE
(c.Nombre=N'Combo Desayuno' AND p.Nombre IN(N'Latte Clásico',N'Brownie')) OR
(c.Nombre=N'Combo Dulce' AND p.Nombre IN(N'Frappé de Chocolate',N'Brownie')) OR
(c.Nombre=N'Combo Tarde' AND p.Nombre IN(N'Té Chai',N'Brownie')) OR
(c.Nombre=N'Combo Premium' AND p.Nombre IN(N'Latte Clásico',N'Frappé de Chocolate',N'Brownie'));

INSERT INTO Menus (Nombre,FechaInicio,FechaFin,HoraInicio,HoraFin,ImagenURL,Disponible,Activo) VALUES
(N'Desayuno',DATEFROMPARTS(YEAR(GETDATE()),1,1),DATEFROMPARTS(YEAR(GETDATE()),12,31),'06:00','11:59',NULL,1,1),
(N'Mediodía',DATEFROMPARTS(YEAR(GETDATE()),1,1),DATEFROMPARTS(YEAR(GETDATE()),12,31),'12:00','15:59',NULL,1,1),
(N'Tarde',DATEFROMPARTS(YEAR(GETDATE()),1,1),DATEFROMPARTS(YEAR(GETDATE()),12,31),'16:00','20:59',NULL,1,1),
(N'Todo el día',DATEFROMPARTS(YEAR(GETDATE()),1,1),DATEFROMPARTS(YEAR(GETDATE()),12,31),'00:00','23:59',NULL,1,1);
INSERT INTO MenuProductos (IdMenu,IdProducto) SELECT m.IdMenu,p.IdProducto FROM Menus m CROSS JOIN Productos p;
INSERT INTO MenuCombos (IdMenu,IdCombo) SELECT m.IdMenu,c.IdCombo FROM Menus m CROSS JOIN Combos c;

INSERT INTO EstacionesCocina (Nombre,Descripcion,ColorHex,Activo) VALUES
(N'Barra',N'Bebidas de café',N'#8B5E3C',1),(N'Cocina',N'Alimentos',N'#B7791F',1),
(N'Repostería',N'Postres',N'#805AD5',1),(N'Empaque',N'Revisión y despacho',N'#2B6CB0',1);
INSERT INTO ProcesosPreparacion (IdProducto,IdEstacion,TiempoPreparacionMin,Orden)
SELECT p.IdProducto,e.IdEstacion,CASE WHEN e.Nombre=N'Empaque' THEN 2 ELSE p.TiempoPreparacion END,
CASE WHEN e.Nombre=N'Empaque' THEN 2 ELSE 1 END
FROM Productos p CROSS JOIN EstacionesCocina e WHERE
(e.Nombre=N'Empaque') OR
(p.IdCategoria=@CatCafe AND e.Nombre=N'Barra') OR (p.IdCategoria=@CatFria AND e.Nombre=N'Barra') OR
(p.IdCategoria=@CatPostre AND e.Nombre=N'Repostería') OR (p.IdCategoria=@CatTe AND e.Nombre=N'Cocina');

INSERT INTO EstadosPedido (Nombre,ColorHex) VALUES
(N'Pendiente',N'#B7791F'),(N'Aceptada',N'#2F855A'),(N'Preparación',N'#805AD5'),
(N'Procesando',N'#2B6CB0'),(N'Entregada',N'#276749');

DECLARE @Cliente INT=(SELECT TOP(1) IdUsuario FROM Usuarios WHERE Correo=N'cliente@moonbrew.cr');
DECLARE @Entregada INT=(SELECT IdEstado FROM EstadosPedido WHERE Nombre=N'Entregada');
DECLARE @Producto INT=(SELECT TOP(1) IdProducto FROM Productos ORDER BY IdProducto);
DECLARE @Precio DECIMAL(10,2)=(SELECT Precio FROM Productos WHERE IdProducto=@Producto);
DECLARE @N INT=1;
WHILE @N<=4
BEGIN
  DECLARE @Sub DECIMAL(10,2)=@Precio*@N, @Tax DECIMAL(10,2)=ROUND(@Precio*@N*0.13,2), @Pedido INT;
  INSERT INTO Pedidos (IdCliente,IdEncargado,IdEstado,ClaveOperacion,FechaPedido,TipoEntrega,DireccionEntrega,CostoEnvio,Subtotal,Impuesto,Total,Observaciones,Activo)
  VALUES(@Cliente,NULL,@Entregada,CONCAT(N'SEED-',@N),DATEADD(DAY,-@N,GETDATE()),N'Recogida en tienda',NULL,0,@Sub,@Tax,@Sub+@Tax,N'Dato de demostración',1);
  SET @Pedido=SCOPE_IDENTITY();
  INSERT INTO DetallePedidos (IdPedido,IdProducto,IdCombo,Cantidad,PrecioUnitario,Subtotal,Impuesto,Observaciones) VALUES(@Pedido,@Producto,NULL,@N,@Precio,@Sub,@Tax,N'Preparación estándar');
  INSERT INTO Pagos (IdPedido,MetodoPago,FechaPago,TotalPagado,EstadoPago) VALUES(@Pedido,N'Efectivo',GETDATE(),@Sub+@Tax,N'Aprobado');
  INSERT INTO PedidoEstadoHistorial (IdPedido,IdEstado,FechaCambio,IdUsuario) VALUES(@Pedido,@Entregada,GETDATE(),NULL);
  SET @N+=1;
END;

IF OBJECT_ID('dbo.ScheduledTaskExecutions','U') IS NULL
BEGIN
  CREATE TABLE dbo.ScheduledTaskExecutions
  (
    IdScheduledTaskExecution INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ScheduledTaskExecutions PRIMARY KEY,
    TaskName NVARCHAR(80) NOT NULL,
    ExecutionKey NVARCHAR(120) NOT NULL,
    StartedAtUtc DATETIME2 NOT NULL,
    CompletedAtUtc DATETIME2 NULL,
    Status NVARCHAR(20) NOT NULL,
    Details NVARCHAR(500) NULL
  );
  CREATE UNIQUE INDEX UX_ScheduledTaskExecutions_Task_Key
    ON dbo.ScheduledTaskExecutions(TaskName,ExecutionKey);
END;

COMMIT TRANSACTION;
GO
