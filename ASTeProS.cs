using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Sandbox.Game.AI.Commands;
using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    public partial class Program : MyGridProgram
    {
        // This file contains your actual script.
        //
        // You can either keep all your code here, or you can create separate
        // code files to make your program easier to navigate while coding.
        //
        // Go to:
        // https://github.com/malware-dev/MDK-SE/wiki/Quick-Introduction-to-Space-Engineers-Ingame-Scripts
        //
        // to learn more about ingame scripts.

        //Αυτόματο Σύστημα Τηλέμετρου και Προσαρμογής Σκοπευτικού ASTeProS
        List<IMyCameraBlock> rangefinder = null;
        IMyProjector sight = null;
        MyDetectedEntityInfo target;
        List<IMyCameraBlock> cameras = new List<IMyCameraBlock>();
        List<IMyProjector> projectors = new List<IMyProjector>();
        double range = 0;
        IMyProgrammableBlock ASTeProS;
        int x;
        int y;
        int z;
        string[] xyz = { "2", "X", "0" };
        Dictionary<int, int> distances = new Dictionary<int, int> {
                { 550,50}, {650,43 }, {750,37 }, {850, 33 }, {950,29 }, {1050, 28 }, {1150, 25 }, {1250,23 }, {1350, 21 }, {1450,20 }, {1550,19 }, {1650, 18 }, {1750, 17 }, {2000, 16}
            };

        public Program()
        {
            GridTerminalSystem.GetBlocksOfType(cameras);
            GridTerminalSystem.GetBlocksOfType(projectors);
            ASTeProS = Me;
            if (cameras.Where(x => x.CustomName.ToLower().Contains("rangefinder")).ToList().Count > 0)
            {
                rangefinder = cameras.Where(x => x.CustomName.ToLower().Contains("rangefinder")).ToList();
                Echo("Success!: Rangefinder found!\n");
            }
            if (projectors.Where(x => x.CustomName.ToLower().Contains("sight")).ToList().Count > 0)
            {
                sight = projectors.Where(x => x.CustomName.ToLower().Contains("sight")).FirstOrDefault();
                Echo("Success!: Sight found!\nMake sure to load the sight projector with the appropriate blueprint for aiming!");
            }
            Me.CustomData = !string.IsNullOrWhiteSpace(Storage) ? Storage : "Insert the Projector values below so that the sight is right infront of your camera. Mark with 'X' the axis that is used to move the sight to and from the camera.\n" +
                "X: 2\n" +
                "Y: X\n" +
                "Z: 0";
        }

        public void Save()
        {
            Storage = ASTeProS.CustomData;
        }

        public void Main(string argument, UpdateType updateSource)
        {
            

            if (!string.IsNullOrWhiteSpace(argument))
            {
                Setup(argument);
            }
            else
            {
                AdjustSight();
            }
            return;
        }

        public void Setup(string argument)
        {
            xyz = argument.Split(',');
            if (xyz.Count() != 3)
            {
                Echo("Warning: Input Argument is invalid or Empty.\nReturning to default values.");
                xyz[0] = "2";
                xyz[1] = "X";
                xyz[2] = "0";
            }
            else
            {
                if ((isnumericchar(xyz[0]) && isnumericchar(xyz[1])) || (isnumericchar(xyz[2]) && isnumericchar(xyz[1])) || (isnumericchar(xyz[2]) && isnumericchar(xyz[0])))
                {
                    ASTeProS.CustomData = "Insert the Projector values below so that the sight is right infront of your camera. Mark with 'X' the axis that is used to move the sight to and from the camera.\n" +
                "X: " + xyz[0] + "\n" +
                "Y: " + xyz[1] + "\n" +
                "Z: " + xyz[2];
                }
            }
        }
        
        public void AdjustSight() 
        {
            string[] cd = ASTeProS.CustomData.Split('\n');
            xyz[0] = cd[1].Split(':')[1].Trim();
            xyz[1] = cd[2].Split(':')[1].Trim();
            xyz[2] = cd[3].Split(':')[1].Trim();
            Echo("X : " + xyz[0] + "\nY : " + xyz[1] + "\nZ : " + xyz[2]);
            if (rangefinder == null)
            {
                Echo("Error: Unable to find rangefinder.\n");
                return;
            }
            if (sight == null)
            {
                Echo("Error: Unable to find projector sight.\n");
                return;
            }
            IMyCameraBlock rf;
            rf = rangefinder.Where(w => w.IsFunctional).FirstOrDefault();

            rf.EnableRaycast = true;

            target = rf.Raycast(2000);
            if (target.IsEmpty())
            {
                return;
            }
            range = Math.Round(Vector3D.Distance((Vector3D)target.HitPosition, rf.GetPosition()));
            if (range <= 550)
            {
                x = isnumericchar(xyz[0]) ? int.Parse(xyz[0]) : distances[550];
                y = isnumericchar(xyz[1]) ? int.Parse(xyz[1]) : distances[550];
                z = isnumericchar(xyz[2]) ? int.Parse(xyz[2]) : distances[550];
                sight.ProjectionOffset = new Vector3I(x, y, z);
            }
            else if (range <= 650)
            {
                x = isnumericchar(xyz[0]) ? int.Parse(xyz[0]) : distances[650];
                y = isnumericchar(xyz[1]) ? int.Parse(xyz[1]) : distances[650];
                z = isnumericchar(xyz[2]) ? int.Parse(xyz[2]) : distances[650];
                sight.ProjectionOffset = new Vector3I(x, y, z);
            }
            else if (range <= 750)
            {
                x = isnumericchar(xyz[0]) ? int.Parse(xyz[0]) : distances[750];
                y = isnumericchar(xyz[1]) ? int.Parse(xyz[1]) : distances[750];
                z = isnumericchar(xyz[2]) ? int.Parse(xyz[2]) : distances[750];
                sight.ProjectionOffset = new Vector3I(x, y, z);
            }
            else if (range <= 850)
            {
                x = isnumericchar(xyz[0]) ? int.Parse(xyz[0]) : distances[850];
                y = isnumericchar(xyz[1]) ? int.Parse(xyz[1]) : distances[850];
                z = isnumericchar(xyz[2]) ? int.Parse(xyz[2]) : distances[850];
                sight.ProjectionOffset = new Vector3I(x, y, z);
            }
            else if (range <= 950)
            {
                x = isnumericchar(xyz[0]) ? int.Parse(xyz[0]) : distances[950];
                y = isnumericchar(xyz[1]) ? int.Parse(xyz[1]) : distances[950];
                z = isnumericchar(xyz[2]) ? int.Parse(xyz[2]) : distances[950];
                sight.ProjectionOffset = new Vector3I(x, y, z);
            }
            else if (range <= 1050)
            {
                x = isnumericchar(xyz[0]) ? int.Parse(xyz[0]) : distances[1050];
                y = isnumericchar(xyz[1]) ? int.Parse(xyz[1]) : distances[1050];
                z = isnumericchar(xyz[2]) ? int.Parse(xyz[2]) : distances[1050];
                sight.ProjectionOffset = new Vector3I(x, y, z);
            }
            else if (range <= 1150)
            {
                x = isnumericchar(xyz[0]) ? int.Parse(xyz[0]) : distances[1150];
                y = isnumericchar(xyz[1]) ? int.Parse(xyz[1]) : distances[1150];
                z = isnumericchar(xyz[2]) ? int.Parse(xyz[2]) : distances[1150];
                sight.ProjectionOffset = new Vector3I(x, y, z);
            }
            else if (range <= 1250)
            {
                x = isnumericchar(xyz[0]) ? int.Parse(xyz[0]) : distances[1250];
                y = isnumericchar(xyz[1]) ? int.Parse(xyz[1]) : distances[1250];
                z = isnumericchar(xyz[2]) ? int.Parse(xyz[2]) : distances[1250];
                sight.ProjectionOffset = new Vector3I(x, y, z);
            }
            else if (range <= 1350)
            {
                x = isnumericchar(xyz[0]) ? int.Parse(xyz[0]) : distances[1350];
                y = isnumericchar(xyz[1]) ? int.Parse(xyz[1]) : distances[1350];
                z = isnumericchar(xyz[2]) ? int.Parse(xyz[2]) : distances[1350];
                sight.ProjectionOffset = new Vector3I(x, y, z);
            }
            else if (range <= 1450)
            {
                x = isnumericchar(xyz[0]) ? int.Parse(xyz[0]) : distances[1450];
                y = isnumericchar(xyz[1]) ? int.Parse(xyz[1]) : distances[1450];
                z = isnumericchar(xyz[2]) ? int.Parse(xyz[2]) : distances[1450];
                sight.ProjectionOffset = new Vector3I(x, y, z);
            }
            else if (range <= 1550)
            {
                x = isnumericchar(xyz[0]) ? int.Parse(xyz[0]) : distances[1550];
                y = isnumericchar(xyz[1]) ? int.Parse(xyz[1]) : distances[1550];
                z = isnumericchar(xyz[2]) ? int.Parse(xyz[2]) : distances[1550];
                sight.ProjectionOffset = new Vector3I(x, y, z);
            }
            else if (range <= 1650)
            {
                x = isnumericchar(xyz[0]) ? int.Parse(xyz[0]) : distances[1650];
                y = isnumericchar(xyz[1]) ? int.Parse(xyz[1]) : distances[1650];
                z = isnumericchar(xyz[2]) ? int.Parse(xyz[2]) : distances[1650];
                sight.ProjectionOffset = new Vector3I(x, y, z);
            }
            else if (range <= 1750)
            {
                x = isnumericchar(xyz[0]) ? int.Parse(xyz[0]) : distances[1750];
                y = isnumericchar(xyz[1]) ? int.Parse(xyz[1]) : distances[1750];
                z = isnumericchar(xyz[2]) ? int.Parse(xyz[2]) : distances[1750];
                sight.ProjectionOffset = new Vector3I(x, y, z);
            }
            else if (range <= 2000)
            {
                x = isnumericchar(xyz[0]) ? int.Parse(xyz[0]) : distances[2000];
                y = isnumericchar(xyz[1]) ? int.Parse(xyz[1]) : distances[2000];
                z = isnumericchar(xyz[2]) ? int.Parse(xyz[2]) : distances[2000];
                sight.ProjectionOffset = new Vector3I(x, y, z);
            }
            sight.UpdateOffsetAndRotation();
            Echo(range.ToString());
        }

        public bool isnumericchar(string s)
        {
            bool result = true;
            for(int i = 0; i < s.Length; i++)
            {
                if (!char.IsDigit(s[i]))
                {
                    result = false;
                    break;
                }
            }
            return result;
        }
    }
}






