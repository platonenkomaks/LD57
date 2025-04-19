using System.Collections;
using Events;
using Game.Scripts.StateMachine.GameLoop;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ElevatorPlatform : MonoBehaviour
{
    [SerializeField] private ElevatorLever lever;
    public float descendSpeed = 8f;
    public float platformWeight = 1f;
    public float baseAscendTime = 60f; // Базовое время подъема (в секундах)
    public float weightTimeAddition = 10f; // Дополнительное время на единицу веса (в секундах)
    public Cog cog;

    public float topY = 100f; // Цель при подъеме
    public float bottomY = 0f; // Цель при спуске

    
    
    public float CurrentSpeed { get; private set; } = 0f;
    private bool isMoving = false;
    private Vector2 targetPosition;

    private void Awake()
    {
        G.ElevatorPlatform = this;
    }

    private void OnDestroy()
    {
        G.ElevatorPlatform = null;
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            Vector2 newPosition =
                Vector2.MoveTowards(transform.position, targetPosition, CurrentSpeed * Time.fixedDeltaTime);

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
        G.PlayerController.disableJump = true;
        G.AudioManager.Stop("Intro");
        G.AudioManager.Stop("ElevatorStart");
        G.AudioManager.Play("ElevatorStart");

        StartCoroutine(DescentAfterDelay(1.5f));
    }

    public IEnumerator DescentAfterDelay(float seconds) //Задержка перед началом движения платформы для анимации рычага 
    {
        yield return new WaitForSeconds(seconds);
        targetPosition = new Vector2(transform.position.x, bottomY);
        CurrentSpeed = descendSpeed;
        isMoving = true;
        G.AudioManager.Stop("ElevatorStop");
        G.AudioManager.Stop("ElevatorStart");
        G.AudioManager.Play("ElevatorMoving");
    }

    public void StartAscent()
    {
        G.AudioManager.Stop("ElevatorStart");
        G.AudioManager.Play("ElevatorStart");

        StartCoroutine(AscentAfterDelay(2.5f));
        G.AudioManager.Stop("ElevatorStop");
        G.AudioManager.Stop("ElevatorStart");
        G.AudioManager.Play("Fight");
    }

    public IEnumerator AscentAfterDelay(float seconds) //Задержка перед началом движения платформы для анимации рычага 
    {
        G.Player.GetComponent<PlayerController>().SetJumpForce(15f);
        yield return new WaitForSeconds(seconds);
        targetPosition = new Vector2(transform.position.x, topY);

        // Расчет скорости на основе требуемого времени подъема
        float totalAscentTime = baseAscendTime + (platformWeight - 1) * weightTimeAddition;
        float distance = Mathf.Abs(topY - transform.position.y);
        CurrentSpeed = Mathf.Max(0.1f, distance / totalAscentTime);

        isMoving = true;
    }

    public void Stop()
    {
        isMoving = false;
        CurrentSpeed = 0f;

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
    public void SetBaseAscendTime(float newTime) => baseAscendTime = newTime;
    public void SetWeightTimeAddition(float addition) => weightTimeAddition = addition;

    public void SetTopY(float y) => topY = y;
    public void SetBottomY(float y) => bottomY = y;

    private void OnArriveToMine()
    {
        G.PlayerController.disableJump = false;
        lever.isLocked = false;
        G.EventManager.Trigger(new SetGameStateEvent { State = GameLoopStateMachine.GameLoopState.Mining });
    }

    private void OnArriveToSurface()
    {
        G.Player.GetComponent<PlayerController>().SetJumpForce(10f);
        G.AudioManager.Stop("Fight");
        G.GoldPilesView.SetEnabled(false);
        G.GoldManager.AddGold(G.ElevatorPlatform.GetComponent<PlatformWeight>().goldOnPlatformBalance);
        
        lever.isLocked = false;
        G.EventManager.Trigger(new SetGameStateEvent { State = GameLoopStateMachine.GameLoopState.Shopping });
        G.UIManager.ShowScreen("ShopScreen");

        // Восстанавливаем здоровье игрока до максимального значения
        if (G.PlayerHealth != null)
        {
            G.PlayerHealth.ResetHealt();
            var healthUI = FindAnyObjectByType<PlayerHealthUI>();
            if (healthUI != null)
            {
                healthUI.UpdateHeartsDisplay();
            }
        }
        else
        {
            Debug.LogError("G.PlayerHealth is null!");
        }
    }
}