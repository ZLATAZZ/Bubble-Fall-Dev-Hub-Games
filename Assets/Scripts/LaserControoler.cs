using UnityEngine;

public class LaserController : MonoBehaviour
{
    public Transform laserOrigin; // Откуда начинается лазер
    public LineRenderer lineRenderer; // Линия для визуализации лазера
    public float laserLength = 10f; // Длина луча
    public float followStrength = 0.5f; // Насколько сильно изгибается за мышью

    void Update()
    {
        // 1. Первая точка - исходная
        Vector3 startPoint = laserOrigin.position;

        // 2. Получаем позицию мыши в мире (на той же плоскости, что и laserOrigin)
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(laserOrigin.forward, laserOrigin.position);
        plane.Raycast(ray, out float enter);
        Vector3 mouseWorldPos = ray.GetPoint(enter);

        // 3. Считаем направление от старта до мыши
        Vector3 toMouse = (mouseWorldPos - startPoint).normalized;

        // 4. Получаем направление вперёд
        Vector3 forward = laserOrigin.forward;

        // 5. Интерполируем между направлением вперёд и направлением к мыши
        Vector3 blendedDir = Vector3.Slerp(forward, toMouse, followStrength);

        // 6. Вторая точка — направление + длина
        Vector3 endPoint = startPoint + blendedDir * laserLength;
        endPoint.y = -1f;

        // 7. Рисуем линию
        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);
    }
}
