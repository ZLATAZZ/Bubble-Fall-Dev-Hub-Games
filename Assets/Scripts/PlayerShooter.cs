using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    bool isAiming = false;
    public Transform shootPoint;
    public ObjectPool ballsPool; 
    public float shootForce = 10f;
    [SerializeField] private LaserController _laser;
    [SerializeField] private ColorsController colorsController;
    GameObject ballGO;


    void Update()
    {
        // ������ ���
        if (Input.GetMouseButtonDown(0))
        {
            ballGO = ballsPool.Get();
            ballGO.transform.position = shootPoint.position;
            ballGO.transform.rotation = Quaternion.identity;
            ballGO.GetComponent<BallController>().Init(colorsController.GetRandomColor(), ballsPool);
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
       
        Vector3 direction = _laser.LaserDirection.normalized;

        // ��������
        ballGO.GetComponent<BallController>().Shoot(direction, shootForce);
      

    }

}
