using System;
using System.Linq;
using OpenTK.Graphics.OpenGL;
using ToyGame.Rendering.Shaders;

namespace ToyGame.Rendering.OpenGL
{
  internal class GLDrawCall : IComparable<GLDrawCall>
  {
    #region Types

    public class GLState : IComparable<GLState>
    {
      #region Fields / Properties

      public static readonly GLState Default = new GLState(CullFaceMode.Back, DepthFunction.Lequal,
        BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha, EnableCap.Texture2D,
        EnableCap.Blend,
        EnableCap.DepthTest,
        EnableCap.Multisample);

      public readonly CullFaceMode CullFaceMode;
      public readonly EnableCap[] EnableCaps;
      public readonly DepthFunction DepthFunction;
      public readonly BlendingFactorSrc BlendingFactorSrc;
      public readonly BlendingFactorDest BlendingFactorDest;
      private readonly string _fullHash;

      #endregion

      public GLState(CullFaceMode cullFaceMode, DepthFunction depthFunction, BlendingFactorSrc blendingFactorSrc,
        BlendingFactorDest blendingFactorDest, params EnableCap[] enableCaps)
      {
        CullFaceMode = cullFaceMode;
        DepthFunction = depthFunction;
        BlendingFactorSrc = blendingFactorSrc;
        BlendingFactorDest = blendingFactorDest;
        EnableCaps = enableCaps;
        _fullHash = "{" + CullFaceMode + "}{" + DepthFunction + "}{" + BlendingFactorSrc + "}{" + BlendingFactorDest +
                    "}{" + EnableCaps.Aggregate("", (left, cap) => left + cap.ToString()) + "}";
      }

      public int CompareTo(GLState other)
      {
        // This is the lazy (and slow) way of doing this... Not that slow, but not as fast as it could be
        return String.Compare(ToString(), other.ToString(), StringComparison.Ordinal);
      }

      public void EnableState()
      {
        foreach (var enableCap in EnableCaps)
        {
          GL.Enable(enableCap);
        }
        GL.CullFace(CullFaceMode);
        GL.BlendFunc(BlendingFactorSrc, BlendingFactorDest);
        GL.DepthFunc(DepthFunction);
      }

      public void DisableState()
      {
        foreach (var enableCap in EnableCaps)
        {
          GL.Disable(enableCap);
        }
        foreach (var enableCap in Default.EnableCaps)
        {
          GL.Enable(enableCap);
        }
        GL.CullFace(Default.CullFaceMode);
        GL.BlendFunc(Default.BlendingFactorSrc, BlendingFactorDest);
        GL.DepthFunc(Default.DepthFunction);
      }

      public override string ToString()
      {
        return _fullHash;
      }
    }

    public class GLTextureBind : IComparable<GLTextureBind>
    {
      #region Fields / Properties

      public readonly GLTexture Texture;
      public readonly TextureUnit TextureUnit;
      public readonly TextureTarget TextureTarget;

      #endregion

      public GLTextureBind(GLTexture texture, TextureTarget textureTarget, TextureUnit textureUnit)
      {
        Texture = texture;
        TextureTarget = textureTarget;
        TextureUnit = textureUnit;
      }

      public int CompareTo(GLTextureBind other)
      {
        var textureDifferent = Texture.CompareTo(other.Texture);
        return textureDifferent != 0 ? textureDifferent : TextureUnit.CompareTo(other.TextureUnit);
      }
    }

    public class UniformBind : IComparable<UniformBind>
    {
      #region Fields / Properties

      /// <summary>
      ///   A string that fully represents the uniforms values. For example, a vector3 might
      ///   look like: "1.32:423.42:63.21".
      /// </summary>
      public readonly string FullValueHash;

      public readonly Action UniformBindAction;

      #endregion

      public UniformBind(string fullValueHash, Action uniformBindAction)
      {
        FullValueHash = fullValueHash;
        UniformBindAction = uniformBindAction;
      }

      public int CompareTo(UniformBind other)
      {
        return String.Compare(FullValueHash, other.FullValueHash, StringComparison.Ordinal);
      }
    }

    #endregion

    #region Fields / Properties

    // Overall sort order for render calls:
    // - GL Program
    //     - GL State (Depth Testing, Back Cull)
    //       - GL Textures
    //         - VAO
    //           - Uniforms: For now, all uniforms are fully rebound, every draw call

    internal readonly GLShaderProgram Program;
    internal readonly GLState State;
    internal readonly GLTextureBind[] TextureBinds;
    internal readonly GLVertexArrayObject VertexArrayObject;
    internal readonly UniformBind[] UniformBinds;
    internal readonly Action DrawAction;
    internal Action BindAction { get; private set; }

    #endregion

    public GLDrawCall(GLShaderProgram program, GLTextureBind[] textureBinds, GLVertexArrayObject vertexArrayObject,
      UniformBind[] uniformBinds, Action drawAction, GLState state = null)
    {
      Program = program;
      TextureBinds = textureBinds;
      VertexArrayObject = vertexArrayObject;
      UniformBinds = uniformBinds;
      DrawAction = drawAction;
      State = state ?? GLState.Default;
    }

