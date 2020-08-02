using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvPatch : MonoBehaviour
{
    public Transform nextPatch;
    public Vector3 Offset;

    public Vector3 initialPosition;
    public bool reset, initiated;

    private void Start()
    {
        initialPosition = transform.localPosition;

        GameManager.instance.OnGameStarted += OnGameStarted;
    }

    void OnGameStarted()
    {
        initiated = true;
    }
     /// <summary>
     /// This function is called when the behaviour becomes disabled or inactive.
     /// </summary>
     void OnDisable()
     {
        GameManager.instance.OnGameStarted -= OnGameStarted;
     }

    private void OnTriggerEnter(Collider other)
    {
        if (!initiated)
            return;

        if (other.tag == "Player" && !reset)
        {
            nextPatch.localPosition = transform.localPosition + Offset;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        reset = false;
    }

    public void ResetPosition()
    {
        reset = true;
        transform.localPosition = initialPosition;
        //nextPatch.localPosition = transform.localPosition + Offset;
    }
}
