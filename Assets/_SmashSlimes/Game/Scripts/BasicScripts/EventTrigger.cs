using UnityEngine;

namespace SmashSlimes
{
    public class EventTrigger : MonoBehaviour
    {
        [SerializeField] private StateEvent _stateEvent = StateEvent.Complete;

        public void OnClick()
        {
            EventManager.Send(_stateEvent);
        }
    }
}