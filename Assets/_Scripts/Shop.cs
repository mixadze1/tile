using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private Game _game;
    [SerializeField] private ShopItem[] _shopItems;
    public void Initialize()
    {
        _shopItems = GetComponentsInChildren<ShopItem>();
        InitializeShopItem();
    }

    private void InitializeShopItem()
    {
        foreach (ShopItem item in _shopItems)
        {
            item.Initialize();
        }
    }

    public void ChangeGameTile(GameTile gameTile)
    {
        _game.ChangeGameTile(gameTile);
    }
}
