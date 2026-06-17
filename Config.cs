using Exiled.API.Features;
using Exiled.API.Interfaces;
using System.ComponentModel;
using System.IO;

namespace BoomBox
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = true;
        [Description("Айди предмета")]
        public static uint IdBoomBox { get; set; } = 7;
        [Description("Название схемы")]
        public static string SchematicName { get; set; } = "BoomBox";
        [Description("Путь к папке с аудио")]
        public string AudioFolder { get; set; } = "BoomBoxAudios";
    }
}
