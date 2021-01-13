using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRMultiplayer.Models
{
    public enum Upgrade // Decompiled SR Code
    {
        // Token: 0x04003116 RID: 12566
        HEALTH_1,
        // Token: 0x04003117 RID: 12567
        HEALTH_2,
        // Token: 0x04003118 RID: 12568
        HEALTH_3,
        // Token: 0x04003119 RID: 12569
        ENERGY_1,
        // Token: 0x0400311A RID: 12570
        ENERGY_2,
        // Token: 0x0400311B RID: 12571
        ENERGY_3,
        // Token: 0x0400311C RID: 12572
        AMMO_1,
        // Token: 0x0400311D RID: 12573
        AMMO_2,
        // Token: 0x0400311E RID: 12574
        AMMO_3,
        // Token: 0x0400311F RID: 12575
        JETPACK,
        // Token: 0x04003120 RID: 12576
        JETPACK_EFFICIENCY,
        // Token: 0x04003121 RID: 12577
        AIR_BURST,
        // Token: 0x04003122 RID: 12578
        RUN_EFFICIENCY,
        // Token: 0x04003123 RID: 12579
        LIQUID_SLOT,
        // Token: 0x04003124 RID: 12580
        AMMO_4,
        // Token: 0x04003125 RID: 12581
        HEALTH_4,
        // Token: 0x04003126 RID: 12582
        RUN_EFFICIENCY_2,
        // Token: 0x04003127 RID: 12583
        GOLDEN_SURESHOT,
        // Token: 0x04003128 RID: 12584
        SPARE_KEY,
        // Token: 0x04003129 RID: 12585
        TREASURE_CRACKER_1 = 100,
        // Token: 0x0400312A RID: 12586
        TREASURE_CRACKER_2,
        // Token: 0x0400312B RID: 12587
        TREASURE_CRACKER_3,
        // Token: 0x0400312C RID: 12588
        TREASURE_CRACKER_4
    }
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
    public class NetworkMailModel
    {
        public byte type;
        public string key;
        public bool read;
    }
    [Serializable]
    public class NetworkOffer
    {
        public List<NetworkRequestItemEntry> requests;
        public List<NetworkItemEntry> rewards;
        public double expireTime;
        public double earlyExchangeTime;
        public string rancherId;
        public string offerId;
        public byte offerType;
    }
    [Serializable]
    public class NetworkRequestItemEntry : NetworkItemEntry
    {
        public int progress;
    }
    [Serializable]
    public class NetworkItemEntry
    {
        public ushort id;
        public ushort specReward;
        public int count;
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
