using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSelecter : MonoBehaviour
{
    private GameObject selectedObject;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                selectedObject = hit.transform.gameObject;
                if(selectedObject.IsInterface(out ISelectable selectable))
                    Cursor3D.Instance.Select(selectable);
            }
        }
    }
}
