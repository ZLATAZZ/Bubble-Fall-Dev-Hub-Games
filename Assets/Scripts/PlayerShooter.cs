using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    bool isAiming = false;
    public Transform shootPoint;
    public ObjectPool ballsPool; 
    public float shootForce = 10f;


    void Update()
    {
        // ������ ���
        if (Input.GetMouseButtonDown(0))
        {
            isAiming = true;
        }

        // ��������� ��� � ��������
        if (Input.GetMouseButtonUp(0) && isAiming)
        {
            FireLaser();
            isAiming = false;
        }

        
    }

    void FireLaser()
    {
        Debug.Log("���!������� ��������");
        GameObject ballGO = ballsPool.Get();
        ballGO.transform.position = shootPoint.position;
        ballGO.transform.rotation = Quaternion.identity;
        Vector3 direction = shootPoint.forward;

        // ��������
        ballGO.GetComponent<BallController>().Shoot(direction, shootForce);

    }

}
