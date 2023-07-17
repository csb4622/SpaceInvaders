using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceInvaders.Controllers;
using SpaceInvaders.Scenes;

namespace SpaceInvaders.Actors;

public abstract class Actor
{
    private Point _drawPosition;
    private Point _centerPosition;
    private Point _size;
    private Session _session;
    private Controller _controller;
    private Point _textureOffset;
    private readonly Texture2D _atlas;

    private bool _dead;
    private bool _dying;

    protected Actor(Session session, Controller controller, Texture2D atlas, Point textureOffset, Point size)
    {
        _size = size;
        _dead = false;
        _dying = false;
        
        _session = session;
        _controller = controller;
        _atlas = atlas;
        _textureOffset = textureOffset;
        controller.SetActor(this);
    }

    public bool IsDead => _dead;

    public bool IsDying => _dying;
    protected void SetDead()
    {
        _dead = true;
    }
    protected Controller Controller => _controller;
    protected Session Session => _session;
    protected Point Size => _size;
    protected Point DrawPosition => _drawPosition;
    public Point Position => _centerPosition;
    public Rectangle Bounds => new(DrawPosition, Size);

    

    protected Point TextureOffset
    {
        get => _textureOffset;
        set => _textureOffset = value;
    }

    protected void SetSize(Point size)
    {
        _size = size;
    }

    protected void SetSize(int x, int y)
    {
        SetSize(new Point(x, y));
    }    
    
    protected Texture2D Atlas => _atlas;

    public void Kill(GameTime gameTime)
    {
        _dying = true;
        OnKill(gameTime);
    }

    public abstract void Destroy();
    
    public void Update(GameTime gameTime)
    {
        if (!IsDead)
        {
            OnUpdate(gameTime);
        }
    }

    public void SetPosition(Point newPosition)
    {
        _centerPosition = newPosition;
        _drawPosition = new Point((newPosition.X - _size.X / 2), (newPosition.Y - _size.Y / 2));
    }
    public void SetPosition(int x, int y)
    {
        SetPosition(new Point(x, y));
    }
    
    public void AddPosition(int x, int y)
    {
        var newPosition =  new Point(Position.X+x, Position.Y+y);
        SetPosition(newPosition);
    }
    public void AddPosition(Point delta)
    {
        AddPosition(delta.X, delta.Y);
    }

    protected virtual void OnKill(GameTime gameTime)
    {
    }

    protected virtual void OnUpdate(GameTime gameTime)
    {
    }

    protected virtual void OnDraw(GameTime gameTime, SpriteBatch spriteBatch)
    {
    }

    private Color GetColor()
    {
        if (_session.Dimensions.Y / 8 > _centerPosition.Y)
        {
            return Color.Red;
        }
        if (_session.Dimensions.Y - (_session.Dimensions.Y / 4) < _centerPosition.Y)
        {
            return Color.Green;
        }
        return Color.White;
    }
    
    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {

        OnDraw(gameTime, spriteBatch);
        spriteBatch.Draw(
            Atlas,
            new Vector2(_drawPosition.X, _drawPosition.Y),
            new Rectangle(_textureOffset, _size),
            GetColor(),
            0f,
            Vector2.Zero, 
            Vector2.One, 
            SpriteEffects.None,
            0.5f);
    }
}