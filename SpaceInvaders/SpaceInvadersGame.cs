using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpaceInvaders.Scenes;

namespace SpaceInvaders;

public class SpaceInvadersGame : Game
{
    private GraphicsDeviceManager? _graphics;
    private SpriteBatch? _spriteBatch;
    
    private Texture2D? _atlas;
    private SpriteFont? _font;

    private Scene? _scene;
    
    private GameState _state;

    private Point _dimensions;


    private int _finalScore;
    private int _highScore;

    private Vector2 _gameOverMessageLocation;
    private readonly string _gameOverMessage;    
    

    public SpaceInvadersGame()
    {
        _finalScore = 0;
        _highScore = 0;
        _gameOverMessage = "Final Score: {0},  Press Enter to try again";
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = false;

        _dimensions = new Point(896, 900);

        _state = GameState.Starting;
    }

    public void SetState(GameState state)
    {
        switch (state)
        {
            case GameState.Menu:
                _scene?.Shutdown();
                _scene = new Menu(this, _atlas!, _font!, _dimensions);
                _scene.Reset();
                _scene.SetupHUD();
                break;
            case GameState.Playing:
                _scene?.Shutdown();
                _scene = new Session(this, _atlas!, _font!, _dimensions, _highScore);
                _scene.Reset();
                _scene.SetupHUD();
                break;
        }

        _state = state;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    public void SetFinalScore(int score)
    {
        _finalScore = score;
        if (_finalScore > _highScore)
        {
            _highScore = _finalScore;
        }
    }
    
    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
        _atlas = Content.Load<Texture2D>("Sheet");
        _font = Content.Load<SpriteFont>("Font");
        TextureHelper.Current.Initialize(_graphics!.GraphicsDevice);


        
        var gameOverMessageSize = _font.MeasureString(_gameOverMessage);
        
        _gameOverMessageLocation =
            new Vector2(_dimensions.X/2 - (gameOverMessageSize.X / 2), _dimensions.Y/2 - (gameOverMessageSize.Y / 2));
        
        _graphics.PreferredBackBufferWidth = _dimensions.X;
        _graphics.PreferredBackBufferHeight = _dimensions.Y;
        _graphics.ApplyChanges();
        
    }

    protected override void Update(GameTime gameTime)
    {
        Keyboard.GetState();
        if (Keyboard.HasBeenPressed(Keys.Escape))
        {
            Exit();
        }

        // TODO: Add your update logic here
        switch (_state)
        {
            case GameState.Menu:
            case GameState.Playing:
  
                _scene?.Update(gameTime);
                break;
            case GameState.GameOver:
                ProcessGameOver(gameTime);
                break;
            case GameState.Starting:
                SetState(GameState.Menu);
                break;
        }
        base.Update(gameTime);
    }
    private void ProcessPause(GameTime gameTime)
    {
        if (Keyboard.HasBeenPressed(Keys.Enter))
        {
            SetState(GameState.Playing);
        }        
    }
    private void ProcessGameOver(GameTime gameTime)
    {
        if (Keyboard.HasBeenPressed(Keys.Enter))
        {
            SetState(GameState.Menu);
        }
    }    

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        // TODO: Add your drawing code here
        if (_spriteBatch != null)
        {
            _spriteBatch.Begin(SpriteSortMode.FrontToBack);
            switch (_state)
            {
                case GameState.Menu:
                case GameState.Playing:
                    _scene?.Draw(gameTime, _spriteBatch);
                    break;
                case GameState.GameOver:
                    _spriteBatch.DrawString(_font, string.Format(_gameOverMessage, _finalScore),
                        _gameOverMessageLocation, Color.White);
                    break;
            }

            _spriteBatch.End();
        }
        base.Draw(gameTime);
    }
}