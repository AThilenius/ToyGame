using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace ToyGame
{
  internal class GLTextureParams
  {

    public static readonly GLTextureParams Default = new GLTextureParams();

    public TextureTarget Target = TextureTarget.Texture2D;
    public All WrapS = All.Repeat;
    public All WrapT = All.Repeat;
    public All MagFilter = All.Linear;
    public All MinFilter = All.LinearMipmapLinear;
    public bool GenerateMipMaps = true;
    public bool UseAnisotropicFiltering = true;

  }
}
