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

		private static byte[,,] ReadBitmapToColorBytes(Bitmap bitmap)
		{
			byte[,,] returnValue = new byte[bitmap.Width, bitmap.Height, 3];
			for (int x = 0; x < bitmap.Width; x++)
			{
				for (int y = 0; y < bitmap.Height; y++)
				{
					Color color = bitmap.GetPixel(x, y);
					returnValue[x, y, 0] = color.R;
					returnValue[x, y, 1] = color.G;
					returnValue[x, y, 2] = color.B;
				}
			}
			return returnValue;
		}

		private static void WriteToBitmap(Bitmap bitmap, Func<int, int, object[]> reader)
		{
			for (int x = 0; x < bitmap.Width; x++)
			{
				for (int y = 0; y < bitmap.Height; y++)
				{
					object[] read = reader(x, y);
					Color color = Color.FromArgb((byte)read[0], (byte)read[1], (byte)read[2]);
					bitmap.SetPixel(x, y, color);
				}
			}
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

			DitheringBase ditherer = GetDitherer(dithering, TrueColorBytesToWebSafeColorBytes);
			using(FileStream bitmapStream = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
			using(var image = new Bitmap(bitmapStream))
			{
				byte[,,] bytes = ReadBitmapToColorBytes(image);

				TempByteImageFormat temp = new TempByteImageFormat(bytes);
				temp = (TempByteImageFormat)ditherer.DoDithering(temp);

				if (outputFormat == OutputFormat.SingleImage)
				{
					// Use same bitmap for writing since we only save one image
					WriteToBitmap(image, temp.GetPixelChannels);

					image.Save(outputFile);
				}
				else if (outputFormat == OutputFormat.HTMLBasic)
				{
					MemoryStream originalImageMemoryStream = new MemoryStream();
					MemoryStream ditheredImageMemoryStream = new MemoryStream();

					image.Save(originalImageMemoryStream, System.Drawing.Imaging.ImageFormat.Png);

					WriteToBitmap(image, temp.GetPixelChannels);

					image.Save(ditheredImageMemoryStream, System.Drawing.Imaging.ImageFormat.Png);

					File.WriteAllText(outputFile, HtmlWriter.GenerateSingleImageHtml((originalImageMemoryStream, "Original"), (ditheredImageMemoryStream, ditherer.GetMethodName())));
				}
			}
		}
	}
}
