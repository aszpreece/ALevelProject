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

    //enum storing all possible screen states
    public enum ScreenState 
    {      
        Active,
        Hidden,
        TransitionOn,
        TransitionOff
    }

    class MScreenManager
    {

        private List<Screen> screens = new List<Screen>(); //contains all screens currently being helf my the manager
        private Input inp; //reference to the input object.

        public MScreenManager(Input input)
        {
            inp = input; //initialise reference to the input object where this class and all reliant classes will get their input from.
        }
        
        public bool update(GameTime gameTime) //return false if no menu items
        {
            List<Screen> screensToUpdate = new List<Screen>();
            if (screens.Count == 0) return false;
            foreach (Screen scr in screens) screensToUpdate.Add(scr);

            //the 'top' of the screens to update list is the most visible screen

            bool hasFocus = true; //assume it has focus currently (As in input typed into the eyboard will be handled by the top-most screen, UNLESS it is a pop up)

           while (screensToUpdate .Count >= 1) //updates each screen
            {
                Screen currentScreen = screensToUpdate[screensToUpdate.Count -1];
                screensToUpdate.RemoveAt(screensToUpdate.Count - 1);            

                if (currentScreen.state == ScreenState.Active || 
                    currentScreen.state == ScreenState.TransitionOn || 
                    currentScreen .state == ScreenState.TransitionOff) //if its active (i.e if it is blocking input to other screens) AND it isn't a popup
                {
                    if (hasFocus && !currentScreen.isPopUp) //if we have found a screen that has focus
                    {
                        currentScreen.update(gameTime, hasFocus);
                        hasFocus = false; //no other screen can have focus in this update pass.
                    }               
                }
            }
           return true;
        }

        //draws all screens in the list of screens.
        public void draw(GameTime gameTime, SpriteBatch sb)
        {
            foreach (Screen scr in screens) scr.draw(gameTime, sb);
        }

        //adds a screen to the list of screens.
        public void addScreen(Screen s)
        {
            if (s != null)
                screens.Add(s);
        }

        //removes a screen from the list of screens.
        public void removeScreen(Screen s)
        {
            if (s != null)
                screens.Remove(s);
        }
    }
}
