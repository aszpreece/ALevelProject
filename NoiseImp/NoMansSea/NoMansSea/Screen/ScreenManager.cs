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

    class ScreenManager
    {

        private List<Screen> screens = new List<Screen>(); //contains all screens currently 
        private Input inp;

        public ScreenManager(Input input)
        {
            inp = input;

        }
        
        public bool update(GameTime gameTime) //return false if no menu items :/
        {
            List<Screen> screensToUpdate = new List<Screen>();
            if (screens.Count == 0) return false;
            foreach (Screen scr in screens) screensToUpdate.Add(scr);

            //the 'top' of the screens to update list is the most visible screen

           bool hasFocus = true, isVisible = true; //assume it has focus currently and assume not covered

           while (screensToUpdate .Count >= 1) //updates each screen
            {
                Screen currentScreen = screensToUpdate[screensToUpdate.Count -1];
                screensToUpdate.RemoveAt(screensToUpdate.Count - 1);            

                if (currentScreen.state == ScreenState.Active || currentScreen.state == ScreenState.TransitionOn || currentScreen .state == ScreenState .TransitionOff ) //if its active (i.e if it is blocking input to other screens) AND it sint a popup
                {
                    if (hasFocus && !currentScreen.isPopUp) //if we have found a screen that has focus
                    {
                        currentScreen.update(gameTime, hasFocus, isVisible);
                        hasFocus = false;
                        isVisible = false;
                    }               
                }

            }

           return true;
        }

        public void draw(GameTime gameTime, SpriteBatch sb)
        {
            foreach (Screen scr in screens) scr.draw(gameTime, sb);
        }

        public void addScreen(Screen s)
        {
            if (s != null)
                screens.Add(s);
        }

        public void removeScreen(Screen s)
        {
            if (s != null)
                screens.Remove(s);
        }
    }
}
