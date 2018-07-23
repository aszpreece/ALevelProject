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
    class ScreenMainMenu : Screen 

    {  

         public ScreenMainMenu(MScreenManager manager, Input input)
            : base(manager, input)
        {

        }
         public override void update(GameTime gameTime, bool hasFocus)
         {
             if (input.isNewPress(Keys.Escape)) //if the player chooses to quit, remove this from the main menu.
             {
                 manager.removeScreen(this);
             }
             else if (input.isNewPress(Keys.Enter)) //if they choose to continue, add the game scren and remove this from the list.
             {
                 manager.addScreen(new ScreenGame(manager, input));
                 manager.removeScreen(this);
             }
             else if (input.isNewPress(Keys.S)) //if they choose to select a seed, add the seed scren and remove this from the list.
             {
                 manager.addScreen(new ScreenSeedDialogue(manager, input));
                 manager.removeScreen(this);
             }

         }

        public override void draw(GameTime gameTime, SpriteBatch sb)
        {
            sb.DrawString(Game1.Font, "Enter: Start\r\nS: Enter Seed\r\nEscape: Exit ", new Vector2((Game1.ScreenX / 2) - (Game1.Font.MeasureString("Enter: Start").X / 2), 100), Color.Beige);  
        }
    }
}
