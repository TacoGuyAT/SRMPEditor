using SRMultiplayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRMPEditor
{
    public class NetworkEntity
    {
        public static Dictionary<int, NetworkEntity> All = new Dictionary<int, NetworkEntity>();
        public int ID;
        public ushort Ident;
        public byte RegionSet;
        public NetworkAnimalModel AnimalModel;
        public NetworkPlortModel PlortModel;
        public NetworkProduceModel ProduceModel;
        public NetworkSlimeModel SlimeModel;
        public Vector3 Position;
        public Vector3 EulerAngles;
        public NetworkEntity()
        {

        }
    }
}
