using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMPOLAssistant.KernelMemory
{
    public interface IKernelMemoryService
    {
        Task ImportDocumentAsync(string filePath, string documentId, string index);
        Task<string> AskAsync(string question, string index);
        Task<string> SearchAsync(string question);
        Task ImportWebPageAsync(string url, string docId, string index);
        Task ImportExcelAsync(string filePath, string docId);


    }
}
