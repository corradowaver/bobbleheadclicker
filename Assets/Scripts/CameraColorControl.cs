using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Camera))]
public class CameraColorControl : MonoBehaviour
{
    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private float _timeLeft;
    private  Color _targetColor;
 
    void Update()
    {
        if (_timeLeft <= Time.deltaTime)
        {
            // transition complete
            // assign the target color
            _camera.backgroundColor = _targetColor;
 
            // start a new transition
            _targetColor = new Color(Random.value, Random.value, Random.value);
            _timeLeft = 1.0f;
        }
        else
        {
            // transition in progress
            // calculate interpolated color
            _camera.backgroundColor = Color.Lerp(_camera.backgroundColor, _targetColor, Time.deltaTime / _timeLeft);
 
            // update the timer
            _timeLeft -= Time.deltaTime;
        }
    }

}
