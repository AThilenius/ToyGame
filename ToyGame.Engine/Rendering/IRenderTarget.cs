namespace ToyGame.Rendering
{
  /// <summary>
  ///   The interface for a render target. Can be either a FBO or actual Viewport
  /// </summary>
  internal interface IRenderTarget
  {
    void Bind();
    void FinalizeRender();
  }
}