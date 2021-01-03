using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRMultiplayer.Models
{
    [Serializable]
    public class NetworkPlayerModel
    {
        public Dictionary<byte, NetworkAmmoModel> ammoDict;
        public byte ammoMode;
        public int currency;
        public float currEnergy;
        public float currHealth;
        public float currRads;
        public byte regionSet;
        public List<byte> upgrades;

        public NetworkPlayerModel()
        {

        }

    }
    [Serializable]
    public class NetworkGadgetsModel
    {
        public List<ushort> availBlueprints;
        public Dictionary<ushort, BlueprintLockData> blueprintLockData;
        public List<ushort> blueprints;
        public Dictionary<ushort, int> craftMatCounts;
        public Dictionary<ushort, int> gadgets;
        public Dictionary<ushort, int> placedGadgetCounts;
        public List<ushort> registeredBlueprints;
        public NetworkGadgetsModel()
        {

        }
    }
    [Serializable]
    public class NetworkDroneModel
    {
        public NetworkAmmoModel ammo;
        public List<ushort> fashions;
        public bool noClip;
        public ProgramData[] programs;
        public double batteryDepleteTime;
        public NetworkDroneModel()
        {

        }
    }
    [Serializable]
    public class NetworkAnimalModel
    {
        public double transformTime;
        public double nextReproduceTime;
        public List<ushort> fashions;

        public NetworkAnimalModel()
        {

        }
    }
    [Serializable]
    public class NetworkPlortModel
    {
        public double destroyTime;

        public NetworkPlortModel()
        {

        }
    }
    [Serializable]
    public class NetworkProduceModel
    {
        public byte state;
        public double progressTime;

        public NetworkProduceModel()
        {

        }
    }
    [Serializable]
    public class NetworkSlimeModel
    {
        public EmotionState emotionAgitation;
        public EmotionState emotionFear;
        public EmotionState emotionHunger;
        public double? disabledAtTime;
        public bool isFeral;
        public bool isGlitch;
        public List<ushort> fashions;

        public NetworkSlimeModel()
        {

        }
    }
    [Serializable]
    public class NetworkMasterSwitchModel
    {
        public byte state;

        public NetworkMasterSwitchModel()
        {

        }
    }
    [Serializable]
    public class NetworkPuzzleSlotModel
    {
        public bool filled;

        public NetworkPuzzleSlotModel()
        {

        }
    }
    [Serializable]
    public class NetworkAccessDoorModel
    {
        public byte state;

        public NetworkAccessDoorModel()
        {

        }
    }
    [Serializable]
    public class NetworkGordoModel
    {
        public int gordoEatenCount;
        public List<ushort> fashions;

        public NetworkGordoModel()
        {

        }
    }
    [Serializable]
    public class NetworkLandPlotModel
    {
        public double nextFeedingTime;
        public int remainingFeedOperations;
        public byte feederCycleSpeed;
        public double collectorNextTime;
        public double attachedDeathTime;
        public byte typeId;
        public byte attachedId;
        public ushort attachedResourceId;
        public List<byte> upgrades;
        public Dictionary<byte, NetworkAmmoModel> siloAmmo;
        public int[] siloStorageIndices;
        public float ashUnits;

        public NetworkLandPlotModel()
        {

        }
    }
    [Serializable]
    public class NetworkSpawnResourceModel : NetworkPositionalModel
    {
        public float storedWater;
        public double nextSpawnTime;
        public bool nextSpawnRipens;

        public NetworkSpawnResourceModel()
        {

        }
    }
    [Serializable]
    public class NetworkTreasurePodModel
    {
        public byte state;
        public Queue<ushort> spawnQueue;

        public NetworkTreasurePodModel()
        {

        }
    }
    [Serializable]
    public class NetworkPositionalModel
    {
        public NetworkVector3Model pos;

        public NetworkPositionalModel()
        {

        }
    }
    [Serializable]
    public class NetworkVector3Model
    {
        public float x;
        public float y;
        public float z;

        public NetworkVector3Model()
        {

        }
    }
    [Serializable]
    public class EmotionState
    {
        public float currVal;

        public EmotionState()
        {

        }
    }
    [Serializable]
    public class NetworkAmmoModel
    {
        public Slot[] slots;
        public int usableSlots;

        public NetworkAmmoModel()
        {

        }

    }
    [Serializable]
    public class NetworkEchoNetModel : NetworkGadgetModel
    {
        public double lastSpawnTime;

        public NetworkEchoNetModel()
        {

        }
    }
    [Serializable]
    public class NetworkExtractorModel : NetworkGadgetModel
    {
        public int cyclesRemaining;
        public int queuedToProduce;
        public double cycleEndTime;
        public double nextProduceTime;

        public NetworkExtractorModel()
        {

        }
    }
    [Serializable]
    public class NetworkSnareModel : NetworkGadgetModel
    {
        public ushort baitTypeId;
        public ushort gordoTypeId;
        public int gordoEatenCount;
        public List<ushort> fashions;
        public int gordoTargetCount;

        public NetworkSnareModel()
        {

        }
    }
    [Serializable]
    public class NetworkWarpDepotModel : NetworkGadgetModel
    {
        public bool isPrimary;
        public NetworkAmmoModel ammo;

        public NetworkWarpDepotModel()
        {

        }
    }
    [Serializable]
    public class NetworkGadgetModel
    {
        public double waitForChargeupTime;

        public NetworkGadgetModel()
        {

        }
    }
    [Serializable]
    public class BlueprintLockData
    {
        public double lockedUntil;
        public bool timedLock;
        public BlueprintLockData()
        {

        }
    }
    [Serializable]
    public struct ProgramData
    {
        public string target;
        public string source;
        public string destination;
    }
    [Serializable]
    public class Slot
    {
        public int count;
        public Dictionary<byte, float> emotions;
        public ushort id;

        public Slot()
        {

        }

    }
}
