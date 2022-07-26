using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private Tile _lockTilePrefab;
    [SerializeField] private GameTile _gameTilePrefab;
    [SerializeField] private Wall _wallPrefab;
    [SerializeField] private LayerMask _lockTileMask;

    private Tile[] _tiles;
    
    private List <Wall> _walls = new List<Wall>();

    private Vector2Int _size;
    public List<GameTile> GameTiles = new List<GameTile>();

    public void Initialize(Vector2Int size)
    {
        CreateBoard(size);
        CreateGameTile(size);
        CreateWall();
    }

    private void Update()
    {
        if(IsWin())
        {
            Debug.Log("win");
        }
    }

    private bool IsWin()
    {Debug.Log(GameTiles.Count);
       if (GameTiles.Count <= 1)
            return true;
        return false;
    }

    public void RestartGame()
    {
        foreach (var gameTile in GameTiles)
        {
            if(gameTile != null)
                Destroy(gameTile.gameObject);
        }
        foreach (var tile in _tiles)
        {
            Destroy(tile.gameObject);
        }
        foreach (var wall in _walls)
        {
            Destroy(wall.gameObject);
        }
        GameTiles.Clear();
        _walls.Clear();
        _size = NewSizeBoard();
        CreateBoard(_size);
        CreateGameTile(_size);
        CreateWall();

    }
    private Vector2Int NewSizeBoard()
    {
       return  new Vector2Int(Random.Range(5, 8), (Random.Range(5, 10)));
    }

    private void CreateBoard(Vector2Int size)
    {
        _size = size;
        Vector2 offset = new Vector2((size.x - 1) * 0.5f, (size.y - 1) * 0.5f);
        _tiles = new Tile[size.x * size.y];
        for (int i = 0, y = 0; y < _size.y; y++)
        {
            for (int x = 0; x < _size.x; x++, i++)
            {
                Tile tile = _tiles[i] = Instantiate(_tilePrefab);
                tile.Type = TileType.Open;
                if (Random.Range(0, 100) < 80)
                {
                    tile.Type = TileType.Open;
                }
                else
                {

                        Tile lockTile = _tiles[i] = Instantiate(_lockTilePrefab);
                        lockTile.transform.localPosition = new Vector3(x - offset.x, 0, y - offset.y);
                        tile.Type = TileType.Lock;
                }
                tile.transform.SetParent(transform, false);
                tile.transform.localPosition = new Vector3(x - offset.x, 0, y - offset.y);
            }
        }
    }

  /*  private bool GetAdjacent(Vector2 positionCenter)
    {
        Collider[] hitColliders = Physics.OverlapSphere(positionCenter, 0, _lockTileMask);
        int countOpenTile = 0;
        foreach (var hitCollider in hitColliders)
        {
            Debug.Log(countOpenTile);
             if (hitCollider.gameObject.GetComponent<Tile>().Type == TileType.Lock)
                countOpenTile++;
        }
        hitColliders = null;
        if (countOpenTile <= 10)
            return true;
        return false;
    }*/

    private void CreateGameTile(Vector2Int size)
    {
        _size = size;
        Vector2 offset = new Vector2((size.x - 1) * 0.5f, (size.y - 1) * 0.5f);
        for (int i = 0, y = 0; y < _size.y; y++)
        {
            for (int x = 0; x < _size.x; x++, i++)
            {
                if (_tiles[i].Type == TileType.Open)
                {
                    if (Random.Range(0, 100) > 80)
                    {
                        GameTile gameTile = Instantiate(_gameTilePrefab);
                        GameTiles.Add(gameTile);
                        gameTile.Initialize(this);
                        gameTile.transform.SetParent(transform, false);
                        gameTile.transform.localPosition = new Vector3(x - offset.x, 0.5f, y - offset.y);
                    }                   
                }
            }
        }
    }

    private void CreateWall()
    {
        Vector2 offset = new Vector2((_size.x) * 0.5f, (_size.y) * 0.5f);
        NorthWall(offset);
        EastWall(offset);
        WestWall(offset);
        SouthWall(offset);
    }

    private void NorthWall(Vector2 offset)
    {
        Wall wall  = Instantiate(_wallPrefab);
        _walls.Add(wall);
        wall.transform.localScale = new Vector3(_size.x + 0.25f, 1, 20.25f);
        wall.transform.position = new Vector3(0, 0, offset.y + 10.2f);
    }

    private void EastWall(Vector2 offset)
    {
        Wall wall  = Instantiate(_wallPrefab);
        _walls.Add(wall);
        wall.transform.localScale = new Vector3(20.25f, 1, _size.y + 10);
        wall.transform.position = new Vector3(offset.x + 10.2f, 0, 0);
    }

    private void WestWall(Vector2 offset)
    {
        Wall wall = Instantiate(_wallPrefab);
        _walls.Add(wall);
        wall.transform.localScale = new Vector3(20.25f, 1, _size.y + 10);
        wall.transform.position = new Vector3(-offset.x - 10.2f, 0, 0);
    }

    private void SouthWall(Vector2 offset)
    {
        Wall wall = Instantiate(_wallPrefab);
        _walls.Add(wall);
        wall.transform.localScale = new Vector3(_size.x + 0.25f, 1, 20.25f);
        wall.transform.position = new Vector3(0, 0, -offset.y - 10.2f);
    }
}
