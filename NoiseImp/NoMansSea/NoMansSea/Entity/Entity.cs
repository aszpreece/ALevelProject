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
    abstract class Entity
    {

        const int MAX_SPEED = Tile.TileSideLengthInPixels;
  
        //every entity in the game requires a velocity (Even if it remains 0 constantly) and a position.
        public Vector2 velocity,
                       position;

        protected int FrameSizeInPixels = 32;

       //these control how fast the entity accelerates and decelerates. 
        protected float DecelerationCoeff = 255,
                        AccelerationPixelsPSPS = 1023;

        //every entity must define its own unique update routine.
        public abstract void update(GameTime gt, Input input, Level level);

        //virtual means the method can be overridden but the method provides a defualt way to render the entity's sprite. 
        public virtual void render(SpriteBatch SB, Vector2 ScrollOffset)
        {
            SB.Draw(Game1.Pixel, new Rectangle((int)(position.X - 0.5f * FrameSizeInPixels - ScrollOffset.X), (int)(position.Y - 0.5f * FrameSizeInPixels - ScrollOffset.Y), (int)FrameSizeInPixels, (int)FrameSizeInPixels), //the destination rectangle
         new Rectangle((int)(FrameSizeInPixels), (int)((FrameSizeInPixels)), FrameSizeInPixels, FrameSizeInPixels), Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 1); //the source rectangle
        }
  
        public void move(GameTime gameTime, Level level)
        {
            capVelocity(gameTime);

            //calculate the two positions the entity will be in if it was only moved in one axis. 
     
            Vector2 newPosX = new Vector2(position.X + velocity.X * (float)gameTime.ElapsedGameTime.TotalSeconds, position.Y), 
                    newPosY = new Vector2(position.X, position.Y + velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds);
  
            //check if there is a tile at either of these positions. If there is half the velocity and reverse it in the appropriate direction.
            //This will make the entity bounce.

            if (checkCollision(newPosX, level))
            {
                velocity *= new Vector2(-0.5f, 0.5f);
            }
                  
            if (checkCollision(newPosY, level))
            {        
                velocity *= new Vector2(0.5f, -0.5f);
            }

            //finally add the displacement to the player. This is calculate by multiplying the velocity by the time since the last update in seconds.
            position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void applyFriction(GameTime gameTime)
        {
            float friction = DecelerationCoeff * (float)gameTime.ElapsedGameTime.TotalSeconds; //calculate the magnitute if the friction to be applied relative to delta time.
            float speed = velocity.Length(); //calculate speed (Length of velocity vector)
            if (friction < speed) //if the magnitude of the friction that is to be applied is smaller than the magnitude of the speed apply the friction.
            {
                velocity += friction * -(velocity / speed);
            }
            else
            {
                velocity = new Vector2(); //if not set the friction to 0.
            }
      
        }

        public void capVelocity(GameTime gameTime)
        {
            float speed = (velocity * (float)gameTime.ElapsedGameTime.TotalSeconds).Length();

            if (speed >= MAX_SPEED){
                velocity.Normalize();
                velocity *= MAX_SPEED / (float)gameTime .ElapsedGameTime .TotalSeconds;
            }

        }

        public bool checkCollision(Vector2 location, Level level)
        {

            Vector2 CurrentTileToCheck = new Vector2();
            TileType posTile;

            //check top left
            CurrentTileToCheck.X = location.X - 0.5f * (FrameSizeInPixels - 1);
            CurrentTileToCheck.Y = location.Y - 0.5f * (FrameSizeInPixels - 1);
            posTile = level.getTileAtPosition(CurrentTileToCheck);

            if (posTile == TileType.Mountain || posTile == TileType.Ridge || posTile == TileType.OceanWall) return true;

            //check top right
            CurrentTileToCheck.X = location.X + 0.5f * (FrameSizeInPixels);
            CurrentTileToCheck.Y = location.Y - 0.5f * (FrameSizeInPixels - 1);
            posTile = level.getTileAtPosition(CurrentTileToCheck);

            if (posTile == TileType.Mountain || posTile == TileType.Ridge || posTile == TileType.OceanWall) return true;

            //check bottom left
            CurrentTileToCheck.X = location.X - 0.5f * (FrameSizeInPixels - 1);
            CurrentTileToCheck.Y = location.Y + 0.5f * (FrameSizeInPixels);
            posTile = level.getTileAtPosition(CurrentTileToCheck);

            if (posTile == TileType.Mountain || posTile == TileType.Ridge || posTile == TileType.OceanWall) return true;

            //check bottom right
            CurrentTileToCheck.X = location.X + 0.5f * (FrameSizeInPixels - 1);
            CurrentTileToCheck.Y = location.Y + 0.5f * (FrameSizeInPixels - 1);
            posTile = level.getTileAtPosition(CurrentTileToCheck);

            if (posTile == TileType.Mountain || posTile == TileType.Ridge || posTile == TileType.OceanWall) return true;

            return false;
        }

    }
}
