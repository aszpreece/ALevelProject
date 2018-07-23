using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;
using System.Diagnostics;
using BasicNoiseLib;

namespace NoMansSea
{
    class Level
    {

        public Noise noise;

        public Byte[,] Map = new Byte[256, 256]; //first 8 bits dictate type (0-15), second dictate light level (if == to 15 then it is a light source)

        public EntityPlayer Player;

        private List<EntityAnimal> Mobs;
                          
        public Input input;

        Vector2 ScrollOffset;

        readonly Vector2 ScreenCentre = new Vector2((int)(Game1.ScreenX / 2), (int)(Game1.ScreenY / 2));

        private int PlayerViewRadius = 21;
                 
        private int seed = 0;



        private double TimeOfDay = 180; //controls time of day in regards to light levels etc.

        public int Seed { get { return seed; } }
        public double Time
        {
            get
            {
                return TimeOfDay;
            }
            set
            {
                TimeOfDay = value % 360; //when set, the value will be set to the remainder of itself from 360 degrees.
            }
        }

        public Level(Input inp)
        {
            input = inp;
            Mobs = new List<EntityAnimal>();
            Player = new EntityPlayer(new Vector2(100 * Tile.TileSideLengthInPixels, 100 * Tile.TileSideLengthInPixels));
            noise = new Noise();

        }

        public void generate(int IX, int IY)
        {


            float NoiseVal;
            TileType Type = 0;

            // (old method)seed = (int)(0.5 * (double)((IX + IY) * (IX + IY + 1) + IY)); //pairing function, gives a unique integer based on two input coordinates
            seed = ((IX << 16) | IY) ^ 0x71EBA1C3; //bit shifts to combine the two coordinates and then adds an XOR bitmask over the top to create seed.

            GenTextures.setEntityTexture(seed, ref Game1.EntityTexture); //A new entity texture must be created using this seed.
        
            noise.setSeed(seed);

            for (int x = 0; x < 256; x++)
            {
                for (int y = 0; y < 256; y++)
                {

                    NoiseVal = (float)noise.perlin(x, y, 0.01f) +                  //creates layered noise. The second layer is at a higher
                                             0.5f * noise.perlin(x, y, 0.05f);     // frequency and so creates more intricate details.

                    float val = 1 - (float)Math.Sqrt(Math.Pow(127.5 - x, 2) + Math.Pow(127.5 - y, 2)) / 127f;
                    NoiseVal *= val + 0.3f;

                    //depending on how high the height map is, set the tile to a different type.
                    if (NoiseVal < 0.9f) Type = TileType.Mountain;
                    if (NoiseVal < 0.85f) Type = TileType.Ridge;
                    if (NoiseVal < 0.70f)
                    {
                        if (new Random(seed + x << y).NextDouble() > 0.99) //use the seed and a combination of of the x and y to seed a random value.
                        { //if the valeu is greater than 0.99, set the tile to a torhc. If else set it to grass.
                            Type = TileType.Torch;
                        }
                        else
                            Type = TileType.Grass;
                    }

                    if (NoiseVal < 0.58f) Type = TileType.Sand;
                    if (NoiseVal < 0.55f) Type = TileType.Shore;
                    if (NoiseVal < 0.45f) Type = TileType.Ocean;

                    if (x == 0 || y == 255 || y == 0 || x == 255)
                    {
                        Type = TileType.OceanWall; //no matter what, the border tiles need to be ocean walls.
                    }
                    Map[x, y] = Tile.combineData(1, Type);

                }

            }

            Player = new EntityPlayer(findEmptySpace(seed)); //ReInitiliase the player object.

            generateEntities(seed + 1);
            generateLightMap();


        }

        private void generateEntities(int seed)
        {
            Random r = new Random(seed);
            Mobs.Clear();

            int EveryHowManyShouldBeParent = r.Next(2, 7);
            int AmountOfEntities = 10 + r.Next(0, 21);

            for (int m = 0; m < AmountOfEntities; m++)
            {
                EntityAnimal animal = new EntityAnimal(findEmptySpace(seed + m), false);

                if (m % EveryHowManyShouldBeParent == 0)
                {
                    animal.isParent = true;
                    EntityAnimal child = new EntityAnimal(animal.position + new Vector2(14, 14), true);
                    child.parent = animal;
                    Mobs.Add(child);
                    m++;
                }
                Mobs.Add(animal);
            }

        }

