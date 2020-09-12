using System;
using System.IO;
using System.Drawing;

namespace Dithery_cli
{
	public static class ReadWriteBitmaps
	{
		public static void WriteToBitmap(Bitmap bitmap, Func<int, int, byte[]> reader)
		{
			for (int y = 0; y < bitmap.Height; y++)
			{
				for (int x = 0; x < bitmap.Width; x++)
				{
					byte[] read = reader(x, y);
					Color color = Color.FromArgb((byte)read[0], (byte)read[1], (byte)read[2]);
					bitmap.SetPixel(x, y, color);
				}
			}
		}

		public static byte[] ReadBitmapToColorBytes(Bitmap bitmap)
		{
			int width = bitmap.Width;
			int height = bitmap.Height;
			int channelsPerPixel = 3;
			byte[] returnValue = new byte[width * height * channelsPerPixel];
			
			for (int y = 0; y < bitmap.Height; y++)
			{
				for (int x = 0; x < bitmap.Width; x++)
				{
					Color color = bitmap.GetPixel(x, y);
					int arrayIndex = y * width * channelsPerPixel + x * channelsPerPixel;
					returnValue[arrayIndex + 0] = color.R;
					returnValue[arrayIndex + 1] = color.G;
					returnValue[arrayIndex + 2] = color.B;
				}
			}
			
			return returnValue;
		}
	}
}