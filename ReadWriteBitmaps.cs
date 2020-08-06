using System;
using System.IO;
using System.Drawing;

namespace Dithery_cli
{
	public static class ReadWriteBitmaps
	{
		public static void WriteToBitmap(Bitmap bitmap, Func<int, int, object[]> reader)
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

		public static byte[,,] ReadBitmapToColorBytes(Bitmap bitmap)
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
	}
}