namespace Prototype_Golem.Physics
{
    public class CircleHitbox : Hitbox
    {
        int radius;
        protected override bool IntersectingCircle(CircleHitbox other)
        {
            throw new System.NotImplementedException();
        }

        protected override bool IntersectingRectangle(RectangleHitbox other)
        {
            throw new System.NotImplementedException();
        }
    }
}