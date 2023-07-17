using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceInvaders.Controllers;
using SpaceInvaders.Scenes;

namespace SpaceInvaders.Actors;

public class Bullet: Actor
{
    private readonly Point _velocity;
    private bool _hitSomething;

    private Point[] _textureOffsets;
    
    private readonly int _animationSpeed;
    private int _currentAnimationTime;
    private int _frameIndex;
    private bool _playingSound;
    
    public Bullet(Session session, Texture2D atlas, Point[] textureOffsets, Point size, Point velocity) : base(session, new BulletController(session, velocity), atlas, textureOffsets[0], size)
    {
        _playingSound = false;
        _textureOffsets = textureOffsets;
        
        _hitSomething = false;
        _velocity = velocity;
        
        _animationSpeed = 200;
        _currentAnimationTime = 0;
        _frameIndex = 0;
    }

    public Point GetVelocity()
    {
        return _velocity;
    }

    protected void UpdateAnimation(GameTime gameTime)
    {
        _currentAnimationTime += gameTime.ElapsedGameTime.Milliseconds;
        if (_currentAnimationTime > _animationSpeed)
        {
            _currentAnimationTime = 0;
            _frameIndex++;
            if (_frameIndex >= _textureOffsets.Length)
            {
                _frameIndex = 0;
            }

            TextureOffset = _textureOffsets[_frameIndex];
        }
    }
    
    protected override void OnUpdate(GameTime gameTime)
    {
        if ( _velocity.Y < 0 && !_playingSound)
        {
            _playingSound = true;
            Session.StartSquareWave(2000, .01);
        }
        var state = Controller.GetState(gameTime);
        SetPosition(
            (int)(Position.X + .2f * gameTime.ElapsedGameTime.Milliseconds * state?.DeltaPosition.X ?? 0),
            (int)(Position.Y + .2f * gameTime.ElapsedGameTime.Milliseconds * state?.DeltaPosition.Y ?? 0));
        
        UpdateAnimation(gameTime);
        
        if ((Position.Y < 30 && _velocity.Y < 0) || (Position.Y > (Session.Dimensions.Y-50) && _velocity.Y > 0))
        {
            Kill(gameTime);
        }
        else
        {
            var hitBarricade = Session.CheckBarricadeCollision(gameTime, Bounds);
            if (hitBarricade != null)
            {
                Kill(gameTime);
            }
            
            
            var hitBullet = Session.CheckBulletCollision(gameTime, this);
            if (hitBullet != null)
            {
                Session.SpawnPinkWave(1000, 0.1, 100);
                _hitSomething = true;
                hitBullet.Kill(gameTime);
                Kill(gameTime);
            }
            
            
            if (_velocity.Y < 0)
            {
                var hitActor = Session.CheckEnemyCollision(gameTime, Bounds);
                if (hitActor != null)
                {
                    _hitSomething = true;
                    hitActor.Kill(gameTime);
                    Kill(gameTime);
                }
            }
            else
            {
                
                if (Session.Player().Bounds.Intersects(Bounds))
                {
                    _hitSomething = true;
                    Session.Player().Kill(gameTime);
                    Kill(gameTime);
                }                
            }
        }
    }

    public override void Destroy()
    {
        SetDead();
        if (_playingSound)
        {
            Session.StopSquareWave();
        }
    }

    protected override void OnKill(GameTime gameTime)
    {
        SetDead();
        if (_playingSound)
        {
            Session.StopSquareWave();
        }
        if (!_hitSomething)
        {
            Session.AddExplosion(ActorCreator.CreateExplosion(Session, Atlas, Position));
        }
    }


    protected override void OnDraw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      //  spriteBatch.Draw(DebugTextureCreator.Current.CreateTexture(), Bounds, Color.Yellow);
    }
}