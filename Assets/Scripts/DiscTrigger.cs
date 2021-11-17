using UnityEngine;

namespace TocaBoca
{
	public class DiscTrigger : MonoBehaviour
	{
		public delegate void EnterAction(GameObject peg, GameObject disc);
		public static event EnterAction OnDiscEnterPeg;
		public delegate void ExitAction(GameObject peg, GameObject disc);
		public static event ExitAction OnDiscExitPeg;

		private const string TagPeg = "Peg";

		private void OnTriggerEnter2D(Collider2D collider)
		{
			if ((collider.tag == TagPeg) && (OnDiscEnterPeg != null))
			{
				GameObject peg = collider.gameObject;
				GameObject disc = this.transform.parent.gameObject;
				OnDiscEnterPeg(peg, disc);
			}
		}

		private void OnTriggerExit2D(Collider2D collider)
		{
			if ((collider.tag == TagPeg) && (OnDiscExitPeg != null))
			{
				GameObject peg = collider.gameObject;
				GameObject disc = this.transform.parent.gameObject;
				OnDiscExitPeg(peg, disc);
			}
		}
	}
}
