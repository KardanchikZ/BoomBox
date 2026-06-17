using Exiled.API.Interfaces;
using System;
using System.Collections.Generic;

namespace BoomBox
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = true;
        public static uint IdBoomBox { get; set; } = 1;

        public static List<String> Musics { get; set; } = null;
    }
}
