using SkiaSharp;
using System.Diagnostics;
using System.Reflection;

namespace PixelDemo;

public class GraphicsDrawable : IDrawable
{
	private Color[] pixels = new Color[60*5];

    public SKBitmap Bitmap { get; set; }

    private int xpos = 0;

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        // Drawing code goes here
        canvas.ResetState();

        if(xpos >= Bitmap.Width)
        {
            xpos = 0;
        }

        //Get pixels.
        for (int i = 0; i < Bitmap.Height; i++)
        {

            var pixel = Bitmap.GetPixel(xpos, i);

            //Draw with MAUI
            canvas.FillColor = new Color(pixel.Red, pixel.Green, pixel.Blue);
            canvas.FillCircle(5, i * 5, 2);
        }
        
        //Go to next column
        xpos = (xpos + 1) % Bitmap.Width;
    }
}



public partial class MainPage : ContentPage
{
    GraphicsDrawable graphics;

    public MainPage()
	{
		InitializeComponent();

        graphics = new GraphicsDrawable();

        MyGraph.Drawable = graphics;


        //Load demo image
        Assembly assembly = GetType().GetTypeInfo().Assembly;
        using (Stream stream = assembly.GetManifestResourceStream("PixelDemo.Resources.Images.demo.png"))
        {
            graphics.Bitmap = SKBitmap.Decode(stream);
        }


        //Refresh timer 
		var tmr = new System.Timers.Timer();
		tmr.Interval = 1000 / 20;
        tmr.Elapsed += Tmr_Elapsed;
		tmr.Start();
    }


    /// <summary>
    /// Request redraw image.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Tmr_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
        Application.Current.Dispatcher.Dispatch(() =>
        {
			MyGraph.Invalidate();
        });
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        PickOptions options = new PickOptions();
        options.PickerTitle = "Select PNG";
        options.FileTypes = FilePickerFileType.Png;

        var result = await FilePicker.PickAsync(options);

        if(result == null) 
        {
            return;
        }

        /* Load PNG  */
        using (Stream stream = File.Open(result.FullPath, FileMode.Open, FileAccess.Read))
        {
            graphics.Bitmap = SKBitmap.Decode(stream);
        }

    }

}

