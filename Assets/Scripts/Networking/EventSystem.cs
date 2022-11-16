using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Event System is used for audio implementation (see audio scripts)
/// obsolete version also had the lava use event system when player fell in lava --> Xavi changed it to work with the networking and we're not using it like that anymore
/// </summary>

public class EventSystem
{
    private static EventSystem instance;
    public static EventSystem Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new EventSystem();
            }
            return instance;
        }
    }

    public delegate void EventListener(string eventName, object param = null);
    private Dictionary<string, List<EventListener>> eventListener;

    EventSystem()
    {
        eventListener = new Dictionary<string, List<EventListener>>();
    }

    public void AddEventListener(string eventID, EventListener listener)
    {
        if (!eventListener.ContainsKey(eventID))
        {
            eventListener.Add(eventID, new List<EventListener>());
        }
        eventListener[eventID].Add(listener);
    }

    public void RemoveEventListener(string eventID, EventListener listener)
    {
        if (eventListener.ContainsKey(eventID))
        {
            eventListener[eventID].Remove(listener);
        }
    }


    public void Fire(string eventID, string eventName, object param = null)
    {
        if (eventListener.ContainsKey(eventID))
            for (int i = eventListener[eventID].Count - 1; i >= 0; i--)
                eventListener[eventID][i](eventName, param);
    }
}
