using System.Collections.Generic;
using System.Linq;

namespace VueCore.Models
{
    public class VisionAnalysis
    {
        public IList<VisionSummary> Descriptions {get;} = new List<VisionSummary>();
        public VisionColor Color {get; private set;}
        public IList<VisionCategory> Categories {get;} = new List<VisionCategory>();
        public IList<VisionSummary> Tags {get;} = new List<VisionSummary>();
        public IList<VisionObject> Objects {get;} = new List<VisionObject>();
        public void AddColor(VisionColor color)
        {
            this.Color  = color;
        }
    }

    public class VisionSummary 
    {
        public VisionSummary(string caption, double confidence)
        {
            Caption = caption;
            Confidence = confidence;
        }

        public string Caption {get;}
        public double Confidence {get;}


    }
    public class VisionColor 
    {
        public bool IsBWImg {get;}
        public string AccentColor {get;}
        public string DominantColorBackground {get;}
        public string DominantColorForeground {get;}
        public IList<string> DominantColors  {get;} = new List<string>();

        public VisionColor(bool IsBWImg, string accentColor, string domBGColor, string domForeColor, IList<string> colors)
        {
            this.IsBWImg = IsBWImg;
            this.AccentColor = FormatForHex(accentColor);
            this.DominantColorBackground = FormatForHex(domBGColor);
            this.DominantColorForeground = FormatForHex(domForeColor);
            if(colors != null)
            {
                foreach(var color in colors)
                {
                    this.DominantColors.Add(FormatForHex(color));
                }
            }
        }
        private string FormatForHex(string color) 
        {
            int n = color.Length;            
            // Iterate over string
            for (int i = 0; i < n; i++)
            {
                char ch = color[i];
            
                // Check if the character
                // is invalid
                if ((ch < '0' || ch > '9') && (ch < 'A' || ch > 'F')) 
                {
                    return color;
                }
            }    
            return $"#{color}"; 

            // if(string.IsNullOrEmpty(color))
            //     return string.Empty;
            // if(int.TryParse(color, out int result))
            // {
            //     return $"#{color}";
            // }
            // else
            // {
            //     return color;
            // }
        }
    }
    public class VisionCategory
    {
        public VisionCategory(string name, double score)
        {
            Name = name;
            Score = score;
        }

        public string Name {get;}
        public double Score {get;}
    }
    public class VisionObject 
    {
        public VisionObject(double confidence, string property, BoundingRect rectangle)
        {
            Confidence = confidence;
            Property = property;
            Rectangle = rectangle;
        }

        public double Confidence {get;}
        public string Property {get;}
        public BoundingRect Rectangle {get;}
    }
    public class BoundingRect
    {
        public BoundingRect(int x, int y, int w, int h)
        {
            X = x;
            Y = y;
            W = w;
            H = h;
        }

        public int X {get;}
        public int Y {get;}
        public int W {get;}
        public int H {get;}
    }
}
/*




// Sunmarizes the image content.
Console.WriteLine("Summary:");
foreach (var caption in results.Description.Captions)
{
    Console.WriteLine($"{caption.Text} with confidence {caption.Confidence}");
}
// Identifies the color scheme.
Console.WriteLine("Color Scheme:");
Console.WriteLine("Is black and white?: " + results.Color.IsBWImg);
Console.WriteLine("Accent color: " + results.Color.AccentColor);
Console.WriteLine("Dominant background color: " + results.Color.DominantColorBackground);
Console.WriteLine("Dominant foreground color: " + results.Color.DominantColorForeground);
Console.WriteLine("Dominant colors: " + string.Join(",", results.Color.DominantColors));
// Display categories the image is divided into.
Console.WriteLine("Categories:");
foreach (var category in results.Categories)
{
    Console.WriteLine($"{category.Name} with confidence {category.Score}");
}
Console.WriteLine("Tags:");
foreach (var tag in results.Tags)
{
    Console.WriteLine($"{tag.Name} {tag.Confidence}");
}




--- TODO
Console.WriteLine("Objects:");
foreach (var obj in results.Objects)
{
    Console.WriteLine($"{obj.ObjectProperty} with confidence {obj.Confidence} at location {obj.Rectangle.X}, " +
      $"{obj.Rectangle.X + obj.Rectangle.W}, {obj.Rectangle.Y}, {obj.Rectangle.Y + obj.Rectangle.H}");
}

// Detects the image types.
Console.WriteLine("Image Type:");
Console.WriteLine("Clip Art Type: " + results.ImageType.ClipArtType);
Console.WriteLine("Line Drawing Type: " + results.ImageType.LineDrawingType);
Console.WriteLine();


Console.WriteLine("Brands:");
foreach (var brand in results.Brands)
{
    Console.WriteLine($"Logo of {brand.Name} with confidence {brand.Confidence} at location {brand.Rectangle.X}, " +
      $"{brand.Rectangle.X + brand.Rectangle.W}, {brand.Rectangle.Y}, {brand.Rectangle.Y + brand.Rectangle.H}");
}

Console.WriteLine("Faces:");
foreach (var face in results.Faces)
{
    Console.WriteLine($"A {face.Gender} of age {face.Age} at location {face.FaceRectangle.Left}, " +
      $"{face.FaceRectangle.Left}, {face.FaceRectangle.Top + face.FaceRectangle.Width}, " +
      $"{face.FaceRectangle.Top + face.FaceRectangle.Height}");
}


// Adult or racy content, if any.
Console.WriteLine("Adult:");
Console.WriteLine($"Has adult content: {results.Adult.IsAdultContent} with confidence {results.Adult.AdultScore}");
Console.WriteLine($"Has racy content: {results.Adult.IsRacyContent} with confidence {results.Adult.RacyScore}");
Console.WriteLine($"Has gory content: {results.Adult.IsGoryContent} with confidence {results.Adult.GoreScore}");

// Celebrities in image, if any.
Console.WriteLine("Celebrities:");
foreach (var category in results.Categories)
{
    if (category.Detail?.Celebrities != null)
    {
        foreach (var celeb in category.Detail.Celebrities)
        {
            Console.WriteLine($"{celeb.Name} with confidence {celeb.Confidence} at location {celeb.FaceRectangle.Left}, " +
              $"{celeb.FaceRectangle.Top}, {celeb.FaceRectangle.Height}, {celeb.FaceRectangle.Width}");
        }
    }
}

// Popular landmarks in image, if any.
Console.WriteLine("Landmarks:");
foreach (var category in results.Categories)
{
    if (category.Detail?.Landmarks != null)
    {
        foreach (var landmark in category.Detail.Landmarks)
        {
            Console.WriteLine($"{landmark.Name} with confidence {landmark.Confidence}");
        }
    }
}
*/