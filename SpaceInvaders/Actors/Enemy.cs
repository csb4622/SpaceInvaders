using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceInvaders.Controllers;
using SpaceInvaders.Scenes;

namespace SpaceInvaders.Actors;

public class Enemy: Actor
{
    private readonly int _deathTimer;
    private int _currentDeathTimer;
    private int _currentFrameYOffset;
    private readonly int _score;

    public Enemy(Session session, Controller controller, Texture2D atlas, Point textureOffset, Point size, int score) : base(session, controller, atlas, textureOffset, size)
    {
        _currentFrameYOffset = 0;
        _deathTimer = 200;
        _currentDeathTimer = 0;
        _score = score;
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        if (IsDying)
        {
            Session.StartWhiteNoiseWave(1000, 0.01);
            _currentDeathTimer += gameTime.ElapsedGameTime.Milliseconds;
            if (_currentDeathTimer > _deathTimer)
            {
                SetDead();
                Session.StopWhiteNoiseWave();
            }
            return;
        }


        if (Position.Y >= Session.Player().Position.Y)
        {
            Session.GameOver();
        }

        var state = Controller.GetState(gameTime);

        if (state?.DeltaPosition.X != 0 || state.DeltaPosition.Y != 0)
        {
            _currentFrameYOffset = (_currentFrameYOffset+1)%2;
            TextureOffset = new Point(TextureOffset.X, _currentFrameYOffset*32);
        }
        
        SetPosition(
            (Position.X + state?.DeltaPosition.X ?? 0),
            (Position.Y + state?.DeltaPosition.Y ?? 0));
        
        
        if (state?.Shoot ?? false)
        {
            var bullet = GetRandomBullet();
            bullet.SetPosition(Position);
            Session.AddBullet(bullet);
        }        
    }

    private Actor GetRandomBullet()
    {
        var bulletType = Random.Shared.Next(0, 3);
        var velocity = new Point(0, 1);
        switch (bulletType)
        {
            case 0:
                return ActorCreator.CreateElectricity(Session, Atlas, velocity);
            case 1:
                return ActorCreator.CreateWaveLaser(Session, Atlas, velocity);
            case 2:
                return ActorCreator.CreateOciLaser(Session, Atlas, velocity);
        }
        return ActorCreator.CreateLaser(Session, Atlas, velocity);
    }
    
    
    protected override void OnDraw(GameTime gameTime, SpriteBatch spriteBatch)
    {
       // spriteBatch.Draw(DebugTextureCreator.Current.CreateTexture(), Bounds, Color.Red);
    }

    public override void Destroy()
    {
        SetDead();
        Session.StopWhiteNoiseWave();
    }

    protected override void OnKill(GameTime gameTime)
    {
        TextureOffset = new Point(373, 0);
        SetSize(423-373, 32);
        Session.IncreasePlayerScore(_score);
    }
}