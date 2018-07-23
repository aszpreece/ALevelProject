using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NoMansSea
{
    enum TileType : byte
    {
        Ocean = 0,
        Shore,
        Sand,
        Grass,
        Ridge,
        Mountain,
        Torch,
        OceanWall
    }

   static class Tile
    {

       public const int TileSideLengthInPixels = 32;

 
       public static byte getLightLevel(byte raw)
       {
           //0001 < type 0010 < light data
           //&                              lets bitwise & it...
           //0000        1111               (00001111 = 15)
           //=
           //0000        0010               we now have our data!
           return (byte)(raw & 0xF);                  
       }

       public static TileType getType(byte raw)
       {
           //0001 < type 0010 < light data
           //&                              lets bitwise & it...
           //1111        0000               (11110000 = 240)
           //=
           //0001        0000               we now have our data! (kind of)
           //----        0001               bit shift right by 4 to get it to a number between 0 and 15
           return (TileType)((raw & 0XF0) >> 4);
       }

       public static byte combineData(byte light, TileType type)
       {
           //0000 0001 < light data
           //0000 1000 < type data
           //shift type left by 4
           //1000 0000 
           //bitwise or...
           //1000 0000 
           //|
           //0000 0001
           //=
           //1000 0001

           return (byte)(((byte)type << 4) | light);

       }


    }
}
