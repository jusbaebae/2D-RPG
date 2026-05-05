using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepUIRotation : MonoBehaviour
{
    private Vector3 initialScale;

    void Start()
    {
        initialScale = transform.localScale;
    }
    void LateUpdate()
    {
        transform.rotation = Quaternion.identity;

        if (transform.parent != null)
        {
            Vector3 parentScale = transform.parent.lossyScale;
            transform.localScale = new Vector3(parentScale.x < 0 ? -initialScale.x : initialScale.x,initialScale.y, initialScale.z);
        }
    }
}
