using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class PointSource : MonoBehaviour
{
    public UnityEvent OnDataChanged;

    [SerializeField] protected Color standardDisplayColor = Color.white;
    protected Vector3 min, max;

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        min = new Vector3(transform.localScale.x * -5f + transform.position.x, 0f, transform.localScale.z * -5f + transform.position.z);
        max = new Vector3(transform.localScale.x * 5f + transform.position.x, 0f, transform.localScale.z * 5f + transform.position.z);
    }

    public Color GetColor()
    {
        return standardDisplayColor;
    }

    public void SendDataChangedEvent()
    {
        OnDataChanged.Invoke();
    }

    public abstract List<Vector3> GetPoints();
    public abstract List<Vector3> GetNewPoints();
    public void PushPoints(Vector3 amount)
    {
        List<Vector3> points = GetPoints();
        for (int i = 0; i < points.Count; i++)
        {
            points[i] += amount;
        }
    }
}
