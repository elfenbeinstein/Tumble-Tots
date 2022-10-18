using System.Collections;
using System.Collections.Generic;

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

        System.Diagnostics.Debug.WriteLine(string.Format("Event {0} fired: {1}, {2}", eventID, eventName, param));
    }
}
