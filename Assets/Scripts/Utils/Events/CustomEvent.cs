
using System;

public enum EventType{All, Message};

public class CustomEvent {
    private EventType eventType;
    private string value;
    private Type valueType;

    public CustomEvent(EventType newEventType, Type newValueType, string newValue)
    {
        eventType = newEventType;
        value = newValue;
        valueType = newValueType;
    }

    public EventType GetEventType()
    {
        return eventType;
    }

    public valueType GetValue<valueType>()
    {
       return (valueType) Convert.ChangeType(value, typeof(valueType));
    }
}