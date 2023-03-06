using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class floatingTextComp : MonoBehaviour
{
    public void DestroyThisFloatingText() 
    {
        Destroy(gameObject);
    }


    private void Update()
    {
        transform.LookAt(Camera.main.transform);
    }
}
