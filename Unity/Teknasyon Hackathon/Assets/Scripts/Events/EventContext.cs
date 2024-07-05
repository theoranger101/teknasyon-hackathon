using UnityEngine;

namespace Events
{
    [CreateAssetMenu(menuName = "Event Context", fileName = "New Event Context")]
    public class EventContext : ScriptableObject
    {
        public void SendEvent()
        {
            EventExtensions.SendEvent(this);
        }
    }
}