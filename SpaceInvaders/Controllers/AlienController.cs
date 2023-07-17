using System;
using Microsoft.Xna.Framework;
using SpaceInvaders.Scenes;

namespace SpaceInvaders.Controllers;

public class AlienController : Controller
{

    public AlienController(Session session) : base(session)
    {
    }

   
    
    public override ControllerState GetState(GameTime gameTime)
    {
        var delta = Session.GetWaveMove();
        var shoot = false;


        if (!Session.ActiveEnemyBullet())
        {
            var shouldShoot = Random.Shared.Next(0, 10000);
            if (shouldShoot > 9990)
            {
                shoot = true;
            }
        }
        
        
        return new ControllerState(delta, shoot);
    }
}