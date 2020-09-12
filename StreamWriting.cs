using System.IO;
using System.Drawing;

namespace Dithery_cli
{
	public static class StreamWriting
	{
		public static void DitherAndWritePngStream(Stream outputStream, Bitmap bitmap, DitheringBase<byte> ditherer, bool writeToSameBitmap = true)
		{
			byte[] bytes = ReadWriteBitmaps.ReadBitmapToColorBytes(bitmap);

			TempByteImageFormat temp = new TempByteImageFormat(bytes, bitmap.Width, bitmap.Height, 3);
			temp = (TempByteImageFormat)ditherer.DoDithering(temp);

			if (writeToSameBitmap)
			{
				ReadWriteBitmaps.WriteToBitmap(bitmap, temp.GetPixelChannels);
				bitmap.Save(outputStream, System.Drawing.Imaging.ImageFormat.Png);
			}
			else
			{
				Bitmap tempBitmap = new Bitmap(bitmap.Width, bitmap.Height, bitmap.PixelFormat);
				ReadWriteBitmaps.WriteToBitmap(tempBitmap, temp.GetPixelChannels);
				tempBitmap.Save(outputStream, System.Drawing.Imaging.ImageFormat.Png);
			}
		}
	}
}