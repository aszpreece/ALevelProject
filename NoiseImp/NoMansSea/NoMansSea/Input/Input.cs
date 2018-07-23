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
    class Input
    {
        KeyboardState currentState, oldState;

        public void update(GameTime gameTime)
        {
            if (currentState == null) currentState = Keyboard.GetState(); //avoid any null reference exceptions
            oldState = currentState;
            currentState = Keyboard.GetState();
        }

        //returns true if the key is a new press this update
        public bool isNewPress(Keys key)
        {
           return(currentState.IsKeyDown(key) && oldState.IsKeyUp(key));
        }

        //returns true if key is simply pressed
        public bool isPressed(Keys key)
        {
            return currentState.IsKeyDown(key);
        }

        public Keys[] getKeysPressed()
        {
            return currentState.GetPressedKeys();
        }
    }
}
