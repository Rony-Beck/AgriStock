using System.Windows.Media;

namespace AgriStockApp.Scripts
{
    class CustomLists
    {
        //Represente les données d'un mod
        public class Mod
        {
            public string Name { get; set; }
            public string Hash { get; set; }
            public string Author { get; set; }
            public string Version { get; set; }
            public string FileNameExt { get; set; }
            public string Path { get; set; }
            public ImageSource Image { get; set; }
            public bool IsInstalled { get; set; }
            public bool IsNew { get; set; }
            public bool IsUpdate { get; set; }
        }
    }
}
