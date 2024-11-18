using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PlayerStateMachine
{
    public PlayerState? currentState { get; private set; }

    public void Initialize(PlayerState _startState)
    {
        currentState = _startState;
        currentState.Show();
    }

    public void ChangeState(PlayerState _newState)
    {
        currentState?.Close();
        currentState = _newState;
        currentState.Show();
    }
}