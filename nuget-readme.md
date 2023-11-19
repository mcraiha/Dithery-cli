# Dithery-cli

Command-line [image dithering](https://en.wikipedia.org/wiki/Dither#Digital_photography_and_image_processing) tool written in C#. Dithering part is done with [CSharp-Dithering](https://github.com/mcraiha/CSharp-Dithering)

## Sample commands

Save single Atkinson dithered image, with true colors to web safe color reduction to PNG file  
`dithery half.png -m atkinson -c TrueColorToWebSafe -f SingleImage -o dither.png`

Save single Stucki dithered image, with true colors to web safe color reduction to HTML file (includes both original and dithered images in one HTML file)  
`dithery half.png -m stucki -c TrueColorToWebSafe -f HTMLBasic -o dither.html`

Save all ditherings to HTML file, with true colors to web safe color reduction for PNG file  
`dithery half.png -m All -c TrueColorToWebSafe -f HTMLBasic -o dither.html`

Save all ditherings to separate .png files, with true colors to web safe color reduction for PNG file   
`dithery half.png -m all -c TrueColorToWebSafe -f SingleImage -o dither.png`

## How to install

Easiest option is to use dotnet global install  
`dotnet tool install -g Dithery-cli`  
and after that you can run Dithery-cli from any location with `dithery`

You can also download Windows release (an .exe file) and Linux release from [Releases](https://github.com/mcraiha/Dithery-cli/releases) page