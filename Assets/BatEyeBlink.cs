using UnityEngine;

public class BatEyeBlink : MonoBehaviour
{
    public SpriteRenderer Renderer;
    public Sprite openEye;
    public Sprite closedEye;
    public float blinkIntervalMin = 2f;
    public float blinkIntervalMax = 5f;
    public float blinkDuration = 0.1f;

    private float _blinkTimer;
    private bool _blinking;

    private void Start()
    {
        _blinkTimer = Random.Range(blinkIntervalMin, blinkIntervalMax);
        Renderer.sprite = openEye;
    }

    private void Update()
    {
        if (_blinking) return;
        _blinkTimer -= Time.deltaTime;
        if (_blinkTimer <= 0f)
            StartCoroutine(Blink());
    }

    System.Collections.IEnumerator Blink()
    {
        _blinking = true;
        Renderer.sprite = closedEye;
        yield return new WaitForSeconds(blinkDuration);
        Renderer.sprite = openEye;
        _blinkTimer = Random.Range(blinkIntervalMin, blinkIntervalMax);
        _blinking = false;
    }
}