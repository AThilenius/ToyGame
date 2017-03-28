using System.Drawing;

namespace ToyGame.GUI
{
  public partial class GuiElement
  {
    public Color BackgroundColor = Color.Transparent;

    public virtual void Draw(Graphics graphics)
    {
      if (BackgroundColor != Color.Transparent)
      {
        graphics.SetClip(WorkingArea);
        graphics.FillRectangle(new SolidBrush(BackgroundColor), WorkingArea);
        graphics.DrawString(WorkingArea.ToString(), new Font("Arial", 16), new SolidBrush(Color.Black),
          WorkingArea.X + 10, WorkingArea.Y + (WorkingArea.Height / 2.0f));
      }
      foreach (var child in Children) child.Draw(graphics);
    }

  }
}
