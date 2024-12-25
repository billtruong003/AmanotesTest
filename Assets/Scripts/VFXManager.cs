using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance;

    public GameObject hitEffectPrefab;
    public GameObject missEffectPrefab;
    public GameObject shieldFXPrefab;

    public int hitPoolSize = 10;
    public int missPoolSize = 10;

    private List<GameObject> hitEffectPool;
    private List<GameObject> missEffectPool;

    public enum VFXType
    {
        Hit,
        Miss,
        Shield
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {

    }

    private void InitializePools()
    {
        hitEffectPool = new List<GameObject>();
        for (int i = 0; i < hitPoolSize; i++)
        {
            GameObject obj = Instantiate(hitEffectPrefab);
            obj.SetActive(false);
            hitEffectPool.Add(obj);
        }

        missEffectPool = new List<GameObject>();
        for (int i = 0; i < missPoolSize; i++)
        {
            GameObject obj = Instantiate(missEffectPrefab);
            obj.SetActive(false);
            missEffectPool.Add(obj);
        }
    }

    public void PlayVFX(VFXType vfxType, Vector3 position)
    {
        switch (vfxType)
        {
            case VFXType.Hit:
                SpawnFromPool(hitEffectPool, hitEffectPrefab, position);
                break;
            case VFXType.Miss:
                SpawnFromPool(missEffectPool, missEffectPrefab, position);
                break;
            case VFXType.Shield:
                break;
        }
    }

    private void SpawnFromPool(List<GameObject> pool, GameObject prefab, Vector3 position)
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].activeInHierarchy)
            {
                pool[i].transform.position = position;
                pool[i].SetActive(true);
                StartCoroutine(ReturnToPool(pool[i], 1f));
                return;
            }
        }

        // If no inactive object is found, instantiate a new one (optional, can be adjusted based on needs)
        GameObject newObj = Instantiate(prefab, position, Quaternion.identity);
        StartCoroutine(ReturnToPool(newObj, 1f));
        if (pool == hitEffectPool)
        {
            hitEffectPool.Add(newObj);
        }
        else if (pool == missEffectPool)
        {
            missEffectPool.Add(newObj);
        }
    }

    private IEnumerator ReturnToPool(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
    }

    public void DisableAllVFXOfType(VFXType vfxType)
    {
        switch (vfxType)
        {
            case VFXType.Hit:
                foreach (var obj in hitEffectPool)
                {
                    obj.SetActive(false);
                }
                break;
            case VFXType.Miss:
                foreach (var obj in missEffectPool)
                {
                    obj.SetActive(false);
                }
                break;
            case VFXType.Shield:
                shieldFXPrefab.SetActive(false);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}