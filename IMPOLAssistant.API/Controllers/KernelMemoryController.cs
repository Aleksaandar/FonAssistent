using IMPOLAssistant.KernelMemory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.KernelMemory.DataFormats.Office;
using Microsoft.KernelMemory.DataFormats.Pdf;
using StackExchange.Redis;

namespace IMPOLAssistant.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KernelMemoryController : ControllerBase
    {
        private readonly IKernelMemoryService _kernelMemoryService;

        public KernelMemoryController(IKernelMemoryService kernelMemoryService)
        {
            _kernelMemoryService = kernelMemoryService;
        }

      [HttpPost("import-document")]
public async Task<IActionResult> ImportDocument([FromForm] IFormFile file, [FromForm] string documentId, [FromForm] string index)
{
    if (file == null || file.Length == 0)
    {
        return BadRequest("A valid file must be provided.");
    }

    if (string.IsNullOrEmpty(documentId))
    {
        return BadRequest("Document ID must be provided.");
    }
            if (string.IsNullOrEmpty(index))
            {
                return BadRequest("Index must be provided.");
            }

    try
    {
      
        var filePath = Path.Combine(Path.GetTempPath(),  Path.GetExtension(file.FileName));

      
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        
        await _kernelMemoryService.ImportDocumentAsync(filePath, documentId,index);

     
        System.IO.File.Delete(filePath);

        return Ok("Document imported successfully.");
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Internal server error: {ex.Message}");
    }
}

        [HttpPost("import-webpage")]
        public async Task<IActionResult> ImportWebPage([FromBody] ImportWebPageRequest request, string index)
        {
            if (string.IsNullOrEmpty(request.Url) || string.IsNullOrEmpty(request.DocId))
            {
                return BadRequest("URL and DocId must be provided.");
            }

            await _kernelMemoryService.ImportWebPageAsync(request.Url, request.DocId,index);
            return Ok("Web page imported successfully.");
        }
        [HttpPost("importExcel")]
        public async Task<IActionResult> ImportExcel(IFormFile file, [FromQuery] string docId)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Invalid file.");
            }

            var filePath = Path.GetTempFileName();

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
#pragma warning disable KMEXP00 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            var text = await new MsExcelDecoder().DecodeAsync("msexcelfile.xlsx");
#pragma warning restore KMEXP00 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            Console.WriteLine(text);

            await _kernelMemoryService.ImportExcelAsync(filePath, docId);

            return Ok("Excel file imported successfully.");
        }




        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] AskRequest request,  string index)
        {
            if (string.IsNullOrEmpty(request.Question))
            {
                return BadRequest("Question must be provided.");
            }

            var result = await _kernelMemoryService.AskAsync(request.Question, index);
            return Ok(result);
        }
        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] AskRequest request)
        {
            if (string.IsNullOrEmpty(request.Question))
            {
                return BadRequest("Question must be provided.");
            }

            var result = await _kernelMemoryService.SearchAsync(request.Question);
            return Ok(result);
        }

        public class ImportWebPageRequest
        {
            public string Url { get; set; }
            public string DocId { get; set; }
        }

        public class AskRequest
        {
            public string Question { get; set; }
        }

    }
}
