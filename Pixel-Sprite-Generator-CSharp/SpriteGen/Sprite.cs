using System;
using System.Windows;

namespace Pixel_Sprite_Generator.SpriteGen
{
    class Sprite
    {
        public int width;
        public int height;
        public Mask mask;
        public int[] data;
        public bool colored;
        double edgeBrightness;
        double colorVariations;
        double brightnessNoise;
        double saturation;
        Random random;
        int SEED;

        public Sprite(int width, int height, Mask mask, 
            bool colored=true, 
            double edgeBrightness=0.3, 
            double colorVariations=0.2,
            double brightnessNoise=0.3,
            double saturation=0.5,
            int SEED=0) 
        {
            this.width     = mask.width * (mask.mirrorX ? 2 : 1);
            this.height    = mask.height * (mask.mirrorY ? 2 : 1);
            this.mask      = mask;
            this.data      = new int[this.width * this.height];
            this.colored   = colored;
            this.edgeBrightness = edgeBrightness;
            this.colorVariations = colorVariations;
            this.brightnessNoise = brightnessNoise;
            this.saturation = saturation;
            if (SEED == 0)
                this.SEED = Environment.TickCount;
            else
                this.SEED = SEED;
            this.init();
        }

        private void init() {
            this.initData();
            this.applyMask();
            this.generateRandomSample();

            if (this.mask.mirrorX) {
                this.mirrorX();
            }

            if (this.mask.mirrorY) {
                this.mirrorY();
            }

            this.generateEdges();
        }

        public int getWidth() {
            return width;
        }
        public int getHeight() {
            return height;
        }
        public int getData (int x, int y) {
            return this.data[y * this.width + x];
        }
        public void setData(int x, int y, int value) {
            this.data[y * this.width + x] = value;
        }

        public void initData() {
            int h = this.height;
            int w = this.width;
            for (int y = 0; y < h; y++) {
                for (int x = 0; x < w; x++) {
                    this.setData(x, y, -1);
                }
            }
        }

        public void mirrorX() {
            int h = this.height;
            int w = (int)Math.Floor(this.width / (double)2);
            for (int y = 0; y < h; y++) {
                for (int x = 0; x < w; x++) {
                    this.setData(this.width - x - 1, y, this.getData(x, y));
                }
            }
        }

        public void mirrorY() {
            int h = (int)Math.Floor(this.height/(double)2);
            int w = this.width;
            for (int y = 0; y < h; y++) {
                for (int x = 0; x < w; x++) {
                    this.setData(x, this.height - y - 1, this.getData(x, y));
                }
            }
        }

        public void applyMask() {
            int h = this.mask.height;
            int w = this.mask.width;

            for (int y = 0; y < h; y++) {
                for (int x = 0; x < w; x++) {
                    this.setData(x, y, this.mask.data[y * w + x]);
                }
            }
        }

        public void generateRandomSample () {
            random = new Random(SEED);
            int h = this.height;
            int w = this.width;

            for (int y = 0; y < h; y++) {
                for (int x = 0; x < w; x++) {
                    int val = this.getData(x, y);
                    if (val == 1) {
                        val = random.Next(0,2);
                    } else if (val == 2) {
                        if (random.NextDouble() > 0.5) {
                            val = 1;
                        } else {
                            val = -1;
                        }
                    } 
                    this.setData(x, y, val);
                }
            }
        }

        public void generateEdges() {
            int h = this.height;
            int w = this.width;
            for (int y = 0; y < h; y++) {
                for (int x = 0; x < w; x++) {
                    if (this.getData(x, y) > 0) {
                        if (y - 1 >= 0 && this.getData(x, y-1) == 0) {
                            this.setData(x, y-1, -1);
                        }
                        if (y + 1 < this.height && this.getData(x, y+1) == 0) {
                            this.setData(x, y+1, -1);
                        }
                        if (x - 1 >= 0 && this.getData(x-1, y) == 0) {
                            this.setData(x-1, y, -1);
                        }
                        if (x + 1 < this.width && this.getData(x+1, y) == 0) {
                            this.setData(x+1, y, -1);
                        }
                    }
                }
            }
        }

        public double[] hslToRgb(double h, double s, double l) {
            double f, p, q, t;
            int i = (int)Math.Floor(h * (double)6);
            f = h * 6 - i;
            p = l * (1 - s);
            q = l * (1 - f * s);
            t = l * (1 - (1 - f) * s);
            switch (i % 6) {
                case 0: return new double[]{l, t, p};
                case 1: return new double[]{q, l, p};
                case 2: return new double[]{p, l, t};
                case 3: return new double[]{p, q, l};
                case 4: return new double[]{t, p, l};
                case 5: return new double[]{l, p, q};
                default: return null;
            }
        }

        public int[] renderPixelData() {
            random = new Random(SEED);
            bool isVerticalGradient   = random.NextDouble() > 0.5;
            double saturation         = Math.Max(Math.Min(random.NextDouble() * this.saturation, 1), 0);
            double hue                = random.NextDouble();
            int[] pixels = new int[height * width * 4];

            int ulen, vlen;
            if (isVerticalGradient) {
                ulen = this.height;
                vlen = this.width;
            } else {
                ulen = this.width;
                vlen = this.height;
            }

            for (int u = 0; u < ulen; u++) {
                // Create a non-uniform random number between 0 and 1 (lower numbers more likely)
                double isNewColor = Math.Abs(((random.NextDouble() * 2 - 1)
                                     + (random.NextDouble() * 2 - 1)
                                     + (random.NextDouble() * 2 - 1)) / 3);
                // Only change the color sometimes (values above 0.8 are less likely than others)
                if (isNewColor > (1 - this.colorVariations)) {
                    hue = random.NextDouble();
                }

                //MessageBox.Show(this.toString());

                for (int v = 0; v < vlen; v++) {
                    int val, index;
                    if (isVerticalGradient) {
                        val   = this.getData(v, u);
                        index = (u * vlen + v) * 4;
                    } else {
                        val   = this.getData(u, v);
                        index = (v * ulen + u) * 4;
                    }

                    double[] rgb = new double[]{1, 1, 1};

                    if (val != 0) {
                        if (this.colored) {
                            // Fade brightness away towards the edges
                            var brightness = Math.Sin(((double)u / (double)ulen) * Math.PI) * (1 - this.brightnessNoise)
                                           + random.NextDouble() * this.brightnessNoise;

                            // Get the RGB color value
                            rgb = this.hslToRgb(hue, saturation, brightness);

                            // If this is an edge, then darken the pixel
                            if (val == -1) {
                                rgb[0] *= this.edgeBrightness;
                                rgb[1] *= this.edgeBrightness;
                                rgb[2] *= this.edgeBrightness;
                            }

                        }  else {
                            // Not colored, simply output black
                            if (val == -1) {
                                rgb = new double[] { 0, 0, 0 };
                            }
                        }
                    }

                    pixels[index + 0] = (int)(rgb[0] * 255);
                    pixels[index + 1] = (int)(rgb[1] * 255);
                    pixels[index + 2] = (int)(rgb[2] * 255);
                    if (val != 0) {
                        pixels[index + 3] = 255;
                    }
                    else {
                        pixels[index + 3] = 0;
                    }
                }
            }

            return pixels;
        }

        public String toString() {
        int h = this.height;
        int w = this.width;
        String output = "";
        for (int y = 0; y < h; y++) {
            for (int x = 0; x < w; x++) {
                var val = this.getData(x, y);
                output += val >= 0 ? " " + val : "" + val;
            }
            output += '\n';
        }
        return output;
        }
    }
}
