using UnityEngine;

namespace TocaBoca
{
	public class GameManager : MonoBehaviour
	{
		public static GameManager instance;
		public GameObject[] pegs;

		private const string TagDiscTouchArea = "DiscTouchArea";
		private const float DiscWithinPegDistanceTolerance = 0.1f;
		private const float PegEntranceHeight = 1.62f;

		private Move currentMove;

		public GameManager()
		{
			instance = null;
			pegs = null;
			currentMove.Clear();
		}

		private void Awake()
		{
			if (instance == null)
			{
				instance = this;
			}
			else if (instance != this)
			{
				Destroy(gameObject);
			}

			DontDestroyOnLoad(gameObject);
			InitializeGame();
		}

		private void Update()
		{
			if (Input.GetMouseButtonDown(0))
			{
				OnTouchBegin();
			}
			else if (Input.GetMouseButtonUp(0))
			{
				OnTouchEnd();
			}
		}

		private void FixedUpdate()
		{
			if (currentMove.isHolding)
			{
				Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				currentMove.disc.UpdateTargetPosition(mousePosition);
			}
		}

		private void OnDestroy()
		{
			DecomissionGame();
		}

		private void InitializeGame()
		{
			DiscTrigger.OnDiscEnterPeg += OnDiscEnterPeg;
			DiscTrigger.OnDiscExitPeg += OnDiscExitPeg;
			Disc.OnDefenderIntruderCollide += OnIntruderDefenderCollide;
		}

		private void OnTouchBegin()
		{
			Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);
			bool isTouchingDisc = ((hit.collider != null) && (hit.collider.tag == TagDiscTouchArea));

			if (isTouchingDisc)
			{
				GameObject discObject = hit.collider.transform.parent.gameObject;
				Disc disc = discObject.GetComponent<Disc>();

				bool isNoMoveInProgress = (currentMove.discObject == null);
				bool isDiscUnoccupied = disc.IsUnoccupied();
				bool isDiscOutsidePeg = ((currentMove.discObject == discObject) && currentMove.isOutsidePeg);
				bool isDiscAllowedToMove = (isNoMoveInProgress && isDiscUnoccupied || isDiscOutsidePeg);

				if (isDiscAllowedToMove)
				{
					currentMove.discObject = discObject;
					currentMove.disc = disc;
					currentMove.isHolding = true;

					disc.Follow();
				}
				else
				{
					disc.Shake();
				}
			}
		}

		private void OnTouchEnd()
		{
			if (currentMove.isHolding)
			{
				GameObject closestPeg = FindClosestPeg(currentMove.discObject.transform.position);

				if (closestPeg != null)
				{
					Vector2 activeDiscPosition = currentMove.discObject.transform.position;
					Vector2 closestPegPosition = closestPeg.transform.position;
					bool isDiscOutsideClosestPeg = ((activeDiscPosition.x - Mathf.Abs(closestPegPosition.x) > DiscWithinPegDistanceTolerance) || currentMove.isOutsidePeg);

					if (isDiscOutsideClosestPeg)
					{
						Vector2 targetPosition = new Vector2(closestPegPosition.x, PegEntranceHeight);
						currentMove.disc.Snap(targetPosition);
					}
					else
					{
						currentMove.disc.Release();
						currentMove.Clear();
					}
				}

				currentMove.isHolding = false;
			}
		}

		private void OnDiscEnterPeg(GameObject pegObect, GameObject enteredDiscObject)
		{
			Peg peg = pegObect.GetComponent<Peg>();

			if (IsDiscAllowedToEnterPeg(enteredDiscObject, pegObect))
			{
				peg.AddDisc(enteredDiscObject);
				peg.LockAllDiscs();
				peg.UnlockTopmostDisc();

				if (currentMove.discObject == enteredDiscObject)
				{
					currentMove.isOutsidePeg = false;
					currentMove.isIntruder = false;

					if (!currentMove.isHolding)
					{
						currentMove.Clear();
					}
				}
			}
			else
			{
				ReboundDisallowedDisc(enteredDiscObject, pegObect);
			}
		}

		private bool IsDiscAllowedToEnterPeg(GameObject discObject, GameObject pegObject)
		{
			bool isAllowed = true;
			Peg peg = pegObject.GetComponent<Peg>();

			if (!peg.IsEmpty())
			{
				GameObject pegsTopmostDiscObject = peg.GetTopmostDisc();
				Disc disc = discObject.GetComponent<Disc>();
				Disc pegsTopmostDisc = pegsTopmostDiscObject.GetComponent<Disc>();
				isAllowed = (disc.index < pegsTopmostDisc.index);
			}

			return isAllowed;
		}

		private void ReboundDisallowedDisc(GameObject discObject, GameObject pegObject)
		{
			Peg peg = pegObject.GetComponent<Peg>();
			GameObject pegsTopmostDiscObject = peg.GetTopmostDisc();
			Disc disc = discObject.GetComponent<Disc>();
			Disc pegsTopmostDisc = pegsTopmostDiscObject.GetComponent<Disc>();
			disc.Intrude();

			if (currentMove.discObject == discObject)
			{
				currentMove.isIntruder = true;
				currentMove.isHolding = false;
				disc.Release();
			}

			pegsTopmostDisc.Defend();
		}

		private void OnDiscExitPeg(GameObject pegObject, GameObject exitedDiscObject)
		{
			Peg peg = pegObject.GetComponent<Peg>();
			peg.RemoveDisc(exitedDiscObject);
			peg.UnlockTopmostDisc();

			if (currentMove.discObject == exitedDiscObject)
			{
				currentMove.isOutsidePeg = true;

				if (currentMove.isIntruder)
				{
					currentMove.isIntruder = false;
					Vector2 targetPosition = new Vector2(currentMove.originPeg.transform.position.x, PegEntranceHeight);
					currentMove.disc.Snap(targetPosition);
					currentMove.disc.Neutralize();
				}
				else
				{
					currentMove.originPeg = pegObject;
				}
			}
			else
			{
				Disc exitedDisc = exitedDiscObject.GetComponent<Disc>();
				Vector2 targetPosition = new Vector2(pegObject.transform.position.x, PegEntranceHeight);
				exitedDisc.Snap(targetPosition);
			}
		}

		private void OnIntruderDefenderCollide(GameObject defenderObject, GameObject intruderObject)
		{
			Disc defender = defenderObject.GetComponent<Disc>();
			Disc intruder = intruderObject.GetComponent<Disc>();
			intruder.Jump();
			defender.Neutralize();
			defender.Release();
		}

		private GameObject FindClosestPeg(Vector2 position)
		{
			GameObject closestPegObject = null;
			float closestPegDistance = 0.0f;

			foreach (GameObject pegObject in pegs)
			{
				float distance = Mathf.Abs(pegObject.transform.position.x - position.x);

				if ((closestPegObject == null) || (distance < closestPegDistance))
				{
					closestPegObject = pegObject;
					closestPegDistance = distance;
				}
			}

			return closestPegObject;
		}

		private void DecomissionGame()
		{
			DiscTrigger.OnDiscExitPeg -= OnDiscExitPeg;
			DiscTrigger.OnDiscExitPeg -= OnDiscExitPeg;
			Disc.OnDefenderIntruderCollide -= OnIntruderDefenderCollide;
		}
	}
}
