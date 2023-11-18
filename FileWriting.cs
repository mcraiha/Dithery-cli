using System;
using System.IO;
using SkiaSharp;

namespace Dithery_cli
{
	public static class FileWriting
	{
		public static void DitherAndWritePngFile(string outputFilename, SKBitmap bitmap, DitheringBase<byte> ditherer, bool writeToSameBitmap = true)
		{
			byte[] bytes = ReadWriteBitmaps.ReadBitmapToColorBytes(bitmap);

			TempByteImageFormat temp = new TempByteImageFormat(bytes, bitmap.Width, bitmap.Height, 3);
			temp = (TempByteImageFormat)ditherer.DoDithering(temp);

			using FileStream fs = new(outputFilename, FileMode.Create);
			if (writeToSameBitmap)
			{
				ReadWriteBitmaps.WriteToBitmap(bitmap, temp.GetPixelChannels);
				bitmap.Encode(fs, SKEncodedImageFormat.Png, quality: 100);
			}
			else
			{
				SKBitmap tempBitmap = new SKBitmap(bitmap.Width, bitmap.Height, bitmap.ColorType, bitmap.AlphaType);
				ReadWriteBitmaps.WriteToBitmap(tempBitmap, temp.GetPixelChannels);
				tempBitmap.Encode(fs, SKEncodedImageFormat.Png, quality: 100);
			}
		}
	}
}