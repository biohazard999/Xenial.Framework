

using System.Diagnostics;

using MyProject;

Console.WriteLine("Hello World");

using var stream = ImageNamesWithSizes.AsStream.aac();
using var image = System.Drawing.Image.FromStream(stream);

var bytes = ImageNamesWithSizes.ResourceNames.AsBytes.aac();


Debug.Assert(stream != null);
