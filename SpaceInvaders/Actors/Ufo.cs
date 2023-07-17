using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceInvaders.Controllers;
using SpaceInvaders.Scenes;

namespace SpaceInvaders.Actors;

public class Ufo: Actor
{
    private readonly int _deathTimer;
    private int _currentDeathTimer;
    private bool _playingSound;
    
    
    public Ufo(Session session, Controller controller, Texture2D atlas, Point textureOffset, Point size) : base(session, controller, atlas, textureOffset, size)
    {
        _playingSound = false;
        _deathTimer = 200;
        _currentDeathTimer = 0;
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        if (IsDying)
        {
            Session.StopDynamicWave();
            _playingSound = false;
            Session.StartWhiteNoiseWave(1000, 0.01);
            _currentDeathTimer += gameTime.ElapsedGameTime.Milliseconds;
            if (_currentDeathTimer > _deathTimer)
            {
                SetDead();
                Session.StopWhiteNoiseWave();
            }
            return;
        }

        if (!_playingSound)
        {
            Session.StartDynamicWave(0.2);
            _playingSound = true;
        }
        else
        {
            Session.ProcessDynamicWave(gameTime);
        }
        
        var state = Controller.GetState(gameTime);
        SetPosition(
            (Position.X + state?.DeltaPosition.X ?? 0),
            (Position.Y + state?.DeltaPosition.Y ?? 0));
    }
    protected override void OnDraw(GameTime gameTime, SpriteBatch spriteBatch)
    {
       // spriteBatch.Draw(DebugTextureCreator.Current.CreateTexture(), Bounds, Color.Red);
    }

    public override void Destroy()
    {
        SetDead();
        Session.StopDynamicWave();
        Session.StopWhiteNoiseWave();
    }

    protected override void OnKill(GameTime gameTime)
    {
        TextureOffset = new Point(TextureOffset.X, TextureOffset.Y+32);
        Session.IncreasePlayerScore(Random.Shared.Next(50,301));
    }
}