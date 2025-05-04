using UnityEngine;

public class Draggable : MonoBehaviour
{
    private Vector3 offset;
    private Vector3 initialPos;
    private LineRenderer lineRenderer;
    public static bool draggingFlag = false;
    public static Draggable current;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

    }


    // Update is called once per frame

    private void Update()
    {

    }
    private void OnMouseDown()
    {

        draggingFlag = true;
        //offset = transform.position - GetMouseWorldPosition();
        initialPos = transform.position;
        current = this;

        lineRenderer.enabled = true;

        Debug.Log("Mouse Down");
    }


    private void OnMouseDrag()
    {
        if (draggingFlag)
        {

            lineRenderer.SetPosition(0, new Vector3(initialPos.x - .3f, initialPos.y - .33f, 0));
            lineRenderer.SetPosition(1, GetMouseWorldPosition());
        }
    }

    private void OnMouseUp()
    {
        draggingFlag = false;
        lineRenderer.enabled = false;
        current = null;
        transform.position = GetMouseWorldPosition();
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return mousePos;

    }
}
