using Exiled.API.Interfaces;
using System.ComponentModel;

namespace BoomBox
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = true;
        [Description("Айди предмета")]
        public static uint IdBoomBox { get; set; } = 7;
        public static string SchematicName { get; set; } = "BoomBox";

        public string AudioFolder { get; set; } = "BoomBoxAudios";
    }
}
