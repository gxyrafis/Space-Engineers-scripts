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
        //Αυτοματοποιημένο Στόχαστρο Ρίψης Βομβών
        IMyMotorStator sightrotor;
        IMyCameraBlock sight;
        IMyShipController refcockpit;
        IMyProgrammableBlock AStRiV;
        List<IMyTerminalBlock> allblocks = new List<IMyTerminalBlock>();
        bool CCIP_flag;
        MyDetectedEntityInfo target;
        double _speedLimit = 104.38;
        double _timescale = 0.1;
        int _camerarotation = -1;
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
            sightrotor = (IMyMotorStator)allblocks.Where(rtr => rtr is IMyMotorStator && rtr.CustomName.Contains("SIGHT")).FirstOrDefault();
            sight = (IMyCameraBlock)allblocks.Where(cam => cam is IMyCameraBlock && cam.CustomName.Contains("SIGHT")).FirstOrDefault();
            refcockpit = (IMyShipController)allblocks.Where(ckp => ckp is IMyShipController).OrderBy(ckp => ckp.CustomName.Contains("REF")).FirstOrDefault();
            AStRiV = Me;
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
            if (!IntegrityCheck())
            {
                return;
            }

            //BASIC BOMBER SIGHT IMPLEMENTATION
            double planeheight;
            refcockpit.TryGetPlanetElevation(MyPlanetElevation.Surface ,out planeheight);

            Vector3D uxuy = refcockpit.GetShipVelocities().LinearVelocity;
            double utotal = refcockpit.GetShipSpeed();

            // Transform into local grid (cockpit-relative)
            MatrixD cockpitMatrix = refcockpit.WorldMatrix;
            Vector3D localVel = Vector3D.TransformNormal(uxuy, MatrixD.Transpose(cockpitMatrix));   //Y Up and Down, Z Front Back, X Sideways

            //Timestamps
            double t_total;
            double t_uymax;
            double localVelYSigned = -localVel.Y;
            t_uymax = (_speedLimit - localVelYSigned) / Math.Abs(refcockpit.GetNaturalGravity().Length());

            double y1 = localVelYSigned * t_uymax + 0.5*refcockpit.GetNaturalGravity().Length() *t_uymax*t_uymax;

            if(y1 > planeheight)    //Bomb will fall before reaching max speed.
            {
                t_total = (-localVelYSigned + Math.Sqrt(localVelYSigned * localVelYSigned + 2 * refcockpit.GetNaturalGravity().Length() * planeheight)) / refcockpit.GetNaturalGravity().Length();
                Echo(t_total.ToString());
            }

            Echo("T_uymax: " + t_uymax + " \nY1: " + y1 + " |\n g: " + refcockpit.GetNaturalGravity().Length());


        }

        public bool IntegrityCheck()
        {
            bool check = true;
            if (sightrotor == null || !sightrotor.IsFunctional)
            {
                check = false;
                Echo("Error: Rotor with 'SIGHT' in its name either doesn't exist or is not operational.\n");
            }
            if (sight == null || !sight.IsFunctional)
            {
                check = false;
                Echo("Error: Camera with 'SIGHT' in its name either doesn't exist or is not operational.\n");
            }

            return check;
        }
    }
}




