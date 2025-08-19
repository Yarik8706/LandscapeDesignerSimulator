using UnityEngine;

public class CameraDrag : MonoBehaviour
{
    public float dragSpeed = 5f; // скорость перемещения камеры

    private Vector3 dragOrigin;
    private bool isDragging = false;

    void Update()
    {
        // Нажатие ПКМ
        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = Input.mousePosition;
            isDragging = true;
        }

        // Отпустили ПКМ
        if (Input.GetMouseButtonUp(1))
        {
            isDragging = false;
        }

        // Перемещение
        if (isDragging)
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
            Vector3 move = new Vector3(-pos.x * dragSpeed, -pos.y * dragSpeed, 0);

            transform.Translate(move, Space.World);

            dragOrigin = Input.mousePosition;
        }
    }
}