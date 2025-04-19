using System.Collections;
using Events;
using Game.Scripts.StateMachine.GameLoop;
using Platform;
using UnityEngine;


public class ElevatorLever : MonoBehaviour
{
    public ElevatorPlatform platform;
    public GameObject hintUI; // UI-объект с текстом подсказки

    public Sprite leverDownSprite;
    public Sprite leverUpSprite;
    
    public Credits credits;

    private bool playerInRange = false;
    private bool isDescending = true;
    private SpriteRenderer _spriteRenderer;
    public bool isLocked = false;

    
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = leverUpSprite;
        if (hintUI != null)
            hintUI.SetActive(false);
    }
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
                
                credits.StartCredits();
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
        _spriteRenderer.sprite = leverDownSprite;
        yield return new WaitForSeconds(seconds);
        platform.StartDescent();
        OnDescend();
    }

    public IEnumerator AscentAfterDelay(float seconds) //Задержка перед началом движения платформы для анимации рычага 
    {
        _spriteRenderer.sprite = leverUpSprite;
        yield return new WaitForSeconds(seconds);
        platform.StartAscent();
        OnAscend();
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;
        
        playerInRange = true;
        if (!isLocked)
        {
            hintUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;
        
        playerInRange = false;
        if (!isLocked)
        {
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