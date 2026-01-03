using System.Collections;
using Unity.XR.CoreUtils;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class LockXRHeight : MonoBehaviour
{
    public float fixedHeight = 1.8f;
    private CharacterController cc;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    void LateUpdate()
    {
        if (cc)
        {
            cc.height = fixedHeight;
            cc.center = new Vector3(0f, fixedHeight / 2f, 0f);
        }
    }
}
