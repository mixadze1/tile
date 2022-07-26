using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameTile : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
 
    [SerializeField] private float _speed;
    [SerializeField] private Transform _particaleLeft;
    [SerializeField] private Transform _particaleRight;
    [SerializeField] private Transform _particaleForward;
    [SerializeField] private Transform _particaleDown;
    [SerializeField,Range(10f,30f)] private float _rotation;
    private GameBoard _board;
    private float _speedRotation = 12;
    private bool _isMove;
    public int NumberGameTile;

    [SerializeField] private TextMeshProUGUI _textNumber;

    private bool _isMoveLeft;
    private bool _isMoveRight;
    private bool _isMoveForward;
    private bool _isMoveDown;

    public bool IsDie;

    private void Awake()
    {
       
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.isKinematic = true;
        SwipeManager.instance.MoveEvent += MoveGameTile;
        _textNumber.text = NumberGameTile.ToString();
    }

    public void Initialize(GameBoard gameBoard)
    {
        _board = gameBoard;
    }


    private void Update()
    {
        if (_rigidbody.velocity.magnitude < 0.25f && gameObject.transform.rotation != Quaternion.Euler(Vector3.zero))
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
        if (collision.gameObject.GetComponent<GameTile>())
        {
            if (collision.gameObject.GetComponent<GameTile>().NumberGameTile == NumberGameTile   )
            {        
                if ((_isMoveDown || _isMoveForward) &&
                    Mathf.Abs( gameObject.transform.position.x) - Mathf.Abs( collision.gameObject.transform.position.x) < 0.3f
                    && Mathf.Abs(gameObject.transform.position.x) - Mathf.Abs(collision.gameObject.transform.position.x) > -0.3f)
                {
                    CreateUpGameTile(collision);
                }
                if((_isMoveLeft || _isMoveRight) &&
                    Mathf.Abs(gameObject.transform.position.z) - Mathf.Abs(collision.gameObject.transform.position.z) < 0.3f
                    &&
                    Mathf.Abs(gameObject.transform.position.z) - Mathf.Abs(collision.gameObject.transform.position.z) > -0.3f)
                {
                    CreateUpGameTile(collision);
                }
            }            
        }
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