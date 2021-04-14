using System.Security.Permissions;

public class Enumerators
{
    public enum DialogType { None, Settings, RoomSettings, Customize }

    public enum CustomizeItem { None, Color, Head, Body, Legs, Shoes }

    public enum RotationType { None, Right, Left }

    public enum GameSetting { MaxPlayers, MinPlayers, FullBots, ArrowSpanwNumber, ArrowTimeSpawn, ArrowCircleTime, ArrowCircleRadius, DeathCollisions};

    public enum ArrowType {Normal, Fire, Circle}

}