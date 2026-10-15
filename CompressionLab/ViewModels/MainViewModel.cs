using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CompressionLab.Compression;

namespace CompressionLab.ViewModels;

public sealed class PixelTile(Rgb24 pixel)
{
    public IBrush Fill { get; } = new SolidColorBrush(Color.FromRgb(pixel.R, pixel.G, pixel.B));
}

public partial class MainViewModel : ViewModelBase
{
    private readonly IReadOnlyList<Rgb24> _source = ImageCompression.CreateSample(24, 12);
    public ObservableCollection<PixelTile> Original { get; } = [];
    public ObservableCollection<PixelTile> Quantized { get; } = [];
    [ObservableProperty] private string _report = "";

    public MainViewModel()
    {
        foreach (var pixel in _source) Original.Add(new(pixel));
        Compress();
    }

    [RelayCommand]
    private void Compress()
    {
        var result = ImageCompression.Compress(_source);
        Quantized.Clear();
        foreach (var pixel in result.QuantizedPixels) Quantized.Add(new(ImageCompression.Expand332(pixel)));
        var saving = 100d * (result.RawBytes - result.RleBytes) / result.RawBytes;
        Report = $"Original RGB: {result.RawBytes} bytes · RGB332 quantizado: {result.QuantizedBytes} bytes · RLE sobre RGB332: {result.RleBytes} bytes ({result.RunCount} sequências) · redução estimada: {saving:F0}%.";
    }
}
