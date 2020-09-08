namespace Prototype_Golem.Physics
{
    public abstract class Hitbox
    {
        public bool IntersectingHitbox(Hitbox other) {
            if (other is RectangleHitbox)
                return IntersectingRectangle((RectangleHitbox)other);
            else if (other is CircleHitbox)
                return IntersectingCircle((CircleHitbox)other);
            else
                throw new System.ArgumentException($"Hitbox type {other.GetType()} is invalid");
        }
        protected abstract bool IntersectingRectangle(RectangleHitbox other);
        protected abstract bool IntersectingCircle(CircleHitbox other);
    }
}