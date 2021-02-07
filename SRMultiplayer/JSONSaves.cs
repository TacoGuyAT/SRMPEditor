using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SRMultiplayer.Models;

namespace SRMultiplayer
{
    [System.Serializable]
    public class PlayerSave
    {
        public float x;
        public float y;
        public float z;
        public float r;
        public NetworkPlayerModel Model;
    }

    [System.Serializable]
    public class NetworkAccessDoorSave
    {
        public string id;
        public float x;
        public float y;
        public float z;
        public NetworkAccessDoorModel Model;
    }

    [System.Serializable]
    public class NetworkGordoSave
    {
        public string id;
        public float x;
        public float y;
        public float z;
        public NetworkGordoModel Model;
    }

    [System.Serializable]
    public class NetworkLandPlotSave
    {
        public string id;
        public float x;
        public float y;
        public float z;
        public NetworkLandPlotModel Model;
    }

    [System.Serializable]
    public class NetworkSpawnResourceSave
    {
        public float x;
        public float y;
        public float z;
        public NetworkSpawnResourceModel Model;
    }

    [System.Serializable]
    public class NetworkTreasurePodSave
    {
        public string id;
        public float x;
        public float y;
        public float z;
        public NetworkTreasurePodModel Model;
    }

    [System.Serializable]
    public class NetworkSwitchSave
    {
        public string id;
        public float x;
        public float y;
        public float z;
        public NetworkMasterSwitchModel Model;
    }

    [System.Serializable]
    public class NetworkPuzzleSlotSave
    {
        public string id;
        public float x;
        public float y;
        public float z;
        public NetworkPuzzleSlotModel Model;
    }

    [System.Serializable]
    public class NetworkEntitySave
    {
        public int ID;
        public ushort Ident;
        public byte RegionSet;
        public float posx;
        public float posy;
        public float posz;
        public float rotx;
        public float roty;
        public float rotz;
        public NetworkSlimeModel SlimeModel;
        public NetworkAnimalModel AnimalModel;
        public NetworkProduceModel ProduceModel;
        public NetworkPlortModel PlortModel;
    }

    [System.Serializable]
    public class NetworkGadgetSave
    {
        public string id;
        public float x;
        public float y;
        public float z;
        public float r;
        public ushort Ident;
        public NetworkWarpDepotModel WarpDepot;
        public NetworkDroneModel Drone;
        public NetworkExtractorModel Extractor;
        public NetworkSnareModel Snare;
        public NetworkEchoNetModel EchoNet;
    }
}
