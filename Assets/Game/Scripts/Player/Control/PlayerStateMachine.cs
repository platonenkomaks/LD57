using Game.Scripts.Events;

using Game.Scripts.StateMachine ;


    public class PlayerStateMachine : StateMachine
    {
        public enum PlayerState
        {
            Mining,
            Carrying,
            Fighting
        }
        
        public PlayerStateMachine()
        {
            G.PlayerStateMachine = this;
        }
    
        private readonly MiningPlayerState _miningPlayerState = new();
        private readonly CarryingPlayerState _carryingPlayerState = new();
        private readonly FightingPlayerState _fightingPlayerState = new();
    

        public void SetState(PlayerState newState)
        {
        
            switch (newState)
            {
                case PlayerState.Mining:
                    ChangeState(_miningPlayerState);
                    break;
                case PlayerState.Carrying:
                    ChangeState(_carryingPlayerState);
                    break;
                case PlayerState.Fighting:
                    ChangeState(_fightingPlayerState);
                    break;
          
            }
      
            G.EventManager.Trigger(new OnPlayerStateChangeEvent()
            {
                State = newState
            });
        }
    }
