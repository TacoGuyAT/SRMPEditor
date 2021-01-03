using SRMPEditor;
using System;
using SRMultiplayer.Models;
using System.Collections.Generic;

public class Player
{
    public float x;
    public float y;
    public float z;
    public float r;

    public Player()
	{
        Dictionary<byte, NetworkAmmoModel> dict = new Dictionary<byte, NetworkAmmoModel>();
        dict.Add(0, new NetworkAmmoModel { slots = new Slot[5], usableSlots = 4 });
        dict.Add(1, new NetworkAmmoModel { slots = new Slot[3], usableSlots = 3 });
        Model = new NetworkPlayerModel { currency = 250, currEnergy = 100, currHealth = 100, ammoDict = dict, upgrades = new List<byte>() };
	}

    public NetworkPlayerModel Model;
}
