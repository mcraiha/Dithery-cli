using System;
using System.Collections.Generic;

namespace Dithery_cli
{
	public static class ColorReductions
	{
		public static object[] TrueColorBytesToWebSafeColorBytes(object[] input)
		{
			object[] returnArray = new object[input.Length];
			for (int i = 0; i < returnArray.Length; i++)
			{
				returnArray[i] = (byte)(Math.Round((byte)input[i] / 51.0) * 51);
			}
			
			return returnArray;
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

		public static object[] TrueColorBytesToCGABytes(object[] input)
		{
			object[] returnArray = new object[input.Length];
			byte[] tempColor = new byte[input.Length];
			for (int i = 0; i < tempColor.Length; i++)
			{
				tempColor[i] = (byte)input[i];
			}

			tempColor = FindNearestColor(tempColor, fullCGAColors);

			for (int i = 0; i < returnArray.Length; i++)
			{
				returnArray[i] = tempColor[i];
			}
			
			return returnArray;
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