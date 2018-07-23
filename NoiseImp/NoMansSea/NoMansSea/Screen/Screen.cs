using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NoMansSea
{
    abstract class Screen
    {

        public ScreenState state { get; set; } //state of the screen, active, inactive or transitioning on or off.
        public bool isPopUp { get; set; }

        protected MScreenManager manager = null; //what screen manager this belongs to
        protected Screen parent = null; //does this screen have a parent?
        protected Input input; //refence to the input object to get input.

        //contructors, one allowing for an optional parent parameter.

        public Screen(MScreenManager manager, Input input)
        {
            this.manager = manager;
            this.input = input;
        }

        public Screen(MScreenManager manager, Input input, Screen parent)
        {
            this.manager = manager;
            this.parent = parent;
            this.input = input;
        }
        
        public abstract void update(GameTime gameTime, bool hasFocus);
        public abstract void draw(GameTime gameTime, SpriteBatch sb);

    }
}
