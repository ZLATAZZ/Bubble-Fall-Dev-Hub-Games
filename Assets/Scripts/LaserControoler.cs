using UnityEngine;

public class LaserController : MonoBehaviour
{
    public Transform laserOrigin; // Откуда начинается лазер
    public LineRenderer lineRenderer; // Линия для визуализации лазера
    public float laserLength = 10f; // Длина луча
    public float followStrength = 0.5f; // Насколько сильно изгибается за мышью

    private Vector3 _laserDirection;

    /// <summary>
    /// Текущее направление лазера (в горизонтальной плоскости).
    /// </summary>
    public Vector3 LaserDirection { get => _laserDirection; }


    /// <summary>
    /// Обновляет визуализацию и направление лазера.
    /// </summary>
    void Update()
    {
        // 1. Первая точка - исходная
        Vector3 startPoint = laserOrigin.position;

        // 2. Позиция мыши в мире (на той же плоскости, что и laserOrigin)
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(laserOrigin.forward, laserOrigin.position);
        plane.Raycast(ray, out float enter);
        Vector3 mouseWorldPos = ray.GetPoint(enter);

        // 3. Направление от старта до мыши
        Vector3 toMouse = (mouseWorldPos - startPoint).normalized;

        // 4. Направление вперёд
        Vector3 forward = laserOrigin.forward;

        // 5. Интерполируем между направлением вперёд и направлением к мыши
        Vector3 blendedDir = Vector3.Slerp(forward, toMouse, followStrength);

        // 6. Вторая точка — направление + длина
        Vector3 endPoint = startPoint + blendedDir * laserLength;
        endPoint.y = -1f;
        _laserDirection = (endPoint - startPoint).normalized;
        _laserDirection.y = 0f;

        // 7. Линия лазера
        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);
    }
}


