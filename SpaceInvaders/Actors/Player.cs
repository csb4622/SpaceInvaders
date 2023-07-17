using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceInvaders.Controllers;
using SpaceInvaders.Scenes;

namespace SpaceInvaders.Actors;

public class Player: Actor
{
    private readonly float _shootDelay;
    private bool _canShoot;
    private float _currentShootTime;

    private readonly int _deathAnimationSpeed;
    private int _currentDeathAnimationTimer;
    private readonly int _deathTime;
    private int _currentDeathTimer;
    private int _currentDeathFrame;
    
    public Player(Session session, Controller controller, Texture2D atlas, Point textureOffset, Point size) : base(session, controller, atlas, textureOffset, size)
    {
        _canShoot = false;
        _shootDelay = 500;
        _currentShootTime = 0;
        _deathAnimationSpeed= 200;
        _currentDeathAnimationTimer = 0;
        _deathTime = 1000;
        _currentDeathTimer = 0;
        _currentDeathFrame = 0;

    }

    public override void Destroy()
    {
        SetDead();
        Session.StopWhiteNoiseWave();
    }
    protected override void OnUpdate(GameTime gameTime)
    {
        if (IsDying)
        {
            Session.StartWhiteNoiseWave(100, 0.05);
            _currentDeathTimer += gameTime.ElapsedGameTime.Milliseconds;
            _currentDeathAnimationTimer += gameTime.ElapsedGameTime.Milliseconds; 
            if (_currentDeathTimer > _deathTime)
            {
                _currentDeathTimer = 0;
                _currentDeathAnimationTimer = 0;
                _currentDeathFrame = 0;
                SetDead();
                Session.StopWhiteNoiseWave();
                Session.LoseLife();
            }
            if (_currentDeathAnimationTimer > _deathAnimationSpeed)
            {
                _currentDeathAnimationTimer = 0;
                _currentDeathFrame = (++_currentDeathFrame) % 2;
                TextureOffset = new Point((252)+(_currentDeathFrame*60),0);
            }
            return;
        }
        
        var state = Controller.GetState(gameTime);

        if ((Position.X - (Size.X / 2) >= -5 || state?.DeltaPosition.X >= 0) &&
            (Position.X + (Size.X / 2) <= Session.Dimensions.X + 5 || state?.DeltaPosition.X <= 0))
        {
            SetPosition(
                (int)(Position.X + .1f * gameTime.ElapsedGameTime.Milliseconds * state?.DeltaPosition.X ?? 0),
                (int)(Position.Y + .1f * gameTime.ElapsedGameTime.Milliseconds * state?.DeltaPosition.Y ?? 0));
        }

        if (!_canShoot)
        {
            _currentShootTime += gameTime.ElapsedGameTime.Milliseconds;
            if (_currentShootTime > _shootDelay)
            {
                _currentShootTime = 0;
                _canShoot = true;
            }
        }
        if (!Session.ActivePlayerBullet() && _canShoot && (state?.Shoot ?? false))
        {
            var bullet = ActorCreator.CreateLaser(Session, Atlas, new Point(0, -1));
            bullet.SetPosition(Position);
            Session.AddBullet(bullet);
            _canShoot = false;
        }
    }

    protected override void OnDraw(GameTime gameTime, SpriteBatch spriteBatch)
    {
    }
}