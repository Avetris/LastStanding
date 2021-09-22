
public struct GameParameter
{
    public float[] Limits;
    public float Step;

    public GameParameter(float[] limit, float step)
    {
        Limits = limit;
        Step = step;
    }
}

public class Constants
{
    #region ScenesNames
    public static string AllScenes = "AllScenes";
    public static string MenuScene = "MenuScene";
    public static string LobbyScene = "LobbyScene";
    public static string GameScene = "GameScene";
    #endregion

    #region PlayerSettings
    public static float WalkSpeed = 200f;
    public static float RunSpeed = 500f;

    #endregion

    public static string LobbyName = "LobbyName";

    public static uint LobbySearch = 20;

    public static int LobbyCodeLength = 5;

    public static int MaxPlayers = 10;
    public static int MinPlayers = 0;

    public static int PlayerPushAngle = 30;
    public static int PlayerPushLateralAngle = 120;
}
