using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Win : MonoBehaviour
{
    [SerializeField] private int _level = 0;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _coin;
    private const string SAFE_LEVEL = "safeLevel";

    public void InitializeLevel()
    {
        _level = PlayerPrefs.GetInt(SAFE_LEVEL);
        if (_level == 0)
            _level++;
        _levelText.text = _level.ToString();
    }

    public void NextLevel()
    {
      
        _level = PlayerPrefs.GetInt(SAFE_LEVEL);
        if (_level == 0)
        { 
            _level++;
        }
        _level++;
        Debug.Log(_level);
        _levelText.text =  _level.ToString();
        PlayerPrefs.SetInt(SAFE_LEVEL, _level);
    }
}
