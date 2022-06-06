using System;
using System.Collections.Generic;

namespace Dithery_cli
{
	public static class ColorReductions
	{
		public static void TrueColorBytesToWebSafeColorBytes(in byte[] input, ref byte[] output)
		{
			for (int i = 0; i < input.Length; i++)
			{
				output[i] = (byte)(Math.Round(input[i] / 51.0) * 51);
			}
		}

		public static void TrueColorBytesToEGABytes(in byte[] input, ref byte[] output)
		{
			for (int i = 0; i < input.Length; i++)
			{
				output[i] = (byte)(Math.Round((byte)input[i] / 85.0) * 85);
			}
		}

		private static readonly List<byte[]> fullCGAColors = new List<byte[]>() 
		{
			new byte[] { 0x00, 0x00, 0x00 }, // black
			new byte[] { 0x00, 0x00, 0xAA }, // blue
			new byte[] { 0x00, 0xAA, 0x00 }, // green
			new byte[] { 0x00, 0xAA, 0xAA }, // cyan
			new byte[] { 0xAA, 0x00, 0x00 }, // red
			new byte[] { 0xAA, 0x00, 0xAA }, // magenta
			new byte[] { 0xAA, 0x55, 0x00 }, // brown
			new byte[] { 0xAA, 0xAA, 0xAA }, // light gray
			new byte[] { 0x55, 0x55, 0x55 }, // dark gray
			new byte[] { 0x55, 0x55, 0xFF }, // light blue
			new byte[] { 0x55, 0xFF, 0x55 }, // light green
			new byte[] { 0x55, 0xFF, 0xFF }, // light cyan
			new byte[] { 0xFF, 0x55, 0x55 }, // light red
			new byte[] { 0xFF, 0x55, 0xFF }, // light magenta
			new byte[] { 0xFF, 0xFF, 0x55 }, // yellow
			new byte[] { 0xFF, 0xFF, 0xFF }, // white
		};

		public static void TrueColorBytesToCGABytes(in byte[] input, ref byte[] output)
		{
			output = FindNearestColor(input, fullCGAColors);
		}

		private static readonly List<byte[]> c64Colors = new List<byte[]>() 
		{
			new byte[] { 0x00, 0x00, 0x00 }, // black
			new byte[] { 0xFF, 0xFF, 0xFF }, // white
			new byte[] { 0x88, 0x00, 0x00 }, // red
			new byte[] { 0xAA, 0xFF, 0xEE }, // cyan
			new byte[] { 0xCC, 0x44, 0xCC }, // violet / purple
			new byte[] { 0x00, 0xCC, 0x55 }, // green
			new byte[] { 0x00, 0x00, 0xAA }, // blue
			new byte[] { 0xEE, 0xEE, 0x77 }, // yellow
			new byte[] { 0xDD, 0x88, 0x55 }, // orange
			new byte[] { 0x66, 0x44, 0x00 }, // brown
			new byte[] { 0xFF, 0x77, 0x77 }, // light red
			new byte[] { 0x33, 0x33, 0x33 }, // dark grey
			new byte[] { 0x77, 0x77, 0x77 }, // grey
			new byte[] { 0xAA, 0xFF, 0x66 }, // light green
			new byte[] { 0x00, 0x88, 0xFF }, // light blue
			new byte[] { 0xBB, 0xBB, 0xBB }, // light grey     
		};

		public static void TrueColorBytesToC64Bytes(in byte[] input, ref byte[] output)
		{
			output = FindNearestColor(input, c64Colors);
		}

		private static byte[] FindNearestColor(byte[] actualColor, List<byte[]> allowedColors)
		{
			int index = 0;
			uint distance = DistanceBetween24BitColors(actualColor, allowedColors[0]);
			for (int i = 1; i < allowedColors.Count; i++)
			{
				uint possibleNewDistance = DistanceBetween24BitColors(actualColor, allowedColors[i]);
				if (possibleNewDistance < distance)
				{
					distance = possibleNewDistance;
					index = i;
				}
			}

			return allowedColors[index];
		}

		private static uint DistanceBetween24BitColors(byte[] firstColors, byte[] secondColors)
		{
			int redDifference = firstColors[0] - secondColors[0];
			int greenDifference = firstColors[1] - secondColors[1];
			int blueDifference = firstColors[2] - secondColors[2];
			return (uint)((redDifference * redDifference) + (greenDifference * greenDifference) + (blueDifference * blueDifference));
		}
	}
}