    public int CompareTo(GLDrawCall other)
    {
      var programDiff = Program.CompareTo(other.Program);
      if (programDiff != 0)
      {
        return programDiff;
      }
      var textuBindCountDiff = TextureBinds.Length - other.TextureBinds.Length;
      if (textuBindCountDiff != 0)
      {
        return textuBindCountDiff;
      }
      // Use an old fasion and ugly but way faster loop for this all of this
      for (var i = 0; i < TextureBinds.Length; i++)
      {
        var textureBindDiff = TextureBinds[i].CompareTo(other.TextureBinds[i]);
        if (textureBindDiff != 0)
        {
          return textureBindDiff;
        }
      }
      var stateDiff = State.CompareTo(other.State);
      if (stateDiff != 0)
      {
        return stateDiff;
      }
      var vaoDiff = VertexArrayObject.CompareTo(other.VertexArrayObject);
      if (vaoDiff != 0)
      {
        return vaoDiff;
      }
      var uniformCountDiff = UniformBinds.Length - other.UniformBinds.Length;
      if (uniformCountDiff != 0)
      {
        return uniformCountDiff;
      }
      for (var i = 0; i < UniformBinds.Length; i++)
      {
        var unfirmDiff = UniformBinds[i].CompareTo(other.UniformBinds[i]);
        if (unfirmDiff != 0)
        {
          return unfirmDiff;
        }
      }
      // They are equal (apart, hopefully, from the draw call)
      return 0;
    }

    public void Draw()
    {
      BindAction();
      DrawAction();
    }

    /// <summary>
    ///   Does a compare against the last state, and generates a new action to be executed
    ///   on the GPU thread that will only change the minimum state needed. All other non
    ///   needed state changes are culled before even getting to the GPU thread. Beautiful.
    /// </summary>
    public void GenerateBindAction(GLDrawCall fromCall)
    {
      Action programSwapAction = null;
      Action stateSwapAction = null;
      Action textureBindAction = null;
      Action voaBindAction = null;
      Action uniformBindAction = null;
      // GL Program
      if (fromCall == null || Program.CompareTo(fromCall.Program) != 0)
      {
        programSwapAction = () => Program.Use();
      }
      // GL State
      if (fromCall == null || State.CompareTo(fromCall.State) != 0)
      {
        stateSwapAction = () =>
        {
          fromCall?.State.DisableState();
          State.EnableState();
        };
      }
      // Texture Binds
      if (fromCall == null || TextureBinds.Length != fromCall.TextureBinds.Length)
      {
        textureBindAction = () =>
        {
          // This can be optimized... but pull the optimization out of the Action
          if (fromCall != null)
          {
            foreach (var binding in fromCall.TextureBinds)
            {
              GL.ActiveTexture(binding.TextureUnit);
              GL.BindTexture(binding.TextureTarget, 0);
            }
          }
          foreach (var binding in TextureBinds)
          {
            binding.Texture.Bind(binding.TextureUnit);
          }
        };
      }
      else
      {
        for (var i = 0; i < TextureBinds.Length; i++)
        {
          if (TextureBinds[i].CompareTo(fromCall.TextureBinds[i]) == 0) continue;
          textureBindAction = () =>
          {
            // This can be optimized... but pull the optimization out of the Action
            foreach (var binding in fromCall.TextureBinds)
            {
              GL.ActiveTexture(binding.TextureUnit);
              GL.BindTexture(binding.TextureTarget, 0);
            }
            foreach (var binding in TextureBinds)
            {
              binding.Texture.Bind(binding.TextureUnit);
            }
          };
          break;
        }
      }
      // Vertex Array Object
      if (fromCall == null || VertexArrayObject.CompareTo(fromCall.VertexArrayObject) != 0)
      {
        voaBindAction = () => { VertexArrayObject.Bind(); };
      }
      // Uniforms
      if (fromCall == null || Program.CompareTo(fromCall.Program) != 0 ||
          UniformBinds.Length != fromCall.UniformBinds.Length)
      {
        uniformBindAction = () =>
        {
          foreach (var uniformBind in UniformBinds)
          {
            uniformBind.UniformBindAction();
          }
        };
      }
      else
      {
        for (var i = 0; i < UniformBinds.Length; i++)
        {
          if (UniformBinds[i].CompareTo(fromCall.UniformBinds[i]) == 0) continue;
          uniformBindAction = () =>
          {
            foreach (var uniformBind in UniformBinds)
            {
              uniformBind.UniformBindAction();
            }
          };
          break;
        }
      }
      BindAction = () =>
      {
        programSwapAction?.Invoke();
        stateSwapAction?.Invoke();
        textureBindAction?.Invoke();
        voaBindAction?.Invoke();
        uniformBindAction?.Invoke();
      };
    }
  }
}