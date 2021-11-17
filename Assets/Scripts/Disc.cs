using UnityEngine;

namespace TocaBoca
{
	public class Disc : MonoBehaviour
	{
		public uint index;
		public enum Role { Neutral, Intruder, Defender };
		public delegate void CollideAction(GameObject defender, GameObject intruder);
		public static event CollideAction OnDefenderIntruderCollide;

		private const string TagDisc = "Disc";
		private bool locked;
		private Role role;
		private DiscMovementController movementController;

		public Disc()
		{
			locked = true;
			role = Role.Neutral;
		}

		public void Lock()
		{
			locked = true;
		}

		public void Unlock()
		{
			locked = false;
		}

		public bool IsUnoccupied()
		{
			return ((locked == false) && movementController.IsFree() && (role == Role.Neutral));
		}

		public void Follow()
		{
			movementController.Follow();
		}

		public void UpdateTargetPosition(Vector2 position)
		{
			movementController.UpdateTargetPosition(position);
		}

		public void Release()
		{
			movementController.Release();
		}

		public void Snap(Vector2 position)
		{
			movementController.Snap(position);
		}

		public void Shake()
		{
			movementController.Shake();
		}

		public void Jump()
		{
			movementController.Jump();
		}

		public Role GetRole()
		{
			return role;
		}

		public void Intrude()
		{
			role = Role.Intruder;
		}

		public void Defend()
		{
			role = Role.Defender;
			Jump();
		}

		public void Neutralize()
		{
			role = Role.Neutral;
		}

		private void Awake()
		{
			Rigidbody2D body = this.gameObject.GetComponent<Rigidbody2D>();
			movementController = new DiscMovementController(body);
		}

		private void FixedUpdate()
		{
			movementController.Update();
		}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			GameObject other = collision.gameObject;
			bool isThisDefender = (role == Role.Defender);
			bool isOtherIntruder = ((other.tag == TagDisc) && (other.GetComponent<Disc>().GetRole() == Role.Intruder));

			if (isThisDefender && isOtherIntruder && (OnDefenderIntruderCollide != null))
			{
				OnDefenderIntruderCollide(this.gameObject, collision.gameObject);
			}
		}
	}
}
