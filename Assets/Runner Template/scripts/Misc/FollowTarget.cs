using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target;

    public Vector3 offset;
    public bool lockX, lockY, lockZ;

    private Vector3 initPosition = Vector3.zero;
    public bool useLocalPosition, followRotation;
    // Start is called before the first frame update
    void Start()
    {
        initPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        var v = useLocalPosition ? transform.localPosition : transform.position;

        v = useLocalPosition ? target.localPosition + offset : target.position + offset;

        if (lockX)
            v.x = initPosition.x + offset.x;
        if (lockY)
            v.y = initPosition.y + offset.y;
        if (lockZ)
            v.z = initPosition.z + offset.z;

        if (useLocalPosition)
            transform.localPosition = v;
        else
            transform.position = v;

        if (followRotation)
            transform.rotation = target.rotation;
    }
}
