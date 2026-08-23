using Microsoft.EntityFrameworkCore;
using MoonBrewCoffee.Infrastructure.Data;

namespace MoonBrewCoffee.Web.Services
{
    public static class DatabaseSchemaInitializer
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            await using var scope = services.CreateAsyncScope();
            var context = scope.ServiceProvider.GetRequiredService<MoonBrewContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseSchemaInitializer");

            try
            {
                var commands = new[]
                {
                    "IF COL_LENGTH('dbo.Pedidos','IdEncargado') IS NULL ALTER TABLE dbo.Pedidos ADD IdEncargado INT NULL;",
                    "IF EXISTS(SELECT 1 FROM sys.columns WHERE object_id=OBJECT_ID('dbo.Usuarios') AND name='Correo' AND max_length=-1) BEGIN UPDATE dbo.Usuarios SET Correo=LEFT(LTRIM(RTRIM(Correo)),256); ALTER TABLE dbo.Usuarios ALTER COLUMN Correo NVARCHAR(256) NOT NULL; END;",
                    "IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('dbo.Usuarios') AND name='IX_Usuarios_Correo') AND NOT EXISTS(SELECT Correo FROM dbo.Usuarios GROUP BY Correo HAVING COUNT(*) > 1) CREATE UNIQUE INDEX IX_Usuarios_Correo ON dbo.Usuarios(Correo);",
                    "IF NOT EXISTS(SELECT 1 FROM sys.foreign_keys WHERE name='FK_Pedidos_Usuarios_IdEncargado') ALTER TABLE dbo.Pedidos WITH CHECK ADD CONSTRAINT FK_Pedidos_Usuarios_IdEncargado FOREIGN KEY(IdEncargado) REFERENCES dbo.Usuarios(IdUsuario);",
                    "IF COL_LENGTH('dbo.DetallePedidos','Impuesto') IS NULL ALTER TABLE dbo.DetallePedidos ADD Impuesto DECIMAL(10,2) NOT NULL CONSTRAINT DF_DetallePedidos_Impuesto DEFAULT(0);",
                    "IF COL_LENGTH('dbo.Pedidos','ClaveOperacion') IS NULL ALTER TABLE dbo.Pedidos ADD ClaveOperacion NVARCHAR(64) NULL;",
                    "UPDATE dbo.Pedidos SET ClaveOperacion=CONCAT(N'LEGACY-',IdPedido) WHERE ClaveOperacion IS NULL OR LTRIM(RTRIM(ClaveOperacion))=N'';",
                    "IF EXISTS(SELECT 1 FROM sys.columns WHERE object_id=OBJECT_ID('dbo.Pedidos') AND name='ClaveOperacion' AND is_nullable=1) ALTER TABLE dbo.Pedidos ALTER COLUMN ClaveOperacion NVARCHAR(64) NOT NULL;",
                    "IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('dbo.Pedidos') AND name='UX_Pedidos_ClaveOperacion') CREATE UNIQUE INDEX UX_Pedidos_ClaveOperacion ON dbo.Pedidos(ClaveOperacion);",
                    @"IF OBJECT_ID('dbo.PedidoProcesos','U') IS NULL CREATE TABLE dbo.PedidoProcesos(
                        IdPedidoProceso INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_PedidoProcesos PRIMARY KEY,
                        IdPedido INT NOT NULL, IdEstacion INT NOT NULL, Orden INT NOT NULL,
                        Descripcion NVARCHAR(180) NOT NULL, Estado NVARCHAR(30) NOT NULL CONSTRAINT DF_PedidoProcesos_Estado DEFAULT N'Pendiente',
                        FechaInicio DATETIME2 NULL, FechaFin DATETIME2 NULL, IdEncargado INT NULL,
                        CONSTRAINT FK_PedidoProcesos_Pedidos FOREIGN KEY(IdPedido) REFERENCES dbo.Pedidos(IdPedido) ON DELETE CASCADE,
                        CONSTRAINT FK_PedidoProcesos_Estaciones FOREIGN KEY(IdEstacion) REFERENCES dbo.EstacionesCocina(IdEstacion),
                        CONSTRAINT FK_PedidoProcesos_Usuarios FOREIGN KEY(IdEncargado) REFERENCES dbo.Usuarios(IdUsuario));",
                    "IF OBJECT_ID('dbo.PedidoProcesos','U') IS NOT NULL AND NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('dbo.PedidoProcesos') AND name='IX_PedidoProcesos_Pedido_Orden') CREATE INDEX IX_PedidoProcesos_Pedido_Orden ON dbo.PedidoProcesos(IdPedido,Orden);",
                    @"IF OBJECT_ID('dbo.PedidoEstadoHistorial','U') IS NULL CREATE TABLE dbo.PedidoEstadoHistorial(
                        IdHistorial INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_PedidoEstadoHistorial PRIMARY KEY,
                        IdPedido INT NOT NULL, IdEstado INT NOT NULL, FechaCambio DATETIME2 NOT NULL, IdUsuario INT NULL,
                        CONSTRAINT FK_PedidoEstadoHistorial_Pedidos FOREIGN KEY(IdPedido) REFERENCES dbo.Pedidos(IdPedido) ON DELETE CASCADE,
                        CONSTRAINT FK_PedidoEstadoHistorial_Estados FOREIGN KEY(IdEstado) REFERENCES dbo.EstadosPedido(IdEstado),
                        CONSTRAINT FK_PedidoEstadoHistorial_Usuarios FOREIGN KEY(IdUsuario) REFERENCES dbo.Usuarios(IdUsuario));",
                    "IF OBJECT_ID('dbo.PedidoEstadoHistorial','U') IS NOT NULL AND NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('dbo.PedidoEstadoHistorial') AND name='IX_PedidoEstadoHistorial_Pedido_Fecha') CREATE INDEX IX_PedidoEstadoHistorial_Pedido_Fecha ON dbo.PedidoEstadoHistorial(IdPedido,FechaCambio);",
                    "IF NOT EXISTS(SELECT 1 FROM dbo.EstadosPedido WHERE Nombre=N'Aceptada') INSERT dbo.EstadosPedido(Nombre,ColorHex) VALUES(N'Aceptada','#2F855A');",
                    "IF NOT EXISTS(SELECT 1 FROM dbo.EstadosPedido WHERE Nombre=N'Pendiente') INSERT dbo.EstadosPedido(Nombre,ColorHex) VALUES(N'Pendiente','#B7791F');",
                    "IF NOT EXISTS(SELECT 1 FROM dbo.EstadosPedido WHERE Nombre=N'Preparación') INSERT dbo.EstadosPedido(Nombre,ColorHex) VALUES(N'Preparación','#805AD5');",
                    "IF NOT EXISTS(SELECT 1 FROM dbo.EstadosPedido WHERE Nombre=N'Procesando') INSERT dbo.EstadosPedido(Nombre,ColorHex) VALUES(N'Procesando','#2B6CB0');",
                    "IF NOT EXISTS(SELECT 1 FROM dbo.EstadosPedido WHERE Nombre=N'Entregada') INSERT dbo.EstadosPedido(Nombre,ColorHex) VALUES(N'Entregada','#276749');",
                    "IF NOT EXISTS(SELECT 1 FROM dbo.EstacionesCocina WHERE Nombre=N'Barra') INSERT dbo.EstacionesCocina(Nombre,Descripcion,ColorHex,Activo) VALUES(N'Barra',N'Preparación de bebidas','#8B5E3C',1);",
                    "IF NOT EXISTS(SELECT 1 FROM dbo.EstacionesCocina WHERE Nombre=N'Cocina') INSERT dbo.EstacionesCocina(Nombre,Descripcion,ColorHex,Activo) VALUES(N'Cocina',N'Preparación de alimentos','#B7791F',1);",
                    "IF NOT EXISTS(SELECT 1 FROM dbo.EstacionesCocina WHERE Nombre=N'Repostería') INSERT dbo.EstacionesCocina(Nombre,Descripcion,ColorHex,Activo) VALUES(N'Repostería',N'Preparación de postres','#805AD5',1);",
                    "IF NOT EXISTS(SELECT 1 FROM dbo.EstacionesCocina WHERE Nombre=N'Empaque') INSERT dbo.EstacionesCocina(Nombre,Descripcion,ColorHex,Activo) VALUES(N'Empaque',N'Revisión y despacho','#2B6CB0',1);"
                    ,@"IF OBJECT_ID('dbo.ScheduledTaskExecutions','U') IS NULL CREATE TABLE dbo.ScheduledTaskExecutions(
                        IdScheduledTaskExecution INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_ScheduledTaskExecutions PRIMARY KEY,
                        TaskName NVARCHAR(80) NOT NULL,
                        ExecutionKey NVARCHAR(120) NOT NULL,
                        StartedAtUtc DATETIME2 NOT NULL,
                        CompletedAtUtc DATETIME2 NULL,
                        Status NVARCHAR(20) NOT NULL,
                        Details NVARCHAR(500) NULL);"
                    ,"IF OBJECT_ID('dbo.ScheduledTaskExecutions','U') IS NOT NULL AND NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('dbo.ScheduledTaskExecutions') AND name='UX_ScheduledTaskExecutions_Task_Key') CREATE UNIQUE INDEX UX_ScheduledTaskExecutions_Task_Key ON dbo.ScheduledTaskExecutions(TaskName,ExecutionKey);"
                };

                foreach (var command in commands)
                {
                    try
                    {
                        await context.Database.ExecuteSqlRawAsync(command);
                    }
                    catch (Exception commandException)
                    {
                        logger.LogWarning(
                            commandException,
                            "No fue posible aplicar una actualización de esquema; se continuará con las siguientes.");
                    }
                }
            }
            catch (Exception exception)
            {
                try
                {
                    logger.LogWarning(exception, "No fue posible comprobar el esquema del Avance 5 al iniciar.");
                }
                catch
                {
                    // El proveedor de Event Log puede no estar disponible en entornos restringidos.
                }
            }
        }
    }
}
