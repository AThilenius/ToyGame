using System.Collections.Generic;
using BulletSharp;
using OpenTK;
using ToyGame.Rendering.OpenGL;
using ToyGame.Utilities;

namespace ToyGame.Gameplay
{
  /// <summary>
  ///   This is a dynamic object, created to host a Level (or several levels for streaming).
  ///   Worlds are not serialized, and only one world will exist in a game, but several exist
  ///   in the editor to host different viewports with different contents.
  /// </summary>
  public class World
  {
    #region Fields / Properties

    private readonly DiscreteDynamicsWorld _physicsWorld;
    private readonly List<CollisionShape> _collisionShapes = new List<CollisionShape>();
    private readonly List<Level> _levels = new List<Level>();

    #endregion

    public World()
    {
      CollisionConfiguration collisionConfiguration = new DefaultCollisionConfiguration();
      var collisionDispatcher = new CollisionDispatcher(collisionConfiguration);
      var broadphase = new DbvtBroadphase();
      _physicsWorld = new DiscreteDynamicsWorld(collisionDispatcher, broadphase, null, collisionConfiguration)
      {
        Gravity = new Vector3(0, -9.81f, 0).ToBullet()
      };
      // Create Ground plane
      var groundShape = new StaticPlaneShape(Vector3.UnitY.ToBullet(), 1);
      _collisionShapes.Add(groundShape);
      CollisionObject ground = AddRigidBody(0, Matrix4.Identity, groundShape);
      ground.UserObject = "Ground";
    }

    public void AddLevel(Level level)
    {
      _levels.Add(level);
      level.World = this;
    }

    public RigidBody AddBoxRigidBody(float mass, Matrix4 startTransform)
    {
      return AddRigidBody(mass, startTransform, new BoxShape(new Vector3(0.5f, 0.5f, 0.5f).ToBullet()));
    }

    public RigidBody AddRigidBody(float mass, Matrix4 startTransform, CollisionShape shape)
    {
      // Rigidbody is dynamic if and only if mass is non zero, otherwise static
      var isDynamic = (mass != 0.0f);
      var localInertia = Vector3.Zero.ToBullet();
      if (isDynamic)
        shape.CalculateLocalInertia(mass, out localInertia);
      // Using motionstate is recommended, it provides interpolation capabilities, and only synchronizes 'active' objects
      var myMotionState = new DefaultMotionState(startTransform.ToBullet());
      var rbInfo = new RigidBodyConstructionInfo(mass, myMotionState, shape, localInertia);
      var body = new RigidBody(rbInfo);
      rbInfo.Dispose();
      _physicsWorld.AddRigidBody(body);
      return body;
    }

    internal void Update()
    {
      _levels.ForEach(level => level.Update());
    }

    internal void EnqueueDrawCalls(GLDrawCallBatch drawCallBatch)
    {
      _levels.ForEach(level => level.EnqueueDrawCalls(drawCallBatch));
    }
  }
}