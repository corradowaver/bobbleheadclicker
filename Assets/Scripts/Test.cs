using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class Test : MonoBehaviour
{
    [SerializeField] private float _force = 1;
    [SerializeField] private AudioClip _clap;
    [SerializeField] private AudioSource _audioSource;

    [SerializeField] private StressReceiver _shakeObj;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                MakeClick();
            }
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            MakeClick();
        }
    }

    private void MakeClick()
    {
        _rigidbody.AddForce(Vector3.forward*_force*2,ForceMode.Impulse);
        _rigidbody.AddForce(Vector3.right*UnityEngine.Random.Range(-_force,_force),ForceMode.Impulse);
        _audioSource.PlayOneShot(_clap);
        _shakeObj.InduceStress(0.5f);
        GameManager.Instance.OnClick();
    }
}
