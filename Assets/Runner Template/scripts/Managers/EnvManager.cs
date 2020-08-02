using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvManager : MonoBehaviour
{
    public static EnvManager instance;

    public EnvPatch[] patches;
    public Transform playerTransform;
    public Vector3 initialPlayerPosition;

    public Vector3 cameraInitialPosition;

    // public MT_MeshTrail.MT_MeshTrailRenderer mT_MeshTrailRenderer;
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        patches = GetComponentsInChildren<EnvPatch>(true);
        initialPlayerPosition = playerTransform.position;

        cameraInitialPosition = Camera.main.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTransform.position.z > 500)
        {
            var pos = playerTransform.position;
            pos.z = initialPlayerPosition.z;
            playerTransform.position = pos;
            foreach (var p in patches)
            {
                p.ResetPosition();
            }
            Camera.main.transform.position = cameraInitialPosition;
            Invoke("initialise", 1);
            Debug.Log("Positions reset.");
        }
    }

    void initialise()
    {
        foreach (var p in patches)
        {
            p.reset = false;
        }

        // mT_MeshTrailRenderer.Clear();
    }
}
