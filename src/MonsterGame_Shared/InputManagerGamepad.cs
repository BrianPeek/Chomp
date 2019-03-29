#if GAMEPAD_API
using System.Collections.Generic;
using Windows.Gaming.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
#if WINDOWS_UAP
using Windows.Foundation.Metadata;
#endif

namespace MonsterGame
{
	// all the gamepad buttons
	public enum InputStates
	{
		Up,
		Down,
		Left,
		Right,
		Back,
		Start,
		Debug,
		FullScreen,
	}

	// state of all buttons
	public struct InputState
	{
		public bool Up, Down, Left, Right;
		public bool Back, Start;
		public bool Debug;
		public bool FullScreen;
		public Point Position;
	}

	/// <summary>
	/// Manager for various input types
	/// </summary>
	public static class InputManager
	{
		private const float DeadZone = 0.3f;

		private static KeyboardState _keyboardState, _lastKeyboardState;
		private static MouseState _mouseState, _lastMouseState;

		private static readonly Dictionary<InputStates, Keys> KeyMap = new Dictionary<InputStates, Keys>();

		private static InputState _currentInputState;
		private static InputState _rawInputState;
		private static double _durationLeft;
		private static double _durationRight;
		private static float _vibrationLeft;
		private static float _vibrationRight;
		private static GamepadReading _gamePadState;
		private static GamepadReading _lastGamePadState;

		public static Gamepad ActiveController { get; set; }
		
		static InputManager()
		{
			// map keyboard keys to gamepad keys
			KeyMap.Add(InputStates.Up, Keys.Up);
			KeyMap.Add(InputStates.Down, Keys.Down);
			KeyMap.Add(InputStates.Left, Keys.Left);
			KeyMap.Add(InputStates.Right, Keys.Right);

			KeyMap.Add(InputStates.Back, Keys.Escape);
			KeyMap.Add(InputStates.Start, Keys.Enter);

			KeyMap.Add(InputStates.FullScreen, Keys.F11);
			KeyMap.Add(InputStates.Debug, Keys.F12);
		}

