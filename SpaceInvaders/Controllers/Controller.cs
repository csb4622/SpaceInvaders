
using Microsoft.Xna.Framework;
using SpaceInvaders.Actors;
using SpaceInvaders.Scenes;

namespace SpaceInvaders.Controllers;

public abstract class Controller
{
    private Actor? _actor;
    private readonly Session _session;

    protected Actor Actor => _actor!;
    protected Session Session => _session;
    
    public abstract ControllerState? GetState(GameTime gameTime);

    protected Controller(Session session)
    {
        _session = session;
    }
    
    public void SetActor(Actor actor)
    {
        _actor = actor;
    }
}