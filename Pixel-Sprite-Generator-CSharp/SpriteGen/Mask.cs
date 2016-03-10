using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pixel_Sprite_Generator.SpriteGen
{
    class Mask
    {
        public int width;
        public int height;
        public int[] data;
        public bool mirrorX;
        public bool mirrorY;

        public Mask(int[] data, int width, int height, bool mirrorX=false, bool mirrorY=false) {
            this.width   = width;
            this.height  = height;
            this.data    = data;
            this.mirrorX = mirrorX;
            this.mirrorY = mirrorY;
        }
    }
}
