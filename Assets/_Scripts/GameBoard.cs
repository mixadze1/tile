using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{   
    [SerializeField] private GameObject _buttonNextLevel;
    [SerializeField] private Restart _restart;
    [SerializeField] private Win _win;
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private Tile _lockTilePrefab;
    [SerializeField] private Wall _wallPrefab;
    [SerializeField, Range(20, 80)] private float _spawnPercentWall;
    [SerializeField, Range(50, 80)] private float _spawnPecrentgameTile;
    [SerializeField, Range (0.5f,1f)] private float _localScaleTile;
    private Game _game;
    private SetupGameTile _setupGameTile;
    private GameTile _gameTilePrefab;
    private Tile[] _tiles;

    private float _sizeWallX = 20.25f;
    private float _sizeWallZ = 10;
    private float _offsetWallX = 0.25f;
    private float _offsetWall = 10.15f;
    
    private List <Wall> _walls = new List<Wall>();

    private Vector2Int _size;

    private bool _isGame;

    public List<GameTile> GameTiles = new List<GameTile>();


    public void Initialize(Vector2Int size, GameTile gameTile, SetupGameTile setupGameTile, Game game)
    {
        _game = game;
        _setupGameTile = setupGameTile;
        _gameTilePrefab = gameTile;
        _win.InitializeLevel();
        _size = NewSizeBoardAndCameraCorrectPosition();
        //_restart.Initialize(this);
        if (game.IsGame)
        { 
            RestartGame();
            return;
        }
        else
        {
            CreateBoard(_size);
            CreateGameTile(_size);
            CreateWall();
            _game.IsGame = true;
        }
        
    }

    private void Update()
    {
        if(IsWin() && _game.IsGame)
        {
            _buttonNextLevel.SetActive(true);
            _game.IsGame = false;
        }
    }

    public bool IsWin()
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
        _size = NewSizeBoardAndCameraCorrectPosition();
        CreateBoard(_size);
        CreateGameTile(_size);
        CreateWall();
        _game.IsGame = true;
    }

    private Vector2Int NewSizeBoardAndCameraCorrectPosition()
    {
        if (_win.Level >= 25)
            return new Vector2Int(Random.Range(8, 10), (Random.Range(8, 10)));
        if (_win.Level >= 50)
        {
            Camera.main.transform.position = new Vector3(transform.position.x, 20f, -6f);
            return new Vector2Int(Random.Range(11, 13), (Random.Range(11, 13)));
        }
           
        if (_win.Level >= 100)
        {
            Camera.main.transform.position = new Vector3(transform.position.x, 26f, -9f);
            return new Vector2Int(Random.Range(14, 16), (Random.Range(12, 16)));
        }
        Camera.main.transform.position = new Vector3(transform.position.x, 15f, -4.5f);
        return  new Vector2Int(Random.Range(5, 7), (Random.Range(5, 7)));
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
                        gameTile.Initialize(this, _setupGameTile);
                        gameTile.transform.SetParent(transform, false);
                        gameTile.transform.localScale = new Vector3(_localScaleTile, _localScaleTile, _localScaleTile);
                        gameTile.transform.localPosition = new Vector3(x - offset.x, 0.5f, y - offset.y);
                    }                   
                }
            }
        }

        if (GameTiles.Count == 0 || GameTiles.Count == 1)
        {
            CreateGameTile(_size);
        }

        int count;
        for (count = 2; count <= GameTiles.Count; count *= 2)
        {
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
