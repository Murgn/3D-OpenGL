using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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

        // Input & Utilities
        Input inp;

        // RenderTargets & Textures
        RenderTarget2D MainTarget;
        Texture2D test_tex;

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
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("Font");
            test_tex = Content.Load<Texture2D>("test_image");

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            inp.Update();
            if (inp.back_down || inp.KeyDown(Keys.Escape)) Exit();

            // TODO: Add your update logic here

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

        protected override void Draw(GameTime gameTime)
        {
            gpu.SetRenderTarget(MainTarget);
            Set3DStates();
            gpu.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Transparent, 1.0f, 0);

            // TODO: RENDER SCENE OBJECTS

            // Draws MainTarget to BackBuffer
            gpu.SetRenderTarget(null);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone);
            spriteBatch.Draw(MainTarget, desktopRect, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
