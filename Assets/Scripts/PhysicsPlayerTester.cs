using UnityEngine;
using System.Collections;


public class PhysicsPlayerTester : MonoBehaviour
{
	// movement config
	public float gravity = -25f;
	public float runSpeed = 8f;
	public float groundDamping = 20f; // how fast do we change direction? higher means faster
	public float inAirDamping = 5f;
	public float jumpHeight = 3f;

	private const float maxBloat = 2.0f;	// Maximum size for bloat on death
	private const float bloatStep = 0.02f;

	[HideInInspector]
	private float normalizedHorizontalSpeed = 0;

	private CharacterController2D _controller;
	private Animator _animator;
	private RaycastHit2D _lastControllerColliderHit;
	private Vector3 _velocity;

	// input
	private bool _right;
	private bool _left;
	private bool _up;

	private int numFags = 0;
	private bool hitEnemy = false;
	private bool dying = false;

	private float? platformLastXPos = null; // stores positon the last collided with moving platform had, to allow player move syncing
	private int lastMovingPlatformID = -1;

	private int levelCompleteDelay = 0;

	void Awake()
	{
		_animator = GetComponent<Animator>();
		_controller = GetComponent<CharacterController2D>();

		// listen to some events for illustration purposes
		_controller.onControllerCollidedEvent += onControllerCollider;
		_controller.onTriggerEnterEvent += onTriggerEnterEvent;
		_controller.onTriggerExitEvent += onTriggerExitEvent;

		// Find out how many fag packets there are to collect for this level.
		numFags = GameObject.FindGameObjectsWithTag("Fags").Length;
	}


	#region Event Listeners

	void onControllerCollider( CharacterController2D.RayWithVelocityInfo hit )
	{
		// logs any collider hits if uncommented. it gets noisy so it is commented out for the demo
		//Debug.Log( "flags: " + _controller.collisionState + ", hit.normal: " + hit.normal );
		bool isDeadlyPlatform = hit._raycastHit2D.collider.gameObject.CompareTag("DeadlyPlatform");
		if (isDeadlyPlatform) {
			if (!_controller.isGrounded && hit._raycastHit2D.collider.gameObject.transform.localScale.y > 0f) {
				DoDeath ();
			} else if (_controller.isGrounded && hit._raycastHit2D.collider.gameObject.transform.localScale.y < 0f) {
				DoDeath ();
			}
			return;
		}

		if (hit._velocity.y < -13f) {
			DoDeath ();
		}

		bool isMovingPlatform = hit._raycastHit2D.collider.gameObject.CompareTag("MovingPlatform");
		if (isMovingPlatform) {
			int platformID = hit._raycastHit2D.collider.gameObject.GetInstanceID();
			if (lastMovingPlatformID != platformID) {
				lastMovingPlatformID = platformID;
				platformLastXPos = null;
			}

			if (platformLastXPos != null) {
				transform.Translate(new Vector3(-(platformLastXPos.Value - hit._raycastHit2D.collider.gameObject.transform.position.x), 0,0));
			}
			platformLastXPos = hit._raycastHit2D.collider.gameObject.transform.position.x;

		} else {
			platformLastXPos = null;
		}
	}

	private bool hacktasticBypassed = true;

	void DoDeath ()
	{
		if (hacktasticBypassed == false) return;

		collider2D.enabled = false;
		hitEnemy = true;
		_velocity.y = Mathf.Sqrt (2f * jumpHeight * -gravity);
		_controller.platformMask = 0;
		_controller.move (_velocity * Time.fixedDeltaTime);
		// make character pass through everything during death
		_animator.SetInteger ("Direction", 3);
		// red eyed death anim
		dying = true;
		// for bloat and rotate animation
		AudioSource source = GetComponent<AudioSource> ();
		audio.PlayOneShot (source.clip);
	}

	void onTriggerEnterEvent( Collider2D col )
	{
		Debug.Log( "onTriggerEnterEvent: " + col.gameObject.name );
		if (dying) return; // wait for death

		if (col.gameObject.layer == 11) { // enemy layer
			DoDeath ();
		}

		if (col.gameObject.name.CompareTo("Fags") == 0) {
			if (hacktasticBypassed == false) { return; } // skip first collision since Unity has bugz

			GameObject completedSoundComponent = GameObject.Find("levelCompleted");
			AudioSource source = completedSoundComponent.GetComponent<AudioSource>();
			audio.PlayOneShot(source.clip);

			Destroy(col.gameObject);
			if (--numFags > 0) return;

			// Stop enemy animations.
			GameObject[] objs = FindObjectsOfType<GameObject>();
			foreach (GameObject obj in objs) {
				if (obj.layer == 11) { // enemy layer
					Animator animator = obj.GetComponent<Animator>();
					if (animator != null) {
						animator.enabled = false;
						animator.animatePhysics = false;
					}
				}
			}
			_animator.enabled = false; // stop bloke's animation

			levelCompleteDelay = 60;
			_animator.SetInteger("Direction", 0);
			GameManager.Get().GameFrozen = true;
		}
	}


