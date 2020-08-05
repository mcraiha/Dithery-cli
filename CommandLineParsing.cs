using System;
using System.IO;

namespace Dithery_cli
{
	public static class CmdParsing
	{
		public static (bool valid, string possibleError, DitheringMethod dithering, ColorReductionMethod colorReduction, OutputFormat outputFormat, string inputFile, string outputFile) ParseParameters(string[] args)
		{
			if (!File.Exists(args[0]))
			{
				return (false, "Cannot find input file!", DitheringMethod.None, ColorReductionMethod.None, OutputFormat.None, "", "");
			}

			string input = args[0];

			int ditheringMethodValueIndex = FindParameterIndex(args, "-m");
			if (ditheringMethodValueIndex < 0)
			{
				return (false, "Missing parameter -m !", DitheringMethod.None, ColorReductionMethod.None, OutputFormat.None, "", "");
			}
			ditheringMethodValueIndex++;
			if (ditheringMethodValueIndex == args.Length)
			{
				return (false, "Missing parameter -m value !", DitheringMethod.None, ColorReductionMethod.None, OutputFormat.None, "", "");
			}

			int colorReductionMethodValueIndex = FindParameterIndex(args, "-c");
			if (colorReductionMethodValueIndex < 0)
			{
				return (false, "Missing parameter -c !", DitheringMethod.None, ColorReductionMethod.None, OutputFormat.None, "", "");
			}
			colorReductionMethodValueIndex++;
			if (colorReductionMethodValueIndex == args.Length)
			{
				return (false, "Missing parameter -c value !", DitheringMethod.None, ColorReductionMethod.None, OutputFormat.None, "", "");
			}

			int formatValueIndex = FindParameterIndex(args, "-f");
			if (formatValueIndex < 0)
			{
				return (false, "Missing parameter -f !", DitheringMethod.None, ColorReductionMethod.None, OutputFormat.None, "", "");
			}
			formatValueIndex++;
			if (formatValueIndex == args.Length)
			{
				return (false, "Missing parameter -f value !", DitheringMethod.None, ColorReductionMethod.None, OutputFormat.None, "", "");
			}

			int outputFileValueIndex = FindParameterIndex(args, "-o");
			if (outputFileValueIndex < 0)
			{
				return (false, "Missing parameter -o !", DitheringMethod.None, ColorReductionMethod.None, OutputFormat.None, "", "");
			}
			outputFileValueIndex++;
			if (outputFileValueIndex == args.Length)
			{
				return (false, "Missing parameter -o value !", DitheringMethod.None, ColorReductionMethod.None, OutputFormat.None, "", "");
			}

			DitheringMethod ditheringMethod = DitheringMethod.None;
			string ditheringMethodString = args[ditheringMethodValueIndex];
			try 
			{
				ditheringMethod = (DitheringMethod) Enum.Parse(typeof(DitheringMethod), ditheringMethodString, ignoreCase: true);
				if (!Enum.IsDefined(typeof(DitheringMethod), ditheringMethod))
				{
					return (false, $"Cannot parse dithering method from {ditheringMethodString} !", DitheringMethod.None, ColorReductionMethod.None, OutputFormat.None, "", "");
				}
		 	}
		 	catch
			{
				return (false, $"Cannot parse dithering method from {ditheringMethodString} !", DitheringMethod.None, ColorReductionMethod.None, OutputFormat.None, "", "");
		 	}
			
			ColorReductionMethod colorReductionMethod = ColorReductionMethod.None;
			string colorReductionMethodString = args[colorReductionMethodValueIndex];
			try 
			{
            	colorReductionMethod = (ColorReductionMethod) Enum.Parse(typeof(ColorReductionMethod), colorReductionMethodString, ignoreCase: true);
            	if (!Enum.IsDefined(typeof(ColorReductionMethod), colorReductionMethod))
				{
					return (false, $"Cannot parse color reduction method from {colorReductionMethodString} !", DitheringMethod.None, ColorReductionMethod.None, OutputFormat.None, "", "");
				}
         	}
         	catch
			{
            	return (false, $"Cannot parse color reduction method from {colorReductionMethodString} !", DitheringMethod.None, ColorReductionMethod.None, OutputFormat.None, "", "");
         	}

			OutputFormat outputFormat = OutputFormat.None;
			string outputFormatString = args[formatValueIndex];
			try 
			{
            	outputFormat = (OutputFormat) Enum.Parse(typeof(OutputFormat), outputFormatString, ignoreCase: true);
            	if (!Enum.IsDefined(typeof(OutputFormat), outputFormat))
				{
					return (false, $"Cannot parse output format from {outputFormatString} !", DitheringMethod.None, ColorReductionMethod.None, OutputFormat.None, "", "");
				}
         	}
         	catch
			{
            	return (false, $"Cannot parse output format from {outputFormatString} !", DitheringMethod.None, ColorReductionMethod.None, OutputFormat.None, "", "");
         	}

			string outputFileName = args[outputFileValueIndex];
			if (File.Exists(outputFileName))
			{
				return (false, $"Cannot overwrite existing file {outputFileName} !", DitheringMethod.None, ColorReductionMethod.None, OutputFormat.None, "", "");
			}

			return (true, "", ditheringMethod, colorReductionMethod, outputFormat, input, outputFileName);
		}

		private static int FindParameterIndex(string[] args, string toSeek)
		{
			return Array.FindIndex(args, item => item == toSeek);
		}
	}
}