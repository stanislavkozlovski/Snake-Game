﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeGame
{
    public enum Direction { Up, Down, Left, Right };
    class Settings
    {
        public static ushort Width { get; set; }
        public static ushort Height { get; set; }
        public static ushort GameSpeed { get; set; }
        public static int Score { get; set; }
        public static ushort Points { get; set; }
        public static bool GameOver { get; set; }
        public static Direction Direction { get; set; }

        public Settings()
        {
            Width = 16;
            Height = 16;
            GameSpeed = 16;
            Score = 0;
            Points = 100;
            GameOver = false;
            Direction = Direction.Right;
        }
    }
}