	void onTriggerExitEvent( Collider2D col )
	{
		Debug.Log( "onTriggerExitEvent: " + col.gameObject.name );

		if (hacktasticCountdown > 0) { // still avoiding bug
			hacktasticCountdown = 20;
			return;
		}
		hacktasticBypassed = true; // just in case it's still active - also multiple platform test TODO for this

		if (col.gameObject.name == "Main Camera") {
			if (hitEnemy) {
				GameManager.Get().LoseLife();
				hitEnemy = false;
			} else if (transform.position.y < 0) { // make sure we aren't simply jumping out of the screen top
				GameObject obj = GameObject.Find("fallingScream");
				AudioSource source = obj.GetComponent<AudioSource>();
				audio.PlayOneShot(source.clip);
			}
		}

		if (col.gameObject.name == "GameRegion") {
			GameManager.Get().LoseLife();
		}
	}

	#endregion

	// the Update loop only gathers input. Actual movement is handled in FixedUpdate because we are using the Physics system for movement
	void Update()
	{
		// a minor bit of trickery here. FixedUpdate sets _up to false so to ensure we never miss any jump presses we leave _up
		// set to true if it was true the previous frame
		_up = _up || Input.GetKey( KeyCode.UpArrow ) || Input.GetButton("Fire1") || Input.GetKey(KeyCode.Space);
		_right = Input.GetKey( KeyCode.RightArrow );
		_left = Input.GetKey( KeyCode.LeftArrow );

		// Only if keys aren't being used, check joysticks etc.
		if (_right == false) {
			_right = Input.GetAxis("Horizontal") > 0;
		}
		if (_left == false) {
			_left = Input.GetAxis("Horizontal") < 0;
		}
	}

	int hacktasticCountdown = 20;

	void FixedUpdate()
	{
		if (!hacktasticBypassed && --hacktasticCountdown == 0) { // Unity bug hack around for triggers.
			hacktasticBypassed = true;
		}

		// Are we in the process of moving to the next level?
		if (levelCompleteDelay > 0) {
			rigidbody2D.velocity = new Vector3();
			if (--levelCompleteDelay == 0) {
				GameManager.Get().NextLevel();
			}
			return;
		}

		// grab our current _velocity to use as a base for all calculations
		_velocity = _controller.velocity;

		if (dying) {
			BloatBloke();
		} else {
			if( _controller.isGrounded )
				_velocity.y = 0;
			
			if( _right )
			{
				normalizedHorizontalSpeed = 1;
				if( transform.localScale.x < 0f )
					transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z );
				
				if( _controller.isGrounded ) {
					_animator.Play( Animator.StringToHash( "Run" ) );
					//_animator.SetInteger("Direction", 2);
				}
			}
			else if( _left )
			{
				normalizedHorizontalSpeed = -1;
				if( transform.localScale.x > 0f )
					transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z );
				
				if( _controller.isGrounded )
					_animator.Play( Animator.StringToHash( "Run" ) );
			}
			else
			{
				normalizedHorizontalSpeed = 0;
				
				if( _controller.isGrounded )
					_animator.Play( Animator.StringToHash( "Idle" ) );
			}
			
			
			// we can only jump whilst grounded
			if( _controller.isGrounded && _up )
			{
				_velocity.y = Mathf.Sqrt( 2f * jumpHeight * -gravity );
				_animator.Play( Animator.StringToHash( "Jump" ) );
				GameObject obj = GameObject.Find("jump");
				AudioSource source = obj.GetComponent<AudioSource>();
				audio.PlayOneShot(source.clip, 0.48f); //MSH: because volume is not handled correctly on AudioSource
				platformLastXPos = null;
			}
		} // !dying
		
		// apply horizontal speed smoothing it
		var smoothedMovementFactor = _controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
		_velocity.x = Mathf.Lerp( _velocity.x, normalizedHorizontalSpeed * runSpeed, Time.fixedDeltaTime * smoothedMovementFactor );
		
		// apply gravity before moving
		_velocity.y += gravity * Time.fixedDeltaTime;

		_controller.move( _velocity * Time.fixedDeltaTime );

		// reset input
		_up = false;
	}

	void BloatBloke() {
		float yscale = transform.localScale.y;
		if (yscale > maxBloat) {
			return;
		}
		yscale += bloatStep;
		
		float xscale = transform.localScale.x;
		if (xscale > 0) {
			xscale += bloatStep;
		}
		else {
			xscale -= bloatStep;
		}

		transform.localScale = new Vector3(xscale, yscale, 1);

		transform.Rotate(new Vector3(0,0,transform.localRotation.z + 1f));
	}
}
