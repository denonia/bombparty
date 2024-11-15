﻿using BombParty.Common;

namespace BombParty
{
    public struct Settings
    {
        public string ServerAddress { get; set; }
        public bool UseDarkTheme { get; set; }
        public PlayerSettings PlayerSettings { get; set; }
    }
}
