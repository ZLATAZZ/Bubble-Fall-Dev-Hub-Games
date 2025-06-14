using UnityEngine;

public class LaserController : MonoBehaviour
{
    public Transform laserOrigin; // ������ ���������� �����
    public LineRenderer lineRenderer; // ����� ��� ������������ ������
    public float laserLength = 10f; // ����� ����
    public float followStrength = 0.5f; // ��������� ������ ���������� �� �����

    private Vector3 _laserDirection;

    /// <summary>
    /// ������� ����������� ������ (� �������������� ���������).
    /// </summary>
    public Vector3 LaserDirection { get => _laserDirection; }


    /// <summary>
    /// ��������� ������������ � ����������� ������.
    /// </summary>
    void Update()
    {
        // 1. ������ ����� - ��������
        Vector3 startPoint = laserOrigin.position;

        // 2. ������� ���� � ���� (�� ��� �� ���������, ��� � laserOrigin)
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(laserOrigin.forward, laserOrigin.position);
        plane.Raycast(ray, out float enter);
        Vector3 mouseWorldPos = ray.GetPoint(enter);

        // 3. ����������� �� ������ �� ����
        Vector3 toMouse = (mouseWorldPos - startPoint).normalized;

        // 4. ����������� �����
        Vector3 forward = laserOrigin.forward;

        // 5. ������������� ����� ������������ ����� � ������������ � ����
        Vector3 blendedDir = Vector3.Slerp(forward, toMouse, followStrength);

        // 6. ������ ����� � ����������� + �����
        Vector3 endPoint = startPoint + blendedDir * laserLength;
        endPoint.y = -1f;
        _laserDirection = (endPoint - startPoint).normalized;
        _laserDirection.y = 0f;

        // 7. ����� ������
        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);
    }
}


