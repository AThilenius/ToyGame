using System;
using OpenTK.Input;

namespace ToyGame.GUI
{
  public partial class GuiElement
  {
    #region Fields / Properties

    public event EventHandler<KeyboardKeyEventArgs> KeyDown;
    public event EventHandler<KeyboardKeyEventArgs> KeyUp;
    public event EventHandler<MouseButtonEventArgs> MouseDown;
    public event EventHandler<MouseButtonEventArgs> MouseUp;
    public event EventHandler<MouseWheelEventArgs> MouseWheel;
    public event EventHandler<MouseButtonEventArgs> Clicked;
    public event EventHandler Focus;
    public event EventHandler LostFocus;
    public event EventHandler MouseEnter;
    public event EventHandler MouseLeave;

    #endregion

    internal virtual void OnKeyDown(KeyboardKeyEventArgs e) => KeyDown?.Invoke(this, e);
    internal virtual void OnKeyUp(KeyboardKeyEventArgs e) => KeyUp?.Invoke(this, e);
    internal virtual void OnMouseDown(MouseButtonEventArgs e) => MouseDown?.Invoke(this, e);
    internal virtual void OnMouseUp(MouseButtonEventArgs e) => MouseUp?.Invoke(this, e);
    internal virtual void OnMouseWheel(MouseWheelEventArgs e) => MouseWheel?.Invoke(this, e);
    internal virtual void OnClicked(MouseButtonEventArgs e) => Clicked?.Invoke(this, e);
    internal virtual void OnFocus() => Focus?.Invoke(this, EventArgs.Empty);
    internal virtual void OnLostFocus() => LostFocus?.Invoke(this, EventArgs.Empty);
    internal virtual void OnMouseEnter() => MouseEnter?.Invoke(this, EventArgs.Empty);
    internal virtual void OnMouseLeave() => MouseLeave?.Invoke(this, EventArgs.Empty);
  }
}