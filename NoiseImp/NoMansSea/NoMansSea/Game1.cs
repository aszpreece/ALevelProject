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

namespace NoMansSea
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public const int ScreenX = 1280, //constants for controlling resolution. 
                         ScreenY = 720;

        MScreenManager screenManager;
        Input inputManager;

        public static SpriteFont Font;

        public static Texture2D Pixel, //stores textures for all types of tile
                                Grass,
                                HighMtn,
                                LowMtn,
                                Sand,
                                Water,
                                Torch,
                                PlayerTexture,
                                EntityTexture;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = ScreenX;
            graphics.PreferredBackBufferHeight = ScreenY;
            graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            //initialise all texture objects.

            Pixel = new Texture2D(this.GraphicsDevice, 1, 1);
            PlayerTexture = new Texture2D(this.GraphicsDevice, 32, 32);
            EntityTexture = new Texture2D(this.GraphicsDevice, 32, 32);
            Grass = new Texture2D(this.GraphicsDevice, Tile.TileSideLengthInPixels, Tile.TileSideLengthInPixels);
            HighMtn = new Texture2D(this.GraphicsDevice, Tile.TileSideLengthInPixels, Tile.TileSideLengthInPixels);
            LowMtn = new Texture2D(this.GraphicsDevice, Tile.TileSideLengthInPixels, Tile.TileSideLengthInPixels);
            Sand = new Texture2D(this.GraphicsDevice, Tile.TileSideLengthInPixels, Tile.TileSideLengthInPixels);
            Water = new Texture2D(this.GraphicsDevice, Tile.TileSideLengthInPixels, Tile.TileSideLengthInPixels);
            Torch = new Texture2D(this.GraphicsDevice, Tile.TileSideLengthInPixels, Tile.TileSideLengthInPixels);

            Pixel.SetData<Color>(new Color[] { Color.White });

            //generate all textures using pre written functions. The fucntions take in a seed to generate them.

            GenTextures.setEntityTexture(1, ref EntityTexture);
            GenTextures.setGrassTexture(18390123, ref Grass);
            GenTextures.setStoneTexture(18390123, ref HighMtn);
            GenTextures.setStoneTexture(2134, ref LowMtn);
            GenTextures.setSandTexture(18390123, ref Sand);
            GenTextures.setWaterTexture(12341174, ref Water);
            GenTextures.setTorchTexture(45642323, ref Torch);

            //Initialize input manager for input
            inputManager = new Input();

            //initialize screen manager object and add an instance of the main menu
            screenManager = new MScreenManager(inputManager);
            screenManager.addScreen(new ScreenMainMenu(screenManager, inputManager));

            base.Initialize();
        }


        protected override void LoadContent()
        {

            spriteBatch = new SpriteBatch(GraphicsDevice);

            Font = Content.Load<SpriteFont>("Kootenay");

        }

        protected override void UnloadContent()
        {

        }




        protected override void Update(GameTime gameTime)
        {
            inputManager.update(gameTime);
          if (!screenManager.update(gameTime)) Exit();

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null);
            screenManager.draw(gameTime, spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }




    }
}
