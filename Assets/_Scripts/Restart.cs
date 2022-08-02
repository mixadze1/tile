using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Restart : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _restartTimerText;
    [SerializeField] private int _timerToCanRestart;
    [SerializeField] private Animator _animator;
    private Coroutine _coroutine;
    private int _rememberTimer;
    private GameBoard _board;
    public void Initialize(GameBoard board)
    {
        _animator.enabled = false;
        _board = board;
        _rememberTimer = 15;
         StartCoroutine(CountDownToCanRestart());
        _restartTimerText.text = _timerToCanRestart.ToString();
    }

    public void RestartGame()
    {
        if (_timerToCanRestart == 0 && !_board.IsWin())
        {
            _animator.enabled = false;
            _timerToCanRestart = _rememberTimer;
            _restartTimerText.text = _timerToCanRestart.ToString();
            _board.RestartGame();
            StartCoroutine(CountDownToCanRestart());

        }
        
    }

    private IEnumerator CountDownToCanRestart()
    {
        while(_timerToCanRestart > 0)
        {
            yield return new WaitForSeconds(1);
            _timerToCanRestart--;
            if (_timerToCanRestart < 0)
                _timerToCanRestart = 0;
            _restartTimerText.text = "";
        }
        _animator.enabled = true;

    }
}
