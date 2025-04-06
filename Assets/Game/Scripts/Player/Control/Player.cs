using UnityEngine;
using UnityEngine.Serialization;


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
       
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        
        G.PlayerStateMachine = new PlayerStateMachine(
            playerSpriteRenderer,
            miningGoldSprite,
            carryingGoldSprite,
            fightingSprite
        );
        
      
    }
    private void Start ()
    {
     
        G.PlayerStateMachine.SetState(PlayerStateMachine.PlayerState.Mining);
    }
    
}