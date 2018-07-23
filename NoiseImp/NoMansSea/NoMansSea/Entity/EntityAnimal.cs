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
    class EntityAnimal : Entity 
    {
        static Random r = new Random();

        float SpeedPixelsPS = 128; //animals don't accelerate, they only have a constant speed. 

        public bool isParent { get; set; }

        bool child;

        public bool isChild
        {
            get { return child; }

            set
            {
                if (value) FrameSizeInPixels = 16; //when set to be a child, make the entity half the size else set it to be normal size.
                else FrameSizeInPixels = Tile.TileSideLengthInPixels;
                child = value;
            }
        }

        public EntityAnimal parent { get; set; }

        public bool isChasing { get; set; }

        public EntityAnimal(Vector2 Position, bool child = false)
        {   
            
            position.X = Position.X;
            position.Y = Position.Y;
            isChild = child;
            velocity = new Vector2();
        }

        public override void update(GameTime gt, Input input, Level level)
        {

            //depending on the status of teh the animal, do different things.

            if (isParent) follow(level.Player, gt, 15, 1);
            else if(isChild ) follow (parent, gt, 30, 1);
            else idleRoam(gt);

            move(gt, level);
        }

        //follows the player if within a specific range.
        private void follow(Entity e, GameTime gt, float maxDist, float minDist = 0)
        {
            Vector2 dist = e.position - position;
            if (dist.Length() < maxDist * Tile.TileSideLengthInPixels && dist.Length() > Tile.TileSideLengthInPixels * minDist)
            {
                isChasing = true;
                dist.Normalize();
                velocity = dist * SpeedPixelsPS;
            }
            else
            {
                isChasing = false;
                velocity = new Vector2();
            }
        }

        //keeps track of how long the naimal has been going in a certain direction.
        float changeDirectionTimer = 0;
        public void idleRoam(GameTime gt)
        {
 
            changeDirectionTimer += (float)gt.ElapsedGameTime.TotalSeconds;

            if (changeDirectionTimer > 3 && !isChasing && r.NextDouble() > 0.8d)
            {
                velocity = new Vector2((float)r.NextDouble() - 0.5f, (float)r.NextDouble() - 0.5f) * SpeedPixelsPS;
                changeDirectionTimer = 0;
            }
        }   

        public override void render(SpriteBatch SB, Vector2 ScrollOffset)
        {
            SB.Draw(Game1.EntityTexture, new Rectangle((int)(position.X - 0.5f * FrameSizeInPixels - ScrollOffset.X), (int)(position.Y - 0.5f * FrameSizeInPixels - ScrollOffset.Y), FrameSizeInPixels, FrameSizeInPixels), //the destination rectangle
       null, Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 1); //the source rectangle from the texture
        }

        public void spawnChild(Level level)
        {
            isParent = true;
            EntityAnimal e = new EntityAnimal(position + new Vector2(10, 10), true);
            level.addEntity(e);
        }
    }
}
