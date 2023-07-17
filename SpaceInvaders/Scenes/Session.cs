using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NAudio.Midi;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using SpaceInvaders.Actors;

namespace SpaceInvaders.Scenes;

public abstract class Scene
{
    private readonly Texture2D _atlas;
    private readonly SpriteFont _font;
    private readonly SpaceInvadersGame _game;
    private readonly Point _dimensions;

    protected Scene(SpaceInvadersGame game, Texture2D atlas, SpriteFont font, Point dimensions)
    {
        _dimensions = dimensions;
        _font = font;
        _atlas = atlas;
        _game = game;
    }

    protected Texture2D Atlas => _atlas;
    protected SpriteFont Font => _font;
    protected SpaceInvadersGame Game => _game;
    
    public Point Dimensions => _dimensions;

    public abstract void Reset();
    public abstract void SetupHUD();
    public abstract void Update(GameTime gameTime);
    public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

    public abstract void Shutdown();


}


public class Session : Scene
{
    
    private readonly MidiOut _outputDevice;
    private readonly WaveOutEvent _squareWave;
    private readonly WaveOutEvent _whiteNoiseWave;
    private readonly WaveOutEvent _pinkWave;
    private readonly WaveOutEvent _dynamicWave;
    private SignalGenerator? _dynamicGenerator;
    private double _currentDynamicFrequency;
    private bool _playingSquareWave;
    private bool _playingWhiteNoiseWave;
    private bool _playingPinkWave;
    private bool _playingDynamicWave;

    private int[] _notes;
    private int _currentNoteIndex;
    
    private Actor? _player;
    private IList<Actor> _waveEnemies;
    private IList<Actor> _extraEnemies;
    private IList<Actor> _bullets;
    private IList<Actor> _explosions;
    private IList<Barricade> _barricades;
    private readonly Point _startingPlayerPosition;
    
    private Point _waveMoveDirection;
    private int _waveMoveDelay;
    private int _currentWaveMoveTimer;
    private bool _waveMoveWait;
    private readonly int _waveXMove;
    private readonly int _waveYMove;
    private readonly int _ufoTimer;
    private int _currentUfoTime;
    private int _wave;
    
    private Vector2 _playerOneScoreLabelPosition;
    private Vector2 _playerOneScorePosition;
    private readonly string _playerOneScoreLabel;
    
    private Vector2 _highScoreLabelPosition;
    private Vector2 _highScorePosition;
    private readonly string _highScoreLabel;    
    
    private Vector2 _playerTwoScoreLabelPosition;
    private Vector2 _playerTwoScorePosition;
    private readonly string _playerTwoScoreLabel;

    private Vector2 _livesNumberPosition;
    private Vector2 _livesStartPosition;

    private int _playerOneScore;
    private readonly int _highScore;
    private int _lives;

    private bool _gameOver;
    private readonly int _gameOverDelay;
    private int _currentGameOverTimer;


    private bool _paused;
    private Vector2 _pauseMessageLocation;
    private readonly string _pauseMessage;

    private int ComputedWaveDelay => Math.Max(_waveMoveDelay * _waveEnemies.Count, 100); 
    
