using System.Drawing;

namespace ToyGame.GUI
{
  public partial class GuiElement
  {
    #region Fields / Properties

    public static Color DefaultFontColor = Color.White;
    public static Font DefaultFont = new Font("Arial", 10);
    public Color BackgroundColor = Color.Transparent;

    #endregion

    public virtual void Draw(Graphics graphics)
    {
      if (BackgroundColor != Color.Transparent)
      {
        graphics.SetClip(WorkingArea);
        if (BackgroundColor != Color.Transparent) graphics.FillRectangle(new SolidBrush(BackgroundColor), WorkingArea);
      }
      foreach (var child in Children) child.Draw(graphics);
    }
  }
}