using Newtonsoft.Json;

namespace DogeHelper.Entities
{
    public class App
    {
        public string Version { get; set; }
        public string VersionCode { get; set; }
        public string Link { get; set; }
        public string Note { get; set; }
    }

    public class Uninstaller
    {
        public string Link { get; set; }
    }

    public class Snet
    {
        public string VersionCode { get; set; }
        public string Link { get; set; }
    }

    public class Magisk
    {
        public string Version { get; set; }
        public string VersionCode { get; set; }
        public string Link { get; set; }
        public string Note { get; set; }
        public string Md5 { get; set; }
    }

    public class MagiskUpdateJson
    {
        public App App { get; set; }
        public Uninstaller Uninstaller { get; set; }
        public Snet Snet { get; set; }
        public Magisk Magisk { get; set; }
    }
}
