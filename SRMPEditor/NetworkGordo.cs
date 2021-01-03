using SRMultiplayer.Models;
using System.Collections.Generic;

namespace SRMPEditor
{
    public class NetworkGordo
    {
        public static Dictionary<string, NetworkGordo> All = new Dictionary<string, NetworkGordo>();
        public Vector3 position;
        public string ID;
        public NetworkGordoModel Model;
        public NetworkGordo()
        {

        }
    }
}