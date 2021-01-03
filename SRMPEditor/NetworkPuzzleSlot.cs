using SRMultiplayer.Models;
using System.Collections.Generic;

namespace SRMPEditor
{
    public class NetworkPuzzleSlot
    {
        public static Dictionary<string, NetworkPuzzleSlot> All = new Dictionary<string, NetworkPuzzleSlot>();
        public Vector3 position;
        public string ID;
        public NetworkPuzzleSlotModel Model;
        public NetworkPuzzleSlot()
        {

        }
    }
}