using Microsoft.EntityFrameworkCore;
using MoonBrewCoffee.Infrastructure.Data;
using MoonBrewCoffee.Infrastructure.Models.Entidades;
using MoonBrewCoffee.Infrastructure.Workflow;

namespace MoonBrewCoffee.Tests;

public class PedidoWorkflowTests
{
    [Fact]
    public void StartingFirstStep_MovesOrderToPreparation()
    {
        Assert.Equal("En preparación", PedidoWorkflowRules.AdvanceStep("Pendiente", false));
        Assert.Equal("Preparación", PedidoWorkflowRules.ResolveOrderStatus(false, false));
    }

    [Fact]
    public void CompletingIntermediateStep_MovesOrderToProcessing()
    {
        Assert.Equal("Completado", PedidoWorkflowRules.AdvanceStep("En preparación", true));
        Assert.Equal("Procesando", PedidoWorkflowRules.ResolveOrderStatus(true, false));
    }

    [Fact]
    public void CompletingLastStep_MovesOrderToDelivered()
    {
        Assert.Equal("Entregada", PedidoWorkflowRules.ResolveOrderStatus(true, true));
    }

    [Fact]
    public void CompletingPendingStep_IsRejected()
    {
        Assert.Throws<InvalidOperationException>(() => PedidoWorkflowRules.AdvanceStep("Pendiente", true));
    }

    [Fact]
    public void OperationKey_HasUniqueDatabaseIndex()
    {
        using var context = new MoonBrewContext(new DbContextOptionsBuilder<MoonBrewContext>()
            .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=MoonBrewModelTest;Trusted_Connection=True")
            .Options);
        var entity = context.Model.FindEntityType(typeof(Pedido));
        var index = entity!.GetIndexes().Single(item => item.Properties.Single().Name == nameof(Pedido.ClaveOperacion));
        Assert.True(index.IsUnique);
    }
}
