using System.Collections;
using UnityEngine;


public class Player : MonoBehaviour
{
    [field: SerializeField] public BatteryLight BatteryLight { get; private set; } 
    
    [Header("Player States Sprites")] 
    public Sprite miningGoldSprite;
    public Sprite carryingGoldSprite;
    public Sprite fightingSprite;

    public SpriteRenderer playerSpriteRenderer;
    

    private void Awake()
    {
        
      
    }
    private IEnumerator Start ()
    {
        yield return null;
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        
        G.PlayerStateMachine = new PlayerStateMachine(
            playerSpriteRenderer,
            miningGoldSprite,
            carryingGoldSprite,
            fightingSprite
        );
        G.PlayerStateMachine.SetState(PlayerStateMachine.PlayerState.Mining);
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            G.PlayerStateMachine.SetState(PlayerStateMachine.PlayerState.Fighting);
        }
        
    }
    
}