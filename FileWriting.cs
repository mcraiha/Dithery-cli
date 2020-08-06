using System;
using System.IO;
using System.Drawing;

namespace Dithery_cli
{
	public static class FileWriting
	{
		public static void DitherAndWritePngFile(string outputFilename, Bitmap bitmap, DitheringBase ditherer, bool writeToSameBitmap = true)
		{
			byte[,,] bytes = ReadWriteBitmaps.ReadBitmapToColorBytes(bitmap);

			TempByteImageFormat temp = new TempByteImageFormat(bytes);
			temp = (TempByteImageFormat)ditherer.DoDithering(temp);

			if (writeToSameBitmap)
			{
				ReadWriteBitmaps.WriteToBitmap(bitmap, temp.GetPixelChannels);
				bitmap.Save(outputFilename, System.Drawing.Imaging.ImageFormat.Png);
			}
		}
	}
}