using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private GameBoard _board;
    [SerializeField] private Vector2Int _boardSize;

    [SerializeField] private float _speedGameTile;

    private void Start()
    {
        _board.Initialize(_boardSize);
    }
}
