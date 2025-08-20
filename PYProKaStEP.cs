using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
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
        IMyShipController refcockpit;
        IMyProgrammableBlock PYProKaStEP;
        List<IMyTerminalBlock> allblocks = new List<IMyTerminalBlock>();
        double _speedLimit = 104.38;
        int iteration = 1;
        Vector3D initialpos;
        double speedbegin = 90;
        bool begin = false;
        public Program()
        {
            // The constructor, called only once every session and
            // always before any other method is called. Use it to
            // initialize your script. 
            //     
            // The constructor is optional and can be removed if not
            // needed.
            // 
            // It's recommended to set Runtime.UpdateFrequency 
            // here, which will allow your script to run itself without a 
            // timer block.
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
            GridTerminalSystem.GetBlocks(allblocks);
            refcockpit = (IMyShipController)allblocks.Where(ckp => ckp is IMyRemoteControl).FirstOrDefault();
            PYProKaStEP = Me;
        }

        public void Save()
        {
            // Called when the program needs to save its state. Use
            // this method to save your state to the Storage field
            // or some other means. 
            // 
            // This method is optional and can be removed if not
            // needed.
        }

        public void Main(string argument, UpdateType updateSource)
        {
            double planeheight;
            refcockpit.TryGetPlanetElevation(MyPlanetElevation.Surface, out planeheight);
            if (refcockpit.GetShipSpeed() > speedbegin && !begin)
            {
                initialpos = refcockpit.GetPosition();
                begin = true;
                IMyShipMergeBlock release = (IMyShipMergeBlock)GridTerminalSystem.GetBlockWithName("RELEASE");
                release.Enabled = false;
            }
            else if(planeheight < 1)
            {
                begin = false;
                if (!string.IsNullOrWhiteSpace(PYProKaStEP.CustomData))
                {
                    PYProKaStEP.CustomData += "-\nEND\n-";
                    PYProKaStEP.Enabled = false;
                }
                
            }
            if (begin) 
            {
                Vector3D currentpost = refcockpit.GetPosition();

                Vector3D uxuy = refcockpit.GetShipVelocities().LinearVelocity;
                double utotal = refcockpit.GetShipSpeed();
                double uvertical = -Vector3D.Dot(uxuy, Vector3D.Normalize(refcockpit.GetNaturalGravity())); //Find DOWNWARDS SPEED based on gravity pull!
                double uhorizontal = (Math.Sqrt(uxuy.X * uxuy.X + uxuy.Y * uxuy.Y));

                // Transform into local grid (cockpit-relative)
                MatrixD cockpitMatrix = refcockpit.WorldMatrix;
                Vector3D localVel = Vector3D.TransformNormal(uxuy, MatrixD.Transpose(cockpitMatrix));   //Y Up and Down, Z Front Back, X Sideways

                //Timestamps
                double t_total;
                double t_uymax;
                t_uymax = (_speedLimit - uvertical) / Math.Abs(refcockpit.GetNaturalGravity().Length());

                double y1 = uvertical * t_uymax + 0.5 * refcockpit.GetNaturalGravity().Length() * t_uymax * t_uymax;

                if (y1 > planeheight)    //Bomb will fall before reaching max speed.
                {
                    t_total = (-uvertical + Math.Sqrt(uvertical * uvertical + 2 * refcockpit.GetNaturalGravity().Length() * planeheight)) / refcockpit.GetNaturalGravity().Length();
                }
                else
                {
                    double y2 = planeheight - y1;
                    double t_impact = y2 / _speedLimit;
                    t_total = t_uymax + t_impact;
                }
                if (iteration % 6 == 0 || iteration == 1) 
                {
                    double seaheight;
                    refcockpit.TryGetPlanetElevation(MyPlanetElevation.Sealevel, out seaheight);
                    PYProKaStEP.CustomData += "\nIt: " + iteration +
                        "\nVy: " + uvertical.ToString("N2") + " || Vx: " + uhorizontal.ToString("N2") + "\nT_uyMAX: " + t_uymax.ToString("N2") + " || T_total: " + t_total.ToString("N2") + "\nY1: " + y1 + " || Y2: " + (planeheight - y1).ToString("N2") +
                        " || Height: " + planeheight.ToString("N2") +" || S Height: "+ seaheight+ "\nD : " + Math.Sqrt((currentpost.X - initialpos.X)*(currentpost.X - initialpos.X) + (currentpost.Y - initialpos.Y) * (currentpost.Y - initialpos.Y)).ToString("N2") +
                        "\nGrav V L: " + refcockpit.GetNaturalGravity().Length().ToString("N2") + "\nGrav V D: " +
                        Math.Abs(refcockpit.GetNaturalGravity().Z).ToString("N2") + "\nT: " + (iteration / 6.00).ToString("N2") +
                        "\n-----";
                }
                iteration++;
            }
        }

    }
}
