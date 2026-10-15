namespace CompressionLab.Compression;

public readonly record struct Rgb24(byte R, byte G, byte B);
public sealed record Run(byte Value, int Length);
public sealed record CompressionReport(int RawBytes, int QuantizedBytes, int RleBytes, int RunCount, IReadOnlyList<byte> QuantizedPixels);

public static class ImageCompression
{
    public static byte Quantize332(Rgb24 pixel) => (byte)((pixel.R >> 5 << 5) | (pixel.G >> 5 << 2) | (pixel.B >> 6));

    public static Rgb24 Expand332(byte value) => new(
        (byte)(((value >> 5) & 7) * 255 / 7),
        (byte)(((value >> 2) & 7) * 255 / 7),
        (byte)((value & 3) * 255 / 3));

    public static IReadOnlyList<Run> EncodeRle(IReadOnlyList<byte> pixels)
    {
        var runs = new List<Run>();
        for (var i = 0; i < pixels.Count;)
        {
            var value = pixels[i]; var end = i + 1;
            while (end < pixels.Count && pixels[end] == value) end++;
            runs.Add(new(value, end - i)); i = end;
        }
        return runs;
    }

    public static byte[] EncodeRleBytes(IReadOnlyList<byte> pixels)
    {
        const int maxRun = 0xFFFFFF;
        var bytes = new List<byte>();
        foreach (var run in EncodeRle(pixels))
        {
            var remaining = run.Length;
            while (remaining > 0)
            {
                var length = Math.Min(remaining, maxRun);
                bytes.Add(run.Value);
                bytes.Add((byte)length); bytes.Add((byte)(length >> 8)); bytes.Add((byte)(length >> 16));
                remaining -= length;
            }
        }
        return bytes.ToArray();
    }

    public static CompressionReport Compress(IReadOnlyList<Rgb24> pixels)
    {
        var quantized = pixels.Select(Quantize332).ToArray();
        var runs = EncodeRle(quantized);
        return new(pixels.Count * 3, quantized.Length, EncodeRleBytes(quantized).Length, runs.Count, quantized);
    }

    public static IReadOnlyList<Rgb24> CreateSample(int width, int height)
    {
        Rgb24[] palette = [new(230, 70, 70), new(55, 160, 115), new(75, 125, 220), new(240, 190, 65), new(145, 95, 200), new(45, 175, 190)];
        return Enumerable.Range(0, height).SelectMany(y => Enumerable.Range(0, width)
            .Select(x => palette[(x / Math.Max(1, width / 3) + (y / Math.Max(1, height / 2)) * 3) % palette.Length])).ToArray();
    }
}
