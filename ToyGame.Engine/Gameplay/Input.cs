using OpenTK;
using OpenTK.Input;

namespace ToyGame.Gameplay
{
  public static class Input
  {
    #region Fields / Properties

    public static Vector2 MousePosition => new Vector2(_mouseState.X, _mouseState.Y);
    private static KeyboardState _keyboardState;
    private static KeyboardState _lastKeyboardState;
    private static MouseState _mouseState;
    private static MouseState _lastMouseState;

    #endregion

    public static bool IsKeyDown(Key key) => _keyboardState.IsKeyDown(key);
    public static bool IsKeyUp(Key key) => _keyboardState.IsKeyUp(key);

    /// <summary>
    ///   Only returns true for a single frame, when the key is first pressed DOWN
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static bool WasKeyPressed(Key key) => !_lastKeyboardState.IsKeyDown(key) && _keyboardState.IsKeyDown(key);

    /// <summary>
    ///   Only returns true for a single frame, when the key is first released (UP)
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static bool WasKeyReleased(Key key) => _lastKeyboardState.IsKeyDown(key) && !_keyboardState.IsKeyDown(key);

    public static bool IsMouseButtonDown(MouseButton button) => _mouseState.IsButtonDown(button);
    public static bool IsMouseButtonUp(MouseButton button) => _mouseState.IsButtonUp(button);

    /// <summary>
    ///   Only returns true for a single frame, when the mouse button is first pressed DOWN
    /// </summary>
    /// <param name="button"></param>
    /// <returns></returns>
    public static bool WasMouseButtonPressed(MouseButton button)
      => !_lastMouseState.IsButtonDown(button) && _mouseState.IsButtonDown(button);

    /// <summary>
    ///   Only returns true for a single frame, when the mouse button is first released (UP)
    /// </summary>
    /// <param name="button"></param>
    /// <returns></returns>
    public static bool WasMouseButtonReleased(MouseButton button)
      => _lastMouseState.IsButtonDown(button) && !_mouseState.IsButtonDown(button);

    internal static void UpdateState(KeyboardState keyboardState, MouseState mouseState)
    {
      _lastKeyboardState = _keyboardState;
      _keyboardState = keyboardState;
      _lastMouseState = _mouseState;
      _mouseState = mouseState;
    }
  }
}