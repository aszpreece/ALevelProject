using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace NoMansSea
{
    class ScreenGame : Screen
    {

        public UInt16 X, Y; //coordinates of the island
        Level Island;

        public ScreenGame(MScreenManager manager, Input input, UInt16 IslandX = 0, UInt16 IslandY = 0)
            : base(manager, input) {
                Island = new Level(input);               
                state = ScreenState.TransitionOn;
                X = IslandX;
                Y = IslandY;
                Island.generate(X, Y);
        }

        //used for the transition fade between islands
        double FadeTimer = 0;  
        int OverlayAlpha = 255;

        //used to time the tile update for the ocean tiles
        double OceanTextureUpdateTimer = 0;

        public override void update(GameTime gameTime, bool hasFocus)
        {
            //each update the timer for the ocean tile update is increased by the elapsed time between the last update and this one.
            OceanTextureUpdateTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (OceanTextureUpdateTimer >= 1) //if the elapsed time is more than one second then generate a new texture for the water tiles and reset the timer
            { 
                GenTextures.setWaterTexture(new Random().Next(), ref Game1.Water); 
                OceanTextureUpdateTimer = 0f; 
            }     
   
            //if the screen is active
            if (state == ScreenState.Active){
             
                //handles input. If T is pressed we need to make the GUI to leave the island appear, if escape is pressed then we need to pause the game  
                if (input.isNewPress(Keys.T))
                {
                    manager.addScreen(new ScreenLeaveIslandDialogue(manager, input, this));
                }
                if (input.isNewPress(Keys.Escape))
                {
                    manager.addScreen(new ScreenPauseGame(manager, input, this));
                }

                Island.update(gameTime); //update the Island object where our level is stored.
                OverlayAlpha = 0;
            }
              //when the screen is transitioning on we need to increase the overlay alpha until it is 255 which means that it is completely transparent.
            else if (state == ScreenState.TransitionOn) 
            {

                Island.update(gameTime);
                FadeTimer += gameTime.ElapsedGameTime.TotalSeconds;
                if (FadeTimer <= 2)
                {
                    OverlayAlpha = 255 - (int)(FadeTimer / 2.0 * 255);
                }
                else
                {
                    FadeTimer = 0;
                    state = ScreenState.Active;
                }
            }
            //when transitioning off we need to decrease the overlay alpha until it is 0. When it is 0 we need to generate a new island
            //and set the screen to transition back on.
            else if (state == ScreenState.TransitionOff)
            {

                OverlayAlpha = (int)(FadeTimer / 2.0 * 255);
          
                FadeTimer += gameTime.ElapsedGameTime.TotalSeconds;
                if (FadeTimer >= 2)
                {
                    Island.generate(X, Y);
                    FadeTimer = 0;
                    state = ScreenState.TransitionOn;
                }

            }

        }


        public override void draw(GameTime gameTime, SpriteBatch sb)
        {
            //only draw if teh screen is active.
            if (state == ScreenState.Active || state == ScreenState .TransitionOn || state == ScreenState .TransitionOff )
                Island.Draw(sb);
            //draw the overlay box over the entire screen.
            sb.Draw(Game1.Pixel, new Rectangle(0, 0, Game1.ScreenX, Game1.ScreenY), new Color(0, 0, 0, OverlayAlpha ));
            sb.DrawString(Game1.Font, "Island Seed: " + Island.Seed.ToString(), new Vector2(4, 4), Color.White);
            sb.DrawString(Game1.Font, "X: " + X.ToString(), new Vector2(4, 24), Color.White);
            sb.DrawString(Game1.Font, "Y: " + Y.ToString(), new Vector2(4, 44), Color.White);


        }
    
    }
}
