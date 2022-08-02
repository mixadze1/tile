using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private Game _game;
    [SerializeField] private GameObject ShopUI;
    [SerializeField] private ShopItem[] _shopItems;

    private GameBoard _board;

    public const string SAFE_GAME_TILE = "SafeGameTile";
    public void Initialize(GameBoard board)
    {
        _board = board;
        _shopItems = GetComponentsInChildren<ShopItem>();
        InitializeShopItem();   
    }

    public SetupGameTile InitializeStartSetupGameTile()
    {
        return StartGameTile(PlayerPrefs.GetInt(SAFE_GAME_TILE));
    }

    public GameTile InitializeGameTile()
    {
        return GameTileCheck(PlayerPrefs.GetInt(SAFE_GAME_TILE));
    }

    private void InitializeShopItem()
    {
        foreach (ShopItem item in _shopItems)
        {
            item.Initialize(_game);
        }
    }

  
    private GameTile GameTileCheck(int numberTile)
    {
        foreach (ShopItem item in _shopItems)
        {
            if (item.CheckItem(numberTile))
            {
                return item.GetPrefab();
            }
        }
        return null;
    }

    private SetupGameTile StartGameTile(int numberTile)
    {
        foreach (ShopItem item in _shopItems)
        {
            if(item.CheckItem(numberTile))
            {
                return item.GetShopItem();
            }            
        }
        return null;  
    }

    public void ActivateShopUI()
    {
        if (!_board.IsWin())
            ShopUI.SetActive(true);
    }

    public void ChangeGameTile(GameTile gameTile, SetupGameTile setupGameTile, int numberItem)
    {
       
        if (_game.IsGame)
        {
            PlayerPrefs.SetInt(SAFE_GAME_TILE, numberItem);
            _game.ChangeGameTile(gameTile, setupGameTile);
        }
            
    }
}
