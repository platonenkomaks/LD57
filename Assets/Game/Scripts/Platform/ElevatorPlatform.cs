using System.Collections;
using Game.Scripts.Events;
using Game.Scripts.StateMachine.GameLoop;
using UnityEngine;

public class ElevatorPlatform : MonoBehaviour
{
    [SerializeField] private ElevatorLever lever;
    public float baseSpeed = 2f;
    public float platformWeight = 1f;
    public float weightPenalty = 0.5f;


    public float topY = 100f; // Цель при подъеме
    public float bottomY = 0f; // Цель при спуске

    private float currentSpeed = 0f;
    private bool isMoving = false;
    private Vector2 targetPosition;


    void Update()
    {
        if (isMoving)
        {
            Vector2 currentPosition = transform.position;
            transform.position = Vector2.MoveTowards(currentPosition, targetPosition, currentSpeed * Time.deltaTime);
            

            if (Vector2.Distance(transform.position, targetPosition) < 0.01f)
            {
                Park();
            }
        }
    }


    public void StartDescent()
    {
        G.AudioManager.Stop("ElevatorStart");
        G.AudioManager.Play("ElevatorStart");

        StartCoroutine(DescentAfterDelay(2f));
    }

    public IEnumerator DescentAfterDelay(float seconds) //Задержка перед началом движения платформы для анимации рычага 
    {
        yield return new WaitForSeconds(seconds);
        targetPosition = new Vector2(transform.position.x, bottomY);
        currentSpeed = baseSpeed;
        isMoving = true;
        G.AudioManager.Stop("ElevatorStop");
        G.AudioManager.Stop("ElevatorStart");
        G.AudioManager.Play("ElevatorMoving");
    }

    public void StartAscent()
    {
        G.AudioManager.Stop("ElevatorStart");
        G.AudioManager.Play("ElevatorStart");

        StartCoroutine(AscentAfterDelay(2f));
        G.AudioManager.Stop("ElevatorStop");
        G.AudioManager.Stop("ElevatorStart");
        G.AudioManager.Play("ElevatorMoving");
    }

    public IEnumerator AscentAfterDelay(float seconds) //Задержка перед началом движения платформы для анимации рычага 
    {
        yield return new WaitForSeconds(seconds);
        targetPosition = new Vector2(transform.position.x, topY);
        currentSpeed = Mathf.Max(0.1f, baseSpeed - platformWeight * weightPenalty);
        isMoving = true;
    }

    public void Stop()
    {
        isMoving = false;
        currentSpeed = 0f;
        G.AudioManager.Stop("ElevatorStart");
        G.AudioManager.Stop("ElevatorMoving");
        G.AudioManager.Play("ElevatorStop");
    }

    private void Park()
    {
        Stop();

        if (Mathf.Approximately(targetPosition.y, topY))
            OnArriveSurface();
        else
            OnArriveMine();
    }


    // Для изменения параметров
    public void SetWeight(float newWeight) => platformWeight = newWeight;
    public void SetBaseSpeed(float newSpeed) => baseSpeed = newSpeed;

    public void SetTopY(float y) => topY = y;
    public void SetBottomY(float y) => bottomY = y;

    private void OnArriveMine()
    {
        lever.isLocked = false;
        G.EventManager.Trigger(new SetGameStateEvent { State = GameLoopStateMachine.GameLoopState.Mining });
    }

    private void OnArriveSurface()
    {
        lever.isLocked = false;
        G.EventManager.Trigger(new SetGameStateEvent { State = GameLoopStateMachine.GameLoopState.Shopping });
    }
}