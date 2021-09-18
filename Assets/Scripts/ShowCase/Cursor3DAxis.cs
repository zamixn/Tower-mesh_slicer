using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CursorAxis { X, Y, Z }

public class Cursor3DAxis : MonoBehaviour
{
    private CursorAxis CursorAxis;
    private Action<Vector3> OnDrag;
    private LayerMask Mask;

    private float ZPos;
    private Vector3 PrevMousePos;
    private bool IsDragging;

    public void Setup(CursorAxis axis, Action<Vector3> onDrag, LayerMask mask)
    {
        CursorAxis = axis;
        OnDrag = onDrag;
        Mask = mask;
    }

    public void SetZ(float z)
    {        
        ZPos = Camera.main.transform.position.z - z;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var objectsHit = Physics.RaycastAll(ray, Mathf.Infinity, Mask);
            foreach (var obj in objectsHit)
            {
                if (obj.transform.gameObject == this.gameObject)
                {
                    IsDragging = true;
                    PrevMousePos = Input.mousePosition;
                    break;
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            PrevMousePos = Vector3.zero;
            IsDragging = false;
        }
    }

    private void FixedUpdate()
    {
        if (IsDragging)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = ZPos;
            PrevMousePos.z = ZPos;
            Vector3 delta = GetMovementDelta(PrevMousePos, mousePos);
            OnDrag?.Invoke(delta);
            PrevMousePos = mousePos;
        }
    }

    private Vector3 GetMovementDelta(Vector3 prevPos, Vector3 currentPos)
    {
        Vector3 worldPrevPos = Camera.main.ScreenToWorldPoint(prevPos);
        Vector3 worldCurrPos = Camera.main.ScreenToWorldPoint(currentPos);
        Vector3 delta = worldCurrPos - worldPrevPos;
        Vector3 normalVector;

        switch (CursorAxis)
        {
            case CursorAxis.X:
                normalVector = Vector3.right;
                break;
            case CursorAxis.Y:
                normalVector = Vector3.up;
                break;
            case CursorAxis.Z:
                normalVector = Vector3.forward;
                break;
            default:
                throw new Exception($"Invalid axis {CursorAxis}");
        }
        delta = Vector3.Project(delta, normalVector);
        return -delta;
    }

    public void SetActive(bool enable)
    {
        gameObject.SetActive(enable);
    }
}