    public Session(SpaceInvadersGame game, Texture2D atlas, SpriteFont font, Point dimensions, int highScore): base(game, atlas, font, dimensions)
    {
        _notes = new[] { 38, 36, 34, 33 };
        _currentNoteIndex = 0;
        _outputDevice = new MidiOut(0); // Use 0 for the default MIDI output device
        _squareWave = new WaveOutEvent();
        _whiteNoiseWave = new WaveOutEvent();
        _pinkWave = new WaveOutEvent();
        _dynamicWave = new WaveOutEvent();
        _playingSquareWave = false;
        _playingWhiteNoiseWave = false;
        _playingPinkWave = false;
        _playingDynamicWave = false;
        _currentDynamicFrequency = 0;
        
        _playerOneScoreLabel = "SCORE(1)";
        _highScoreLabel = "HIGH SCORE";
        _playerTwoScoreLabel = "SCORE(2)";
        _pauseMessage = "Press Enter to resume";
        _paused = false;
        _highScore = highScore;
        _gameOver = false;
        _gameOverDelay = 500;
        _currentGameOverTimer = 0;
        _currentWaveMoveTimer = 0;
        _waveMoveWait = true;
        _waveMoveDelay = 23;
        _wave = 0;
        _waveXMove = 10;
        _waveYMove = 10;
        _waveMoveDirection = new Point(_waveXMove, 0);
        _ufoTimer = 30000;
        _currentUfoTime = 0;
        
        _startingPlayerPosition = new Point(32, dimensions.Y - 74);
        _waveEnemies = new List<Actor>();
        _extraEnemies = new List<Actor>();
        _bullets = new List<Actor>();
        _explosions = new List<Actor>();
        _barricades = new List<Barricade>();
        _playerOneScore = 0; 
    }

    
    #region CalledFromGame
    public override void Reset()
    {
        SoftReset();
        _wave = 0;
        _currentWaveMoveTimer = 0;
        _waveMoveWait = true;
        _waveEnemies.Clear();
        _barricades.Clear();
        SpawnEnemies();
        SpawnBarricades();
        _waveMoveDirection = new Point(_waveXMove, 0);
        _lives = 3;
        _playerOneScore = 0;
        _gameOver = false;
        _currentGameOverTimer = 0;
    }
    public override void SetupHUD()
    {
        var _playerOneScoreLabelSize = Font.MeasureString(_playerOneScoreLabel);
        var _highScoreLabelSize = Font.MeasureString(_highScoreLabel);
        var _playerTwoScoreLabelSize = Font.MeasureString(_playerTwoScoreLabel);
        
        
        var pauseMessageSize = Font.MeasureString(_pauseMessage);
        _pauseMessageLocation =
            new Vector2(Dimensions.X/2 - (pauseMessageSize.X / 2), Dimensions.Y/2 - (pauseMessageSize.Y / 2));
        
        
        
        _playerOneScoreLabelPosition = new Vector2(Dimensions.X/4-(_playerOneScoreLabelSize.X/2), 1);
        _highScoreLabelPosition = new Vector2(Dimensions.X/2-(_highScoreLabelSize.X/2), 1);
        _playerTwoScoreLabelPosition = new Vector2((Dimensions.X/4*3)-(_playerTwoScoreLabelSize.X/2), 1);
        
        _playerOneScorePosition = new Vector2(Dimensions.X/4-(_playerOneScoreLabelSize.X/2), 20);
        _highScorePosition = new Vector2(Dimensions.X/2-(_highScoreLabelSize.X/2), 20);
        _playerTwoScorePosition = new Vector2((Dimensions.X/4*3)-(_playerTwoScoreLabelSize.X/2), 20);

        _livesNumberPosition = new Vector2(5, (Dimensions.Y - 30));
        _livesStartPosition = new Vector2(40, Dimensions.Y - 35);
    }
    public override void Update(GameTime gameTime)
    {
        if (Keyboard.HasBeenPressed(Keys.Enter))
        {
            _paused = !_paused;
            return;
        }
        if (_paused)
        {
            return;
        }


        if (_gameOver)
        {
            _currentGameOverTimer += gameTime.ElapsedGameTime.Milliseconds;
            if (_currentGameOverTimer > _gameOverDelay)
            {
                _currentGameOverTimer = 0;
                Game.SetState(GameState.GameOver);
            }
            return;
        }

        _player?.Update(gameTime);
        if (_player?.IsDying ?? false)
        {
            return;
        }
        
        
        // Wave is completed, Spawning next wave
        if (!_waveEnemies.Any())
        {
            NextWave();
        }
        MoveEnemyWave(gameTime);
        
        
        CheckIfNeedUfo(gameTime);
        
        ProcessActors(_waveEnemies, gameTime);
        ProcessActors(_extraEnemies, gameTime);
        ProcessActors(_bullets, gameTime);
        ProcessActors(_explosions, gameTime);
    }
    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        if (_paused)
        {
            DrawText(_pauseMessage, _pauseMessageLocation, spriteBatch);
            return;
        }
        
        
        DrawText(_playerOneScoreLabel, _playerOneScoreLabelPosition, spriteBatch);
        DrawText(_playerOneScore.ToString(), _playerOneScorePosition, spriteBatch);
        DrawText(_highScoreLabel, _highScoreLabelPosition, spriteBatch);
        DrawText(_highScore.ToString(), _highScorePosition, spriteBatch);
        DrawText(_playerTwoScoreLabel, _playerTwoScoreLabelPosition, spriteBatch);
        DrawText("000", _playerTwoScorePosition, spriteBatch);
        
