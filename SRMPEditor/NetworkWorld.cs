using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SRMultiplayer.Models;

namespace SRMPEditor
{
    [Serializable]
    public class NetworkWorld
    {
        public enum GameMode
        {
            Classic = 0,
//            AssholeMode = 1,
            Casual = 2
        }
        public struct CurrValue
        {
            public float currValue;
            public float prevValue;
        }
        public GameMode Mode;
        public double WorldTime;
        public int Keys;
        public Dictionary<ushort, int> Progress = new Dictionary<ushort, int>();
        public List<ushort> PediaIDs = new List<ushort>();
        public List<byte> UnlockedZoneMaps = new List<byte>();
        public Dictionary<byte, ushort> Palette = new Dictionary<byte, ushort>();
        public Dictionary<ushort, float> Saturation = new Dictionary<ushort, float>();
        public Dictionary<ushort, CurrValue> TempPrices = new Dictionary<ushort, CurrValue>();
        public float Seed;
        public bool SharedCurrency;
        public bool SharedUpgrades;
        public NetworkGadgetsModel GadgetsModel = new NetworkGadgetsModel();
        public int TotalCurrency;
        public List<byte> AllUpgrades = new List<byte>();
        public List<NetworkMailModel> AllMail = new List<NetworkMailModel>();
        public List<NetworkOffer> Offers = new List<NetworkOffer>();

    }
   
}
