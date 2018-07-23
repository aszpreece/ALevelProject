using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BasicNoiseLib;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace NoMansSea
{
    static class GenTextures
    {

        static Noise noise = new Noise();

       //All these subroutines use nested loops to go through each of the points of a texture and then generate
       //perlin noise values at that point. Each of them generate their textures slightly differently. 

        public static void setGrassTexture(int seed, ref Texture2D texture)
        {
            List<Color> colours = new List<Color>();
            noise.setSeed(seed);
            for (int x = 0; x < Tile.TileSideLengthInPixels; x++)
            {
                for (int y = 0; y < Tile.TileSideLengthInPixels; y++)
                {
                    float n = (float)noise.perlin(x, y, 0.9f) +                  //layered noise
                                             0.5f * noise.perlin(x, y, 0.3f);
                    colours.Add(new Color(30, (int)(n * 153f), 30)); //as grass is green, we only really want to be producing
                                                                            //different shades of that colour. This when we are calculating
                                                                            //this value, we only want to be modifying the G value of the RGB.
                }

            }
            texture.SetData<Color>(colours.ToArray());
        }

        public static void setStoneTexture(int seed, ref Texture2D texture)
        {
            List<Color> colours = new List<Color>();

            noise.setSeed(seed);
            for (int x = 0; x < Tile.TileSideLengthInPixels; x++)
            {
                for (int y = 0; y < Tile.TileSideLengthInPixels; y++)
                {
                    float n = 0.5f * (float)noise.perlin(x, y, 0.5f) +                  //layered noise
                                             0.5f * noise.perlin(x, y, 0.6f);
                    colours.Add(new Color((int)(n * 140f), (int)(n * 150f), (int)(n * 165f)));

                    //as we wish to produce a gray scale texture, all of the starting values for the R, G and B
                    //components must be close to one another. Therefore we have used 140, 150 and 165.

                }

            }
            texture.SetData<Color>(colours.ToArray());

        }
        public static void setSandTexture(int seed, ref Texture2D texture)
        {
            List<Color> colours = new List<Color>();

            noise.setSeed(seed);
            for (int x = 0; x < Tile.TileSideLengthInPixels; x++)
            {
                for (int y = 0; y < Tile.TileSideLengthInPixels; y++)
                {
                    float n = 0.5f * (float)Math.Sqrt(noise.perlin(x, y, 1.2f)); //layered noise not necessary

                    colours.Add(new Color((int)(n * 300) + 20, (int)(n * 240) + 20, (int)(n * 110) + 20));

                    //I simply had to experiment with different values and equations to come up with this algorithm. The Noise
                    //we produce however is a very height frequency as sand is very fine. 
                }

            }
            texture.SetData<Color>(colours.ToArray());
        }
        public static void setWaterTexture(int seed, ref Texture2D texture)
        {
            List<Color> colours = new List<Color>();

            noise.setSeed(seed);
            for (int x = 0; x < Tile.TileSideLengthInPixels; x++)
            {
                for (int y = 0; y < Tile.TileSideLengthInPixels; y++)
                {
                    float n = (float)Math.Sqrt(noise.perlin(x, y, 0.2f)); //putting the value of the noise through the sqrt function decreases
                                                                          //the distribution of the values, making them much more smooth.

                    if (n >= 0.9) colours.Add(Color.White); //This makes the highest 10% of pixels glint white. 
                    else colours.Add(new Color(30, 30, (int)(n * 255) + 40)); //As water is blue, we only really want to scale this value.
                }

            }
            texture.SetData<Color>(colours.ToArray());

        }
        public static void setEntityTexture(int seedShape, ref Texture2D texture)
        {
            Color[,] colours = new Color[32, 32];
            List<Point> surrounding = new List<Point>();

            //populate texture grid so it is not null. 
            for (int x = 0; x < 32; x++)
            {
                for (int y = 0; y < 32; y++)
                {
                    colours[x, y] = Color.Transparent;

                }
            }

            Random shape = new Random(seedShape);

            //calculate colours for use in the entity.

            Color[] scheme = new Color[6];
            scheme[0] = new Color(shape.Next(0, 33) * 8 - 1, shape.Next(0, 33) * 8 - 1, shape.Next(0, 33) * 8 - 1);
            scheme[1] = scheme[0];
            scheme[2] = scheme[0];
            scheme[3] = scheme[0];
            scheme[4] = new Color(scheme[0].ToVector3() * 0.5f);
            scheme[5] = new Color(scheme[0].ToVector3() * 1.5f);

            //the max amount of pixels in the texture. 
            int size = 290;

            int Y = 15;
            int X = 15;
          
            //add a starting pixel to the texture and then mirrior it on the right side. 
            colours[X, Y] = scheme[shape.Next(0, 3)]; 
            colours[X + 1, Y] = colours[X, Y];

            //we will attempt to add 'size' amount of pixels to the texture.
            for (int pixelsAdded = 0; pixelsAdded < size;)
            {
                for (int x = 0; x < 32; x++)
                {
                    for (int y = 0; y < 32; y++)
                    {
                        if (colours[x, y] != Color.Transparent)
                        {
                            //find all valid surrounding pixels around the point we have found. 
                            Point p = new Point(x - 1, y);
                            if (isPointEmptyAndValid(p, colours)) surrounding.Add(p);
                            p.X += 2;
                            if (isPointEmptyAndValid(p, colours)) surrounding.Add(p);
                            p.X -= 1;
                            p.Y += 1;
                            if (isPointEmptyAndValid(p, colours)) surrounding.Add(p);
                            p.Y -= 2;
                            if (isPointEmptyAndValid(p, colours)) surrounding.Add(p);
                        }

                    }

                }

                if (surrounding.Count == 0) break; //if there are no surrounding tiles, break out of the loop.
                foreach (Point p in surrounding)
                {
                    //for each surrounding point found in thsi iteration, calculate a random number between 0 and 1. 
                    //If this value is larger than 0.9 then add a pixel of a random colour from the pre determined
                    //colour scheme to the texture and mirror it on the other side. 

                    if (shape.NextDouble() > 0.9d)
                    {
                        colours[p.X, p.Y] = scheme[shape.Next(0, scheme.Length)];
                        colours[(31 - (p.X)), p.Y] = colours[p.X, p.Y];
                        pixelsAdded++;
                    }
                }
               
               
                surrounding.Clear();

            }

            //simply parses the 2D array into a list that can then be used to set a texture.

            List<Color> data = new List<Color>();

            for (int x = 0; x < 32; x++)
            {

                for (int y = 0; y < 32; y++)
                {

                    data.Add(colours[y, x]);

                }

            }

            texture.SetData<Color>(data.ToArray());

        }

        //checks if the point is transparent (empty) and if it is within the bounds of the area we are working with. 
        static bool isPointEmptyAndValid(Point p, Color[,] col) { 
            return ((col[p.X & 0XF, p.Y & 31] == Color.Transparent) && p.Y >= 0 && p.Y < 32 && p.X >= 0 && p.X <= 15); }
       
        public static void setTorchTexture(int seed, ref Texture2D texture) //builds on top of the grass texture data, adding a torch.
        {
            if (Game1.Grass == null) return;

            Color[] data = new Color[Tile.TileSideLengthInPixels * Tile.TileSideLengthInPixels];
            Game1.Grass.GetData<Color>(data);
            noise.setSeed(seed);
            for (int x = Tile.TileSideLengthInPixels / 2 - 2; x < Tile.TileSideLengthInPixels / 2 + 2; x++)
            {
                for (int y = Tile.TileSideLengthInPixels / 2 - 5; y < Tile.TileSideLengthInPixels / 2 + 5; y++)
                {           
                        data[x + (y * Tile.TileSideLengthInPixels)] = Color.BlanchedAlmond;
                    
                }
            }
            texture.SetData<Color>(data);
        }

    }
}
