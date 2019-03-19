using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class GameEventListener : MonoBehaviour
{
    public GameEvent Event = null;
    public UnityEvent response = null;

    public void OnEnable()
    {
        Event.RegisterListeners(this);
    }

    public void OnDisable()
    {
        Event.UnregisterListener(this);
    }

    public void OnEventRaised()
    {
        response.Invoke();
    }

}
