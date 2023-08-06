using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Energistics.Enemies
{
	[DisallowMultipleComponent]
	public class BotEyes : MonoBehaviour
	{
		[SerializeField] float viewRadius;
		[SerializeField] [Range(0, 360)] public float viewAngle;
		[SerializeField] LayerMask targetMask;
		[SerializeField] LayerMask obstacleMask;

		[SerializeField] List<Transform> visibleTargets = new List<Transform>();
		public List<Transform> VisibleTargets { get { return visibleTargets; } }

		void Start()
		{
			StartCoroutine("FindTargetsWithDelay", 0.25f);
		}

		// This is the Coroutine that calls the algorithm - started in start method - you can make changes here and in the start method
		IEnumerator FindTargetsWithDelay(float delay)
		{
			while (true)
			{
				yield return new WaitForSeconds(delay);
				FindVisibleTargets();
			}
		}

		//this is the algorithm that controls detecting visibility
		void FindVisibleTargets()
		{
			visibleTargets.Clear();
			Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

			for (int i = 0; i < targetsInViewRadius.Length; i++)
			{
				Transform target = targetsInViewRadius[i].transform;
				Vector3 directionToTarget = (target.position - transform.position).normalized;
				if (Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2)
				{
					float distanceToTarget = Vector3.Distance(transform.position, target.position);

					if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask))
					{
						visibleTargets.Add(target.root);
					}
				}
			}
		}


		//this method is simply for the editor's graphics, chances are you won't need to change it for functionality
		public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
		{
			if (!angleIsGlobal)
			{
				angleInDegrees += transform.eulerAngles.y;
			}
			return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
		}
	}
}