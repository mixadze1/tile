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
    private GameBoard _board;
    private SetupColor _setupColor;

    private float _diaposonMatch = 0.2f;
    private float _speedRotation = 12;

    private bool _isMove;

    private bool _isMoveLeft;
    private bool _isMoveRight;
    private bool _isMoveForward;
    private bool _isMoveDown;

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

    public void Initialize(GameBoard gameBoard, SetupColor setupColor)
    {
        _board = gameBoard;
        ColorInitialize(setupColor);
        TrailInitialize();
    }

    private void ColorInitialize(SetupColor setupColor)
    {
        _setupColor = setupColor;
        _materials = _setupColor.Materials;
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

    void MoveGameTile(bool[] swipes)
    {

        if (IsDie || _isMove)
            return;     
        try
        {
            _isMove = true;
            _rigidbody.isKinematic = false;
            if (swipes[(int)SwipeManager.Direction.Left])
            {
                ConstainersLeftRight();
                _isMoveLeft = true;
                _rigidbody.velocity = Vector3.left * _speed;
            }
            if (swipes[(int)SwipeManager.Direction.Right])
            {
                ConstainersLeftRight();
                _isMoveRight = true;
                _rigidbody.velocity = Vector3.right * _speed;
            }

            if (swipes[(int)SwipeManager.Direction.Up])
            {

                ConstainersForwardDown();
                _isMoveForward = true;
               _rigidbody.velocity = Vector3.forward * _speed;
            }
            if (swipes[(int)SwipeManager.Direction.Down])
            {
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<GameTile>() &&
            collision.gameObject.GetComponent<GameTile>().NumberGameTile == NumberGameTile)
        {
            if ((_isMoveDown || _isMoveForward) &&
                Mathf.Abs(gameObject.transform.position.x) - Mathf.Abs(collision.gameObject.transform.position.x) < _diaposonMatch
                && Mathf.Abs(gameObject.transform.position.x) - Mathf.Abs(collision.gameObject.transform.position.x) > -_diaposonMatch)
            {
                CreateUpGameTile(collision);
                ChangeColor();
            }
            if ((_isMoveLeft || _isMoveRight) &&
                Mathf.Abs(gameObject.transform.position.z) - Mathf.Abs(collision.gameObject.transform.position.z) < _diaposonMatch
                &&
                Mathf.Abs(gameObject.transform.position.z) - Mathf.Abs(collision.gameObject.transform.position.z) > -_diaposonMatch)
            {
                CreateUpGameTile(collision);
                ChangeColor();
            }
        }
    }

    private void ChangeColor()
    {
        gameObject.GetComponent<MeshRenderer>().material = _materials[NumberGameTile];
        foreach(var trail in _trailRenderers)
        {
            trail.gameObject.SetActive(false);
        }
        _trailRenderers[NumberGameTile - 1].gameObject.SetActive(true);
    }

    private void CreateUpGameTile(Collision collision)
    {
        Destroy(collision.gameObject);
        _board.GameTiles.Remove(collision.gameObject.GetComponent<GameTile>());
        collision.gameObject.GetComponent<GameTile>().IsDie = true;
        gameObject.transform.position = collision.gameObject.transform.position;
        NumberGameTile++;
        _textNumber.text = NumberGameTile.ToString();
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