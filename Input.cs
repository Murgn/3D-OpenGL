using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;

namespace My3DGame
{
    class Input
    {
        public const ButtonState ButtonUp = ButtonState.Released;
        public const ButtonState ButtonDown = ButtonState.Pressed;

        // Keyboard Input
        public KeyboardState kb, okb; 
        public bool shift_down, control_down, alt_down, shift_press, control_press, alt_press;
        public bool old_shift_down, old_control_down, old_alt_down;

        // Mouse Input
        public MouseState ms, oms;
        public bool left_click, mid_click, right_click, left_down, mid_down, right_down;
        public int mosx, mosy;
        public Vector2 mosV;
        public Point mp;

        // Gamepad Input
        public GamePadState gp, ogp;
        public bool A_down, B_down, X_down, Y_down, RB_down, LB_down, start_down, back_down, leftStick_down, rightStick_down;
        public bool A_press, B_press, X_press, Y_press, RB_press, LB_press, start_press, back_press, leftStick_press, rightStick_press;

        float screenScaleX, screenScaleY; // Used to scale mouse coordinates to match game resolution

        // Constructor
        public Input(PresentationParameters pp, RenderTarget2D target)
        {
            screenScaleX = 1.0f / ((float)pp.BackBufferWidth / (float)target.Width);
            screenScaleY = 1.0f / ((float)pp.BackBufferHeight / (float)target.Height);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool KeyPress(Keys k) { if (kb.IsKeyDown(k) && okb.IsKeyUp(k)) return true; else return false; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool KeyDown(Keys k) { if (kb.IsKeyDown(k)) return true; else return false; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ButtonPress(Buttons button) { if (gp.IsButtonDown(button) && ogp.IsButtonUp(button)) return true; else return false; }

        // Update
        public void Update()
        {
            old_alt_down = alt_down; old_shift_down = shift_down; old_control_down = control_down;
            okb = kb; oms = ms; ogp = gp;
            kb = Keyboard.GetState(); ms = Mouse.GetState(); gp = GamePad.GetState(0);

            // Keyboard Input
            shift_down = shift_press = control_down = control_press = alt_down = alt_press = false;
            if (kb.IsKeyDown(Keys.LeftShift) || kb.IsKeyDown(Keys.RightShift)) shift_down = true;
            if (kb.IsKeyDown(Keys.LeftControl) || kb.IsKeyDown(Keys.RightControl)) shift_down = true;
            if (kb.IsKeyDown(Keys.LeftAlt) || kb.IsKeyDown(Keys.RightAlt)) shift_down = true;
            if ((shift_down) && (!old_shift_down)) shift_press = true;
            if ((control_down) && (!old_control_down)) control_press = true;
            if ((alt_down) && (!old_alt_down)) alt_press = true;

            // Mouse Input
            mosV = new Vector2((float)ms.Position.X * screenScaleX, (float)ms.Position.Y * screenScaleY);
            mosx = (int)mosV.X; mosy = (int)mosV.Y; mp = new Point(mosx, mosy);
            left_click = mid_click = right_click = left_down = rightStick_down = mid_down = false;
            if (ms.LeftButton == ButtonDown) left_down = true;
            if (ms.MiddleButton == ButtonDown) mid_down = true;
            if (ms.RightButton == ButtonDown) right_down = true;
            if ((left_down) && (oms.LeftButton == ButtonUp)) left_click = true;
            if ((mid_down) && (oms.MiddleButton == ButtonUp)) mid_click = true;
            if ((right_down) && (oms.RightButton == ButtonUp)) right_click = true;

            // Gamepad Input
            A_down = B_down = X_down = Y_down = RB_down = LB_down = start_down = back_down = start_down = back_down = leftStick_down = rightStick_down = false;
            A_press = B_press = X_press = Y_press = RB_press = LB_press = start_press = back_press = start_press = back_press = leftStick_press = rightStick_press = false;
            if (gp.Buttons.A == ButtonState.Pressed) { A_down = true; if (gp.Buttons.A == ButtonState.Released) A_press = true; }
            if (gp.Buttons.B == ButtonState.Pressed) { B_down = true; if (gp.Buttons.B == ButtonState.Released) B_press = true; }
            if (gp.Buttons.X == ButtonState.Pressed) { X_down = true; if (gp.Buttons.X == ButtonState.Released) X_press = true; }
            if (gp.Buttons.Y == ButtonState.Pressed) { Y_down = true; if (gp.Buttons.Y == ButtonState.Released) Y_press = true; }
            if (gp.Buttons.RightShoulder == ButtonState.Pressed) { RB_down = true; if (gp.Buttons.RightShoulder == ButtonState.Released) RB_press = true; }
            if (gp.Buttons.LeftShoulder == ButtonState.Pressed) { LB_down = true; if (gp.Buttons.LeftShoulder == ButtonState.Released) LB_press = true; }
            if (gp.Buttons.Back == ButtonState.Pressed) { back_down = true; if (gp.Buttons.Back == ButtonState.Released) back_press = true; }
            if (gp.Buttons.Start == ButtonState.Pressed) { start_down = true; if (gp.Buttons.Start == ButtonState.Released) start_press = true; }
            if (gp.Buttons.LeftStick == ButtonState.Pressed) { leftStick_down = true; if (gp.Buttons.LeftStick == ButtonState.Released) leftStick_press = true; }
            if (gp.Buttons.RightStick == ButtonState.Pressed) { rightStick_down = true; if (gp.Buttons.RightStick == ButtonState.Released) rightStick_press = true; }
        }
         
    }
}
