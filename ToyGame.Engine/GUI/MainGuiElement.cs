using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using ToyGame.Rendering;
using ToyGame.Rendering.OpenGL;
using ToyGame.Rendering.Shaders;

namespace ToyGame.GUI
{
  /// <summary>
  ///   A specialized subclass of GuiElement for managing the main element (that is spanned across the entire screen)
  /// </summary>
  public class MainGuiElement : GuiElement, IRenderable
  {
    #region Fields / Properties

    public GuiElement FocusedElement { get; private set; }
    private static GLMesh _glMesh;
    private static GLGuiShader _guiShader;
    private readonly Window _window;
    private GLDynamicTexture _glTexture;
    private GLVertexArrayObject _vertexArrayObject;
    private GuiElement _lastMouseOver;

    #endregion

    public MainGuiElement(Window window)
    {
      _window = window;
      // Setup static mesh if needed, note that it's in screen-space coordinates as no transformation is applied
      _glMesh = _glMesh ??
                new GLMesh(
                  new[] {new Vector3(-1, -1, 0), new Vector3(1, -1, 0), new Vector3(1, 1, 0), new Vector3(-1, 1, 0)},
                  new uint[] {0, 1, 3, 1, 2, 3});
      // Setup static shader if needed. Normally this would be managed by a material, but this is a one-off exception
      if (_guiShader == null)
      {
        _guiShader = new GLGuiShader();
        _guiShader.GpuAllocateDeferred();
      }
      // Setup defaults
      Width = new SFixed(window.NativeWindow.Width);
      Height = new SFixed(window.NativeWindow.Height);
      ChildAlignment = ChildAlignments.Row;
      // GL Deynamic Texture
      // Re-upload texutre to GPU
      _glTexture = new GLDynamicTexture(GLTextureParams.Default, _window.UnscaledSize.Width, _window.UnscaledSize.Height);
      _glTexture.GpuAllocateDeferred();
      // Register NativeWindow events
      window.NativeWindow.KeyDown += (sender, args) => FocusedElement?.OnKeyDown(args);
      window.NativeWindow.KeyUp += (sender, args) => FocusedElement?.OnKeyUp(args);
      window.NativeWindow.Resize += (sender, args) => _glTexture.Resize(_window.UnscaledSize.Width, _window.UnscaledSize.Height);
      window.NativeWindow.MouseDown += (sender, args) =>
      {
        var clickedElement = GetTopmostElementAtPoint(args.Position);
        if (clickedElement == FocusedElement) return;
        // De-Focus last focused element
        FocusedElement?.OnLostFocus();
        // Focus the new one
        FocusedElement = clickedElement;
        FocusedElement.OnFocus();
      };
      window.NativeWindow.MouseUp += (sender, args) =>
      {
        var clickedElement = GetTopmostElementAtPoint(args.Position);
        // If the mouse is still within the control's bounds fire click event
        if (clickedElement == FocusedElement) FocusedElement?.OnClicked(args);
        // TODO: Fire the drag release event here
        clickedElement?.OnMouseUp(args);
      };
      window.NativeWindow.MouseMove += (sender, args) =>
      {
        var mouseOverElement = GetTopmostElementAtPoint(args.Position);
        if (mouseOverElement != _lastMouseOver) _lastMouseOver?.OnMouseLeave();
        _lastMouseOver = mouseOverElement;
        mouseOverElement?.OnMouseEnter();
      };
      window.NativeWindow.MouseWheel += (sender, args) => FocusedElement?.OnMouseWheel(args);
      window.NativeWindow.FocusedChanged += (sender, args) =>
      {
        if (window.NativeWindow.Focused)
        {
          // Reforcused
          FocusedElement?.OnFocus();
        }
        else
        {
          // Unfocused
          FocusedElement?.OnLostFocus();
          _lastMouseOver?.OnMouseLeave();
        }
      };
    }

    public void EnqueueDrawCalls(GLDrawCallBatch drawCallbatch)
    {
      using (var graphics = Graphics.FromImage(_glTexture.Bitmap))
      {
        graphics.Clear(Color.Transparent);
        graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
        Draw(graphics);
      }
      if (_vertexArrayObject == null)
      {
        _vertexArrayObject = new GLVertexArrayObject(_glMesh, _guiShader);
        _vertexArrayObject.GpuAllocateDeferred();
      }
      drawCallbatch.AddDrawCall(new GLDrawCall(_guiShader,
        new[] {new GLDrawCall.GLTextureBind(_glTexture, TextureTarget.Texture2D, TextureUnit.Texture0)},
        _vertexArrayObject, new GLDrawCall.UniformBind[0],
        () =>
          GL.DrawElements(PrimitiveType.Triangles, _glMesh.IndexBuffer.BufferCount, DrawElementsType.UnsignedInt,
            IntPtr.Zero)));
    }
  }
}