using System.Collections.Generic;
using SRMultiplayer.Models;

namespace SRMPEditor
{
    public class NetworkSwitch
    {
        public static Dictionary<string, NetworkSwitch> All = new Dictionary<string, NetworkSwitch>();
        public Vector3 position;
        public string ID;
        public NetworkMasterSwitchModel Model;
        public NetworkSwitch()
        {

        }
    }
}