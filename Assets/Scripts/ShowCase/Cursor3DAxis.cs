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

    private void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.LogError("Mouse button down");
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
            Debug.LogError("Mouse button up");
            PrevMousePos = Vector3.zero;
            IsDragging = false;
        }

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
        switch (CursorAxis)
        {
            case CursorAxis.X:
                delta = new Vector3(delta.x.Sign() * delta.magnitude, 0, 0);
                break;
            case CursorAxis.Y:
                delta = new Vector3(0, delta.y.Sign() * delta.magnitude, 0);
                break;
            case CursorAxis.Z:
                delta = new Vector3(0, 0, delta.z.Sign() * delta.magnitude);
                break;
            default:
                throw new Exception($"Invalid axis {CursorAxis}");
        }
        return -delta;
    }

    public void SetActive(bool enable)
    {
        gameObject.SetActive(enable);
    }
}
