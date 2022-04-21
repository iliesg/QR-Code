using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TD_4_GOURRI_FOURDAIN
{
    class Pixel
    {

        /// Attributs

        int blue;
        int green;
        int red;
        int[] pixels;


        /// Accesseurs

        public int Blue
        {
            get { return blue; }
            set { blue = value; }
        }
        public int Green
        {
            get { return green; }
            set { green = value; }
        }
        public int Red
        {
            get { return red; }
            set { red = value; }
        }
        public int[] Pixels
        {
            get { return pixels; } 
            set { pixels = value; }
        }

        /// Constructeurs

        public Pixel(int B, int G, int R)
        {
            if (R >= 0 && R < 256 && B >= 0 && B < 256 && G >= 0 && G < 256)
            {
                blue = B;
                green = G;
                red = R;
            }
        }

        public Pixel(Pixel pixel)
        {
            blue = pixel.Blue;
            green = pixel.Green;
            red = pixel.Red;
        }

        public Pixel(int[] pixels)
        {
            this.pixels = new int[3];
            for (int i = 0; i < pixels.Length; i++)
            {
                this.pixels[i] = pixels[i];
            }
        }

        /// Méthodes

        /// <summary>
        /// Permet de transformer un pixel en nuance de gris
        /// </summary>
        /// <returns></returns>
        public void NuanceGris()
        {
            int gris = (this.red + this.blue + this.green) / 3;
            this.red = gris;
            this.blue = gris;
            this.green = gris;
        }

        /// <summary>
        /// Permet de transformer un pixel en noir ou blanc
        /// </summary>
        /// <returns></returns>
        public void NoirBlanc()
        {
            int gris = (this.red + this.blue + this.green) / 3;
            if (gris <= 128)
            {
                this.red = 0;
                this.blue = 0;
                this.green = 0;
            }
            else
            {
                this.red = 255;
                this.blue = 255;
                this.green = 255;
            }
        }
    }
}

