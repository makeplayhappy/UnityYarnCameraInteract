using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class TouchcastSettings : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isInteractable = true;
    public Transform touchTransform;

    [HideInInspector]
    public Vector3 startPosition;


    void Awake(){

        startPosition = touchTransform.position;
        
    }

}
