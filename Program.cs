using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;

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
		TrueColorToWebSafe
	}

	public enum OutputFormat
	{
		None,
		SingleImage,
		HTMLBasic,
	}

	class Program
	{
		private static DitheringBase GetDitherer(DitheringMethod method, Func<object[],object[]> colorfunc) => 
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

		private static void PrintHelp()
		{
			var ditheringMethodAsArray = Enum.GetValues(typeof(DitheringMethod));		
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
			Console.WriteLine(" Format (for output):");
			Console.WriteLine("  html for HTML file output");
		}

		private static void InvalidParametersComplain(string possibleError)
		{
			Console.WriteLine($"Cannot parse parameters! Error: {possibleError}");
			Console.WriteLine("Please use --help");
		}

		private static object[] TrueColorBytesToWebSafeColorBytes(object[] input)
		{
			object[] returnArray = new object[input.Length];
			for (int i = 0; i < returnArray.Length; i++)
			{
				returnArray[i] = (byte)(Math.Round((byte)input[i] / 51.0) * 51);
			}
			
			return returnArray;
		}

		static void Main(string[] args)
		{
			Console.WriteLine("dithery-cli");
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

			if (dithering == DitheringMethod.All)
			{

			}
			else
			{
				DitheringBase ditherer = GetDitherer(dithering, TrueColorBytesToWebSafeColorBytes);
				using(FileStream bitmapStream = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
				using(var image = new Bitmap(bitmapStream))
				{
					if (outputFormat == OutputFormat.SingleImage)
					{
						FileWriting.DitherAndWritePngFile(outputFile, image, ditherer);
					}
					else if (outputFormat == OutputFormat.HTMLBasic)
					{
						MemoryStream originalImageMemoryStream = new MemoryStream();
						image.Save(originalImageMemoryStream, System.Drawing.Imaging.ImageFormat.Png);

						MemoryStream ditheredImageMemoryStream = new MemoryStream();
						StreamWriting.DitherAndWritePngStream(ditheredImageMemoryStream, image, ditherer);

						File.WriteAllText(outputFile, HtmlWriter.GenerateSingleImageHtml((originalImageMemoryStream, "Original"), (ditheredImageMemoryStream, ditherer.GetMethodName())));
					}
				}
			}
		}
	}
}
