using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class ButtonSelectionScript : MonoBehaviour {

    private ContentHolder holder;

    private GameObject PlayerChar;
    private GameObject PlayerMissile;
    
    private Image playerCharImage;
    private Image playerMissileImage;
    private Image itemImage;
    
    private Button buySkinButton;
    private Button buyMissileButton;
    private Button wearSkinButton;
    private Button wearMissileButton;

	private Color lightGray;

    void Awake()
    {
        Transform t        = GetComponentInChildren<Transform>().Find("ItemImage");

        itemImage          = t.GetComponent<Image>();
        holder             = GetComponent<ContentHolder>();

        playerCharImage    = GameObject.Find("PlayerChar").GetComponent<Image>();
        playerMissileImage = GameObject.Find("PlayerMissile").GetComponent<Image>();        

        buyMissileButton   = GameObject.Find("btnMissileBuy").GetComponent<Button>();
        buySkinButton      = GameObject.Find("btnSkinBuy").GetComponent<Button>();
        wearSkinButton     = GameObject.Find("btnSkinWear").GetComponent<Button>();
		wearMissileButton  = GameObject.Find("btnMissileWear").GetComponent<Button>();
		lightGray = new Color(0.78f, 0.78f, 0.78f, 0.5f);
	}
	
	public void Select()
    {
 
        if (holder.itemClass == "Skin")
        {
            playerCharImage.sprite = itemImage.sprite;
			MarketManagerScript.selectedSkin = holder.catalogItem;
			ColorBlock cb = wearSkinButton.colors;

			if (holder.skin.isBought)
            {
                buySkinButton.interactable = false;
				if(holder.skin.skinID == PlayFabGameBridge.currentSkin.ItemId)
				{
					wearSkinButton.interactable = false;
					cb.disabledColor = Color.green;
					wearSkinButton.colors = cb;
				}
				else
				{
					wearSkinButton.interactable = true;
					cb.disabledColor = lightGray;
					wearSkinButton.colors = cb;
				}
            }
            else
            {
                buySkinButton.interactable = true;
				wearSkinButton.interactable = false;
				cb.disabledColor = lightGray;
				wearSkinButton.colors = cb;
			}
		}

        if (holder.itemClass == "Missile")
        {
            playerMissileImage.sprite           = itemImage.sprite;
            MarketManagerScript.selectedMissile = holder.catalogItem;
			ColorBlock cb = wearMissileButton.colors;

            if (holder.missile.isBought)
            {
                buyMissileButton.interactable  = false;
				if(holder.missile.missileID == PlayFabGameBridge.currentMissile.ItemId)
				{
					wearMissileButton.interactable = false;
					cb.disabledColor = Color.green;
					wearMissileButton.colors = cb;
				}
				else
				{
					wearMissileButton.interactable = true;
					cb.disabledColor = lightGray;
					wearMissileButton.colors = cb;
				}
            }
            else
            {
                buyMissileButton.interactable  = true;
				wearMissileButton.interactable = false;
				cb.disabledColor = lightGray;
				wearMissileButton.colors = cb;
			}
		} 
    }
}
