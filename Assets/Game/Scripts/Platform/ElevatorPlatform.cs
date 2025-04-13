using System.Collections;
using Events;
using Game.Scripts.StateMachine.GameLoop;
using UnityEngine;

public class ElevatorPlatform : MonoBehaviour
{
    [SerializeField] private ElevatorLever lever;
    public float baseSpeed = 2f;
    public float descendSpeed = 10f;
    public float platformWeight = 1f;
    public float weightPenalty = 0.5f;
    public Cog cog;

    public float topY = 100f; // Цель при подъеме
    public float bottomY = 0f; // Цель при спуске

    private float currentSpeed = 0f;
    private bool isMoving = false;
    private bool isDescending = false; // Флаг для определения направления движения
    private Vector2 targetPosition;
    private Transform playerTransform; // Ссылка на трансформ игрока
    private Vector2 playerOffset; // Смещение игрока относительно платформы

    void Update()
    {
        if (isMoving)
        {
            Vector2 currentPosition = transform.position;
            Vector2 newPosition = Vector2.MoveTowards(currentPosition, targetPosition, currentSpeed * Time.deltaTime);
            
            // Если платформа движется вниз и игрок зафиксирован
            if (isDescending && playerTransform != null)
            {
                // Перемещаем игрока вместе с платформой
                playerTransform.position = newPosition + playerOffset;
            }
            
            transform.position = newPosition;
            cog.StartRotation();

            if (Vector2.Distance(transform.position, targetPosition) < 0.01f)
            {
                Park();
            }
        }
        else
        {
            cog.StopRotation();
        }
    }

    public void StartDescent()
    {
        G.AudioManager.Stop("ElevatorStart");
        G.AudioManager.Play("ElevatorStart");

        // Фиксируем позицию игрока относительно платформы
        if (G.Player != null)
        {
            playerTransform = G.Player.transform;
            playerOffset = playerTransform.position - transform.position;
        }

        StartCoroutine(DescentAfterDelay(1.5f));
    }

    public IEnumerator DescentAfterDelay(float seconds) //Задержка перед началом движения платформы для анимации рычага 
    {
        yield return new WaitForSeconds(seconds);
        targetPosition = new Vector2(transform.position.x, bottomY);
        currentSpeed = descendSpeed;
        isMoving = true;
        isDescending = true; // Устанавливаем флаг движения вниз
        G.AudioManager.Stop("ElevatorStop");
        G.AudioManager.Stop("ElevatorStart");
        G.AudioManager.Play("ElevatorMoving");
    }

    public void StartAscent()
    {
        G.AudioManager.Stop("ElevatorStart");
        G.AudioManager.Play("ElevatorStart");

        StartCoroutine(AscentAfterDelay(1.5f));
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
        isDescending = false; // Сбрасываем флаг движения вниз
    }

    public void Stop()
    {
        isMoving = false;
        isDescending = false; // Сбрасываем флаг движения вниз
        currentSpeed = 0f;
        
        // Разблокируем управление игроком и анимации
        if (G.Player != null)
        {
            var playerController = G.Player.GetComponent<PlayerController>();
            var playerAnimator = G.Player.GetComponent<Animator>();
            
            if (playerController != null)
            {
                playerController.enabled = true;
            }
            
            if (playerAnimator != null)
            {
                playerAnimator.SetBool("IsIdle", false);
                // Возвращаем анимацию в нормальное состояние
                playerAnimator.SetBool("IsMoving", false);
            }
        }
        playerTransform = null;
        
        G.AudioManager.Stop("ElevatorStart");
        G.AudioManager.Stop("ElevatorMoving");
        G.AudioManager.Play("ElevatorStop");
    }

    private void Park()
    {
        Stop();

        if (Mathf.Approximately(targetPosition.y, topY))
            OnArriveToSurface();
        else
            OnArriveToMine();
    }

    // Для изменения параметров
    public void SetWeight(float newWeight) => platformWeight = newWeight;
    public void SetBaseSpeed(float newSpeed) => baseSpeed = newSpeed;

    public void SetTopY(float y) => topY = y;
    public void SetBottomY(float y) => bottomY = y;

    private void OnArriveToMine()
    {
        lever.isLocked = false;
        G.EventManager.Trigger(new SetGameStateEvent { State = GameLoopStateMachine.GameLoopState.Mining });
    }

    private void OnArriveToSurface()
    {   G.GoldPilesView.SetEnabled(false);
        lever.isLocked = false;
        G.EventManager.Trigger(new SetGameStateEvent { State = GameLoopStateMachine.GameLoopState.Shopping });
        G.UIManager.ShowScreen("ShopScreen");
       
    }
}