# compression-lab

Compare source pixels with RGB332 quantization and run-length encoding (RLE). The app shows the resulting image and byte counts.

## Run

Requires the .NET 10 SDK. Browser builds also need the WebAssembly workload: `dotnet workload install wasm-tools`.

### Desktop

~~~sh
dotnet run --project CompressionLab.Desktop/CompressionLab.Desktop.csproj
~~~

### Browser

~~~sh
dotnet run --project CompressionLab.Browser/CompressionLab.Browser.csproj
~~~

## Example

Run the compression demo and compare the image and byte counts.
