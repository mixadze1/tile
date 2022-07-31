using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private GameBoard _board;
    [SerializeField] private GameTile _gameTile;
    [SerializeField] private Vector2Int _boardSize;
    [SerializeField] private Shop _shop;

    private void Start()
    {
        _board.Initialize(_boardSize, _gameTile);
        _shop.Initialize();
    }

    public void ChangeGameTile(GameTile gameTile)
    {
        _board.Initialize(_boardSize, gameTile);
    }
}
