namespace IMPOLAssistant.SemanticKernel
{
    public interface ITabelaLeguraKernelService
    {
        Task<string> ProcessUserQueryAsync(string query);
    }
}

