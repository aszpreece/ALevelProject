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
    class ScreenPauseGame : Screen 

    {


        public ScreenPauseGame(MScreenManager manager, Input input, Screen ParentScreen)
            : base(manager, input) { parent = ParentScreen; }
        
        

        public override void update(GameTime gameTime, bool hasFocus)
        {

            //if the person decides not to quit then remove this screen.
            if (input.isNewPress(Keys.N))
            {
                manager.removeScreen(this);
            }
            //if the suer decided to quit then remove this screen, the game screen and add a new screen for the main menu.
            else if (input.isNewPress(Keys.Y))
            {
                manager.removeScreen(this);
                manager.removeScreen(parent);
                manager.addScreen(new ScreenMainMenu(manager, input));
            }
          
        }

        public override void draw(GameTime gameTime, SpriteBatch sb)
        {
            sb.DrawString(Game1.Font, "Do you want to quit? Y: Yes N: No", 
                new Vector2(Game1.ScreenX / 2 - Game1.Font.MeasureString("Do you want to quit? Y: Yes N: No").X / 2, 100), 
                Color.Beige);
        }

    
    }
}
