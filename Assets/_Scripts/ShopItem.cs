﻿using TMPro;
using UnityEngine;

public class ShopItem : MonoBehaviour
{
    [SerializeField] private GameTile _gameTilePrefab;
    [SerializeField] private SetupGameTile _setupGameTile;
    [SerializeField] private Shop _shop;
    [SerializeField] private  GameObject _buyShopItem;
    [SerializeField] private GameObject _lockGameTile;
    [SerializeField] private int _priceItem = 300;
    [SerializeField] private TextMeshProUGUI _priceText;
    [SerializeField] private string ShopItemName;
    [SerializeField] private bool _isUnlockItem;
    [SerializeField] private int _numberItem;

    private Game _game;

    public void Initialize(Game game)
    {
        _game = game;
       var buyItem =  PlayerPrefs.GetInt(ShopItemName);
        if (buyItem != 0 || _isUnlockItem)
        {
            UnlockItem();
        }
        _priceText.text = _priceItem.ToString();
    }

    private void UnlockItem()
    {
        _priceText.gameObject.SetActive(false);
        PlayerPrefs.SetInt(ShopItemName, 1);
        _lockGameTile.SetActive(false);
        _buyShopItem.SetActive(false);
    }

    public void BuyItem()
    {
        if (GUIManager._instance.Coin >= _priceItem && _game.IsGame)
        {
            UnlockItem();
            GUIManager._instance.Coin -= _priceItem;
        }
        
    }

    public bool CheckItem(int number)
    {
        if (number == _numberItem)
            return true;
        return false;
    }

    public SetupGameTile GetShopItem()
    {
        return _setupGameTile;
    }

    public GameTile GetPrefab()
    {
        return _gameTilePrefab;
    }

    public void EquipGameTile()
    {
        _shop.ChangeGameTile(_gameTilePrefab, _setupGameTile, _numberItem);
    }

  
}