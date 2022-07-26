using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private GameBoard _board;
    [SerializeField] private Vector2Int _boardSize;

    private void Start()
    {
        _board.Initialize(_boardSize);
    }
}
