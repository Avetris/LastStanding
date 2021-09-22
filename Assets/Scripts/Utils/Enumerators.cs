using System.Security.Permissions;

public class Enumerators
{
    public enum DialogType { None, Settings, RoomSettings, Customize }

    public enum CustomizeItem { None, Color, Head, Body, Legs, Shoes }

    public enum RotationType { None, Right, Left }

    public enum GameSetting { 
        Host_Address,
        Lobby_Id, 
        Max_Players, 
        Min_Players, 
        Hide_Lobby_Code,
        Arrow_Spawn_Number, 
        Arrow_Spawn_Interval, 
        Arrow_Circle_Spawn_Interval, 
        Arrow_Circle_Close_Radius, 
        Death_Collisions };

    public enum ArrowType { Normal, Fire, Circle }
}