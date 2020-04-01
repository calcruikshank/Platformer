using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class ControllerExtension : MonoBehaviour
{
	[SerializeField] private float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
	[SerializeField] private float m_DashSpeed = 400f;         
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded                     


	const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	public bool m_Grounded;            // Whether or not the player is grounded.

	private Rigidbody2D m_Rigidbody2D;

	private Vector3 m_Velocity = Vector3.zero;

	private int numofJumps = 0;
	private int maxJumps = 2;




	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]

	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnDashEvent;


	

	private void Awake()
	{
		
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnDashEvent == null)
			OnDashEvent = new BoolEvent();
	}




	private void FixedUpdate()
	{

		bool wasGrounded = m_Grounded;
		m_Grounded = false;
		//faceMouse();
		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
				if (!wasGrounded)
					OnLandEvent.Invoke();
			}

		}

	}


	public void Move(float move, bool dash, bool jump)
	{
		// dash
		if (dash) dashCharacter();

		void dashCharacter()
		{

			//Check where mouse in in relation to player
			Vector3 mousePosition = Input.mousePosition;

			mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
			Vector3 direction = new Vector3(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y);
			direction = direction.normalized;

			//0 out vertical and horizontal velocity 
			m_Rigidbody2D.velocity = new Vector3(0, 0);


			//add force in the direction of mouse position

			m_Rigidbody2D.AddForce(direction * m_DashSpeed);
			//m_Rigidbody2D.velocity = transform.right = -direction * m_DashSpeed;
			dash = false;

		}



		//only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl)
		{
			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);

			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

		}
		// Jump if player is grounded or jump number is less than max jumps
		if (jump) JumpCharacter();

		void JumpCharacter()
		{
			if (m_Grounded)
			{
				numofJumps = 0;
			}
			if (m_Grounded || numofJumps < maxJumps)
			{
				// Add a vertical force to the player.
				
				m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, 0);	//zero out vertical momentum
				m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
				numofJumps += 1;
				m_Grounded = false;
			}
			jump = false;
		}

	}


}
