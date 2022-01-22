using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {

	public CharacterController2D controller;
	public Animator animator;
	public PlayerInput input;

	private const float WalkSpeed = 20f;
	private const float RunSpeed = 40f;
	
	private float moveSpeed = WalkSpeed;
	private float horizontalMove = 0f;
	private bool jump = false;
	private bool dash = false;

	//bool dashAxis = false;
	
	private void OnRun(InputValue value) {
		moveSpeed = value.isPressed ? RunSpeed : WalkSpeed;
	}

	private void OnHorizMove(InputValue value) {
		horizontalMove = value.Get<float>() * moveSpeed;
	}

	private void OnJump(InputValue value) {
		jump = true;
	}

	private void OnDash(InputValue value) {
		dash = true;
	}

	// Update is called once per frame
	void Update () {
		animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
	}

	public void OnFall()
	{
		animator.SetBool("IsJumping", true);
	}

	public void OnLanding()
	{
		animator.SetBool("IsJumping", false);
	}

	void FixedUpdate ()
	{
		// Move our character
		controller.Move(horizontalMove * Time.fixedDeltaTime, jump, dash);
		jump = false;
		dash = false;
	}
}
