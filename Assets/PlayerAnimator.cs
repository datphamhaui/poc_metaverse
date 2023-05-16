using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : NetworkBehaviour
{
	// PRIVATE MEMBERS

	private PlayerController _controller;
	private Animator _animator;
	private int _lastVisibleJump;

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
		_animator = GetComponentInChildren<Animator>();
	}

	// PRIVATE METHODS

	private void UpdateAnimations()
	{
		if (_lastVisibleJump < _controller.JumpCount)
		{
			_animator.SetTrigger("Jump");
		}
		else if (_lastVisibleJump > _controller.JumpCount)
		{
			// Cancel Jump
		}

		_lastVisibleJump = _controller.JumpCount;

		_animator.SetFloat("Speed", _controller.Speed);
	}
}
