using System;
using System.Collections.Generic;
using UnityEngine;

namespace TocaBoca
{
	public class DiscMovementController
	{
		public enum Movement { Free, Follow, Snap, Shake, Jump };

		private const float BalanceRestore = -0.4f;
		private const float BalanceDamping = -0.01f;
		private const float Tilt = -0.4f;

		private const float FollowTranslationForce = 300.0f;
		private const float FollowTranslationDamping = -15.0f;

		private const float SnapTranslationForce = 150.0f;
		private const float SnapTranslationDamping = -15.0f;
		private const float SnapTranslationReleaseLimit = 0.1f;
		private const float SnapTranslationVelocityReleaseLimitX = 0.2f;
		private const float SnapTranslationVelocityReleaseLimitY = 1.0f;
		private const float SnapRotationReleaseLimit = 20.0f;
		private const float SnapRotationVelocityReleaseLimit = 30.0f;

		private const float ShakeForce = 35.0f;
		private const float ShakeSpeed = 30.0f;
		private const float ShakeTime = 0.5f;

		private const float JumpImpulseForce = 200.0f;
		private const float JumpLiftForce = 20.0f;
		private const float JumpTime = 1.0f;

		private Rigidbody2D body;
		private Movement movement;
		private Vector2 targetPosition;
		private float timer;
		private Dictionary<Movement, Action> updateFunctions;

		public DiscMovementController(Rigidbody2D body)
		{
			this.body = body;
			movement = Movement.Free;
			targetPosition = Vector2.zero;
			timer = 0.0f;

			updateFunctions = new Dictionary<Movement, Action>()
			{
				{ Movement.Follow, FollowUpdate },
				{ Movement.Snap, SnapUpdate },
				{ Movement.Shake, ShakeUpdate },
				{ Movement.Jump, JumpUpdate }
			};
		}

		public void Update()
		{
			if (movement != Movement.Free)
			{
				updateFunctions[movement]?.Invoke();
			}
		}

		public bool IsFree()
		{
			return (movement == Movement.Free);
		}

		public void Follow()
		{
			movement = Movement.Follow;
		}

		public void UpdateTargetPosition(Vector2 position)
		{
			targetPosition = position;
		}

		public void Release()
		{
			movement = Movement.Free;
			targetPosition = Vector2.zero;
		}

		public void Snap(Vector2 position)
		{
			movement = Movement.Snap;
			targetPosition = position;
		}

		public void Shake()
		{
			movement = Movement.Shake;
			timer = 0.0f;
		}

		public void Jump()
		{
			body.AddForce(JumpImpulseForce * Vector2.up);
			movement = Movement.Jump;
			timer = 0.0f;
		}

		private void FollowUpdate()
		{
			Vector2 deltaPosition = targetPosition - body.position;
			Vector2 force = Vector2.zero;
			float torque = 0.0f;

			force += FollowTranslationForce * deltaPosition;
			force += FollowTranslationDamping * body.velocity;

			torque += BalanceRestore * body.rotation;
			torque += BalanceDamping * body.angularVelocity;
			torque += Tilt * body.velocity.x;

			body.AddForce(force);
			body.AddTorque(torque);
		}

		private void SnapUpdate()
		{
			Vector2 deltaPosition = targetPosition - body.position;
			Vector2 force = Vector2.zero;
			float torque = 0.0f;

			force += SnapTranslationForce * deltaPosition;
			force += SnapTranslationDamping * body.velocity;

			torque += BalanceRestore * body.rotation;
			torque += BalanceDamping * body.angularVelocity;
			torque += Tilt * body.velocity.x;

			if
			(
				Mathf.Abs(deltaPosition.x) < SnapTranslationReleaseLimit
				&& Mathf.Abs(body.velocity.x) < SnapTranslationVelocityReleaseLimitX
				&& Mathf.Abs(body.velocity.y) < SnapTranslationVelocityReleaseLimitY
				&& Mathf.Abs(body.rotation) < SnapRotationReleaseLimit
				&& Mathf.Abs(body.angularVelocity) < SnapRotationVelocityReleaseLimit
			)
			{
				movement = Movement.Free;
				targetPosition = Vector2.zero;
			}

			body.AddForce(force);
			body.AddTorque(torque);
		}

		private void ShakeUpdate()
		{
			Vector2 force = ShakeForce * Mathf.Sin(ShakeSpeed * timer) * body.transform.right;
			body.AddForce(force);

			timer += Time.deltaTime;

			if (timer >= ShakeTime)
			{
				movement = Movement.Free;
				timer = 0.0f;
			}
		}

		private void JumpUpdate()
		{
			Vector2 force = JumpLiftForce * Vector2.up;
			body.AddForce(force);

			timer += Time.deltaTime;

			if (timer >= JumpTime)
			{
				movement = Movement.Free;
				timer = 0.0f;
			}
		}
	}
}
