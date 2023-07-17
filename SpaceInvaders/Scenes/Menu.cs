using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceInvaders.Scenes;

public class Menu: Scene
{
    
    private Vector2 _insertCoinMessageLocation;
    private Vector2 _oneOrTwoMessageLocation;
    private readonly string _insertCoinMessage;
    private readonly string _oneOrTwoMessage;
    
    
    public Menu(SpaceInvadersGame game, Texture2D atlas, SpriteFont font, Point dimensions) : base(game, atlas, font, dimensions)
    {
        _insertCoinMessage = "INSERT COIN";
        _oneOrTwoMessage = "<1  OR  2  PLAYERS>";
    }

    public override void Reset()
    {
        
    }

    public override void SetupHUD()
    {
        var insertSize = Font.MeasureString(_insertCoinMessage);
        var oneOrTwoSize = Font.MeasureString(_oneOrTwoMessage);
        
        _insertCoinMessageLocation = new Vector2(Dimensions.X/2-(insertSize.X/2), Dimensions.Y/2-(insertSize.Y/2));
        _oneOrTwoMessageLocation = new Vector2(Dimensions.X/2-(oneOrTwoSize.X/2), _insertCoinMessageLocation.Y+(insertSize.Y*2));
    }

    public override void Update(GameTime gameTime)
    {
        if (Keyboard.HasBeenPressed(Keys.Enter))
        {
            Game.SetState(GameState.Playing);
        }
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        DrawText(_insertCoinMessage, _insertCoinMessageLocation, spriteBatch);
        DrawText(_oneOrTwoMessage, _oneOrTwoMessageLocation, spriteBatch);
    }
    
    
    private void DrawText(string text, Vector2 position, SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(Font, text, position, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 1 );
    }
    
    

    public override void Shutdown()
    {

    }
}