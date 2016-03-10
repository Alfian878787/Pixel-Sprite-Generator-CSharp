using Pixel_Sprite_Generator.SpriteGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Pixel_Sprite_Generator
{
    public partial class Window1 : Window
    {
        Mask spaceship = new Mask(new int[]{
                    0, 0, 0, 0, 0, 0,
                    0, 0, 0, 0, 1, 1,
                    0, 0, 0, 0, 1,-1,
                    0, 0, 0, 1, 1,-1,
                    0, 0, 0, 1, 1,-1,
                    0, 0, 1, 1, 1,-1,
                    0, 1, 1, 1, 2, 2,
                    0, 1, 1, 1, 2, 2,
                    0, 1, 1, 1, 2, 2,
                    0, 1, 1, 1, 1,-1,
                    0, 0, 0, 1, 1, 1,
                    0, 0, 0, 0, 0, 0
            }, 6, 12, true, false);
        Sprite sprite;

        public Window1()
        {
            InitializeComponent();
            newBitmap();
        }

        public void newBitmap() {
            int width = 12;
            int height = 12;
            int stride = width * 4;

            this.sprite = new Sprite(12, 12, spaceship, true);
            int[] spritePixels = this.sprite.renderPixelData();

            // Create a writeable bitmap (which is a valid WPF Image Source
            WriteableBitmap bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);

            // Create an array of pixels to contain pixel color values
            uint[] pixels = new uint[width * height];

            int red;
            int green;
            int blue;
            int alpha;

            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    int i = width * y + x;
                    int index = (width * y + x)*4;

                    red = spritePixels[index];
                    green = spritePixels[index + 1];
                    blue = spritePixels[index + 2];
                    alpha = spritePixels[index + 3];

                    pixels[i] = (uint)((alpha << 24) + (red << 16) + (green << 8) + blue);
                }
            }

            // apply pixels to bitmap
            bitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, width * 4, 0);

            // set image source to the new bitmap
            this.MainImage.Source = bitmap;
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            newBitmap();
        }
    }
}
