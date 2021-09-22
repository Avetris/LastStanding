
public enum StateType { None, Moving, Falling, Listening };

public abstract class State
{
    public abstract void Init();

    public abstract void Update();

    public abstract StateType NextState();

    public abstract void Destroy();
}