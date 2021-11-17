using UnityEngine;

namespace TocaBoca
{
	public struct Move
	{
		private const string TagDisc = "Disc";

		public Disc disc { get; set; }
		public GameObject discObject { get; set; }
		public GameObject originPeg { get; set; }
		public bool isHolding { get; set; }
		public bool isOutsidePeg { get; set; }
		public bool isIntruder { get; set; }

		public void Clear()
		{
			disc = null;
			discObject = null;
			originPeg = null;
			isHolding = false;
			isOutsidePeg = false;
			isIntruder = false;
		}
	}
}
