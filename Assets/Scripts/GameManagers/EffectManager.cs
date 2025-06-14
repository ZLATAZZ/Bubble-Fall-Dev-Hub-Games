using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance { get; private set; }

    [Header("Particles")]
    public GameObject reflectParticle;

    private void Awake()
    {
        // ��������
        if (Instance != null && Instance != this)
        {
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

    }

    /// <summary>
    /// ������� ������� �� �������
    /// </summary>
    public void SpawnParticle(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab != null)
        {
            Instantiate(prefab, position, rotation);
        }
    }

    /// <summary>
    /// ������� ���������� �������
    /// </summary>
    public void SpawnReflect(Vector3 position, Vector3 normal)
    {
        if (reflectParticle != null)
        {
            Quaternion rot = Quaternion.LookRotation(normal);
            Instantiate(reflectParticle, position, rot);
        }
    }
}
