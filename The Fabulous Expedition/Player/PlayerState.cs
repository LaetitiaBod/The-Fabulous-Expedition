using Raylib_cs;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using static Raylib_cs.Raylib;
using static Program;

public class PlayerState
{
    protected PlayerStateMachine stateMachine;
    protected Player player;

    protected float stateTimer;
	public Animator anim;

	public PlayerState(Player _player, PlayerStateMachine _stateMachine, Animator _anim)
    {
        player = _player;
        stateMachine = _stateMachine;
		anim = _anim;
	}

    public virtual void Show() { }

	public virtual void Update() { }

    public virtual void Draw() { }

    public virtual void Close() { }
}