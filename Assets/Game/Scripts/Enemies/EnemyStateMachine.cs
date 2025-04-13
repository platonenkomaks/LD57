using UnityEngine;
using System.Collections.Generic;

public enum EnemyStateID
{
    Patrol,
    Wander,
    Chase,
    Attack,
    TakeDamage,
    Retreat
}

// Базовый класс для состояния
public abstract class EnemyState
{
    public Enemy enemy;
    public EnemyStateID stateID;
    
    public EnemyStateID ID => stateID;
    
    public EnemyState(Enemy enemy, EnemyStateID stateID)
    {
        this.enemy = enemy;
        this.stateID = stateID;
    }
    
    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void Exit() { }
}

// Машина состояний
public class EnemyStateMachine
{
    private Dictionary<EnemyStateID, EnemyState> states = new Dictionary<EnemyStateID, EnemyState>();
    private EnemyState currentState;

    public EnemyState CurrentState => currentState;
    
    public int GetStatesCount() => states.Count;

    public void RegisterState(EnemyState state)
    {
        states[state.ID] = state;
    }

    public void ChangeState(EnemyStateID newStateID)
    {
        if (currentState != null && currentState.ID == newStateID)
        {
            Debug.Log($"EnemyStateMachine: already in state {newStateID}");
            return;
        }

        if (states.TryGetValue(newStateID, out EnemyState newState))
        {
            Debug.Log($"EnemyStateMachine: changing state from {currentState?.ID} to {newStateID}");
            currentState?.Exit();
            currentState = newState;
            currentState.Enter();
        }
        else
        {
            Debug.LogError($"EnemyStateMachine: state {newStateID} not found in registered states");
        }
    }

    public void Update()
    {
        currentState?.Update();
    }

    public void FixedUpdate()
    {
        currentState?.FixedUpdate();
    }
}
