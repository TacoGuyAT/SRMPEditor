using SRMultiplayer.Models;
using System.Collections.Generic;

namespace SRMPEditor
{
    public class NetworkSpawnResource
    {
        public static Dictionary<Vector3, NetworkSpawnResource> All = new Dictionary<Vector3, NetworkSpawnResource>();
        public Vector3 position;
        public NetworkSpawnResourceModel Model;
        public NetworkSpawnResource()
        {

        }
    }
}