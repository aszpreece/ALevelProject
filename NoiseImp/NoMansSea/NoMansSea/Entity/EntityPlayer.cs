using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;

namespace NoMansSea
{
    class EntityPlayer : Entity
    {

        public EntityPlayer(Vector2 Position)
        {
            position.X = Position.X;
            position.Y = Position.Y;
            velocity = new Vector2();
        }


        public override void update(GameTime gt, Input input, Level level)
        {
            //calculates the direction the player is accelerating in based on which arrow keys they are pressing. 
            Vector2 AccelerationDirection = new Vector2();

            if (input.isPressed(Keys.Down))
            {
                AccelerationDirection.Y = 1;
            }
            else if (input.isPressed(Keys.Up))
            {
                AccelerationDirection.Y = -1;
            }
            if (input.isPressed(Keys.Left))
            {
                AccelerationDirection.X = -1;

            }
            else if (input.isPressed(Keys.Right))
            {
                AccelerationDirection.X = 1;
            }

            if (AccelerationDirection.LengthSquared() != 0)
                AccelerationDirection.Normalize();

            //accelerates the player in the direction they were trying to move in by their acceleration constant. 
            velocity += AccelerationDirection * (float)gt.ElapsedGameTime.TotalSeconds * AccelerationPixelsPSPS;
            //applies friction to slow the player down.         
            applyFriction(gt);
          
            move(gt, level);
        }
    }
}