        spriteBatch.Draw(TextureHelper.Current.CreateTexture(), new Rectangle(0, Dimensions.Y-40, Dimensions.X, 1), Color.Green);
        
        DrawText(_lives.ToString(), _livesNumberPosition, spriteBatch);
        for (var i = 0; i < _lives-1; ++i)
        {
            spriteBatch.Draw(
                Atlas,
                new Vector2((_livesStartPosition.X + (i*70)), _livesStartPosition.Y),
                new Rectangle(new Point(196, 0), new Point(52,32)),
                Color.Green,
                0f,
                Vector2.Zero, 
                Vector2.One, 
                SpriteEffects.None,
                1);
        }
        
        
        for (var i = 0; i < _barricades.Count; ++i)
        {
            _barricades[i].Draw(gameTime, spriteBatch);
        }        
        
        
        _player?.Draw(gameTime, spriteBatch);

        for (var i = 0; i < _waveEnemies.Count; ++i)
        {
            _waveEnemies[i].Draw(gameTime, spriteBatch);
        }

        for (var i = 0; i < _extraEnemies.Count; ++i)
        {
            _extraEnemies[i].Draw(gameTime, spriteBatch);
        }
        
        for (var i = 0; i < _bullets.Count; ++i)
        {
            _bullets[i].Draw(gameTime, spriteBatch);
        }
        for (var i = 0; i < _explosions.Count; ++i)
        {
            _explosions[i].Draw(gameTime, spriteBatch);
        }          
    }
    public override void Shutdown()
    {
        _outputDevice.Dispose();
        _squareWave.Dispose();
        _whiteNoiseWave.Dispose();
        _pinkWave.Dispose();
        _dynamicWave.Dispose();
    }
    #endregion

    #region CalledFromActor
    public void GameOver()
    {
        StopSquareWave();
        StopWhiteNoiseWave();
        StopDynamicWave();
        _lives = 0;
        _gameOver = true;
        _currentGameOverTimer = 0;
        Game.SetFinalScore(_playerOneScore);
    }
    public void IncreasePlayerScore(int score)
    {
        _playerOneScore += score;
        if (_playerOneScore % 1500 == 0)
        {
            ++_lives;
        }
    }
    public bool ActivePlayerBullet()
    {
        for (var i = 0; i < _bullets.Count; ++i)
        {
            if (((Bullet)_bullets[i]).GetVelocity().Y < 0)
            {
                return true;
            }
        }
        return false;
    }
    public void AddBullet(Actor actor)
    {
        _bullets.Add(actor);
    }
    public void AddExplosion(Actor actor)
    {
        _explosions.Add(actor);
    }
    public void LoseLife()
    {
        _lives--;
        if (_lives < 1)
        {
            GameOver();
        }
        else
        {
            SoftReset();    
        }
        
    }
    public Actor? CheckEnemyCollision(GameTime gameTime, Rectangle rectangle)
    {
        for(var i = 0; i < _waveEnemies.Count; ++i)
        {
            if (_waveEnemies[i].Bounds.Intersects(rectangle))
            {
                return _waveEnemies[i];
            }
        }
        for(var i = 0; i < _extraEnemies.Count; ++i)
        {
            if (_extraEnemies[i].Bounds.Intersects(rectangle))
            {
                return _extraEnemies[i];
            }
        }
        return null;
    }
    public Actor? CheckBulletCollision(GameTime gameTime, Actor bullet)
    {
        for(var i = 0; i < _bullets.Count; ++i)
        {
            var checkingBullet = _bullets[i];
            if (checkingBullet == bullet)
            {
                return null;
            }
            if (checkingBullet.Bounds.Intersects(bullet.Bounds))
            {
                return _bullets[i];
            }
        }

        return null;
    }
    public Barricade? CheckBarricadeCollision(GameTime gameTime, Rectangle bounds)
    {
        for(var i = 0; i < _barricades.Count; ++i)
        {
            if (_barricades[i].Bounds.Intersects(bounds))
            {
                if (_barricades[i].CheckHit(bounds))
                {
                    return _barricades[i];
                }
            }
        }
        return null;
    }    
    public Actor Player()
    {
        return _player!;
    }
    public void SpawnBackgroundSound(int note, int duration)
    {
        var thread = new Thread(() => PlayNote(note, TimeSpan.FromMilliseconds(duration)));
        thread.Start();
    }
    public void StartNoteOnChannel(int note, int channel)
    {
        _outputDevice.Send(MidiMessage.StartNote(note,100, channel).RawData);
    }
    public void StopNoteOnChannel(int note, int channel)
    {
        _outputDevice.Send(MidiMessage.StopNote(note, 100, channel).RawData);
    }
    public void StartSquareWave(int frequency, double gain)
    {
        if (!_playingSquareWave)
        {
            var generator = new SignalGenerator
            {
                Frequency = frequency,
                Gain = gain,
                Type = SignalGeneratorType.Square
            };
            _squareWave.Init(generator);
            _squareWave.Play();
            _playingSquareWave = true;
        }
    }
    public void StopSquareWave()
    {
        if (_playingSquareWave)
        {
            _squareWave.Stop();
            _playingSquareWave = false;
        }
    }
    public void StartDynamicWave(double gain)
    {
        if (!_playingDynamicWave)
        {
            _dynamicGenerator = new SignalGenerator
            {
                Frequency = 200,
                Gain = gain,
                Type = SignalGeneratorType.Sin
            };
            _dynamicWave.Init(_dynamicGenerator);
            _dynamicWave.Play();
            _playingDynamicWave = true;
        }
    }
    public void ProcessDynamicWave(GameTime gameTime)
    {
        _currentDynamicFrequency += gameTime.ElapsedGameTime.Milliseconds;
        const double frequency1 = 200;
        const double frequency2 = 800;
        const double duration = 100;
        const double phaseIncrement = Math.PI * 2 / duration;
        
        
        if (_playingDynamicWave)
        {
            var phase = phaseIncrement * _currentDynamicFrequency;
            var frequency = (frequency2 - frequency1) / 2 * Math.Sin(phase) + (frequency2 + frequency1) / 2;
            _dynamicGenerator!.Frequency = frequency;
        }
    }
    public void StopDynamicWave()
    {
        if (_playingDynamicWave)
        {
            _dynamicWave.Stop();
            _playingDynamicWave = false;
            _dynamicGenerator = null;
            _currentDynamicFrequency = 0;
        }
    }
    public void StartWhiteNoiseWave(int frequency, double gain)
    {
        if (!_playingWhiteNoiseWave)
        {
            var generator = new SignalGenerator
            {
                Frequency = frequency,
                Gain = gain,
                Type = SignalGeneratorType.White
            };
            _whiteNoiseWave.Init(generator);
            _whiteNoiseWave.Play();
            _playingWhiteNoiseWave = true;
        }
    }
    public void StopWhiteNoiseWave()
    {
        if (_playingWhiteNoiseWave)
        {
            _whiteNoiseWave.Stop();
            _playingWhiteNoiseWave = false;
        }
    }
    public void SpawnPinkWave(int frequency, double gain, int duration)
    {
        var thread = new Thread(() =>
        {
            if (!_playingPinkWave)
            {
                var generator = new SignalGenerator
                {
                    Frequency = frequency,
                    Gain = gain,
                    Type = SignalGeneratorType.Pink
                };
                _pinkWave.Init(generator);
                _pinkWave.Play();
                _playingPinkWave = true;
                
                Thread.Sleep(TimeSpan.FromMilliseconds(duration));
                _pinkWave.Stop();
                _playingPinkWave = false;
            }
        });
        thread.Start();
    }
    #endregion
    
    #region CalledFromController
    public bool ActiveEnemyBullet()
    {
        for (var i = 0; i < _bullets.Count; ++i)
        {
            if (((Bullet)_bullets[i]).GetVelocity().Y > 0)
            {
                return true;
            }
        }
        return false;
    }
    public Point GetWaveMove()
    {
        return _waveMoveWait ? Point.Zero : _waveMoveDirection;
    }   
    #endregion
    
    
    
    
    
    
    #region CalledFromUpdate
    private void NextWave()
    {
        ++_wave;
        SoftReset();
        _currentWaveMoveTimer = 0;
        _waveMoveWait = true;
        _waveEnemies.Clear();
        _extraEnemies.Clear();
        _barricades.Clear();
        SpawnEnemies();
        SpawnBarricades();
        _waveMoveDirection = new Point(_waveXMove, 0);
    }
    private void CheckIfNeedUfo(GameTime gameTime)
    {
        if (!_extraEnemies.Any() && _waveEnemies.Count > 8)
        {
            _currentUfoTime += gameTime.ElapsedGameTime.Milliseconds;
            if (_currentUfoTime > _ufoTimer)
            {
                _currentUfoTime = 0;
                SpawnUfo();
            }
        }
    }
    private void ProcessActors(IList<Actor> actors, GameTime gameTime)
    {
        if (actors.Any())
        {
            var i = 0;
            while (actors.Count > i)
            {
                var actor = actors[i];
                if (actor.IsDead)
                {
                    actors.Remove(actor);
                }
                else
                {
                    actors[i++].Update(gameTime);
                }
            }
        }
    }
    private void MoveEnemyWave(GameTime gameTime)
    {
        _currentWaveMoveTimer += gameTime.ElapsedGameTime.Milliseconds;
        if (_currentWaveMoveTimer > ComputedWaveDelay)
        {
            _waveMoveWait = false;
            _currentWaveMoveTimer = 0;
            if ( _waveMoveDirection.X == _waveXMove && EnemiesAtRight())
            {
                _waveMoveDirection.X = -_waveXMove;
                _waveMoveDirection.Y = _waveYMove;
            }
            else if (_waveMoveDirection.X  == -_waveXMove && EnemiesAtLeft())
            {
                _waveMoveDirection.X  = _waveXMove;
                _waveMoveDirection.Y = _waveYMove;
            }
            else
            {
                _waveMoveDirection.Y = 0;
            }
            var note = _notes[_currentNoteIndex];
            _currentNoteIndex = (++_currentNoteIndex) % 4;
            SpawnBackgroundSound(note, 25);
        }
        else
        {
            _waveMoveWait = true;
        }
    }    
    #endregion
    

    private void SpawnUfo()
    {
        var ufo = ActorCreator.CreateUfo(this, Atlas);
        ufo.SetPosition(-50,30);
        _extraEnemies.Add(ufo);
    }
    private void SoftReset()
    {
        StopSquareWave();
        StopWhiteNoiseWave();
        StopDynamicWave();
        _currentDynamicFrequency = 0;
        _currentNoteIndex = 0;
        _paused = false;
        _player = null;
        _player = ActorCreator.CreatePlayer(this, Atlas);
        _player.SetPosition(_startingPlayerPosition);
        _bullets.Clear();
        _explosions.Clear();
        _currentUfoTime = 0;
    }
    private void SpawnEnemies()
    {
        var maxRows = 5;
        var maxColumns = 9;
        var startY = Math.Min(400, 120+(_wave*20));
        var startX = 110;
        var offsetY = 80;
        var offsetX = 80;

        for (var y = 0; y < maxRows; ++y)
        {
            for (var x = 0; x < maxColumns; ++x)
            {
                switch (y)
                {
                    case 0:
                        var squid = ActorCreator.CreateSquid(this, Atlas);
                        squid.SetPosition(startX+(x*offsetX),startY+(y*offsetY));
                        _waveEnemies.Add(squid);
                        break;
                    case 1:
                    case 2:
                        var bug = ActorCreator.CreateBug(this, Atlas);
                        bug.SetPosition(startX+(x*offsetX),startY+(y*offsetY));
                        _waveEnemies.Add(bug);
                        break;
                    case 3:
                    case 4:
                        var lander = ActorCreator.CreateLander(this, Atlas);
                        lander.SetPosition(startX+(x*offsetX),startY+(y*offsetY));
                        _waveEnemies.Add(lander);                        
                        break;                    
                }
            }
        }
        
    }
    private void SpawnBarricades()
    {
        var startY = 700;
        var startX = 100;
        var offsetX = 200;
        
        
            for (var i = 0; i < 4; ++i)
            {
                var barricade = new Barricade(startX+(i*offsetX), startY);
                _barricades.Add(barricade);
            }
    }
    private void PlayNote(int note, TimeSpan duration)
    {
        StartNoteOnChannel(note, 1);
        Thread.Sleep(duration);
        StopNoteOnChannel(note, 1);
    }
    private bool EnemiesAtRight()
    {
        for(var i = 0; i < _waveEnemies.Count; ++i)
        {
            if (_waveEnemies[i].Position.X > Dimensions.X-70)
            {
                return true;
            }
        }
        return false;
    }
    private bool EnemiesAtLeft()
    {
        for(var i = 0; i < _waveEnemies.Count; ++i)
        {
            if (_waveEnemies[i].Position.X < 70)
            {
                return true;
            }
        }
        return false;
    }
    private void DrawText(string text, Vector2 position, SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(Font, text, position, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 1 );
    }
}