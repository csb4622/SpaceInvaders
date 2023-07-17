using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceInvaders;

public class Barricade
{
    private readonly Point _size;

    private readonly bool[] _block;

    private readonly Point _position;

    public Rectangle Bounds => new(_position, new Point(_size.X*4, _size.Y*4));


    private bool[] BuildBarricade()
    {
        return new[]
        {
            false,false,false,false,false,true,true,true,true,true,true,true,true,true,true,true,true,false,false,false,false,false,
            false,false,false,false,true,true,true,true,true,true,true,true,true,true,true,true,true,true,false,false,false,false,
            false,false,false,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,false,false,false,
            false,false,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,false,false,
            false,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,false,
            true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,
            true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,
            true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,
            true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,
            true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,
            true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,
            true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,
            true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,
            true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,
            true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,
            true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,
            true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,true,
            true,true,true,true,true,true,true,true,false,false,false,false,false,false,true,true,true,true,true,true,true,true,
            true,true,true,true,true,true,true,false,false,false,false,false,false,false,false,true,true,true,true,true,true,true,
            true,true,true,true,true,true,false,false,false,false,false,false,false,false,false,false,true,true,true,true,true,true,
            true,true,true,true,true,false,false,false,false,false,false,false,false,false,false,false,false,true,true,true,true,true,
            true,true,true,true,false,false,false,false,false,false,false,false,false,false,false,false,false,false,true,true,true,true,
        };
    }


    public bool CheckHit(Rectangle bounds)
    {
        for (var i = 0; i < _block.Length; ++i)
        {
            var y = i / _size.X;
            var x = i - (y * _size.X);
            if (new Rectangle(_position.X+(x*4), _position.Y+(y*4), 4, 4).Intersects(bounds))
            {
                if (_block[i])
                {
                    var explodeWidth = 5;
                    var explodeHeight = 9;
                    var startX = Math.Max(0, x - (explodeWidth / 2));
                    var startY = Math.Max(0, y - (explodeHeight / 2));
                    var endX = Math.Min(_size.X - 1, x + (explodeWidth / 2));
                    var endY = Math.Min(_size.Y - 1, y + (explodeHeight / 2));

                    for (var explodeY = startY; explodeY <= endY; ++explodeY)
                    {
                        for (var explodeX = startX; explodeX <= endX; ++explodeX)
                        {
                            if (explodeX == startX || explodeX == endX)
                            {
                                if (Random.Shared.NextDouble() <= 0.5d)
                                {
                                    continue;
                                }
                            }
                            _block[(explodeY * _size.X) + explodeX] = false;
                        }
                    }
                    return true;
                }
            }
        }
        return false;
    }
    
    public Barricade(int x, int y)
    {
        _position = new Point(x, y);
        _size = new Point(22, 22);
        _block = BuildBarricade();
    }


    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        for(var i = 0; i < _block.Length; ++i)
        {
            if (_block[i])
            {
                var y = i / _size.X;
                var x = i - (y * _size.X);
                spriteBatch.Draw(
                    TextureHelper.Current.CreateTexture(),
                    new Rectangle(_position.X+(x*4), _position.Y+(y*4), 4, 4),
                    new Rectangle(0,0,1,1),
                    Color.Green,
                    0f,
                    Vector2.Zero,
                    SpriteEffects.None,
                    0f);
            }
        }
    }
}