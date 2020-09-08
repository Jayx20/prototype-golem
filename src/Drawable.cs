using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Prototype_Golem
{
    public class Drawable
    {
        public TextureID TextID {get; protected set;}
        public Rectangle TextRect {get; protected set;}
        public SpriteEffects Effects {get; protected set;} = SpriteEffects.None;

    }
}