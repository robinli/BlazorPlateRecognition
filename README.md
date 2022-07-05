
# Blazor Plate Recognition

## Development
- Visual Studio 2022
- Blazor Web Assembly Project
- .NET 6
- Azure Computer Vision


## First Step before Run

Add your Computer Vision subscription key and endpoint

```
namespace ImageResizeOCRBlazorWasm.Server
{
    public class ImageRecognizer
    {
        // Add your Computer Vision subscription key and endpoint
        static string subscriptionKey = "YOUR_KEY_1";
        static string endpoint = "YOUR_Endpoint";
	}	
}
```

## Reference

- [Blazor: Resize and Upload Image Files](https://www.prowaretech.com/articles/current/blazor/wasm/resize-and-upload-image-files).
- [Quickstart: Optical character recognition (OCR)](https://docs.microsoft.com/en-us/azure/cognitive-services/Computer-vision/quickstarts-sdk/client-library?pivots=programming-language-csharp&tabs=visual-studio).
- [【c#】計算C#中不規則多邊形的面積](https://www.796t.com/post/NXI5ZTA=.html).
