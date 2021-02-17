using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace My3DGame
{
    public class Game1 : Game
    {
        // Display
        const int SCREENWIDTH = 1024, SCREENHEIGHT = 576;
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

        // Rectangles
        Rectangle desktopRect;
        Rectangle screenRect;


        public Game1()
        {
            // FOR DEBUGGING PURPOSES, I HAVE -10 FROM HEIGHT AND WIDTH, IN FULL GAME GET RID OF THE -10.
            int desktop_width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 10;
            int desktop_height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 10;

            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = desktop_width,
                PreferredBackBufferHeight = desktop_height,
                IsFullScreen = false,
                PreferredDepthStencilFormat = DepthFormat.None,
                GraphicsProfile = GraphicsProfile.HiDef
            };
            Window.IsBorderless = true;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            gpu = GraphicsDevice;
            PresentationParameters pp = gpu.PresentationParameters;
            spriteBatch = new SpriteBatch(gpu);
            MainTarget = new RenderTarget2D(gpu, SCREENWIDTH, SCREENHEIGHT, false, pp.BackBufferFormat, DepthFormat.Depth24);
            screenWidth = MainTarget.Width;
            screenHeight = MainTarget.Height;
            desktopRect = new Rectangle(0, 0, pp.BackBufferWidth, pp.BackBufferHeight);
            screenRect = new Rectangle(0, 0, screenWidth, screenHeight);

            inp = new Input(pp, MainTarget);

            // Init 3D
            cam = new Camera(gpu, Vector3.Down, inp); // Change Vector3.Down to Vector3.Up if the models need it
            basic3D = new Basic3DObjects(gpu, cam.up, Content);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("Font");

            basic3D.AddFloor(320, 180, Vector3.Zero, Vector3.Zero, "bingus", null);
            basic3D.AddCube(50, 50, 50, Vector3.Zero, Vector3.Zero, "erik", null);    // Object 1
            basic3D.AddCube(50, 50, 50, Vector3.Zero, Vector3.Zero, "test_image", null);     // Object 0
           
            basic3D.objex[1].pos = new Vector3(10, -20, 40);
           


        }

        // ADD YOUR GAME LOGIC HERE
        protected override void Update(GameTime gameTime)
        {
            inp.Update();
            if (inp.back_down || inp.KeyDown(Keys.Escape)) Exit(); // change to menu for exit later

            Console.Write("Leftstick Y: " + inp.gp.ThumbSticks.Left.Y + "\n");
            Console.Write("Leftstick X: " + inp.gp.ThumbSticks.Left.X + "\n");

            //cam.MoveCamera(new Vector3(inp.Horizontal, 0, inp.Vertical));
            cam.MoveCamera(new Vector3(inp.gp.ThumbSticks.Left.Y * -1, inp.gp.ThumbSticks.Right.Y, inp.gp.ThumbSticks.Left.X));
            cam.Update_Player_Cam();
            if (inp.KeyDown(Keys.Up)) basic3D.objex[0].pos.Z++;
            if (inp.KeyDown(Keys.Down)) basic3D.objex[0].pos.Z--;
            if (inp.KeyDown(Keys.Left)) basic3D.objex[0].pos.X--;
            if (inp.KeyDown(Keys.Right)) basic3D.objex[0].pos.X++;
            if (inp.KeyDown(Keys.PageUp)) basic3D.objex[0].pos.Y--;
            if (inp.KeyDown(Keys.PageDown)) basic3D.objex[0].pos.Y++;
           // basic3D.objex[1].rot.Y += 0.03f;                       // rotate just for fun
            basic3D.objex[1].UpdateTransform();



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
            Set3DStates();
            gpu.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Transparent, 1.0f, 0);

            // Render 3D Objects
            basic3D.Draw(cam);

            // Draws MainTarget to BackBuffer
            gpu.SetRenderTarget(null);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone);
            spriteBatch.Draw(MainTarget, desktopRect, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
