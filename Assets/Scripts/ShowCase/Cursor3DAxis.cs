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

    private Vector3 PrevMousePos;
    private bool IsDragging;

    public void Setup(CursorAxis axis, Action<Vector3> onDrag, LayerMask mask)
    {
        CursorAxis = axis;
        OnDrag = onDrag;
        Mask = mask;
    }

    private void FixedUpdate()
    {
        if (IsDragging)
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3 delta = GetMovementDelta(PrevMousePos, mousePos);
            OnDrag?.Invoke(delta);
            PrevMousePos = mousePos;
        }
    }

    private void OnMouseDown()
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

    private void OnMouseUp()
    {
        PrevMousePos = Vector3.zero;
        IsDragging = false;
    }

    private Vector3 GetMovementDelta(Vector3 prevPos, Vector3 currentPos)
    {
        Vector3 worldPrevPos = Camera.main.ScreenToWorldPoint(prevPos);
        Vector3 worldCurrPos = Camera.main.ScreenToWorldPoint(currentPos);
        Vector3 delta;
        switch (CursorAxis)
        {
            case CursorAxis.X:
                delta = new Vector3(worldCurrPos.x - worldPrevPos.x, 0, 0);
                break;
            case CursorAxis.Y:
                delta = new Vector3(0, worldCurrPos.y - worldPrevPos.y, 0);
                break;
            case CursorAxis.Z:
                delta = new Vector3(0, 0, worldCurrPos.z - worldPrevPos.z);
                break;
            default:
                throw new Exception($"Invalid axis {CursorAxis}");
        }
        Debug.LogError($"{prevPos.ToString(5)}->{currentPos.ToString(5)} -> {worldPrevPos.ToString(5)}->{worldCurrPos.ToString(5)} = {delta.ToString(5)}");
        return delta;
    }

    public void SetActive(bool enable)
    {
        gameObject.SetActive(enable);
    }
}
