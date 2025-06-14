using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������� ��� ��������. ��������� ���������������� �������� ������ �� ����������� � ������������.
/// </summary>
public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int initialSize = 10;

    private readonly Queue<GameObject> pool = new Queue<GameObject>();

    private void Awake()
    {
        if (prefab == null)
        {
            Debug.LogError("ObjectPool: Prefab is not assigned!");
            return;
        }

        for (int i = 0; i < initialSize; i++)
        {
            AddObjectToPool();
        }
    }

    /// <summary>
    /// ��������� ����� ������ � ���.
    /// </summary>
    private GameObject AddObjectToPool()
    {
        GameObject obj = Instantiate(prefab, transform);
        obj.SetActive(false);
        pool.Enqueue(obj);
        return obj;
    }

    /// <summary>
    /// �������� ������ �� ����.
    /// </summary>
    public GameObject Get()
    {
        if (prefab == null)
        {
            Debug.LogError("ObjectPool: Cannot spawn object because prefab is null.");
            return null;
        }

        if (pool.Count == 0)
        {
            AddObjectToPool();
        }

        GameObject obj = pool.Dequeue();
        obj.SetActive(true);
        return obj;
    }

    /// <summary>
    /// ���������� ������ ������� � ���.
    /// </summary>
    public void ReturnToPool(GameObject obj)
    {
        if (obj == null)
        {
            Debug.LogWarning("ObjectPool: Tried to return a null object.");
            return;
        }

        obj.SetActive(false);
        obj.transform.SetParent(transform);
        pool.Enqueue(obj);
    }
}
