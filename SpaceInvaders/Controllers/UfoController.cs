using System;
using Microsoft.Xna.Framework;
using SpaceInvaders.Scenes;

namespace SpaceInvaders.Controllers;

public class UfoController : Controller
{
    public UfoController(Session session) : base(session)
    {
    }

    public override ControllerState GetState(GameTime gameTime)
    {
        if (Actor.Position.X > Session.Dimensions.X + 50)
        {
            Actor.Destroy();
        }
        var delta = new Point((int)(gameTime.ElapsedGameTime.Milliseconds * .1), 0);
        return new ControllerState(delta, false);
    }
}