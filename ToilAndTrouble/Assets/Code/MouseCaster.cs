using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCaster : MonoBehaviour
{
    public LayerMask grabbablesMask;  

    Camera mainCamera;
  
    void Start()
    {
        mainCamera = Camera.main;
    }

    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit;

            if (hit = Physics2D.GetRayIntersection(mainCamera.ScreenPointToRay(Input.mousePosition)))
            {
                if (hit.transform.tag == "Grabbable" || hit.transform.tag == "Ingredient")
                hit.transform.GetComponent<GrabbableObject>().Grabbed();
            }
                
        }
    }
}
