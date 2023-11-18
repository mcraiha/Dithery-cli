# Dithery-cli

Command-line [image dithering](https://en.wikipedia.org/wiki/Dither#Digital_photography_and_image_processing) tool written in C#. Dithering part is done with [CSharp-Dithering](https://github.com/mcraiha/CSharp-Dithering)

## Build status

![](https://github.com/mcraiha/Dithery-cli/workflows/.NET%20Core/badge.svg)

## Sample commands

Save single Atkinson dithered image, with true colors to web safe color reduction to PNG file  
`dotnet run -- half.png -m atkinson -c TrueColorToWebSafe -f SingleImage -o dither.png`  
or  
`dithery half.png -m atkinson -c TrueColorToWebSafe -f SingleImage -o dither.png`

Save single Stucki dithered image, with true colors to web safe color reduction to HTML file (includes both original and dithered images in one HTML file)  
`dotnet run -- half.png -m stucki -c TrueColorToWebSafe -f HTMLBasic -o dither.html`  
or  
`dithery half.png -m stucki -c TrueColorToWebSafe -f HTMLBasic -o dither.html`

Save all ditherings to HTML file, with true colors to web safe color reduction for PNG file  
`dotnet run -- half.png -m All -c TrueColorToWebSafe -f HTMLBasic -o dither.html`  
or  
`dithery half.png -m All -c TrueColorToWebSafe -f HTMLBasic -o dither.html`

Save all ditherings to separate .png files, with true colors to web safe color reduction for PNG file  
`dotnet run -- half.png -m all -c TrueColorToWebSafe -f SingleImage -o dither.png`  
or  
`dithery half.png -m all -c TrueColorToWebSafe -f SingleImage -o dither.png`

## How to install

Easiest option is to use dotnet global install  
`dotnet tool install -g Dithery-cli`  
and after that you can run Dithery-cli from any location with `dithery`

You can also download Windows release (an .exe file) and Linux release from [Releases](https://github.com/mcraiha/Dithery-cli/releases) page

## How to build

`dotnet build`

## How to package

`dotnet pack`

## Linux issues

You have to have **libgdiplus** in your system. Otherwise you will get following error
> Unhandled exception. System.TypeInitializationException: The type initializer for 'Gdip' threw an exception.
 ---> System.DllNotFoundException: Unable to load shared library 'libgdiplus' or one of its dependencies. In order to help diagnose loading problems, consider setting the LD_DEBUG environment variable: liblibgdiplus: cannot open shared object file: No such file or directory

## Input files

In theory System.Drawing.Bitmap should support BMP, GIF, EXIF, JPG, PNG and TIFF file formats. I have only tested it with PNG and JPG files.  

## Output files

Output can be either PNG file(s) or HTML file with embedded PNG images.  

## License

Text in this document and source code files are released into the public domain. See [LICENSE](https://github.com/mcraiha/Dithery-cli/blob/master/LICENSE) file.  