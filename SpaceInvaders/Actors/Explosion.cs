using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceInvaders.Controllers;
using SpaceInvaders.Scenes;

namespace SpaceInvaders.Actors;

public class Explosion: Actor
{
    private readonly int _lifeTime;
    private int _currentLife;
    
    
    public Explosion(Session session, Texture2D atlas, Point textureOffset, Point size, Point position) : base(session, new EmptyController(session), atlas, textureOffset, size)
    {
        SetPosition(position);
        _lifeTime = 200;
        _currentLife = 0;
    }


    public override void Destroy()
    {
        SetDead();
    }

    protected override void OnKill(GameTime gameTime)
    {
        SetDead();
        base.OnKill(gameTime);
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        _currentLife += gameTime.ElapsedGameTime.Milliseconds;
        if (_currentLife > _lifeTime)
        {
            Kill(gameTime);
        }
    }
}