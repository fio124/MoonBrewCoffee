namespace MoonBrewCoffee.Infrastructure.Workflow
{
    public static class PedidoWorkflowRules
    {
        public static string AdvanceStep(string currentState, bool complete)
        {
            if (complete)
            {
                if (currentState != "En preparación")
                    throw new InvalidOperationException("La etapa debe iniciarse antes de completarse.");
                return "Completado";
            }

            if (currentState != "Pendiente")
                throw new InvalidOperationException("Esta etapa ya fue iniciada.");
            return "En preparación";
        }

        public static string ResolveOrderStatus(bool completedAction, bool allStepsCompleted) =>
            allStepsCompleted ? "Entregada" : completedAction ? "Procesando" : "Preparación";
    }
}
