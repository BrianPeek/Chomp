using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace MonsterGame
{
	public class GameScreen : Screen
	{
		private enum GameScreenState
		{
			GetReady,
			Play,
			PlayerEaten,
			LevelComplete,
			GameOver
		}

		private readonly SpriteBatch _spriteBatch;
		private readonly Player _player;
		private readonly Enemy _enemy;
		private readonly ScoreDisplay _scoreDisplay;
		private readonly GetReady _getReady;
		private readonly GameOver _gameOver;
		private readonly PauseDialog _pauseDialog;
		private readonly LivesDisplay _livesDisplay;
		private readonly Texture2D _background;
		private GameScreenState _state;
		private Maze _maze;
		private int _levelIndex;
		private ILevel _currentLevel = Levels.AllLevels[0];
		private int _score;
		private readonly PathFinder _pathFinder;
		private int _numLives;
		private int _gameOverTimer;
		private Direction _lastDirection = Direction.None;

		private const int NumberOfLives = 2;
		private const int GameOverTimeout = 5000;

		public GameScreen(Game game) : base(game)
		{
			_state = GameScreenState.GetReady;

			_spriteBatch = new SpriteBatch(game.GraphicsDevice);

			_player = new Player();
			_enemy = new Enemy();
			_pathFinder = new PathFinder();

			_scoreDisplay = new ScoreDisplay { Position = new Vector2(210, 10) };

			_getReady = new GetReady(300);
			_getReady.Center();

			_gameOver = new GameOver(300);
			_gameOver.Center();

			_pauseDialog = new PauseDialog(game);
			_pauseDialog.Center();

			_livesDisplay = new LivesDisplay
			{
				Position = new Vector2(240, 1040)
			};
			
			_background = game.Content.Load<Texture2D>("gfx\\maze\\Background");

			_numLives = NumberOfLives;

			InitializeLevel(0);
		}

		public void InitializeLevel(int level)
		{
			_state = GameScreenState.GetReady;

			AudioManager.SoundEffectInstances[AudioManager.Cue.GameplayMusic].Stop();
			_currentLevel = Levels.AllLevels[level];
			_maze = new Maze(MonsterGame.Instance, _currentLevel.Data);
			_player.SetTilePosition((int)_maze.PlayerStart.Y, (int)_maze.PlayerStart.X);
			_player.Reset();
			_enemy.SetTilePosition((int)_maze.EnemyStart.Y, (int)_maze.EnemyStart.X);
			_enemy.Reset();
			_getReady.Reset();
			AudioManager.SoundEffectInstances[AudioManager.Cue.GetReadyMusic].Play();
			_lastDirection = Direction.None;

			_pathFinder.Initialize(_maze);
		}

		public void ResetLevel()
		{
			_state = GameScreenState.GetReady;
			_lastDirection = Direction.None;

			AudioManager.SoundEffectInstances[AudioManager.Cue.GameplayMusic].Stop();
			_player.SetTilePosition((int)_maze.PlayerStart.Y, (int)_maze.PlayerStart.X);
			_player.Reset();
			_enemy.SetTilePosition((int)_maze.EnemyStart.Y, (int)_maze.EnemyStart.X);
			_enemy.Reset();
			_getReady.Reset();
			AudioManager.SoundEffectInstances[AudioManager.Cue.GetReadyMusic].Play();
		}

		public override void Update(GameTime gameTime)
		{
			if(MonsterGame.Instance.IsPaused)
			{
				_pauseDialog.Update(gameTime);
				return;
			}

			if(InputManager.CurrentState.Start || InputManager.CurrentState.Back)
			{
				_pauseDialog.Show();
				MonsterGame.Instance.IsPaused = !MonsterGame.Instance.IsPaused;
			}

			switch(_state)
			{
				case GameScreenState.GetReady:
					if(AudioManager.SoundEffectInstances[AudioManager.Cue.GetReadyMusic].State == SoundState.Stopped)
					{
						_getReady.Hide();
						_state = GameScreenState.Play;
						AudioManager.SoundEffectInstances[AudioManager.Cue.GameplayMusic].IsLooped = true;
						AudioManager.SoundEffectInstances[AudioManager.Cue.GameplayMusic].Volume = 0.6f;
						AudioManager.SoundEffectInstances[AudioManager.Cue.GameplayMusic].Play();
					}
					break;
				case GameScreenState.Play:
					MovePlayer();
					MoveEnemy();
					HandleCollisions();
					break;
				case GameScreenState.PlayerEaten:
					if(AudioManager.SoundEffectInstances[AudioManager.Cue.PlayerEaten].State != SoundState.Playing)
					{
						_numLives--;
						ResetLevel();
					}
					break;
				case GameScreenState.LevelComplete:
					if(AudioManager.SoundEffectInstances[AudioManager.Cue.LevelCompleted].State != SoundState.Playing)
						MoveToNextLevel();
					break;
				case GameScreenState.GameOver:
					_gameOverTimer += gameTime.ElapsedGameTime.Milliseconds;
					if(_gameOverTimer >= GameOverTimeout)
						MonsterGame.Instance.SetState(GameState.TitleScreen);
					break;
			}

			_player.Update(gameTime);

			MazeTile mt = _maze.GetTile(_player.Row, _player.Column);
			if(mt != null)
			{
				switch(mt.DotType)
				{
					case MazeTile.DotTileType.None:
						break;
					case MazeTile.DotTileType.Dot:
						_player.EatDot();
						_score += 100;
						break;
					case MazeTile.DotTileType.Icon:
						_player.EatIcon();
						_score += 1000;
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}

				mt.DotType = MazeTile.DotTileType.None;

				if(_maze.IsComplete)
				{
					_state = GameScreenState.LevelComplete;
					AudioManager.SoundEffectInstances[AudioManager.Cue.GameplayMusic].Stop();
					_player.Move(Direction.None);
					_enemy.Move(Direction.None);
					AudioManager.SoundEffectInstances[AudioManager.Cue.LevelCompleted].Play();
				}
			}

			_enemy.Update(gameTime);
			_getReady.Update(gameTime);
			_gameOver.Update(gameTime);
			_livesDisplay.NumberOfLives = _numLives;
			_livesDisplay.Update(gameTime);
			_maze.Update(gameTime);
			_scoreDisplay.Update(gameTime, XboxLiveManager.Profile.Gamertag, _score);
		}

		private void MoveToNextLevel()
		{
			_levelIndex = (_levelIndex+1) % Levels.AllLevels.Count;
			InitializeLevel(_levelIndex);
		}

		private void HandleCollisions()
		{
			if(!MonsterGame.Instance.DebugMode && _enemy.CollisionBox.Intersects(_player.CollisionBox))
			{
				if(_numLives == 0)
					GameOver();
				else
					PlayerEaten();
			}
		}

		private void PlayerEaten()
		{
			_state = GameScreenState.PlayerEaten;
			AudioManager.SoundEffectInstances[AudioManager.Cue.GameplayMusic].Stop();
			AudioManager.SoundEffectInstances[AudioManager.Cue.PlayerEaten].Play();
			_enemy.Move(Direction.None);
			_player.Die();
		}

		private void GameOver()
		{
			_state = GameScreenState.GameOver;
			AudioManager.SoundEffectInstances[AudioManager.Cue.GameplayMusic].Stop();
			AudioManager.SoundEffectInstances[AudioManager.Cue.GameOver].Play();
			_enemy.Move(Direction.None);
			_player.Die();
			_gameOver.Show();
		}

		public override void Draw(GameTime gameTime)
		{
			_spriteBatch.Begin();
				_spriteBatch.Draw(_background, Vector2.Zero, Color.White);
				_maze.Draw(gameTime, _spriteBatch);
				_player.Draw(gameTime, _spriteBatch);
				_enemy.Draw(gameTime, _spriteBatch);
				_scoreDisplay.Draw(gameTime, _spriteBatch);
				_getReady.Draw(gameTime, _spriteBatch);
				_gameOver.Draw(gameTime, _spriteBatch);
				_livesDisplay.Draw(gameTime, _spriteBatch);
				_pauseDialog.Draw(gameTime, _spriteBatch);

				if(MonsterGame.Instance.DebugMode)
				{
					FontManager.DrawString(_spriteBatch, "Col: " + _player.Column + ", Row: " + _player.Row + ", " + _maze.GetTile(_player.Row, _player.Column).TileType, Vector2.Zero, Color.White);
					FontManager.DrawString(_spriteBatch, "X: " + _player.Position.X + ", Y: " + _player.Position.Y, new Vector2(0, 40), Color.White);
					FontManager.DrawString(_spriteBatch, "L: " + _player.BoundingBox.Left + ", R: " + _player.BoundingBox.Right, new Vector2(0, 80), Color.White);
					FontManager.DrawString(_spriteBatch, "T: " + _player.BoundingBox.Top + ", B: " + _player.BoundingBox.Bottom, new Vector2(0, 120), Color.White);
					FontManager.DrawString(_spriteBatch, "W: " + _player.BoundingBox.Width + ", H: " + _player.BoundingBox.Height, new Vector2(0, 160), Color.White);
				}
			_spriteBatch.End();
		}

		private void MovePlayer()
		{
			if(InputManager.RawState.Up)
				MovePlayer(Direction.Up);
			else if(InputManager.RawState.Down)
				MovePlayer(Direction.Down);
			else if(InputManager.RawState.Left)
				MovePlayer(Direction.Left);
			else if(InputManager.RawState.Right)
				MovePlayer(Direction.Right);
			else
				MovePlayer(_lastDirection);
		}

		private void MoveEnemy()
		{
			_pathFinder.FindPath(new Point(_enemy.Column, _enemy.Row), new Point(_player.Column, _player.Row));

			if(_pathFinder.SearchStatus == SearchStatus.PathFound)
			{
				LinkedList<Point> path = _pathFinder.FinalPath();
				if(path.Count == 1)
					return;
				path.RemoveFirst();
				Point nextTile = path.First.Value;
				if(nextTile.X > _enemy.Column)
					_enemy.Move(Direction.Right);
				if(nextTile.X < _enemy.Column)
					_enemy.Move(Direction.Left);
				if(nextTile.Y > _enemy.Row)
					_enemy.Move(Direction.Down);
				if(nextTile.Y < _enemy.Row)
					_enemy.Move(Direction.Up);
			}
		}

		private void MovePlayer(Direction direction)
		{
			switch(direction)
			{
				case Direction.None:
					_player.Move(Direction.None);
					return;
				case Direction.Up:
					if(_maze.GetTile(_player.Row - 1, _player.Column).TileType == MazeTile.MazeTileType.Path ||
						_player.BoundingBox.Top > ((_player.Row)*Maze.TileHeight) + Maze.MazeOffset.Y)
					{
						_lastDirection = Direction.Up;
						_player.Move(Direction.Up);
					}
					else if(direction == _lastDirection)
						Stop(_player);
					else
						MovePlayer(_lastDirection);
					break;
				case Direction.Down:
					if(_maze.GetTile(_player.Row + 1, _player.Column).TileType == MazeTile.MazeTileType.Path ||
					   _player.BoundingBox.Bottom < ((_player.Row + 1)*Maze.TileHeight) + Maze.MazeOffset.Y)
					{
						_lastDirection = Direction.Down;
						_player.Move(Direction.Down);
					}
					else if(direction == _lastDirection)
						Stop(_player);
					else
						MovePlayer(_lastDirection);
					break;
				case Direction.Left:
					if(_maze.GetTile(_player.Row, _player.Column - 1).TileType == MazeTile.MazeTileType.Path ||
						_player.BoundingBox.Left > ((_player.Column)*Maze.TileWidth) + Maze.MazeOffset.X)
					{
						_lastDirection = Direction.Left;
						_player.Move(Direction.Left);
					}
					else if(direction == _lastDirection)
						Stop(_player);
					else
						MovePlayer(_lastDirection);
					break;
				case Direction.Right:
					if(_maze.GetTile(_player.Row, _player.Column + 1).TileType == MazeTile.MazeTileType.Path ||
					   _player.BoundingBox.Right < ((_player.Column + 1)*Maze.TileWidth) + Maze.MazeOffset.X)
					{
						_lastDirection = Direction.Right;
						_player.Move(Direction.Right);
					}
					else if(direction == _lastDirection)
						Stop(_player);
					else
						MovePlayer(_lastDirection);
					break;
				default:
					throw new ArgumentOutOfRangeException("direction", direction, null);
			}
		}

		private void Stop(MonsterSprite sprite)
		{
			_lastDirection = Direction.None;
			sprite.Move(Direction.None);
		}
	}
}
