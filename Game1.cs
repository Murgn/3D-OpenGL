using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace My3DGame
{
    public class Game1 : Game
    {
        // Display
        const int SCREENWIDTH = 2024, SCREENHEIGHT = 1576;
        GraphicsDeviceManager graphics;
        GraphicsDevice gpu;
        SpriteBatch spriteBatch;
        SpriteFont font;
        static public int screenWidth, screenHeight;
        Camera cam;

        // Input & Utilities
        Input inp;

        // RenderTargets & Textures
        RenderTarget2D MainTarget;

        //3D
        Basic3DObjects basic3D;
        Model landscape;
        Sky sky;

        // Rectangles
        Rectangle desktopRect;
        Rectangle screenRect;


        public Game1()
        {
            // FOR DEBUGGING PURPOSES, I HAVE -10 FROM HEIGHT AND WIDTH, IN FULL GAME GET RID OF THE -10.
            int desktop_width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            int desktop_height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = desktop_width,
                PreferredBackBufferHeight = desktop_height,
                IsFullScreen = false,
                PreferredDepthStencilFormat = DepthFormat.None,
                GraphicsProfile = GraphicsProfile.HiDef
            };
            //Window.IsBorderless = true;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            gpu = GraphicsDevice;
            PresentationParameters pp = gpu.PresentationParameters;
            MainTarget = new RenderTarget2D(gpu, SCREENWIDTH, SCREENHEIGHT, false, pp.BackBufferFormat, DepthFormat.Depth24);
            screenWidth = MainTarget.Width;
            screenHeight = MainTarget.Height;
            desktopRect = new Rectangle(0, 0, pp.BackBufferWidth, pp.BackBufferHeight);
            screenRect = new Rectangle(0, 0, screenWidth, screenHeight);

            inp = new Input(pp, MainTarget);

            // Init 3D
            cam = new Camera(gpu, Vector3.Up, inp);
            basic3D = new Basic3DObjects(gpu, cam.up, Content);
            sky = new Sky(gpu, Content);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(gpu);
            font = Content.Load<SpriteFont>("Font");

            // Basic 3D
            basic3D.AddCube(50, 50, 50, Vector3.Zero, new Vector3(3.14f,0,0), "bingus", null);
            basic3D.objex[0].pos.Y = 68; basic3D.objex[0].UpdateTransform();
            basic3D.AddCube(50, 50, 0, Vector3.Zero, Vector3.Zero, "erik", null);    // Object 1
            basic3D.objex[1].pos.Y = 5; basic3D.objex[1].UpdateTransform();

            //3D Model Loading
            sky.Load("sky_model");
            landscape = Content.Load<Model>("landscape");
            
        }

        // ADD YOUR GAME LOGIC HERE
        protected override void Update(GameTime gameTime)
        {
            inp.Update();
            if (inp.back_down || inp.KeyDown(Keys.Escape)) Exit(); // change to menu for exit later

            cam.Update_Player_Cam();
            if (inp.KeyDown(Keys.Up)) basic3D.objex[0].pos.Z++;
            if (inp.KeyDown(Keys.Down)) basic3D.objex[0].pos.Z--;
            if (inp.KeyDown(Keys.Left)) basic3D.objex[0].pos.X++;
            if (inp.KeyDown(Keys.Right)) basic3D.objex[0].pos.X--;
            if (inp.KeyDown(Keys.PageUp)) basic3D.objex[0].pos.Y++;
            if (inp.KeyDown(Keys.PageDown)) basic3D.objex[0].pos.Y--;
            basic3D.objex[0].rot.Y += 0.03f;                       // rotate just for fun
            basic3D.objex[0].UpdateTransform();



            base.Update(gameTime);
        }

        // Sets 3D States
        void Set3DStates()
        {
            gpu.BlendState = BlendState.NonPremultiplied; gpu.DepthStencilState = DepthStencilState.Default;
            if(gpu.RasterizerState.CullMode == CullMode.None)
            {
                RasterizerState rs = new RasterizerState { CullMode = CullMode.CullCounterClockwiseFace };
                gpu.RasterizerState = rs; // Device state change requires new instances of RasterizerState
            }
        }

        // RENDER SCENE OBJECTS
        protected override void Draw(GameTime gameTime)
        {
            gpu.SetRenderTarget(MainTarget);
            gpu.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Transparent, 1.0f, 0);

            // Render 3D Objects
            sky.Draw(cam); // Make sure you draw sky first
            Set3DStates();
            basic3D.Draw(cam);

            //Render Models
            DrawModel(landscape);
            
            // Draws MainTarget to BackBuffer
            gpu.SetRenderTarget(null);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone);
            spriteBatch.Draw(MainTarget, desktopRect, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        void DrawModel(Model model)
        {
            foreach(ModelMesh mesh in model.Meshes)
            {
                foreach(BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                    effect.TextureEnabled = true;
                    effect.View = cam.view;
                    effect.Projection = cam.proj;
                    effect.AmbientLightColor = new Vector3(0.1f, 0.2f, 0.3f);
                    effect.DiffuseColor = new Vector3(0.94f, 0.94f, 0.94f);
                    effect.FogEnabled = true;
                    effect.FogStart = 15f;
                    effect.FogEnd = 500f;
                    effect.FogColor = new Vector3(0f, 0f, 0f);
                }
                mesh.Draw();
            }
        }
    }
}
