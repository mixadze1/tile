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

    private bool _isMove;
    public int NumberGameTile;

    [SerializeField] private TextMeshProUGUI _textNumber;

    private bool _isRotationLeft;
    private bool _isRotationRight;
    private bool _isRotationForward;
    private bool _isRotationDown;

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
        if (_rigidbody.velocity.magnitude < 0.25f)
        {
            RotationOff();
            gameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
            _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            _isMove = false;
            ParticaleOff();
        }
        RotationOn();
    }   

    private void RotationOff()
    {
        _isRotationLeft = false;
        _isRotationRight = false;
        _isRotationForward = false;
        _isRotationDown = false;
    }
    
    private void RotationOn()
    {
        if (_isRotationLeft)
            gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, _rotation));

        if (_isRotationRight)
            gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -_rotation));
        if (_isRotationDown)
            gameObject.transform.rotation = Quaternion.Euler(new Vector3(-_rotation, 0, 0));
        if (_isRotationForward)
            gameObject.transform.rotation = Quaternion.Euler(new Vector3(_rotation, 0, 0));
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
                _rigidbody.constraints = RigidbodyConstraints.FreezePositionY |
                    RigidbodyConstraints.FreezePositionZ |
                    RigidbodyConstraints.FreezeRotationX | 
                    RigidbodyConstraints.FreezeRotationY;

                _isRotationLeft = true;
                _rigidbody.velocity = Vector3.left * _speed;
                _particaleLeft.gameObject.SetActive(true);
            }
            if (swipes[(int)SwipeManager.Direction.Right])
            {
                _rigidbody.constraints = RigidbodyConstraints.FreezePositionY |
                       RigidbodyConstraints.FreezePositionZ |
                       RigidbodyConstraints.FreezeRotationX |
                       RigidbodyConstraints.FreezeRotationY;

                _isRotationRight = true;
                _rigidbody.velocity = Vector3.right * _speed;
                _particaleRight.gameObject.SetActive(true);
            }

            if (swipes[(int)SwipeManager.Direction.Up])
            {
                _rigidbody.constraints = RigidbodyConstraints.FreezePositionY |
                       RigidbodyConstraints.FreezePositionX |
                       RigidbodyConstraints.FreezeRotationZ |
                       RigidbodyConstraints.FreezeRotationY;

                _isRotationForward = true;
               _rigidbody.velocity = Vector3.forward * _speed;
                _particaleForward.gameObject.SetActive(true);
            }
            if (swipes[(int)SwipeManager.Direction.Down])
            {
                _rigidbody.constraints = RigidbodyConstraints.FreezePositionY |
                       RigidbodyConstraints.FreezePositionX |
                       RigidbodyConstraints.FreezeRotationZ |
                       RigidbodyConstraints.FreezeRotationY;

                _isRotationDown = true;
                _rigidbody.velocity = -Vector3.forward * _speed;
                _particaleDown.gameObject.SetActive(true);
            }
        }
        catch
        {
            return;
        }
        
    }

    private IEnumerator Move(Vector3 side)
    {
        yield return new WaitForFixedUpdate();
        

    }

    private void ParticaleOff()
    {
        _particaleLeft.gameObject.SetActive(false);
        _particaleDown.gameObject.SetActive(false);
        _particaleRight.gameObject.SetActive(false);
        _particaleForward.gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.GetComponent<GameTile>() &&
            collision.gameObject.GetComponent<SphereCollider>().radius <= 0.15f)
        {
            if (collision.gameObject.GetComponent<GameTile>().NumberGameTile == NumberGameTile  )
            {         
                Destroy(collision.gameObject);
                _board.GameTiles.Remove(collision.gameObject.GetComponent<GameTile>());
                collision.gameObject.GetComponent<GameTile>().IsDie = true;

                NumberGameTile++;
                _textNumber.text = NumberGameTile.ToString();
            }            
        }
    }

    private void CreateUpGameTile()
    {
        Instantiate(this);
    }
}

public enum GameTileType
{
  
}