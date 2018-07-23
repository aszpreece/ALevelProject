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
    class ScreenSeedDialogue : Screen
    {

        public ScreenSeedDialogue(MScreenManager manager, Input input)
            : base(manager, input) { }

        float KeyTimer;
        string currentEntry;

        UInt16 X, Y;
        char XORY = 'X' ; //controls whether we are taking the coordinate for the X or Y at this current time.
        bool InvalidEntry = false;
        public override void update(GameTime gameTime, bool hasFocus)
        {
            
            //A timer so too many keys aren't added too quickly.
            if (KeyTimer > 0) KeyTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (KeyTimer <= 0)
            {
                Keys[] pressedKeys = input.getKeysPressed();
                
                foreach (Keys key in pressedKeys)
                {    
                    if (key == Keys.Back && currentEntry.Length >= 1) //handles the user pressing backspace.
                    {
                        currentEntry = currentEntry.Remove(currentEntry.Length - 1);
                        KeyTimer = 0.125f; //sets the timer to 1/8th of a second
                    }
                    //Numerical keys (0 to 9) are in the format 'D' followed by the number. This checks that the code begins with a D,
                    //Is more than two characters long, and that its last character is a digit. If it is add it to the current entry.
                    string code = key.ToString();
                    if (code[0] == 'D' && code.Length > 1 && char.IsDigit(code, 1))
                    {
                        if (InvalidEntry) //if the user previously entered an invalid number we need to get rid of the error message
                        {
                            currentEntry = "";
                            InvalidEntry = false;
                        }
                        currentEntry += code[1];
                        KeyTimer = 0.125f; //sets the timer to 1/8th of a second
                        break;
                    }
                }
            }
            if (input.isNewPress(Keys.Escape))
            {
                manager.removeScreen(this);
                manager.addScreen(new ScreenMainMenu(manager, input));
            }
            
            else if (input.isNewPress(Keys.Enter))
            {
                UInt16 AttemptedEntry;
            
                    if (!UInt16.TryParse(currentEntry, out AttemptedEntry)) //checks if the has entered a valid unsigned short.
                    {
                        currentEntry = "Error"; //displays an error if not
                        InvalidEntry = true;
                    }else if (XORY == 'X') //if the user is being prompted for the X value
                    {
                        X = AttemptedEntry;
                        currentEntry = "";
                        XORY = 'Y';
                    }
                    else if (XORY == 'Y') //when the user has entered the Y value we can launch the game screen with the entered coords.
                    {
                        Y = AttemptedEntry;
                        manager.removeScreen(this);
                        manager.addScreen(new ScreenGame(manager, input, X, Y));

                    }

            }          
        }

        public override void draw(GameTime gameTime, SpriteBatch sb)
        {          
            sb.DrawString(Game1.Font, "Enter the " + XORY + " of the island you would like to visit:" + currentEntry,
                new Vector2(Game1.ScreenX / 2 - Game1.Font.MeasureString("Enter the" + XORY + "of the island you would like to visit:" + currentEntry).X / 2, 100),
                Color.Beige);        
        }
    }
}
