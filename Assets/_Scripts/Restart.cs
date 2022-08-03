using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Restart : MonoBehaviour
{
  /*  [SerializeField] private int _timerToCanRestart;
    [SerializeField] private Animator _animator;
    private int _rememberTimer;
    private GameBoard _board;
    public void Initialize(GameBoard board)
    {
        _animator.enabled = false;
        _board = board;
        _rememberTimer = 3;
        StartCoroutine(CountDownToCanRestart());
    }

    private void LateUpdate()
    {
        if (_board.IsWin())
            StartCoroutine(CountDownToCanRestart());
    }

    public void RestartGame()
    {
        if (!_board.IsWin())
        {
            _animator.enabled = false;
            _timerToCanRestart = _rememberTimer;
            _board.RestartGame();
            StartCoroutine(CountDownToCanRestart());
        }

    }

    private IEnumerator CountDownToCanRestart()
    {
        _timerToCanRestart = 15;
        while (_timerToCanRestart > 0)
        {
            yield return new WaitForSeconds(1);
            _timerToCanRestart--;
            if (_timerToCanRestart < 0)
                _timerToCanRestart = 0;
        }
        _animator.enabled = true;

    }*/
}
