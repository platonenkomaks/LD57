using Game.Scripts.Events;
using Game.Scripts.StateMachine.GameLoop;
using UnityEngine;


public class ElevatorLever : MonoBehaviour
{
    public ElevatorPlatform platform;
    public GameObject hintUI; // UI-объект с текстом подсказки

    private bool playerInRange = false;
    private bool isDescending = true;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (isDescending)
            {
                platform.StartDescent();
                OnDescend();
            }
            else
            {
                platform.StartAscent();
                OnAscend();
            }

            isDescending = !isDescending;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (hintUI != null)
                hintUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (hintUI != null)
                hintUI.SetActive(false);
        }
    }

    private void OnDescend()
    {
        G.EventManager.Trigger(new SetGameStateEvent { State = GameLoopStateMachine.GameLoopState.Descend });
    }
    
    private void OnAscend()
    {
        G.EventManager.Trigger(new SetGameStateEvent { State = GameLoopStateMachine.GameLoopState.Ascend });
    }
}