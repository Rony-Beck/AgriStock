namespace AgriStockApp.Scripts
{
    public static class FS_Api
    {
        //Server Stats JSon
        public static string Stats(string FS_Host, string FS_Key)
        {
            return FS_Host + "/feed/dedicated-server-stats.json?code=" + FS_Key;
        }

        //Server Map PNG
        public static string Map(string FS_Host, string FS_Key)
        {
            return FS_Host + "/feed/dedicated-server-stats-map.jpg?code=" + FS_Key + "&quality=100&size=2048";
        }

        //Career Savegame XML
        public static string Career(string FS_Host, string FS_Key)
        {
            return FS_Host + "/feed/dedicated-server-savegame.html?code=" + FS_Key + "&file=careerSavegame";
        }

        //Economy Savegame XML
        public static string Economy(string FS_Host, string FS_Key)
        {
            return FS_Host + "/feed/dedicated-server-savegame.html?code=" + FS_Key + "&file=economy";
        }

        //Vehicles Savegame XML
        public static string Vehicles(string FS_Host, string FS_Key)
        {
            return FS_Host + "/feed/dedicated-server-savegame.html?code=" + FS_Key + "&file=vehicles";
        }
    }
}
