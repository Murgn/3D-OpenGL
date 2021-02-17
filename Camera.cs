using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace My3DGame
{
    class Camera
    {
        public const float CAM_HEIGHT_OFFSET = 80f; // default up-distance from the player's root position (depends on character size = 80 up in y direction to look at head)

        public const float FAR_PLANE = 1000;        // farthest camera can see (dont render things further than this plane)
        public Vector3 pos, target;                 // camera position, target to look at
        public Matrix view, proj, view_proj;        // view/projection transforms used to transform world vertices to screen coordinates relative to camera
        public Vector3 up;                          // up direction for camera and world geometry (may depend on imported geometry's up direction
        float current_angle;                        // player-relative angle offset of camera
        float angle_velocity;                       // speed of camera rotation
        float radius = 100.0f;                      // distance of camera from player (to look at)
        Vector3 unit_direction;                     // direction of camera (normalized to distance of 1 unit)

        Input inp;                                  // allow access to input class so can control camera from this class if want to

        public Camera(GraphicsDevice gpu, Vector3 UpDirection, Input input) // HARDCODE THE UP POSITION LATER, GET RID OF THE VECTOR3
        {
            up = UpDirection;
            pos = new Vector3(0, 25, -100);                                // CHANGE CAMERA POS LATER
            target = Vector3.Zero;
            view = Matrix.CreateLookAt(pos, target, up);
            proj = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, gpu.Viewport.AspectRatio, 0.1f, FAR_PLANE);
            view_proj = view * proj;
            inp = input;
            unit_direction = view.Forward; unit_direction.Normalize();
        }

        // Simple camera movement
        public void MoveCamera(Vector3 move)
        {
            pos += move;
            view = Matrix.CreateLookAt(pos, target, up);
            view_proj = view * proj;
        }

        // new_target should usually be player position
        public void UpdateTarget(Vector3 new_target)
        {
            target = new_target; target.Y -= 10;
            view = Matrix.CreateLookAt(pos, target, up);
            view_proj = view * proj;
        }

        public void Update_Player_Cam()
        {
            #region TEMPORARY_CAMERA_CONTROL
            if (inp.KeyDown(Keys.Space)) { pos.Y += 5; }
            if (inp.KeyDown(Keys.LeftControl)) { pos.Y -= 5; }
            if (inp.KeyDown(Keys.A)) { pos.X += 5; }
            if (inp.KeyDown(Keys.D)) { pos.X -= 5; }
            if (inp.KeyDown(Keys.W)) { pos.Z += 5; }
            if (inp.KeyDown(Keys.S)) { pos.Z -= 5; }
            #endregion

            UpdateTarget(Vector3.Zero); // Set this to UpdateTarget(player.pos) later
        }
    }
}
