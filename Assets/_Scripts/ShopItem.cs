using UnityEngine;

public class ShopItem : MonoBehaviour
{
    [SerializeField] private GameTile _gameTilePrefab;
    [SerializeField] private Shop _shop;
    [SerializeField] private  GameObject _buyShopItem;
    [SerializeField] private GameObject _lockGameTile;

    [SerializeField] private string ShopItemName;

    public void Initialize()
    {
       var buyItem =  PlayerPrefs.GetInt(ShopItemName);
        if (buyItem != 0)
        {
            UnlockItem();
        }
    }

    private void UnlockItem()
    {
        PlayerPrefs.SetInt(ShopItemName, 1);
        _lockGameTile.SetActive(false);
        _buyShopItem.SetActive(false);
    }

    public void BuyItem()
    {
        UnlockItem();
    }

    public void ChangeGameTile()
    {
        _shop.ChangeGameTile(_gameTilePrefab);
    }

  
}