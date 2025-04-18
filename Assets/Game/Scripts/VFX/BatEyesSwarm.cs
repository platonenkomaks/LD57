using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;

public class BatEyesSwarm : MonoBehaviour
{
    public GameObject eyePrefab;
    public int eyeCount = 4;
    public float minRadius = 2f;
    public float maxRadius = 5f;
    public float blinkDuration = 0.1f;

    private Transform _player;
    private List<GameObject> eyes = new List<GameObject>();
    private bool isSpawning = false;

    private PlayerController _playerController;
    
    
    void Start()
    {
        _player = G.Player.transform;
        _playerController = _player.GetComponent<PlayerController>();
        if (eyePrefab == null || _player == null) return;
        
    }

    private void Update()
    {
        if (G.BatteryLight != null)
        {
            minRadius = G.BatteryLight.GetComponent<Light2D>().pointLightOuterRadius;
        }

        if (_playerController != null)
        {
            if (_playerController.isGrounded == false)
            {
                DestroyEyes();
            }
        }
       
        
    }

    void SpawnEye()
    {
        if (!isSpawning || eyes.Count >= eyeCount) return; // Check if the limit is reached

        Vector2 randomDir = Random.insideUnitCircle.normalized;
        float radius = Random.Range(minRadius + 0.1f, maxRadius);
        Vector3 offset = new Vector3(randomDir.x, randomDir.y, 0f) * radius;

        Vector3 position = _player.position + offset;

        GameObject eye = Instantiate(eyePrefab, position, Quaternion.identity, transform);
        eyes.Add(eye);
        StartCoroutine(EyeLifecycle(eye));
    }

    IEnumerator EyeLifecycle(GameObject eye)
    {
        SpriteRenderer renderer = eye.GetComponent<SpriteRenderer>();

        if (eye != null && renderer != null)
        {
            renderer.enabled = false;
            yield return new WaitForSeconds(blinkDuration); // Время до появления глаза

            if (eye != null && renderer != null)
                renderer.enabled = true;
        }

        // Время жизни глаза
        yield return new WaitForSeconds(Random.Range(2f, 5f)); // Увеличьте или уменьшите диапазон

        if (eyes.Contains(eye))
        {
            eyes.Remove(eye);
            if (eye != null) Destroy(eye);
        }

        if (isSpawning) SpawnEye();
    }

    public void StartSpawning()
    {
        if (isSpawning) return;

        isSpawning = true;

        StartCoroutine(SpawnEyesWithDelay());
    }

    IEnumerator SpawnEyesWithDelay()
    {
        while (isSpawning)
        {
            SpawnEye();
            yield return new WaitForSeconds(1f); // Задержка между появлениями глаз
        }
    }

    public void DestroyEyes()
    {
        foreach (var eye in eyes)
        {
            if (eye != null) Destroy(eye);
        }
        eyes.Clear();
    }

    public void StopSpawning()
    {
        isSpawning = false;

        foreach (var eye in eyes)
        {
            if (eye != null) Destroy(eye);
        }
        eyes.Clear();
    }

    private void OnDrawGizmos()
    {
        if (_player == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_player.position, minRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_player.position, maxRadius);
    }
}
