using System.Collections;
using Game.Scripts.Events;
using Game.Scripts.StateMachine.GameLoop;
using UnityEngine;


public class ElevatorLever : MonoBehaviour
{
    public ElevatorPlatform platform;
    public GameObject hintUI; // UI-объект с текстом подсказки

    private bool playerInRange = false;
    private bool isDescending = true;

    public bool isLocked = false;

    void Update()
    {
        if (!isLocked && playerInRange && Input.GetKeyDown(KeyCode.S))
        {
            G.AudioManager.Play("Lever");
            isLocked = true;
            hintUI.SetActive(false);
            
            if (isDescending)
            {
                StartCoroutine(DescentAfterDelay(1f));
            }
            else
            {
                StartCoroutine(AscentAfterDelay(1f));
            }

            isDescending = !isDescending;
        }
    }

    public IEnumerator DescentAfterDelay(float seconds) //Задержка перед началом движения платформы для анимации рычага 
    {
        yield return new WaitForSeconds(seconds);
        platform.StartDescent();
        OnDescend();
    }

    public IEnumerator AscentAfterDelay(float seconds) //Задержка перед началом движения платформы для анимации рычага 
    {
        yield return new WaitForSeconds(seconds);
        platform.StartAscent();
        OnAscend();
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isLocked && other.CompareTag("Player"))
        {
            playerInRange = true;
            if (hintUI != null)
                hintUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!isLocked && other.CompareTag("Player"))
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