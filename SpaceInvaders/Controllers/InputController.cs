using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SpaceInvaders.Scenes;

namespace SpaceInvaders.Controllers;

public class InputController: Controller
{
    public override ControllerState GetState(GameTime gameTime)
    {
        var delta = Point.Zero;
        var shoot = false;
        if (Keyboard.IsPressed(Keys.Left))
        {
            delta = new Point(-1, 0);
        }
        if (Keyboard.IsPressed(Keys.Right))
        {
            delta = new Point(1, 0);
        }
        
        if (Keyboard.HasBeenPressed(Keys.Space))
        {
            shoot = true;
        }

        delta.X *= (int)(.2 * gameTime.ElapsedGameTime.Milliseconds);
        delta.Y *= (int)(.2 * gameTime.ElapsedGameTime.Milliseconds);
        
        return new ControllerState(delta, shoot);
    }

    public InputController(Session session) : base(session)
    {
    }
}