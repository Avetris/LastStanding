

public class LogManager
{

    private enum LogType { Debug, Info, Error };

    public static void Debug(string className, string methodName, string text)
    {
        Print(className, methodName, text, LogType.Debug);
    }

    public static void Info(string className, string methodName, string text)
    {
        Print(className, methodName, text, LogType.Info);
    }

    public static void Error(string className, string methodName, string text)
    {
        Print(className, methodName, text, LogType.Error);
    }

    private static void Print(string className, string methodName, string text, LogType type)
    {
        string message;
        if (methodName == null)
        {
            message = $"{className} --> {text}";
        }
        else
        {
            message = $"{className}::{methodName} --> {text}";
        }

        switch (type)
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