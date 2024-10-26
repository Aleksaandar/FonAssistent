using Microsoft.KernelMemory;
using OfficeOpenXml;


namespace IMPOLAssistant.KernelMemory
{
    public class KernelMemoryService : IKernelMemoryService
    {
        private readonly IKernelMemory _kernelMemory;

        public KernelMemoryService(IKernelMemory kernelMemory)
        {
            _kernelMemory = kernelMemory;
        }

        public Task ImportDocumentAsync(string filePath, string documentId, string index)
        {
            return _kernelMemory.ImportDocumentAsync(filePath, documentId, index: index);
        }

        public Task ImportWebPageAsync(string url, string docId, string index)
        {
            return _kernelMemory.ImportWebPageAsync(url, docId,index:index);
        }

        public async Task<string> AskAsync(string question,string index)
        {
            var result = await _kernelMemory.AskAsync(question,index:index);
            
            return result.Result;
        }
        public async Task<string> SearchAsync(string question)
        {
            var result = await _kernelMemory.SearchAsync(question);

          
            if (result.Results == null || !result.Results.Any())
            {
                return "No results found.";
            }

           
            var resultStrings = result.Results.Select(citation =>
                $"Source name: {citation.SourceName}, SourceUrl: {citation.SourceUrl}"); 
            var resultString = string.Join("\n", resultStrings);

            return resultString;
        }
        public async Task ImportExcelAsync(string filePath, string docId)
        {
            using var package = new ExcelPackage(new FileInfo(filePath));
            var worksheet = package.Workbook.Worksheets[0]; 

            for (int row = 1; row <= worksheet.Dimension.Rows; row++)
            {
                var content = "";
                for (int col = 1; col <= worksheet.Dimension.Columns; col++)
                {
                    content += worksheet.Cells[row, col].Text + " ";
                }

                
                await _kernelMemory.ImportDocumentAsync(content, $"{docId}_row{row}");
            }
        }
    }
}
