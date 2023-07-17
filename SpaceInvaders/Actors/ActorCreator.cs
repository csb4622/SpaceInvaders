using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceInvaders.Controllers;
using SpaceInvaders.Scenes;

namespace SpaceInvaders.Actors;

public static class ActorCreator
{
    public static Actor CreatePlayer(Session session, Texture2D atlas) => new Player(session, new InputController(session), atlas, new Point(196, 0), new Point(52, 32));
    public static Actor CreateSquid(Session session, Texture2D atlas) => new Enemy(session, new AlienController(session), atlas, new Point(0, 0), new Point(32, 32), 30);
    public static Actor CreateBug(Session session, Texture2D atlas) => new Enemy(session, new AlienController(session), atlas, new Point(32, 0), new Point(43, 32), 20);
    public static Actor CreateLander(Session session, Texture2D atlas) => new Enemy(session, new AlienController(session), atlas, new Point(76, 0), new Point(131-76, 32), 10);
    public static Actor CreateUfo(Session session, Texture2D atlas) => new Ufo(session, new UfoController(session), atlas, new Point(132, 0), new Point(195-132, 32));

    public static Actor CreateLaser(Session session, Texture2D atlas, Point velocity) => new Bullet(session, atlas, new[]{new Point(292,32)}, new Point(12,32), velocity);
    public static Actor CreateElectricity(Session session, Texture2D atlas, Point velocity) => new Bullet(session, atlas, new[]{new Point(196,32), new Point(208,32), new Point(220,32), new Point(232,32)}, new Point(12,32), velocity);
    public static Actor CreateWaveLaser(Session session, Texture2D atlas, Point velocity) => new Bullet(session, atlas, new[]{new Point(244,32), new Point(256,32), new Point(268,32), new Point(280,32)}, new Point(12,32), velocity);
    public static Actor CreateOciLaser(Session session, Texture2D atlas, Point velocity) => new Bullet(session, atlas, new[]{new Point(304,32), new Point(316,32), new Point(328,32)}, new Point(12,32), velocity);
    
    
    public static Actor CreateExplosion(Session session, Texture2D atlas, Point position) => new Explosion(session, atlas, new Point(344,32), new Point(363-344,32), position);
}