using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private GameBoard _board;
    [SerializeField] private GameTile _gameTile;
    [SerializeField] private Vector2Int _boardSize;
    [SerializeField] private Shop _shop;
    [SerializeField] private SetupColor _setupColor;

    public bool IsGame;

    private void Start()
    {
        _shop.Initialize();
        _board.Initialize(_boardSize, _gameTile, _setupColor,this);
        
    }

    public void ChangeGameTile(GameTile gameTile, SetupColor setupColor)
    {
        _board.Initialize(_boardSize, gameTile, setupColor,this);
    }

    public void AddMoney()
    {
        GUIManager._instance.Coin += 1000;
    }
}
