using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeManager : MonoBehaviour
{
    public static SwipeManager instance;
    public enum Direction { Left, Right, Up, Down };

    bool[] swipe = new bool[4];
    bool touchMoved;

    Vector2 _startTouch;
    Vector2 _swipeDelta;

    const float SWIPE_THRFSHOLD = 40;

    public delegate void MoveDelegate(bool[] swipes);
    public MoveDelegate MoveEvent;

    public delegate void ClickDelegate(Vector2 pos);
    public ClickDelegate ClickEvent;

   private Vector2 TouchPosition()
    {
        return (Vector2)Input.mousePosition;
    }

    private bool TouchBegan()
    {
        return Input.GetMouseButtonDown(0);
    }
    private bool TouchEnded()
    {
        return Input.GetMouseButtonUp(0);
    }
    private bool GetTouch()
    {
        return Input.GetMouseButton(0);
    }
    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) 
            return; 

        if (TouchBegan())
        {
            _startTouch = TouchPosition();
            touchMoved = true;
        }
        else if (TouchEnded() && touchMoved == true)
        {
            SendSwipe();
            touchMoved = false;
        }
        _swipeDelta = Vector2.zero;
        if (touchMoved && GetTouch())
        {
            _swipeDelta = TouchPosition() - _startTouch;
        }
        if (_swipeDelta.magnitude > SWIPE_THRFSHOLD)
        {
            if (Mathf.Abs(_swipeDelta.x) > Mathf.Abs(_swipeDelta.y))
            {
                swipe[(int)Direction.Left] = _swipeDelta.x < 0;
                swipe[(int)Direction.Right] = _swipeDelta.x > 0;
            }
            else
            {
                swipe[(int)Direction.Down] = _swipeDelta.y < 0;
                swipe[(int)Direction.Up] = _swipeDelta.y > 0;
            }
            SendSwipe();
        }
    }
    private void SendSwipe()
    {
        if (swipe[0] || swipe[1] || swipe[2] || swipe[3])
        {
            Debug.Log(swipe[0] + "|" + swipe[1] + "|" + swipe[2] + "|" + swipe[3]);
            MoveEvent?.Invoke(swipe); 
        }
        else
        {
            Debug.Log("Click");
            ClickEvent?.Invoke(TouchPosition());
        }
        Reset();
    }
    private void Reset()
    {
        _startTouch = _swipeDelta = Vector2.zero;
        touchMoved = false;
        for (int i = 0; i < 4; i++)
        {
            swipe[i] = false;
        }
    }
}