using System.Collections.Generic;
using UnityEngine;

namespace TocaBoca
{
	public class Peg : MonoBehaviour
	{
		private Stack<GameObject> discObjects = new Stack<GameObject>();

		public bool IsEmpty()
		{
			return (discObjects.Count == 0);
		}

		public GameObject GetTopmostDisc()
		{
			GameObject topmostDisc = null;

			if (discObjects.Count > 0)
			{
				topmostDisc = discObjects.Peek();
			}

			return topmostDisc;
		}

		public void AddDisc(GameObject disc)
		{
			discObjects.Push(disc);
		}

		public void RemoveDisc(GameObject disc)
		{
			if ((discObjects.Count > 0) && (discObjects.Peek() == disc))
			{
				discObjects.Pop();
			}
		}

		public void LockAllDiscs()
		{
			foreach (GameObject gameObject in discObjects)
			{
				Disc disc = gameObject.GetComponent<Disc>();
				disc.Lock();
			}
		}

		public void UnlockTopmostDisc()
		{
			if (discObjects.Count > 0)
			{
				GameObject topmostDisc = discObjects.Peek();
				Disc disc = topmostDisc.GetComponent<Disc>();
				disc.Unlock();
			}
		}
	}
}
