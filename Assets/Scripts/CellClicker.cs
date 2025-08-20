using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DefaultNamespace
{
    public class CellClicker : MonoBehaviour
    {
        private Camera _camera;

        private void Start()
        {
            _camera = Camera.main;
        }

        void Update()
        {
            // ЛКМ
            if (Input.GetMouseButtonUp(0))
                HandleClick(0);

            // ПКМ
            if (Input.GetMouseButtonUp(1))
                HandleClick(1);
        }

        void HandleClick(int button)
        {
            Debug.Log("dafsdfa");
            // Если клик попал на UI – выходим
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            // Получаем позицию мыши в мировых координатах
            Vector2 mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);

            // Создаем луч из камеры в точку мыши
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null)
            {
                Debug.Log("fasdfasd");
                if (hit.collider.gameObject.TryGetComponent(out Cell cell)) // проверяем именно наш объект
                {
                    Debug.Log("fasdfasd");
                    cell.Click(button);
                }
            }
        }
    }
}