using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor3D : MonoBehaviour
{
    private static Cursor3D _instance;
    public static Cursor3D Instance { get 
        {
            if (_instance == null)
                _instance = FindObjectOfType<Cursor3D>();
            return _instance;
        } }

    [SerializeField] private string AxisCursorLayer;
    [SerializeField] private Cursor3DAxis XAxis;
    [SerializeField] private Cursor3DAxis YAxis;
    [SerializeField] private Cursor3DAxis ZAxis;

    private ISelectable SelectedObject;

    private void Start()
    {
        BindAxes();
        Hide();
    }

    private void BindAxes()
    {
        var mask = LayerMask.GetMask(AxisCursorLayer);
        XAxis.Setup(CursorAxis.X, Drag, mask);
        YAxis.Setup(CursorAxis.Y, Drag, mask);
        ZAxis.Setup(CursorAxis.Z, Drag, mask);
    }

    private void Drag(Vector3 delta)
    {
        if (SelectedObject != null)
        {
            SelectedObject.Move(delta);
        }
    }



    public void Point(Vector3 position)
    {
        transform.position = position;
    }

    public void Select(ISelectable obj)
    {
        SelectedObject = obj;
        Show(obj.GetPosition());
    }

    public void Show(Vector3 position)
    {
        Show();
        Point(position);
    }

    public void Show()
    {
        Enable(true);
    }

    public void Hide()
    {
        SelectedObject = null;
        Enable(false);
    }

    private void Enable(bool enable)
    {
        XAxis.SetActive(enable);
        ZAxis.SetActive(enable);
        YAxis.SetActive(enable);
    }
}
