using System;
using System.IO;
using System.Text;

namespace Dithery_cli
{
	public static class HtmlWriter
	{
		private static readonly string singleFileTemplate = @"<!doctype html>
<html lang=""en"">
<head>
  <meta charset=""utf-8"">
  <title>Dithery output</title>
  <meta name=""description"" content=""Generated with Dithery"">
</head>
<body>
{0} <br> {1}
</body>
</html>";

private static readonly string allFilesTemplate = @"<!doctype html>
<html lang=""en"">
<head>
  <meta charset=""utf-8"">
  <title>Dithery output</title>
  <meta name=""description"" content=""Generated with Dithery"">
</head>
<body>
{0}
</body>
</html>";

		public static string GenerateSingleImageHtml((MemoryStream pngData, string text) original, (MemoryStream pngData, string text) dithered)
		{
			string originalImageHTML = GenerateImgSrcContent(original.pngData, original.text);
			string ditheredImageHTML = GenerateImgSrcContent(dithered.pngData, dithered.text);
			return string.Format(singleFileTemplate, originalImageHTML, ditheredImageHTML);
		}

		public static string GenerateMultiImageHtml((MemoryStream pngData, string text)[] images)
		{
			StringBuilder builder = new StringBuilder();
			foreach ((MemoryStream pngData, string text) in images)
			{
				builder.AppendLine($"{GenerateImgSrcContent(pngData, text)} <br>");
			}
			return string.Format(allFilesTemplate, builder.ToString());
		}

		private static string GenerateImgSrcContent(MemoryStream pngMemoryStream, string title)
		{
			return $"<h3>{title}</h3><img alt=\"{title}\" src=\"data:image/png;base64,{Convert.ToBase64String(pngMemoryStream.ToArray())}\" />";
		}
	}
}