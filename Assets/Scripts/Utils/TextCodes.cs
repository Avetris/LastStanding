public class TextCodes
{
    public static class Error
    {
        public static string Generic = "error_generic";
        public static class Lobby
        {
            public static string Create = "error_create_lobby";
            public static string Update = "error_update_lobby";
            public static string Find = "error_find_lobby";
            public static string FindCode = "error_find_lobby_code";
            public static string FindCodeNotFOund = "error_find_lobby_code_not_found";
            public static string Join = "error_join_lobby";
            public static string Destroy = "error_destroy_lobby";
            public static string Leave = "error_leaving_lobby";
            public static string RemoveAttribute = "error_remove_attribute_lobby";
            public static string UpdateAttribute = "error_update_attribute_lobby";
        }
    }

    public static string Creating = "creating_lobby";
    public static string Joining = "joining_to_lobby";
}
