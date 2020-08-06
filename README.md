# Dithery-cli
 Command-line image dithering tool

## Sample command
Save single Atkinson dithered image, with true colors to web safe color reduction to PNG file 
`dotnet run -- half.png -m atkinson -c TrueColorToWebSafe -f SingleImage -o dither.png`

Save single Stucki dithered image, with true colors to web safe color reduction to HTML file (includes both original and dithered images in one HTML file) 
`dotnet run -- half.png -m stucki -c TrueColorToWebSafe -f HTMLBasic -o dither.html`