        public void Draw(SpriteBatch SB)
        {
            //calculates the scroll offset, which is relative to the player coordinates. 

            ScrollOffset = new Vector2(Player.position.X - ScreenCentre.X, Player.position.Y - ScreenCentre.Y);

            if (Map == null) return;

            TileType Type;
            Color col = Color.White;

            //calculate the tiles that are within the player's field of view, so we aren'trendering hundreds of Tiles that are off-screen. This is capped between 0 and the max Tile index.
            int startX = (int)MathHelper.Clamp((float)Math.Floor(Player.position.X / Tile.TileSideLengthInPixels) - PlayerViewRadius, 0, 256),
                startY = (int)MathHelper.Clamp((float)Math.Floor(Player.position.Y / Tile.TileSideLengthInPixels) - PlayerViewRadius, 0, 256),
                endX = (int)MathHelper.Clamp((float)Math.Floor(Player.position.X / Tile.TileSideLengthInPixels) + PlayerViewRadius, 0, 256),
                endY = (int)MathHelper.Clamp((float)Math.Floor(Player.position.Y / Tile.TileSideLengthInPixels) + PlayerViewRadius, 0, 256);


            Texture2D texture = Game1.Pixel;
            Color filter = Color.White;
            float light;
            for (int y = startY; y < endY; y++)
            {

                for (int x = startX; x < endX; x++)
                {
                    //calculates the percentage the tile should be lit by according to its pre-deteremined light level and the time of day.
                    light = (float)((Tile.getLightLevel(Map[x, y]) + 1) * 16) / 255 + ((float)Math.Sin(MathHelper.ToRadians((float)TimeOfDay)) + 1) / 2.0f;

                    filter = Color.White;
                    Type = Tile.getType(Map[x, y]);

                    //finds out what texture and what filter the Tile should use to render.
                    switch (Type)
                    {
                        case TileType.Mountain:

                            texture = Game1.HighMtn;
                            break;
                        case TileType.Ridge:

                            texture = Game1.LowMtn; filter = Color.Gray;
                            break;
                        case TileType.Grass:

                            texture = Game1.Grass;
                            break;
                        case TileType.Sand:

                            texture = Game1.Sand;
                            break;
                        case TileType.Shore:

                            texture = Game1.Water;
                            filter = Color.Azure;
                            break;
                        case TileType.Ocean:

                            texture = Game1.Water; filter = Color.SkyBlue;
                            break;
                        case TileType.OceanWall:

                            texture = Game1.Water; filter = Color.DarkSlateBlue;
                            break;
                        case TileType.Torch:

                            texture = Game1.Torch;
                            break;
                    }

                    //draws the Tile with the correct texture and filter.
                    SB.Draw(texture, new Rectangle((int)(x * Tile.TileSideLengthInPixels - ScrollOffset.X), (int)(y * Tile.TileSideLengthInPixels - ScrollOffset.Y), Tile.TileSideLengthInPixels, Tile.TileSideLengthInPixels), new Color((filter.ToVector3() * light)));

                }

            }
            //renders all the entites and draws the island's seed.
            Player.render(SB, ScrollOffset);

            foreach (EntityAnimal testMob in Mobs)
                testMob.render(SB, ScrollOffset);

         //   SB.DrawString(Game1.Font, "Island: " + seed.ToString(), new Vector2(4, 4), Color.White);

        }

        public void update(GameTime gametime)
        {
            //makes it so a day and night cycle lasts a minute.
            Time += gametime.ElapsedGameTime.TotalMinutes  * 360;

            Player.update(gametime, input, this);

            List<EntityAnimal> CurrentMobs = Mobs;
            for (int i = 0; i < CurrentMobs.Count; i++)
                CurrentMobs[i].update(gametime, input, this);
            
        }

        private void generateLightMap()
        {
            List<Point> propogationQueue = new List<Point>();
            //firstly finds all the torches on the map to begin propogation from.
            for (int x = 0; x < 255; x++)
            {
                for (int y = 0; y < 255; y++)
                {
                    if (Tile.getType(Map[x, y]) == TileType.Torch)
                    {
                        propogationQueue.Add(new Point(x, y));
                        Map[x, y] = Tile.combineData((byte)15, Tile.getType(Map[x, y]));

                    }
                }
            } 

            //won't stop until all appropriate tiles have been lit. 
            while (propogationQueue.Count > 0)
            {

                Point LightPoint = propogationQueue[0]; 

                byte lightLevel = Tile.getLightLevel(Map[LightPoint.X, LightPoint.Y]);

                //finds the valid surrounding tiles.
                List<Point> surrounding = getVaildSurroundingPoints(LightPoint);
                //for every surrounding point if the light level is lower than this tile's
                //light level -1, set the light level of the tile to this tile's light level -1.
                foreach (Point s in surrounding)
                {
                    if (Tile.getLightLevel(Map[s.X, s.Y]) < lightLevel - 1)
                    {
                        Map[s.X, s.Y] = Tile.combineData((byte)(lightLevel - 1), Tile.getType(Map[s.X, s.Y]));
                        propogationQueue.Add(s);

                    }

                }
                propogationQueue.RemoveAt(0);
            }
        }

        List<Point> getVaildSurroundingPoints(Point p)
        {
            List<Point> points = new List<Point>();
            if (Tile.getType(Map[p.X, p.Y]) != TileType.Ridge)
            {
                if (p.X - 1 >= 0) points.Add(new Point(p.X - 1, p.Y));
                if (p.X + 1 <= 255) points.Add(new Point(p.X + 1, p.Y));
                if (p.Y - 1 >= 0) points.Add(new Point(p.X, p.Y - 1));
                if (p.Y + 1 <= 255) points.Add(new Point(p.X, p.Y + 1));
            }
            return points;
        }

        public Vector2 findEmptySpace(int seed)
        {
            Random r = new Random(seed);
            int x, y;
            TileType t;

            do
            {
                x = r.Next(50, 205);
                y = r.Next(50, 205);
                t = Tile.getType(Map[x, y]);
            } while (t == TileType.OceanWall || t == TileType.Mountain || t == TileType.Ridge);

            return new Vector2(x * Tile.TileSideLengthInPixels, y * Tile.TileSideLengthInPixels);
        }

        public TileType getTileAtPosition(Vector2 pos)
        {
            return Tile.getType(Map[toGridCoordinate(pos.X), toGridCoordinate(pos.Y)]);
        }

        public int toGridCoordinate(float p)
        {
            return (int)Math.Floor(p / Tile.TileSideLengthInPixels);
        }

        public void addEntity(EntityAnimal e)
        {
            Mobs.Add(e);
        }
    }

}

