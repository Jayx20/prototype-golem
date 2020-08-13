namespace Prototype_Golem
{
    public static class Constants
    {
        //Graphics
        public const int SCREEN_WIDTH = 1280;
        public const int SCREEN_HEIGHT = 720;

        //Player Control Timing
        //All times are in frames, so 1/60th of a second
        public const int JUMP_REMEMBER_TIME = 4;
        public const int COYOTE_TIME = 4;
        
        //Player Physics
        public const float GRAVITY_STRENGTH = 0.02f;
        public const float MOVEMENT_SPEED = 0.2f;
        public const float JUMP_REDUCTION_MULTIPLIER = .75f;
        public const float JUMP_FORCE = .42f;

        //Camera
        public const float CAMERA_SPEED = 5f;
        public const float CAMERA_ZOOM_SPEED = .05f;

        //Collision
        public const float COLLISION_PUSH_DISTANCE = 0.001f;
    }
}