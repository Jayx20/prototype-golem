using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System.Collections.Generic;

using TiledSharp;

namespace Prototype_Golem
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch spriteBatch;

        Camera camera = new Camera();
        TmxMap testmap1;
        Dictionary<string, Texture2D> tilemapTextures = new Dictionary<string, Texture2D>();

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            testmap1 = new TmxMap("Maps/test1.tmx");

            Texture2D prototexture = Content.Load<Texture2D>("Images/prototype1"); //this variable will dissapear so the stupid name is fine the actual data is saved in the dictionary

            tilemapTextures.Add("prototype1", prototexture);

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            //TODO: input class to at the very least add multiple buttons for one thing for controller support
            //additionally, an input class would make this look less dumb.
            if(Keyboard.GetState().IsKeyDown(Keys.Left)) {
                camera.Pos += new Vector2(5.0f,0);
            }
            if(Keyboard.GetState().IsKeyDown(Keys.Right)) {
                camera.Pos += new Vector2(-5.0f,0);
            }
            if(Keyboard.GetState().IsKeyDown(Keys.Up)) {
                camera.Pos += new Vector2(0,5.0f);
            }
            if(Keyboard.GetState().IsKeyDown(Keys.Down)) {
                camera.Pos += new Vector2(0,-5.0f);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            TmxTileset baseTileset = testmap1.Tilesets["prototype1"];
            int columns = baseTileset.Columns ?? -1;
            if (columns == -1) {
                throw new System.ArgumentException("HOW");
            } 

            //i should probably make a class or something that combines the TmxTileset and the actual Texture2D or just find a way to load the texture from the tileset instead of from monogames content manager
            //TODO: make this less stupid

            Texture2D tilesetTexture;

            if (!tilemapTextures.TryGetValue("prototype1", out tilesetTexture)) { //"prototype1" would be a variable after we find what tileset the map uses
                //runs if loading the image failed
                throw new System.ArgumentException("Loading tilemap texture failed.");
            }

            TmxLayer baseLayer = testmap1.Layers["base"];

            spriteBatch.Begin();

            var tiles = baseLayer.Tiles;
            foreach(TmxLayerTile tile in tiles) {
                if (tile.Gid == 0) continue; //air
                
                Rectangle destRect = new Rectangle(tile.X*testmap1.TileWidth, tile.Y*testmap1.TileHeight, testmap1.TileWidth, testmap1.TileHeight); //where on the screen the tile is going to be drawn, based on the position of the tile

                destRect.Location += camera.Pos.ToPoint();
                //TODO: stop here if the location of the tile is outside the screen

                int textureId = tile.Gid - baseTileset.FirstGid; 

                //funny math that has to do with 2d arrays and stuff
                    int textureX = textureId % columns;
                    int textureY = textureId / columns;

                Rectangle sourceRect = new Rectangle(textureX*testmap1.TileWidth, textureY*testmap1.TileHeight, testmap1.TileWidth, testmap1.TileHeight);
                spriteBatch.Draw(tilesetTexture, destRect, sourceRect, Color.White);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
