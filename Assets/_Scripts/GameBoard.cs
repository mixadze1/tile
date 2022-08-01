using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    [SerializeField] private SetupColor _setupColor;
    [SerializeField] private GameObject _buttonNextLevel;
    [SerializeField] private Win _win;
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private Tile _lockTilePrefab;
   
    [SerializeField] private Wall _wallPrefab;
    [SerializeField, Range(20, 80)] private float _spawnPercentWall;
    [SerializeField, Range(50, 80)] private float _spawnPecrentgameTile;

    private GameTile _gameTilePrefab;
    private Tile[] _tiles;

    private float _sizeWallX = 20.25f;
    private float _sizeWallZ = 10;
    private float _offsetWallX = 0.25f;
    private float _offsetWall = 10.2f;

    
    private List <Wall> _walls = new List<Wall>();

    private Vector2Int _size;

    private bool _isGame;

    public List<GameTile> GameTiles = new List<GameTile>();


    public void Initialize(Vector2Int size, GameTile gameTile)
    {
        _gameTilePrefab = gameTile;
        _win.InitializeLevel();
        _size = NewSizeBoard();
        if (_isGame)
        { RestartGame(); }
        else
        {
            CreateBoard(_size);
            CreateGameTile(_size);
            CreateWall();
        }
        
    }

    private void Update()
    {
        if(IsWin() && _isGame)
        {
            _buttonNextLevel.SetActive(true);
            _win.NextLevel();
            _isGame = false;
        }
    }

    private bool IsWin()
    {
       if (GameTiles.Count <= 1)
            return true;
        return false;
    }

    public void RestartGame()
    {
        _buttonNextLevel.SetActive(false);
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
                if (Random.Range(0, 100) > _spawnPercentWall)
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
                    if (Random.Range(0, 100) > _spawnPecrentgameTile)
                    {
                        GameTile gameTile = Instantiate(_gameTilePrefab);
                        GameTiles.Add(gameTile);
                        gameTile.Initialize(this, _setupColor);
                        gameTile.transform.SetParent(transform, false);
                        gameTile.transform.localPosition = new Vector3(x - offset.x, 0.5f, y - offset.y);
                    }                   
                }
            }
        }
        int count;
        for (count = 2; count <= GameTiles.Count; count *= 2)
        {
            if (GameTiles.Count == 0)
                CreateGameTile(_size);

            if (count == GameTiles.Count)
            {
                return;
            }           
        }
        AmountGameTileMultiplyTwo(count);
        if (GameTiles.Count < count )
        {
            AmountGameTileMultiplyTwo(count/2);
        }
        _isGame = true;
       
    }

    private void AmountGameTileMultiplyTwo(int count)
    {
        if (GameTiles.Count > count)
        {
            foreach (GameTile tile in GameTiles.ToArray())
            {
                Destroy(tile.gameObject);
                GameTiles.Remove(tile);
                if (count == GameTiles.Count)
                {
                    return;
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
    #region CreateWall
    private void NorthWall(Vector2 offset)
    {
        Wall wall  = Instantiate(_wallPrefab);
        _walls.Add(wall);
        wall.transform.localScale = new Vector3(_size.x + _sizeWallX, 1, _sizeWallX);
        wall.transform.position = new Vector3(0, 0, offset.y + _offsetWall);
    }

    private void EastWall(Vector2 offset)
    {
        Wall wall  = Instantiate(_wallPrefab);
        _walls.Add(wall);
        wall.transform.localScale = new Vector3(_sizeWallX, 1, _size.y + _sizeWallZ);
        wall.transform.position = new Vector3(offset.x + _offsetWall, 0, 0);
    }

    private void WestWall(Vector2 offset)
    {
        Wall wall = Instantiate(_wallPrefab);
        _walls.Add(wall);
        wall.transform.localScale = new Vector3(_sizeWallX, 1, _size.y + _sizeWallZ);
        wall.transform.position = new Vector3(-offset.x - _offsetWall, 0, 0);
    }

    private void SouthWall(Vector2 offset)
    {
        Wall wall = Instantiate(_wallPrefab);
        _walls.Add(wall);
        wall.transform.localScale = new Vector3(_size.x + _offsetWallX, 1, _sizeWallX);
        wall.transform.position = new Vector3(0, 0, -offset.y - _offsetWall);
    }
    #endregion
}
