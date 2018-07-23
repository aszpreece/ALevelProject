using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BasicNoiseLib;

namespace BasicNoiseLib
{

    //Noise algorithm by Ken Perlin. Implementation by Tomasz Preece candindate number 3609

    public class Noise
    {
       private int seed;
             
       private int[] p = new int[512]; //our hash table. doubled up to avoid index out of range exceptions (overflows)

       private Random r;

       public Noise(int seed = 0)
       {
           setSeed(seed);
       }

        //creates a hash table to use with our noise function. Must be called before use.
        public void setSeed(int s)
        {
            seed = s;
            r=   new Random(seed);

            int[] permutation = Enumerable.Range(0, 256).ToArray(); // all numbers 0 to 255 inclusive
            

            for (int i = 0; i < permutation.Length; i++)//shuffle the array 
            {
                int src = r.Next(permutation.Length); //take a random value between 0 and 255. This is our source index
                int val = permutation[src]; //take the value at this index
                permutation[src] = permutation[i]; //set the value at our source index to the the index of our i
                permutation[i] = val; //vice versa
                p[i] = permutation[i];
                p[i + 256] = permutation[i]; //double up the array. this avoids overflow errors
        

            }

        }

        private readonly static int[] grads = new int[] { //gradients from the centre of a square to the corners. Stored in plain integers for the sake of not having to create a vector structure.
    0, 1, 
    1, 1,
    1, 0,
    1,-1,
    0,-1,
   -1,-1,
   -1, 0,
   -1, 1};

 


        public float perlin(float x, float y, float scale)
        {
            x *= scale;
            y *= scale;

            int xi = (int)x & 255;                              // Calculate the "unit cube" that the point asked will be located in. This also wraps the 
            int yi = (int)y & 255;                              // coordinates if they are beyond 255 allowing the noise to tile
          
             x = x - (int)x;   //calculate the coordinates in the unit cube the point is
             y = y - (int)y;

            int

        //hash function

         c00 = p[xi + p[yi]],   //calculate gradient indexes for each of the four points of the unit square (using our hash table)
         c10 = p[xi + 1 + p[yi]],   //will be used in the dot product calculation method
         c01 = p[xi + p[yi + 1]],
         c11 = p[xi + 1 + p[yi + 1]];


            //00------10
            //!        !
            //!        !
            //!        !
            //01------11

            float //calculate the dot product of the static coordinate vectors and the vectors from the point in the centre to the corners
        dp00 = getDotProduct(c00 % 8, x, y),        //cxx is a number 0 to 255. Its remainder from 8 will be a number from 0 to 7. This gives us
        dp10 = getDotProduct(c10 % 8, x - 1, y),    //the gradient index for our gradient table 'grads' that will be used at this coordinate
        dp01 = getDotProduct(c01 % 8, x, y - 1),
        dp11 = getDotProduct(c11 % 8, x - 1, y - 1);

            x = ease(x); //put x through our ease curve

            //re use old variables dp00 and dp10
            dp00 = lerp(dp00, dp10, x); //interpolate between all the values to find a weighted average of all the dot products
            dp01 = lerp(dp01, dp11, x);

            return (lerp(dp00, dp01, ease(y)) + 1) / 2;
        }

        #region Helper

        public static float ease(float t) //puts t through an ease curve (y = 6x^5-15x^4+10x^3) as defined by ken perlin
        {
            return t * t * t * (10 + t * (-15 + t * 6));

        }


        // Precise method which guarantees v = v1 when t = 1 (floats can be silly).
        public static float lerp(float v0, float v1, float t) //linear interpolation between two values, with t as our weighting. 
        {
            return (1 - t) * v0 + t * v1;
        }

        private static float getDotProduct(int i, float x, float y) //dot product of one of the gradients and x,y
        {
            //* by 2 as each vector is stored at two indexes in two seperate integers
            return grads[i * 2] * x + grads[i * 2 + 1] * y;

        }


        #endregion
    }
}
