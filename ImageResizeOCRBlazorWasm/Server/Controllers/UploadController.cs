using ImageResizeOCRBlazorWasm.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ImageResizeOCRBlazorWasm.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IWebHostEnvironment env;

        public UploadController(IWebHostEnvironment env)
        {
            this.env = env;
        }


        [HttpPost]
        public async Task<string?> Post([FromBody] ImageFile file)
        {
            if (file!=null)
            {
                var buf = Convert.FromBase64String(file.base64data);
                string filePath = Path.Combine(env.ContentRootPath, "upload"
                    , Guid.NewGuid().ToString("N") + "-" + file.fileName);
                await System.IO.File.WriteAllBytesAsync(filePath, buf);

                try
                {
                //Azure FormRecognizer
                    ImageRecognizer ir = new ImageRecognizer();
                    var stream = new MemoryStream(Convert.FromBase64String(file.base64data));
                    string img_Text = await ir.Process(stream);
                    return img_Text;
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
                
            }
            return null;
        }
    }
}
