using System;
using System.IO;
using System.Collections.Generic;
using SkiaSharp;

using System.Linq;

namespace Dithery_cli
{
	public enum DitheringMethod
	{
		None,
		All,
		Atkinson,
		Burkes,
		Fake,
		FloydSteinberg,
		JarvisJudiceNinke,
		Sierra,
		SierraLite,
		SierraTwoRow,
		Stucki
	}

	public enum ColorReductionMethod
	{
		None,
		TrueColorToWebSafe,
		TrueColorToFullCGA,
		TrueColorToFullEGA,
		TrueColorToFullC64,
		TrueColorToFullPICO8
	}

	public enum OutputFormat
	{
		None,
		SingleImage,
		HTMLBasic,
	}

	public class VersionInfos
	{
		public string GetAssemblyVersion()
		{
			string returnValue = GetType().Assembly.GetName().Version.ToString();
			return returnValue.Remove(returnValue.Length - 2);
		}
	}

	class Program
	{
		private static DitheringBase<byte> GetDitherer(DitheringMethod method, DitheringBase<byte>.ColorFunction colorfunc) => 
		method switch
		{
			DitheringMethod.Atkinson => new AtkinsonDitheringRGBByte(colorfunc),
			DitheringMethod.Burkes => new BurkesDitheringRGBByte(colorfunc),
			DitheringMethod.Fake => new FakeDitheringRGBByte(colorfunc),
			DitheringMethod.FloydSteinberg => new FloydSteinbergDitheringRGBByte(colorfunc),
			DitheringMethod.JarvisJudiceNinke => new JarvisJudiceNinkeDitheringRGBByte(colorfunc),
			DitheringMethod.Sierra => new SierraDitheringRGBByte(colorfunc),
			DitheringMethod.SierraLite => new SierraLiteDitheringRGBByte(colorfunc),
			DitheringMethod.SierraTwoRow => new SierraTwoRowDitheringRGBByte(colorfunc),
			DitheringMethod.Stucki => new StuckiDitheringRGBByte(colorfunc),
			_ => throw new ArgumentException(message: "invalid dithering", paramName: method.ToString()),
		};

		private static DitheringBase<byte>.ColorFunction GetColorReductionMethod(ColorReductionMethod method) =>
		method switch
		{
			ColorReductionMethod.TrueColorToWebSafe => ColorReductions.TrueColorBytesToWebSafeColorBytes,
			ColorReductionMethod.TrueColorToFullCGA => ColorReductions.TrueColorBytesToCGABytes,
			ColorReductionMethod.TrueColorToFullEGA => ColorReductions.TrueColorBytesToEGABytes,
			ColorReductionMethod.TrueColorToFullC64 => ColorReductions.TrueColorBytesToC64Bytes,
			ColorReductionMethod.TrueColorToFullPICO8 => ColorReductions.TrueColorBytesToPICO8Bytes,
			_ => throw new ArgumentException(message: "invalid color reduction", paramName: method.ToString()),
		};

		private static readonly Dictionary<DitheringMethod, string> ditheringDescriptions = new Dictionary<DitheringMethod, string>()
		{
			{ DitheringMethod.All, "Apply all dithering methods to create multiple files" },
			{ DitheringMethod.Atkinson, "Atkinson dithering" },
			{ DitheringMethod.Burkes, "Burkes dithering" },
			{ DitheringMethod.Fake, "Fake means no dithering (but color reduction is still done)" },
			{ DitheringMethod.FloydSteinberg, "Floyd-Steinberg dithering" },
			{ DitheringMethod.JarvisJudiceNinke, "Jarvis-Judice-Ninke dithering" },
			{ DitheringMethod.Sierra, "Sierra dithering" },
			{ DitheringMethod.SierraLite, "Sierra lite dithering" },
			{ DitheringMethod.SierraTwoRow, "Sierra two row dithering" },
			{ DitheringMethod.Stucki, "Stucki dithering" },
		};

		private static readonly Dictionary<ColorReductionMethod, string> colorReductionDescriptions = new Dictionary<ColorReductionMethod, string>()
		{
			{ ColorReductionMethod.TrueColorToWebSafe, "True colors to Web safe colors (216 different colors)" },
			{ ColorReductionMethod.TrueColorToFullCGA, "True colors to full palette CGA colors (16 different colors)" },
			{ ColorReductionMethod.TrueColorToFullEGA, "True colors to full palette EGA colors (64 different colors)" },
			{ ColorReductionMethod.TrueColorToFullC64, "True colors to full palette C64 colors (16 different colors)" },
			{ ColorReductionMethod.TrueColorToFullPICO8, "True colors to full palette PICO-8 colors (16 different colors)" },
		};

		private static readonly Dictionary<OutputFormat, string> outputFormatDescriptions = new Dictionary<OutputFormat, string>()
		{
			{ OutputFormat.SingleImage, "Output a single image file (or image files in case of All dither method)" },
			{ OutputFormat.HTMLBasic, "Output a single HTML file" },
		};

