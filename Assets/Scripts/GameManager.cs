using System;
using System.Collections;
using System.Collections.Generic;
using Core.Math;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;
    private BigNumber _startDamage = new BigNumber("10");
    private BigNumber _currentScore = BigNumber.Zero;
    private int _clicksCount = 0;
    
    private BigNumber _score = BigNumber.Zero;
    
    public static GameManager Instance
    {
        get => _instance;
    }

    private static GameManager _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _scoreText.text = _currentScore.ToString();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnClick()
    {
        _clicksCount++;
        BigNumber damage = (_startDamage * (100 + 10*_clicksCount*_clicksCount)) / 100;
        bool isCriticalHit = Random.Range(0, 100) < 30;
        if (isCriticalHit)
        {
            damage = damage * 2;
        }

        DamagePopup.Create(new Vector3(Random.Range(-12.0f,4),Random.Range(-24.0f,24)), damage, isCriticalHit);
        _currentScore = _currentScore + damage;
        _scoreText.text = _currentScore.ToString();
    }
}
