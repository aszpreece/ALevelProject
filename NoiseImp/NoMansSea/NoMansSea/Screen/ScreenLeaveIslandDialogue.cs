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


namespace NoMansSea
{
    class ScreenLeaveIslandDialogue : Screen 

    {

        public Screen Parent;

        public ScreenLeaveIslandDialogue(MScreenManager manager, Input input, Screen ParentScreen)
            : base(manager, input) { Parent = ParentScreen; }
        
        public override void update(GameTime gameTime, bool hasFocus)
        {
            state = ScreenState.Active;
            ScreenGame game = (ScreenGame)Parent;
            if (input.isNewPress(Keys.Up ))
            {
                game.Y -= 1;
                game.state = ScreenState.TransitionOff;
                manager.removeScreen(this);     
            }
            else if (input.isNewPress(Keys.Down))
            {
                game.Y += 1;
                game.state = ScreenState.TransitionOff;
                manager.removeScreen(this);    
            }
            else if (input.isNewPress(Keys.Left))
            {
                game.X -= 1;
                game.state = ScreenState.TransitionOff;
                manager.removeScreen(this);     
            }
            else if (input.isNewPress(Keys.Right))
            {
                game.X += 1;
                game.state = ScreenState.TransitionOff;
                manager.removeScreen(this);    
            }
            else if (input.isNewPress(Keys.Escape))
            {
                manager.removeScreen(this);      
            }

        }

        public override void draw(GameTime gameTime, SpriteBatch sb)
        {
            int messageLength = (int)Game1.Font.MeasureString("Which direction would you like to sail in?").X;
            sb.Draw(Game1.Pixel, new Rectangle(Game1.ScreenX / 2 - 400, Game1.ScreenY / 2 - 300, 800, 600), new Color(10, 10, 120, 230));
            sb.DrawString(Game1.Font, "Which direction would you like to sail in?", new Vector2(Game1.ScreenX / 2 - messageLength / 2, 100), Color.White);

            sb.Draw (Game1.Pixel ,new Rectangle (Game1.ScreenX / 2 - 100 - 100,Game1.ScreenY /2 ,100,100),Color.DarkSlateGray); //draw left
            sb.DrawString(Game1.Font, "Left Arrow", new Vector2(Game1.ScreenX / 2 - 100 - 100,Game1.ScreenY /2), Color.White);

            sb.Draw(Game1.Pixel, new Rectangle(Game1.ScreenX / 2 + 100 , Game1.ScreenY / 2, 100, 100), Color.DarkSlateGray); //draw right
            sb.DrawString(Game1.Font, "Right Arrow", new Vector2(Game1.ScreenX / 2 + 100 , Game1.ScreenY / 2), Color.White);

            sb.Draw(Game1.Pixel, new Rectangle(Game1.ScreenX / 2 - 50, Game1.ScreenY / 2 - 100, 100, 100), Color.DarkSlateGray); //draw up
            sb.DrawString(Game1.Font, "Up Arrow", new Vector2(Game1.ScreenX / 2 - 50, Game1.ScreenY / 2 - 100), Color.White);

            sb.Draw(Game1.Pixel, new Rectangle(Game1.ScreenX / 2 - 50, Game1.ScreenY / 2 + 100, 100, 100), Color.DarkSlateGray); //draw down
            sb.DrawString(Game1.Font, "Down Arrow", new Vector2(Game1.ScreenX / 2 - 50, Game1.ScreenY / 2 + 100), Color.White);
        }   
    }
}
