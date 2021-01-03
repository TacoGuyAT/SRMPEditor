using SRMultiplayer.Models;
using System.Collections.Generic;

namespace SRMPEditor
{
    public class NetworkLandPlot
    {
        public static Dictionary<string, NetworkLandPlot> All = new Dictionary<string, NetworkLandPlot>();
        public Vector3 position;
        public string ID;
        public NetworkLandPlotModel Model;
        public NetworkLandPlot()
        {

        }
    }
}