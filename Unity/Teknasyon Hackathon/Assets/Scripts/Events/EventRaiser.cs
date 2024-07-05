using System.Collections;
using UnityEngine;

namespace Events
{
	public class EventRaiser : MonoBehaviour
	{

		public EventContext eventContext;

		public float delay;

		public void Raise()
		{
			if (delay < Mathf.Epsilon)
			{
				RaiseNow();
			}
			else
			{
				StartCoroutine(WaitAndRaise());
			}
		}
		
		public void RaiseNow()
		{
			eventContext.SendEvent();
		}


		private IEnumerator WaitAndRaise()
		{
			yield return new WaitForSecondsRealtime(delay);

			RaiseNow();
		}
	}
}
