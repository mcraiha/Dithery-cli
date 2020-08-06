using System.IO;
using System.Drawing;

namespace Dithery_cli
{
	public static class StreamWriting
	{
		public static void DitherAndWritePngStream(Stream outputStream, Bitmap bitmap, DitheringBase ditherer, bool writeToSameBitmap = true)
		{
			byte[,,] bytes = ReadWriteBitmaps.ReadBitmapToColorBytes(bitmap);

			TempByteImageFormat temp = new TempByteImageFormat(bytes);
			temp = (TempByteImageFormat)ditherer.DoDithering(temp);

			if (writeToSameBitmap)
			{
				ReadWriteBitmaps.WriteToBitmap(bitmap, temp.GetPixelChannels);
				bitmap.Save(outputStream, System.Drawing.Imaging.ImageFormat.Png);
			}
		}
	}
}