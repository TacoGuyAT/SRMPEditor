using SRMultiplayer.Models;
using System.Collections.Generic;

namespace SRMPEditor
{
    public class NetworkGadget
    {
        public static Dictionary<string, NetworkGadget> All = new Dictionary<string, NetworkGadget>();
        public Vector3 position;
        public float Rotation;
        public ushort Ident;
        public string ID;
        public NetworkDroneModel Drone;
        public NetworkEchoNetModel EchoNet;
        public NetworkExtractorModel Extractor;
        public NetworkSnareModel Snare;
        public NetworkWarpDepotModel WarpDepot;
        public NetworkGadget()
        {

        }
    }
}