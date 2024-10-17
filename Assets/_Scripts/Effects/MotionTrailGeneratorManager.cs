using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionTrailGeneratorManager : MonoBehaviour
{
    MotionTrailGenerator[] _generators;

    private void Awake()
    {
        _generators = GetComponentsInChildren<MotionTrailGenerator>();
    }

    public void On()
    {
        for (int i = 0; i < _generators.Length; i++)
        {
            _generators[i].On();
        }
    }

    public void Off()
    {
        for (int i = 0; i < _generators.Length; i++)
        {
            _generators[i].Off();
        }
    }
}
