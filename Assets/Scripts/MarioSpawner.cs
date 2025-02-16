using UnityEngine;

public class MarioSpawner : MonoBehaviour
{
    [SerializeField] private GameObject marioPrefab;
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private float minX = -2f;
    [SerializeField] private float maxX = 2f;
    [SerializeField] private float spawnHeight = 5f;
    [SerializeField] private int maxSimultaneousMarios = 5; // Maximum de Marios en même temps

    private float nextSpawnTime;

    private void Update()
    {
        // Ne spawn que si on n'a pas dépassé le maximum de Marios
        if (Time.time >= nextSpawnTime && CountActiveMarios() < maxSimultaneousMarios)
        {
            SpawnMario();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    private void SpawnMario()
    {
        Vector3 spawnPosition = new Vector3(
            Random.Range(minX, maxX),
            spawnHeight,
            0f
        );
        
        GameObject mario = Instantiate(marioPrefab, spawnPosition, Quaternion.identity);
        mario.transform.parent = transform; // Pour garder la hiérarchie propre
    }

    private int CountActiveMarios()
    {
        return transform.childCount;
    }
} 