using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using BasicNoiseLib;

namespace NoiseTest
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    /// 




    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        float[,] noiseVal = new float[256, 256];
        private Texture2D Pix;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Pix = new Texture2D(GraphicsDevice, 1, 1);
            Pix.SetData<Color>(new Color[] { Color.White });
            graphics.PreferredBackBufferWidth = 900;
            graphics.PreferredBackBufferHeight = 900;
            graphics.ApplyChanges();
      

            //    Console.WriteLine(genVectors(11, 20).ToString());


            base.Initialize();


        }



        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

           //setTextureData();
            setIslandData();
             // TODO: Add your update logic here

            base.Update(gameTime);
        }

        public Noise noise = new Noise(0);
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            //DIARY
            //Day 1: Started on initial noise implementation (it failed)
            //Day 2: Made a little more progress but still not quite right
            //Day 3: Finally got some noise going
            //       Started on creating island algorithm and came up with a good prototype
            //Day 4: Decided to tweak algorithm so islands were contained within the screen.
            //       Came up with better gradient equation
            //       Changed layered noise algorithm
            //       Changed perlin function to take a scale value 
            //       Finalised island tech demo
            //Day 5: Started on actual game
            //       Came up with storage solution for level
            //Day 6: Character created with accleration based movement
            //Day 7: Entities are now all rendered using one method allowing for more than one entity to exist. Previously this was not possible due to some poor coding
            //       The player can no longer go through walls and will bounce off them
            //       The player will go slower in water

          //  renderTexture(spriteBatch);
            renderIsland(spriteBatch);
         
            spriteBatch.End();

            base.Draw(gameTime);
        }

        int i = 300;


        int h = 0;
        public void renderTexture(SpriteBatch spriteBatch)
        {
            h += 10;
                for (int x = 0; x < 255; x++)
            {
                for (int y = 0; y < 255; y++)
                {

                 
                    float d = noise.perlin(x + h, y, 0.3f);
                
                    Color c = new Color (Color.White.ToVector3() * d);
                    spriteBatch.Draw(Pix, new Rectangle(x * 2, y * 2, 2, 2), c);                                           
                }

            }

        }
       
        
        
        // float el = noiseVal[x, y];
       

        public void setTextureData()
        {
            
            for (int x = 0; x < 255; x++)
            {
                for (int y = 0; y < 255; y++)
                {
                    float d = noise.perlin(x + h, y, 0.3f);

                    noiseVal[x, y] = d; // 1 - (float)Math.Sqrt(Math.Pow(127.5 - x, 2) + Math.Pow(127.5 - y, 2)) / 127f;                 //layered noise
                                                            

                }

            }

        }

        public void renderIsland(SpriteBatch spriteBatch)
        {
            for (int x = 0; x < 256; x++)
            {
                for (int y = 0; y < 256; y++)
                {

                    Color col = Color.Green;

                    float NoiseVal = noiseVal[x, y];

                    if (NoiseVal < 0.9f) col = Color.Gray;
                    if (NoiseVal < 0.85f) col = Color.DarkGray;
                    if (NoiseVal < 0.70f)
                    {
                        col = Color.Green;
                    }

                    if (NoiseVal < 0.58f) col = Color.MintCream;
                    if (NoiseVal < 0.55f) col = Color.Blue;
                    if (NoiseVal < 0.45f) col = Color.DarkBlue;
                    if (NoiseVal > 0.9f) col = Color.Gray;

                    spriteBatch.Draw(Pix, new Rectangle(x + 512, y, 1, 1), new Color((int)(noiseVal[x, y] * 255f), (int)(noiseVal[x, y] * 255f), (int)(noiseVal[x, y] * 255f)));
                    spriteBatch.Draw(Pix, new Rectangle(x * 2, y * 2, 2, 2), col);

                }

            }
        }
  
        public void setIslandData()
        {
            i++;
            noise.setSeed(i);



            for (int x = 0; x < 256; x++)
            {
                for (int y = 0; y < 256; y++)
                {
              noiseVal[x, y] = (float)noise.perlin(x, y, 0.01f)+// +                  //layered noise
                                       0.5f * noise.perlin(x, y, 0.05f);
                      
                   float val = 1 - (float)Math.Sqrt(Math.Pow(127.5 - x, 2) + Math.Pow(127.5 - y, 2)) / 127.5f;
                   noiseVal[x, y] *= val + 0.3f;
             
                }

            }

        }
    }
}
