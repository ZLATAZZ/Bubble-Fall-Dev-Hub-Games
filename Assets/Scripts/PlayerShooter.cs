using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    bool isAiming = false;
    public Transform shootPoint;
    public ObjectPool ballsPool; 
    public float shootForce = 10f;
    public GameObject _gridParent;
    [SerializeField] private LaserController _laser;
    [SerializeField] private ColorsController colorsController;
    GameObject ballGO;
    public static PlayerShooter Instance { get; private set; }

    public int matchCount = 3;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    void Update()
    {
        // Нажали ЛКМ
        if (Input.GetMouseButtonDown(0))
        {
            ballGO = ballsPool.Get();
            ballGO.transform.position = shootPoint.position;
            ballGO.transform.rotation = Quaternion.identity;
            ballGO.GetComponent<BallController>().Init(colorsController.GetRandomColor(), ballsPool);
            isAiming = true;
        }

        // Отпустили ЛКМ — стреляем
        if (Input.GetMouseButtonUp(0) && isAiming)
        {
            FireLaser();
            isAiming = false;
        }

        
    }

    void FireLaser()
    {
        Debug.Log("Пиу!Выстрел выполнен");
       
        Vector3 direction = _laser.LaserDirection.normalized;

        // Стреляем
        ballGO.GetComponent<BallController>().Shoot(direction, shootForce);
      

    }

}
