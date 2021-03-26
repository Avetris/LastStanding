using System;
using System.Collections.Generic;

public class EventManager
{
    #region SINGLETON
    private static EventManager _instance;
    public static EventManager singleton
    {
        get
        {
            if (_instance == null)
            {
                _instance = new EventManager();
            }
            return _instance;
        }
    }
    #endregion

    private static Dictionary<string, List<CustomEvent>> eventDictionary = new Dictionary<string, List<CustomEvent>>();

    public static Action<string> OnEventUpdated;

    public void CreateEvent<T>(string sceneName, EventType eventType, T eventValue)
    {
        List<CustomEvent> events;
        if(!eventDictionary.TryGetValue(sceneName, out events)){
            events = new List<CustomEvent>();
        }
        events.Add(new CustomEvent(eventType, typeof(T), eventValue.ToString()));

        eventDictionary.Add(sceneName, events);

        OnEventUpdated?.Invoke(sceneName);
    }

    public List<CustomEvent> GetEventOfType(string sceneName, EventType eventType, bool removeEvents = true)
    {
        List<CustomEvent> returnEvents = new List<CustomEvent>();
        List<CustomEvent> events = new List<CustomEvent>();
        if (eventDictionary.TryGetValue(sceneName, out events))
        {
            for(int i = 0; i < events.Count; i++)
            {
                if(events[i].GetEventType() == eventType || eventType == EventType.All)
                {
                    returnEvents.Add(events[i]);
                    if(removeEvents)
                    {
                        events.RemoveAt(i);
                        i--;
                    }
                }
            }
        }
        return returnEvents;
    }

}