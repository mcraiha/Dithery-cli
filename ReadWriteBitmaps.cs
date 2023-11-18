using System;
using System.IO;
using SkiaSharp;

namespace Dithery_cli
{
	public static class ReadWriteBitmaps
	{
		public static void WriteToBitmap(SKBitmap bitmap, Func<int, int, byte[]> reader)
		{
			for (int y = 0; y < bitmap.Height; y++)
			{
				for (int x = 0; x < bitmap.Width; x++)
				{
					byte[] read = reader(x, y);
					SKColor color = new SKColor((byte)read[0], (byte)read[1], (byte)read[2]);
					bitmap.SetPixel(x, y, color);
				}
			}
		}

		public static byte[] ReadBitmapToColorBytes(SKBitmap bitmap)
		{
			int width = bitmap.Width;
			int height = bitmap.Height;
			int channelsPerPixel = 3;
			byte[] returnValue = new byte[width * height * channelsPerPixel];
			
			for (int y = 0; y < bitmap.Height; y++)
			{
				for (int x = 0; x < bitmap.Width; x++)
				{
					SKColor color = bitmap.GetPixel(x, y);
					int arrayIndex = y * width * channelsPerPixel + x * channelsPerPixel;
					returnValue[arrayIndex + 0] = color.Red;
					returnValue[arrayIndex + 1] = color.Green;
					returnValue[arrayIndex + 2] = color.Blue;
				}
			}
			
			return returnValue;
		}
	}
}