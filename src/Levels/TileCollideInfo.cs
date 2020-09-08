namespace Prototype_Golem.Levels
{
    public struct TileCollideInfo
    {
        public int MaskID {get; private set;}
        public byte Flags {get; private set;}
        // Only smallest 4 bytes are used at present:
        // 1 - Collide from Top
        // 2 - Collide from Left
        // 4 - Collide from Bottom
        // 8 - Collide from Right
        public TileCollideInfo(int maskID, int flagID) {
            MaskID = maskID;
            Flags = (byte)flagID;
            if (Flags != 0 && maskID == 0) maskID = 1; //if there is a flag for collision, but no mask, just use a solid block mask
        }
    }
}