		private static void PrintHelp()
		{
			var ditheringMethodAsArray = Enum.GetValues(typeof(DitheringMethod));
			var colorReductionMethodAsArray = Enum.GetValues(typeof(ColorReductionMethod));	
			var outputFormatsAsArray = Enum.GetValues(typeof(OutputFormat));	
			Console.WriteLine("");
			Console.WriteLine("Dithery-cli is a command-line image dithering tool");
			Console.WriteLine("");
			Console.WriteLine("-- Help --");
			Console.WriteLine(" Usage:");
			Console.WriteLine(" dithery imagefile -m dithering_method -c color_reduction_method -f format -o outputfile");
			Console.WriteLine("");
			Console.WriteLine(" Dithering methods (for output):");
			foreach (DitheringMethod method in ditheringMethodAsArray)
			{
				if (method == DitheringMethod.None)
				{
					continue;
				}
				Console.WriteLine($"  {method} - {ditheringDescriptions[method]}");
			}
			Console.WriteLine("");
			Console.WriteLine(" Color reduction methods (for output):");
			foreach (ColorReductionMethod method in colorReductionMethodAsArray)
			{
				if (method == ColorReductionMethod.None)
				{
					continue;
				}
				Console.WriteLine($"  {method} - {colorReductionDescriptions[method]}");
			}

			Console.WriteLine("");
			Console.WriteLine(" Format (for output):");
			foreach (OutputFormat format in outputFormatsAsArray)
			{
				if (format == OutputFormat.None)
				{
					continue;
				}
				Console.WriteLine($"  {format} - {outputFormatDescriptions[format]}");
			}
		}

		private static void InvalidParametersComplain(string possibleError)
		{
			Console.WriteLine($"Cannot parse parameters! Error: {possibleError}");
			Console.WriteLine("Please use --help");
		}

		static void Main(string[] args)
		{
			Console.WriteLine($"dithery-cli v{new VersionInfos().GetAssemblyVersion()}");
			if (args.Length == 0 || args[0] == "--help" || args[0] == "-h")
			{
				PrintHelp();
				return;
			}

			(bool valid, string possibleError, DitheringMethod dithering, ColorReductionMethod colorReduction, OutputFormat outputFormat, string inputFile, string outputFile) = CmdParsing.ParseParameters(args);

			if (!valid)
			{
				InvalidParametersComplain(possibleError);
				return;
			}

			var colorReductionMethod = GetColorReductionMethod(colorReduction);

			if (dithering == DitheringMethod.All)
			{
				var valuesAsList = new List<DitheringMethod>(Enum.GetValues(typeof(DitheringMethod)).Cast<DitheringMethod>());
				valuesAsList.Remove(DitheringMethod.All);
				valuesAsList.Remove(DitheringMethod.None);

				using(FileStream bitmapStream = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
				using(var image = SKBitmap.Decode(bitmapStream))
				{
					if (outputFormat == OutputFormat.SingleImage)
					{
						foreach (DitheringMethod ditheringMethod in valuesAsList)
						{
							DitheringBase<byte> ditherer = GetDitherer(ditheringMethod, colorReductionMethod);
							string modifiedOutputFile = outputFile.Replace(".png", $"{ditherer.GetFilenameAddition()}.png");
							if (File.Exists(modifiedOutputFile))
							{
								Console.WriteLine($"Cannot overwrite existing file {modifiedOutputFile}");
								return;
							}
							FileWriting.DitherAndWritePngFile(modifiedOutputFile, image, ditherer, writeToSameBitmap: false);
						}
					}
					else if (outputFormat == OutputFormat.HTMLBasic)
					{
						List<(MemoryStream pngData, string text)> images = new List<(MemoryStream pngData, string text)>();

						MemoryStream originalImageMemoryStream = new MemoryStream();
						image.Encode(originalImageMemoryStream, SKEncodedImageFormat.Png, quality: 100);
						images.Add((originalImageMemoryStream, "Original"));
						
						foreach (DitheringMethod ditheringMethod in valuesAsList)
						{
							DitheringBase<byte> ditherer = GetDitherer(ditheringMethod, colorReductionMethod);
							MemoryStream ditheredImageMemoryStream = new MemoryStream();
							StreamWriting.DitherAndWritePngStream(ditheredImageMemoryStream, image, ditherer, writeToSameBitmap: false);
							images.Add((ditheredImageMemoryStream, ditherer.GetMethodName()));
						}

						File.WriteAllText(outputFile, HtmlWriter.GenerateMultiImageHtml(images.ToArray()));
					}
				}
			}
			else
			{
				DitheringBase<byte> ditherer = GetDitherer(dithering, colorReductionMethod);
				using(FileStream bitmapStream = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
				using(var image = SKBitmap.Decode(bitmapStream))
				{
					if (outputFormat == OutputFormat.SingleImage)
					{
						FileWriting.DitherAndWritePngFile(outputFile, image, ditherer);
					}
					else if (outputFormat == OutputFormat.HTMLBasic)
					{
						MemoryStream originalImageMemoryStream = new MemoryStream();
						image.Encode(originalImageMemoryStream, SKEncodedImageFormat.Png, quality: 100);

						MemoryStream ditheredImageMemoryStream = new MemoryStream();
						StreamWriting.DitherAndWritePngStream(ditheredImageMemoryStream, image, ditherer);

						File.WriteAllText(outputFile, HtmlWriter.GenerateSingleImageHtml((originalImageMemoryStream, "Original"), (ditheredImageMemoryStream, ditherer.GetMethodName())));
					}
				}
			}
		}
	}
}
