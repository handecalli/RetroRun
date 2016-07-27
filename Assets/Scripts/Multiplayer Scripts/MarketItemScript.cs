using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using PlayFab.ClientModels;
public class MarketItemScript : MonoBehaviour {
    
	
	public delegate void MarketItemAction();
	public static event MarketItemAction InitializeDone;

    private List<CatalogItem> items;
    private Dictionary<string, ContentHolder> itemContentHolders;
    private Dictionary<string, Image> itemImages;
    public Sprite[] SkinImageList;
    public Sprite[] MissileImageList;
    public Button   itemButton;    
        
    private GameObject skinContentPanel;
    private GameObject missileContentPanel;

    private ContentHolder holder; 
    private Transform transformItemImage;
    private Image itemImage;   
    private Text priceText;
    private string price;

    private bool dictionariesCreated = false;

    private int indexCatalog       = 0;
    private int indexSkinSprite    = 0;
    private int indexMissileSprite = 0;

    public static bool MarketLoaded = false;
        
    void AssignStoreItems()
    {
        List<CatalogItem> items = CatalogScript.items;                
        skinContentPanel        = GameObject.Find("SkinContentPanel");
        missileContentPanel     = GameObject.Find("MissileContentPanel");
        itemImages              = new Dictionary<string, Image>();
        itemContentHolders      = new Dictionary<string, ContentHolder>();

        if (!dictionariesCreated)
        {
            foreach (Missile missile in PlayFabGameBridge.missiles.Values)
            {
                Initialize(missile.price, items[indexCatalog]);
                indexCatalog++;
            }

            foreach (Skin skin in PlayFabGameBridge.skins.Values)
            {
                Initialize(skin.price, items[indexCatalog]);
                indexCatalog++;
            }

            if (items.Count == indexCatalog)
            {
                dictionariesCreated = true;
            }
        }
		InitializeDone();
        InitializationEventManager.TriggerNextInitialization();
    }
    public void Initialize(uint price, CatalogItem item)
    {
        Button newButton   = Instantiate(itemButton);

        transformItemImage = newButton.GetComponentInChildren<Transform>().Find("ItemImage");
        itemImage          = transformItemImage.GetComponent<Image>();
        priceText          = newButton.GetComponentInChildren<Text>();
        holder             = newButton.GetComponent<ContentHolder>();        

        holder.catalogItem = item;
        holder.itemClass   = item.ItemClass;
        priceText.text     = price.ToString();

        if (item.ItemClass == "Skin")
        {
            itemImage.sprite = SkinImageList[indexSkinSprite];
            holder.skin = PlayFabGameBridge.skins[item.ItemId];        

            if (!PlayFabGameBridge.boughtSkins.ContainsKey(holder.catalogItem.ItemId))
            {
                itemImage.color = new Color(0.1f, 0.1f, 0.1f, 1f); 
                holder.skin.isBought = false;
            }
            else
            {
                itemImage.color = Color.white;
                holder.skin.isBought = true;
            }
                
            newButton.transform.SetParent(skinContentPanel.transform);
            indexSkinSprite++;

        }

        if (item.ItemClass == "Missile")
        {
            itemImage.sprite = MissileImageList[indexMissileSprite];         
            holder.missile = PlayFabGameBridge.missiles[item.ItemId];    
            
            if (!PlayFabGameBridge.boughtMissiles.ContainsKey(holder.catalogItem.ItemId))
            {
				itemImage.color = new Color(0.1f, 0.1f, 0.1f, 1f); 
				holder.missile.isBought = false;
            }
            else
            {
                itemImage.color = Color.white;
                holder.missile.isBought = true;
            }
                                 
            newButton.transform.SetParent(missileContentPanel.transform);
            indexMissileSprite++;

        }

        itemImages.Add(item.ItemId, itemImage);
        itemContentHolders.Add(item.ItemId, holder);

    }

    public void UpdateMarketItems(string key)
    {
        itemImages[key].color = Color.white;
        if (itemContentHolders[key].itemClass == "Skin")
        {
            itemContentHolders[key].skin.isBought = true;
        }

        else if (itemContentHolders[key].itemClass == "Missile")
        {
            itemContentHolders[key].missile.isBought = true;
        }            
        
    }

    public void UpdateGrantedItems()
    {
        //default Skin item
        itemImages["S00"].color                 = Color.white;
        itemContentHolders["S00"].skin.isBought = true;
        
        //default Missile item
        itemImages["M00"].color                 = Color.white;
        itemContentHolders["M00"].missile.isBought = true;
    }

    void OnEnable() 
    {
        CatalogScript.InitMarketItems      += AssignStoreItems;
        PurchaseScript.updateMarketItems   += UpdateMarketItems;
        PlayerDataManagerScript.UpdateGrantedItems += UpdateGrantedItems;
    }

}
