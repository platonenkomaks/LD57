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
                // Переводим игрока в idle анимацию и блокируем
                if (G.Player != null)
                {
                    var playerController = G.Player.GetComponent<PlayerController>();
                    var playerAnimator = G.Player.GetComponent<Animator>();
                    
                    if (playerAnimator != null)
                    {
                        playerAnimator.SetBool("IsIdle", true);
                    }
                    
                    if (playerController != null)
                    {
                        playerController.enabled = false;
                    }
                }
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
        yield return new WaitForSeconds(seconds);
        platform.StartDescent();
        _spriteRenderer.sprite = leverDownSprite;
        OnDescend();
    }

    public IEnumerator AscentAfterDelay(float seconds) //Задержка перед началом движения платформы для анимации рычага 
    {
        yield return new WaitForSeconds(seconds);
        _spriteRenderer.sprite = leverUpSprite;
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