using Microsoft.SemanticKernel;


namespace IMPOLAssistant.SemanticKernel
{

    public class SemanticKernelService : ISemanticKernelService
    {
        private readonly Kernel kernel;


        public SemanticKernelService(Kernel kernel)
        {
            this.kernel = kernel;
        }

        public async Task<string> ProcessUserQueryAsync(string query)
        {
            var result = await kernel.InvokePromptAsync(query);
            var value = result.GetValue<string>();
            return value != null ? value : string.Empty;
        }
    }
}

