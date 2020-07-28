namespace Prototype_Golem
{
    public class Area
    {
        int width; //the width of the map in tiles
        int height; //the height of the map in tiles

        int[] tiles;

        private Area(int width, int height, int[] tiles) {
            this.width = width;
            this.height = height;
            this.tiles = tiles;
        }

        static Area load() {
            int[] test = {3,2,1};
            return new Area(1,2,test);
        }
    }
}