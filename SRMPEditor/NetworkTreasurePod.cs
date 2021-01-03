using SRMultiplayer.Models;
using System.Collections.Generic;

namespace SRMPEditor
{
    internal class NetworkTreasurePod
    {
        public static Dictionary<string, NetworkTreasurePod> All = new Dictionary<string, NetworkTreasurePod>();
        internal Vector3 position;

        public NetworkTreasurePod()
        {
        }

        public string ID { get; internal set; }
        public NetworkTreasurePodModel Model { get; internal set; }
    }
}