		public static void Update(GameTime gameTime)
		{
			if(Gamepad.Gamepads.Count > 0)
				ActiveController = Gamepad.Gamepads[0];

			TouchPanel.EnabledGestures = GestureType.Tap | GestureType.HorizontalDrag | GestureType.VerticalDrag;

#if WINDOWS_UAP
			if(!ApiInformation.IsApiContractPresent("Windows.Phone.PhoneContract", 1))
#endif
			{
				// handle vibration
				if(_durationLeft > 0.0f)
				{
					_durationLeft -= gameTime.ElapsedGameTime.TotalMilliseconds;

					if(_durationLeft < 0.0f)
						SetVibration(0.0f, _vibrationRight, 0, _durationRight);
				}

				if(_durationRight > 0.0f)
				{
					_durationRight -= gameTime.ElapsedGameTime.TotalMilliseconds;

					if(_durationRight < 0.0f)
						SetVibration(_vibrationLeft, 0.0f, _durationLeft, 0);
				}

				// get current gamepad and keyboard state
				if(ActiveController != null)
					_gamePadState = ActiveController.GetCurrentReading();
				else
					_gamePadState = new GamepadReading();
			}

			_keyboardState = Keyboard.GetState();
			_mouseState = Mouse.GetState();

			_currentInputState.Position =  _mouseState.Position;

			// for each button, get it's actual state (we only want one "true" per button press...holding a button doesn't stay "true"
			_currentInputState.Up = GetKeyState(_gamePadState.Buttons.HasFlag(GamepadButtons.DPadUp), _lastGamePadState.Buttons.HasFlag(GamepadButtons.DPadUp), InputStates.Up);
			_currentInputState.Down = GetKeyState(_gamePadState.Buttons.HasFlag(GamepadButtons.DPadDown), _lastGamePadState.Buttons.HasFlag(GamepadButtons.DPadDown), InputStates.Down);
			_currentInputState.Left = GetKeyState(_gamePadState.Buttons.HasFlag(GamepadButtons.DPadLeft), _lastGamePadState.Buttons.HasFlag(GamepadButtons.DPadLeft), InputStates.Left);
			_currentInputState.Right = GetKeyState(_gamePadState.Buttons.HasFlag(GamepadButtons.DPadRight), _lastGamePadState.Buttons.HasFlag(GamepadButtons.DPadRight), InputStates.Right);

			_currentInputState.Back = GetKeyState(_gamePadState.Buttons.HasFlag(GamepadButtons.View), _lastGamePadState.Buttons.HasFlag(GamepadButtons.View), InputStates.Back) |
										GetKeyState(_gamePadState.Buttons.HasFlag(GamepadButtons.B), _gamePadState.Buttons.HasFlag(GamepadButtons.B), InputStates.Back);
			_currentInputState.Start = GetKeyState(_gamePadState.Buttons.HasFlag(GamepadButtons.Menu), _lastGamePadState.Buttons.HasFlag(GamepadButtons.Menu), InputStates.Start) | 
										GetKeyState(_gamePadState.Buttons.HasFlag(GamepadButtons.A), _lastGamePadState.Buttons.HasFlag(GamepadButtons.A), InputStates.Start);

			_currentInputState.Debug = GetKeyState(_gamePadState.Buttons.HasFlag(GamepadButtons.LeftShoulder), _lastGamePadState.Buttons.HasFlag(GamepadButtons.LeftShoulder), InputStates.Debug);
			_currentInputState.FullScreen = GetKeyState(_gamePadState.Buttons.HasFlag(GamepadButtons.RightShoulder), _lastGamePadState.Buttons.HasFlag(GamepadButtons.RightShoulder), InputStates.FullScreen);

			// now setup a state for the real gamepad state ("true" for the length of time held down)
			_rawInputState.Up = GetKeyState(_gamePadState.Buttons.HasFlag(GamepadButtons.DPadUp), InputStates.Up) | GetThumbstickState(InputStates.Up);
			_rawInputState.Down = GetKeyState(_gamePadState.Buttons.HasFlag(GamepadButtons.DPadDown), InputStates.Down) | GetThumbstickState(InputStates.Down);
			_rawInputState.Left = GetKeyState(_gamePadState.Buttons.HasFlag(GamepadButtons.DPadLeft), InputStates.Left) | GetThumbstickState(InputStates.Left);
			_rawInputState.Right = GetKeyState(_gamePadState.Buttons.HasFlag(GamepadButtons.DPadRight), InputStates.Right) | GetThumbstickState(InputStates.Right);

			_rawInputState.Back = GetKeyState(_gamePadState.Buttons.HasFlag(GamepadButtons.View), InputStates.Back) |
												GetKeyState(_gamePadState.Buttons.HasFlag(GamepadButtons.B), InputStates.Back);
			_rawInputState.Start = GetKeyState(_gamePadState.Buttons.HasFlag(GamepadButtons.Menu), InputStates.Start) |
												GetKeyState(_gamePadState.Buttons.HasFlag(GamepadButtons.A), InputStates.Start);

			_lastGamePadState = _gamePadState;
			_lastKeyboardState = _keyboardState;
			_lastMouseState = _mouseState;

			TouchPanel.GetState();

			while(TouchPanel.IsGestureAvailable)
			{
				GestureSample gesture = TouchPanel.ReadGesture();
				switch(gesture.GestureType)
				{
					case GestureType.Tap:
						_currentInputState.Start = true;
						break;
					case GestureType.HorizontalDrag:
						if(gesture.Delta.X > 0)
							_currentInputState.Right = _rawInputState.Right = true;
						if(gesture.Delta.X < 0)
							_currentInputState.Left = _rawInputState.Left = true;
						break;
					case GestureType.VerticalDrag:
						if(gesture.Delta.Y > 0)
							_currentInputState.Down = _rawInputState.Down = true;
						if(gesture.Delta.Y < 0)
							_currentInputState.Up = _rawInputState.Up = true;
						break;
				}
			}
		}

		private static bool GetThumbstickState(InputStates map)
		{
			switch(map)
			{
				case InputStates.Up:
					return (_gamePadState.LeftThumbstickY > DeadZone);
				case InputStates.Down:
					return (_gamePadState.LeftThumbstickY < -DeadZone);
				case InputStates.Left:
					return (_gamePadState.LeftThumbstickX < -DeadZone);
				case InputStates.Right:
					return (_gamePadState.LeftThumbstickX > DeadZone);
				default:
					return false;
			}
		}

		private static bool GetKeyState(bool buttonState, InputStates map)
		{
			// pressed or not
			return	(buttonState) || (_keyboardState.IsKeyDown(KeyMap[map]));
		}

		private static bool GetKeyState(bool buttonState, bool lastButtonState, InputStates map)
		{
			// single press
			return	(buttonState && !lastButtonState) || 
					(_keyboardState.IsKeyDown(KeyMap[map]) && !_lastKeyboardState.IsKeyDown(KeyMap[map]));
		}

		public static void SetVibration(float left, float right, double durationLeft, double durationRight)
		{
			_durationLeft = durationLeft;
			_vibrationLeft = left;

			_durationRight = durationRight;
			_vibrationRight = right;

			if(ActiveController != null)
				ActiveController.Vibration = new GamepadVibration() { LeftMotor = _vibrationLeft, RightMotor = _vibrationRight };
		}

		public static void SetVibration(float left, float right, double duration)
		{
			_durationLeft = _durationRight = duration;

			_vibrationLeft = left;
			_vibrationRight = right;

			if(ActiveController != null)
				ActiveController.Vibration = new GamepadVibration() { LeftMotor = _vibrationLeft, RightMotor = _vibrationRight };
		}

		public static InputState CurrentState
		{
			get { return _currentInputState; }
		}

		public static InputState RawState
		{
			get { return _rawInputState; }
		}
	}
}
#endif