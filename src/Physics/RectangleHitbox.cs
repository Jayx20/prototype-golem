using Microsoft.Xna.Framework;

namespace Prototype_Golem.Physics
{
    public class RectangleHitbox : Hitbox
    {
        Rectangle bounds;
        protected override bool IntersectingCircle(CircleHitbox other)
        {
            throw new System.NotImplementedException();
        }

        protected override bool IntersectingRectangle(RectangleHitbox other)
        {
            if (bounds.Intersects(other.bounds)) return true;
            else return false;
        }
    }
}