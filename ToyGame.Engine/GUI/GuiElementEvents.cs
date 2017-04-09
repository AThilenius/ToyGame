using System;
using OpenTK.Input;

namespace ToyGame.GUI
{
  public partial class GuiElement
  {
    #region Fields / Properties

    public Action<GuiElement, KeyboardKeyEventArgs> KeyDown;
    public Action<GuiElement, KeyboardKeyEventArgs> KeyUp;
    public Action<GuiElement, MouseButtonEventArgs> MouseDown;
    public Action<GuiElement, MouseButtonEventArgs> MouseUp;
    public Action<GuiElement, MouseWheelEventArgs> MouseWheel;
    public Action<GuiElement, MouseButtonEventArgs> Clicked;
    public Action<GuiElement>Focus;
    public Action<GuiElement> LostFocus;
    public Action<GuiElement> MouseEnter;
    public Action<GuiElement> MouseLeave;

    #endregion

    internal virtual void OnKeyDown(KeyboardKeyEventArgs e) => KeyDown?.Invoke(this, e);
    internal virtual void OnKeyUp(KeyboardKeyEventArgs e) => KeyUp?.Invoke(this, e);
    internal virtual void OnMouseDown(MouseButtonEventArgs e) => MouseDown?.Invoke(this, e);
    internal virtual void OnMouseUp(MouseButtonEventArgs e) => MouseUp?.Invoke(this, e);
    internal virtual void OnMouseWheel(MouseWheelEventArgs e) => MouseWheel?.Invoke(this, e);
    internal virtual void OnClicked(MouseButtonEventArgs e) => Clicked?.Invoke(this, e);
    internal virtual void OnFocus() => Focus?.Invoke(this);
    internal virtual void OnLostFocus() => LostFocus?.Invoke(this);
    internal virtual void OnMouseEnter() => MouseEnter?.Invoke(this);
    internal virtual void OnMouseLeave() => MouseLeave?.Invoke(this);
  }
}
