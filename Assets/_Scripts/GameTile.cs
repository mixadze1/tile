using TMPro;
using UnityEngine;

public class GameTile : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;  

    [SerializeField] private TextMeshProUGUI _textNumber;

    [SerializeField] private float _speed;
    [SerializeField,Range(10f,30f)] private float _rotation;
    [SerializeField] private Material[] _materials;
    [SerializeField] private TrailRenderer[] _trailRenderers;
    [SerializeField] private ParticleSystem _particleSystem;

    private AudioSource _audioMove;
    private AudioSource _audioMatch;
    private GameBoard _board;
    private SetupGameTile _setupColor;
   
    private float _diaposonMatch = 0.2f;
    private float _speedRotation = 12;

   
    private bool _isMove;

    private bool _isMoveLeft;
    private bool _isMoveRight;
    private bool _isMoveForward;
    private bool _isMoveDown;

    public bool IsAlreadyMatch;
    public int NumberGameTile;

    public bool IsDie { get; private set; }

    private void Awake()
    {   
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.isKinematic = true;
        SwipeManager.instance.MoveEvent += MoveGameTile;
        _textNumber.text = NumberGameTile.ToString();
        GetComponentInChildren<Canvas>().gameObject.SetActive(false);
    }

    public void Initialize(GameBoard gameBoard, SetupGameTile setupGameTile)
    {
        _board = gameBoard;
        InitializeGameTile(setupGameTile);
       
        TrailInitialize();
    }

    private void InitializeGameTile(SetupGameTile setupGameTile)
    {
        _setupColor = setupGameTile;
        _materials = _setupColor.Materials;
        _particleSystem = _setupColor.ParticleSystem;
        _audioMove = _setupColor.Move;
        _audioMatch = _setupColor.Match;

    }

    private void TrailInitialize()
    {
        _trailRenderers = GetComponentsInChildren<TrailRenderer>();
        foreach (var trail in _trailRenderers)
        {
            trail.gameObject.SetActive(false);
        }
        _trailRenderers[NumberGameTile - 1].gameObject.SetActive(true);
    }

    private void Update()
    {
        if (_rigidbody.velocity.magnitude < 1f && gameObject.transform.rotation != Quaternion.Euler(Vector3.zero))
        {
            MoveOff();
            gameObject.transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.Euler(Vector3.zero), Time.deltaTime * _speedRotation);
            _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            _isMove = false;
        }
        RotationOn();
    }

    private void MoveOff()
    {
        _isMoveLeft = false;
        _isMoveRight = false;
        _isMoveForward = false;
        _isMoveDown = false;
    }
    
    private void RotationOn()
    {
        if (_isMoveLeft)
            gameObject.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(0, 0, _rotation)), Time.deltaTime * _speedRotation);

        if (_isMoveRight)
            gameObject.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(0, 0, -_rotation)), Time.deltaTime * _speedRotation);
        if (_isMoveDown)
            gameObject.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(-_rotation, 0, 0)), Time.deltaTime * _speedRotation);
        if (_isMoveForward)
            gameObject.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(_rotation, 0, 0)), Time.deltaTime * _speedRotation);
    }

    private bool IsMove()
    {
       if (_isMove)
            return true;
        return false;
    }

    void MoveGameTile(bool[] swipes)
    {

        if (IsDie || IsMove())
            return;
        
        try
        {        
            _isMove = true;
            _rigidbody.isKinematic = false;
            if (swipes[(int)SwipeManager.Direction.Left])
            {
                AudioMove();
                ConstainersLeftRight();
                _isMoveLeft = true;
                _rigidbody.velocity = Vector3.left * _speed;
            }
            if (swipes[(int)SwipeManager.Direction.Right])
            {
                AudioMove();
                ConstainersLeftRight();
                _isMoveRight = true;
                _rigidbody.velocity = Vector3.right * _speed;
            }

            if (swipes[(int)SwipeManager.Direction.Up])
            {
                AudioMove();
                ConstainersForwardDown();
                _isMoveForward = true;
               _rigidbody.velocity = Vector3.forward * _speed;
            }
            if (swipes[(int)SwipeManager.Direction.Down])
            {
                AudioMove();
                ConstainersForwardDown();
                _isMoveDown = true;
                _rigidbody.velocity = -Vector3.forward * _speed;
            }
        }
        catch
        {
            return;
        }    
    }

    private void AudioMove()
    {
        AudioSource audioSource = Instantiate(_audioMove);
        audioSource.pitch = Random.Range(0.7f, 0.9f);
        audioSource.transform.SetParent(transform, true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<GameTile>() &&
            collision.gameObject.GetComponent<GameTile>().NumberGameTile == NumberGameTile)
        {
            if (collision.gameObject.GetComponent<GameTile>().IsAlreadyMatch || IsAlreadyMatch)
            {              
                return;
            }
            if ((_isMoveDown || _isMoveForward) &&
                Mathf.Abs(gameObject.transform.position.x) - Mathf.Abs(collision.gameObject.transform.position.x) < _diaposonMatch
                && Mathf.Abs(gameObject.transform.position.x) - Mathf.Abs(collision.gameObject.transform.position.x) > -_diaposonMatch)
            {
                IsAlreadyMatch = true;
                collision.gameObject.GetComponent<GameTile>().IsAlreadyMatch = true;

                CreateUpGameTile(collision);
                ChangeColor();

            }
            if ((_isMoveLeft || _isMoveRight) &&
                Mathf.Abs(gameObject.transform.position.z) - Mathf.Abs(collision.gameObject.transform.position.z) < _diaposonMatch
                &&
                Mathf.Abs(gameObject.transform.position.z) - Mathf.Abs(collision.gameObject.transform.position.z) > -_diaposonMatch)
            {
                IsAlreadyMatch = true;
                collision.gameObject.GetComponent<GameTile>().IsAlreadyMatch = true;
                CreateUpGameTile(collision);
                ChangeColor();

            }
        }
    }

    private void ChangeColor()
    {
        try
        {
            gameObject.GetComponent<MeshRenderer>().material = _materials[NumberGameTile];
        }
        catch
        {
            gameObject.GetComponent<MeshRenderer>().material = _materials[0];
        }
        foreach (var trail in _trailRenderers)
        {
            trail.gameObject.SetActive(false);
        }

        try
        {
            _trailRenderers[NumberGameTile - 1].gameObject.SetActive(true);
        }
        catch
        {

            _trailRenderers[0].gameObject.SetActive(true);
        }
    }

    private void CreateUpGameTile(Collision collision)
    {
        Destroy(collision.gameObject);
        ParticleSystem particle = Instantiate(_particleSystem);
        AudioSource audioSource = Instantiate(_audioMatch);
        audioSource.transform.SetParent(transform, true);
        particle.transform.position = collision.transform.position;
        particle.transform.position = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
        _board.GameTiles.Remove(collision.gameObject.GetComponent<GameTile>());
        collision.gameObject.GetComponent<GameTile>().IsDie = true;
        gameObject.transform.localPosition = collision.gameObject.transform.localPosition;
        NumberGameTile++;
        _textNumber.text = NumberGameTile.ToString();
        IsAlreadyMatch = false;
    }

    private void ConstainersLeftRight()
    {
        _rigidbody.constraints = RigidbodyConstraints.FreezePositionY |
                     RigidbodyConstraints.FreezePositionZ |
                     RigidbodyConstraints.FreezeRotationX |
                     RigidbodyConstraints.FreezeRotationY;
    }

    private void ConstainersForwardDown()
    {
        _rigidbody.constraints = RigidbodyConstraints.FreezePositionY |
                      RigidbodyConstraints.FreezePositionX |
                      RigidbodyConstraints.FreezeRotationZ |
                      RigidbodyConstraints.FreezeRotationY;
    }
}