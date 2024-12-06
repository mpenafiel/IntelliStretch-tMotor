using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace IntelliStretch.Games
{
    public enum GameCategory
    {
        Flash = 0,
        TV3D = 1,
        WPF = 2,
        External = 3
    }

    public enum ControlMode
    {
        Position = 0,
        Velocity = 1,
        Torque = 2
    }

    public enum GameMode
    {
        AssistAndResist = 0,
        Assistive = 1,
        Resistive = 2,
        AssistOrResist = 3
    }

    public enum Direction
    {
        X = 1,
        Y = 2,
        XY = 3,
        Z = 4,
        XZ = 5,
        YZ = 6,
        XYZ = 7
    }

    public class GameInfo
    {
        // Constructors
        public GameInfo() { }

        // Properties
        public string Name { get; set; }
        public string ClassName { get; set; }
        public GameCategory Category { get; set; }
        public string Preview { get; set; }
        public string Source { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Direction ControlDirection { get; set; }
        public GameMode GameMode { get; set; }
        public ControlMode ControlMode { get; set; }
        public string Description { get; set; }
        public bool IsInUse { get; set; }
    }

}
