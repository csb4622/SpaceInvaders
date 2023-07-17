using Microsoft.Xna.Framework;
using SpaceInvaders.Scenes;

namespace SpaceInvaders.Controllers;

public class BulletController : Controller
{
    private readonly Point _velocity;
    
    public BulletController(Session session, Point velocity) : base(session)
    {
        _velocity = velocity;
    }

    public override ControllerState GetState(GameTime gameTime)
    {
        return new ControllerState(new Point((int)(_velocity.X*.4*gameTime.ElapsedGameTime.Milliseconds), (int)(_velocity.Y*.4*gameTime.ElapsedGameTime.Milliseconds)), false);
    }
}