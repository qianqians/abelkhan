using Stride.Core;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Input;

namespace Abelkhan.Stride;

/// <summary>
/// 和 Stride 模板一样的自由相机，纯粹为了让场景能看（网络接入跟它没关系）。
/// </summary>
public class FreeCameraController : SyncScript
{
    private const float MaximumPitch = MathUtil.PiOverTwo * 0.99f;

    private Vector3 _upVector = Vector3.UnitY;
    private Vector3 _translation;
    private float _yaw;
    private float _pitch;

    public Vector3 KeyboardMovementSpeed { get; set; } = new(5.0f);

    public Vector2 KeyboardRotationSpeed { get; set; } = new(3.0f);

    public float SpeedFactor { get; set; } = 5.0f;

    public override void Start()
    {
        base.Start();
        _upVector = Vector3.UnitY;
    }

    public override void Update()
    {
        var deltaTime = (float)Game.UpdateTime.Elapsed.TotalSeconds;
        _translation = Vector3.Zero;
        _yaw = 0f;
        _pitch = 0f;

        if (Input.HasKeyboard)
        {
            var dir = Vector3.Zero;
            if (Input.IsKeyDown(Keys.W) || Input.IsKeyDown(Keys.Up)) dir.Z += 1;
            if (Input.IsKeyDown(Keys.S) || Input.IsKeyDown(Keys.Down)) dir.Z -= 1;
            if (Input.IsKeyDown(Keys.A) || Input.IsKeyDown(Keys.Left)) dir.X -= 1;
            if (Input.IsKeyDown(Keys.D) || Input.IsKeyDown(Keys.Right)) dir.X += 1;
            if (Input.IsKeyDown(Keys.Q)) dir.Y -= 1;
            if (Input.IsKeyDown(Keys.E)) dir.Y += 1;

            var speed = deltaTime;
            if (Input.IsKeyDown(Keys.LeftShift) || Input.IsKeyDown(Keys.RightShift))
            {
                speed *= SpeedFactor;
            }

            if (dir.Length() > 1f)
            {
                dir = Vector3.Normalize(dir);
            }

            _translation += dir * KeyboardMovementSpeed * speed;

            var rotation = Vector2.Zero;
            if (Input.IsKeyDown(Keys.NumPad2)) rotation.X += 1;
            if (Input.IsKeyDown(Keys.NumPad8)) rotation.X -= 1;
            if (Input.IsKeyDown(Keys.NumPad4)) rotation.Y += 1;
            if (Input.IsKeyDown(Keys.NumPad6)) rotation.Y -= 1;
            if (rotation.Length() > 1f)
            {
                rotation = Vector2.Normalize(rotation);
            }

            rotation *= KeyboardRotationSpeed * deltaTime;
            _pitch += rotation.X;
            _yaw += rotation.Y;
        }

        UpdateTransform();
    }

    private void UpdateTransform()
    {
        var rotation = Matrix.RotationQuaternion(Entity.Transform.Rotation);
        var right = Vector3.Cross(rotation.Forward, _upVector);
        var up = Vector3.Cross(right, rotation.Forward);
        right.Normalize();
        up.Normalize();

        var currentPitch = MathUtil.PiOverTwo - MathF.Acos(Vector3.Dot(rotation.Forward, _upVector));
        _pitch = MathUtil.Clamp(currentPitch + _pitch, -MaximumPitch, MaximumPitch) - currentPitch;

        var finalTranslation = _translation;
        finalTranslation.Z = -finalTranslation.Z;
        finalTranslation = Vector3.TransformCoordinate(finalTranslation, rotation);

        Entity.Transform.Position += finalTranslation;
        Entity.Transform.Rotation *= Quaternion.RotationAxis(right, _pitch) * Quaternion.RotationAxis(_upVector, _yaw);
    }
}
