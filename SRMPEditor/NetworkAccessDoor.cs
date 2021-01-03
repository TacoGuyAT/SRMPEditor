using SRMultiplayer.Models;
using System.Collections.Generic;

namespace SRMPEditor
{
    public class NetworkAccessDoor
    {
        public static Dictionary<string, NetworkAccessDoor> All = new Dictionary<string, NetworkAccessDoor>();
        public Vector3 position;
        public string ID;
        public NetworkAccessDoorModel Model;
        public NetworkAccessDoor()
        {

        }

    }
}