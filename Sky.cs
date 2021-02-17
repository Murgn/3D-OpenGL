using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace My3DGame
{
    class Sky
    {
        GraphicsDevice gpu;
        ContentManager Content;
        Model model;
        public Texture2D texture;
        Matrix translate;  // used to adjust starting position of dome
        float rotate;
        readonly float rotatespeed = 0.002f;

        // Constructor
        public Sky(GraphicsDevice GPU, ContentManager content)
        {
            gpu = GPU; Content = content;
        }

        // Load
        public void Load(string SkyModelName)
        {
            model = Content.Load<Model>(SkyModelName);
            rotate = 0;
        }

        // Draw
        public void Draw(Camera cam)
        {
            gpu.BlendState = BlendState.Opaque;                 // No transparency
            gpu.RasterizerState = RasterizerState.CullNone;     // No backface culling
            gpu.DepthStencilState = DepthStencilState.None;     // No depth
            gpu.SamplerStates[0] = SamplerState.LinearWrap;     // Texture uv wrapping
            Matrix view = cam.view;
            view.Translation = Vector3.Zero;                    // Cancels translation

            foreach(ModelMesh mesh in model.Meshes)
            {
                foreach(BasicEffect effect in mesh.Effects)
                {
                    if (mesh.Name == "cloud_layer")
                    {
                        gpu.BlendState = BlendState.Additive;
                        effect.World = Matrix.CreateFromYawPitchRoll(rotate, 0, 0);
                        rotate += rotatespeed;
                    }
                    else
                    {
                        gpu.BlendState = BlendState.Opaque;
                    }
                    effect.LightingEnabled = false;
                    effect.View = view;                                 // Viewing angle of skydome
                    effect.Projection = cam.proj;                       // Perspective projection
                    effect.TextureEnabled = true;                       // Make it visible
                    if (texture != null) effect.Texture = texture;      // Lets us switch sky textures
                }
                mesh.Draw();
            }
        }
    }
}
