using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
	// PRIVATE MEMBERS

	private PlayerController _controller;
	private Animator _animator;
	private int _lastVisibleJump;

	private static readonly int MoveSpeedHash = Animator.StringToHash("MoveSpeed");
	private static readonly int JumpHash = Animator.StringToHash("JumpTrigger");
	private static readonly int FreeFallHash = Animator.StringToHash("FreeFall");
	private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");

	// NetworkBehaviour INTERFACE

	public override void Spawned()
	{
		_lastVisibleJump = _controller.JumpCount;
	}

	public override void Render()
	{
		UpdateAnimations();
	}

	// MONOBEHAVIOUR

	protected void Awake()
	{
		_controller = GetComponentInChildren<PlayerController>();
		_animator = transform.GetChild(1).gameObject.GetComponent<Animator>();
	}

	// PRIVATE METHODS

	private void UpdateAnimations()
	{
		if (_lastVisibleJump < _controller.JumpCount)
		{
			_animator.SetTrigger(JumpHash);
		}
		else if (_lastVisibleJump > _controller.JumpCount)
		{
			// Cancel Jump
		}

		_lastVisibleJump = _controller.JumpCount;

		_animator.SetFloat(MoveSpeedHash, 3);
	}
}
