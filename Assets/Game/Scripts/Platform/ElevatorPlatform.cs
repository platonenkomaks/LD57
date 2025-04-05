using UnityEngine;

public class ElevatorPlatform : MonoBehaviour
{
    public float baseSpeed = 2f;
    public float platformWeight = 1f;
    public float weightPenalty = 0.5f;


    public float topY = 100f; // Цель при подъеме
    public float bottomY = 0f; // Цель при спуске

    private float currentSpeed = 0f;
    private bool isMoving = false;
    private Vector2 targetPosition;

    private bool isParking = false;

    void Update()
    {
        if (isMoving)
        {
            Vector2 currentPosition = transform.position;
            transform.position = Vector2.MoveTowards(currentPosition, targetPosition, currentSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, targetPosition) < 90f && !isParking)
            {
                isParking = true;

                G.AudioManager.Stop("ElevatorStop");
                G.AudioManager.Stop("ElevatorStart");
                G.AudioManager.Play("ElevatorMoving");
            }

            if (Vector2.Distance(transform.position, targetPosition) < 0.01f)
            {
                Park();
            }
        }
    }

    public void StartDescent()
    {
        targetPosition = new Vector2(transform.position.x, bottomY);
        currentSpeed = baseSpeed;
        isMoving = true;
        G.AudioManager.Stop("ElevatorStart");
        G.AudioManager.Play("ElevatorStart");
    }


    public void StartAscent()
    {
        targetPosition = new Vector2(transform.position.x, topY);
        currentSpeed = Mathf.Max(0.1f, baseSpeed - platformWeight * weightPenalty);
        isMoving = true;
        G.AudioManager.Stop("ElevatorStart");
        G.AudioManager.Play("ElevatorStart");
    }

    public void Stop()
    {
        isMoving = false;
        isParking = false;
        currentSpeed = 0f;
        G.AudioManager.Stop("ElevatorStart");
        G.AudioManager.Stop("ElevatorMoving");
        G.AudioManager.Play("ElevatorStop");
    }

    private void Park()
    {
        Stop();
    }


    // Для изменения параметров
    public void SetWeight(float newWeight) => platformWeight = newWeight;
    public void SetBaseSpeed(float newSpeed) => baseSpeed = newSpeed;

    public void SetTopY(float y) => topY = y;
    public void SetBottomY(float y) => bottomY = y;
}