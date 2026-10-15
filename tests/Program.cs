using CompressionLab.Compression;

var runs = ImageCompression.EncodeRle([1, 1, 1, 2, 2, 3]);
if (runs.Count != 3 || runs[0].Length != 3 || runs[1].Value != 2) throw new Exception("RLE output is incorrect.");
var report = ImageCompression.Compress(ImageCompression.CreateSample(24, 12));
if (report.RawBytes != 864 || report.QuantizedBytes != 288 || report.RleBytes != ImageCompression.EncodeRleBytes(report.QuantizedPixels).Length || report.RleBytes >= report.RawBytes) throw new Exception("Compression size calculation is invalid.");
Console.WriteLine("Compression checks passed.");
