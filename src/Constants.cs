namespace Prototype_Golem
{
    public static class Constants
    {
        //Graphics
        public const int TILE_WIDTH = 32; //key variable
        public const int SCREEN_WIDTH = 1280;
        public const int SCREEN_HEIGHT = 720;

        //Player Control Timing
        //All times are in frames, so 1/60th of a second
        public const int JUMP_REMEMBER_TIME = 4;
        public const int COYOTE_TIME = 4;
        
        //Player Physics
        public const float GRAVITY_STRENGTH = 0.64f;
        public const float JUMP_REDUCTION_MULTIPLIER = .75f;
        public const float JUMP_FORCE = 13.44f;
        public const float PLR_MAX_SPEED = 7.0f;
        public const float PLR_ACCELERATION = 1.2f;
        public const float ACCELERATION_DAMPING = .1f; //damping that makes it harder to gain momentum as you go faster
        public const float PLR_FRICTION = 0.1f; //slowdown when player stops trying to move
        public const float STOP_SPEED = .1f; //if player slows down below this speed they will just stop

        //Animations
        public const int FRAMES_PER_ANIMATION_STEP = 3;

        //Camera
        public const float CAMERA_SPEED = 8f;
        public const float CAMERA_ZOOM_SPEED = .02f;

        //Collision
        public const float COLLISION_PUSH_DISTANCE = 0.001f;
    }
}