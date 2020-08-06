using System;
using System.IO;

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

		public static string GenerateSingleImageHtml((MemoryStream pngData, string text) original, (MemoryStream pngData, string text) dithered)
		{
			string originalImageHTML = GenerateImgSrcContent(original.pngData, original.text);
			string ditheredImageHTML = GenerateImgSrcContent(dithered.pngData, dithered.text);
			return string.Format(singleFileTemplate, originalImageHTML, ditheredImageHTML);
		}

		private static string GenerateImgSrcContent(MemoryStream pngMemoryStream, string title)
		{
			return $"<h3>{title}</h3><img alt=\"{title}\" src=\"data:image/png;base64,{Convert.ToBase64String(pngMemoryStream.ToArray())}\" />";
		}
	}
}