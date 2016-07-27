using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlayFab.ClientModels;
public class Missile
{
    public string missileID;
    public string missileName;
    public Sprite missileSprite;
    public bool isBought;
    public uint price;
}

public class Skin
{
    public string skinID;
    public string skinName;
    public Sprite skinSprite;    
    public bool isBought;
    public uint price;
}

public class PlayFabGameBridge : MonoBehaviour {
    public static int wonRaces;
    public static int totalRaces;
    public static int userBalance;    

    public static Dictionary<string, Skin>    skins;
    public static Dictionary<string, Missile> missiles;

    public static ItemInstance currentSkin;
    public static ItemInstance currentMissile;
	
	public static Skin    defaultSkin    = new Skin    { skinID    = "S00", skinName    = "Green Skin",       price = 0, isBought = true};
    public static Missile defaultMissile = new Missile { missileID = "M00", missileName = "Shuriken Missile", price = 0, isBought = true};

    public static Dictionary<string, ItemInstance> boughtSkins;
    public static Dictionary<string, ItemInstance> boughtMissiles;

    public static bool catalogConstructed = false;

}
