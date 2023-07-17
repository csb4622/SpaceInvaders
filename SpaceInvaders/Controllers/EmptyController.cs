using Microsoft.Xna.Framework;
using SpaceInvaders.Scenes;

namespace SpaceInvaders.Controllers;

public class EmptyController: Controller
{
    public EmptyController(Session session) : base(session)
    {
    }
    public override ControllerState? GetState(GameTime gameTime)
    {
        return null;
    }
}