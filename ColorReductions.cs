using System;

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
	}
}