

public class LogManager{

    private enum LogType {Debug, Info, Error};

    public static void Debug(string className, string text)
    {
        Print(className, text, LogType.Debug);
    }

    public static void Info(string className, string text)
    {
        Print(className, text, LogType.Info);
    }

    public static void Error(string className, string text)
    {
        Print(className, text, LogType.Error);
    }

    private static void Print(string className, string text, LogType type)
    {
        string message = $"{className} -->  {text}";

        switch(type)
        {
            case LogType.Debug: 
                UnityEngine.Debug.Log(message); 
                break;
            case LogType.Info: 
                UnityEngine.Debug.Log(message); 
                break;
            case LogType.Error: 
                UnityEngine.Debug.LogError(message); 
                break;
        }
    }
}