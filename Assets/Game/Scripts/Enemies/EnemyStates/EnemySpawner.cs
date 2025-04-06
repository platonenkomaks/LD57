using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Enemy enemyPrefab;
    public Transform spawnPoint;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab != null && spawnPoint != null)
        {
            var enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            enemy.Init(G.Player.transform);
        }
        else
        {
            Debug.LogError("Enemy prefab or spawn point is not assigned.");
        }
    }
}