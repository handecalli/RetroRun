using UnityEngine;
using System.Collections;
using Photon;
public class PlayerControls : UnityEngine.MonoBehaviour {


	public float jumpForce = 4f;
	public float runSpeed  = 5f;

	public int   missileCount = 1;
	public float missileForce = 1000f;
	public Sprite missileSkin;
	public LayerMask    groundLayer;

    public string missileSoundId = "missileSound";
    public string jumpSoundId = "jumpSound";

	private Vector2     movement;
	private Transform   groundCheck;
	private Transform   wallCheck;
	private Transform   ceilingCheck;
	private Transform   ceilingCheck2;
	private Rigidbody2D rb;
	private Animator    anim;

    public GameObject missilePrefab;
    private GameObject missile;

	private bool isGrounded  = false;
	private bool isWalled    = false;
	private bool isCeiled    = false;

	private bool jump        = false;
	private bool isStopped   = false;
	private bool isSleeping  = false;
	private bool isAwaking   = false;
	private bool isFinished  = false;
	private bool isStunning  = false;
	private bool isDancing   = false;
    private bool jumpClicked = false;
    private bool fireClicked = false;

	private float tempRun;
	private float tempJump;

	
	ObjectPoolScript objectPoolScript;
	
	public delegate void PlayerAction(GameObject go);
	public static event PlayerAction OnMissiled;
	public static event PlayerAction OnEnd;

    PhotonView m_PhotonView;

	void Awake()
	{
		groundCheck      = transform.Find("groundCheck");
		wallCheck        = transform.Find("wallCheck");
		ceilingCheck     = transform.Find("ceilingCheck");
		ceilingCheck2    = transform.Find("ceilingCheck2");

		rb               = GetComponent<Rigidbody2D>();
		anim             = GetComponent<Animator>();
		objectPoolScript = GetComponent<ObjectPoolScript> ();
        m_PhotonView     = GetComponent<PhotonView>();
        
	}
	
	void Start () 
	{
		transform.position = new Vector2(transform.position.x, transform.position.y);
		tempRun = runSpeed;
		tempJump = jumpForce;
	}

	void Update()
	{       
        Move();
	}

    private void Move()
    {
        #if UNITY_STANDALONE || UNITY_WEBPLAYER

        //Jump
        if ((Input.GetKeyDown("space") || Input.GetButtonDown("Jump")) && (isGrounded || isWalled) && !isCeiled)
        {
            jump = true;
        }

		// Missile
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            FireMissile();
        }

        #endif

        if (jumpClicked && (isGrounded || isWalled) && !isCeiled)
        {
            jump = true;
        }

        if (fireClicked)
        {
            FireMissile();
        }

        if (isFinished) 
		{
			OnEnd(gameObject);
		}

		if(!isGrounded && !isWalled)
		{
			anim.SetBool("isJumping", true);
		}
		else if(isGrounded || isWalled)
		{
			anim.SetBool("isJumping", false);
		}

        if (!jump)
        {
			movement = new Vector2(runSpeed, 0f);
        }
        else if (jump)
        {
            movement = new Vector2(runSpeed * 0.7f, 0f);
        }

		if(isWalled)
		{
			movement = new Vector2(runSpeed * 0.3f, 0f);
			anim.SetBool("isClimbing", true);
		}
		else
		{
			anim.SetBool("isClimbing", false);
		}
		
		if(isWalled && isSleeping)
		{
			anim.SetInteger("count", 7);
			anim.SetBool("isClimbing", false);
		}
		else if (isWalled && isAwaking)
		{
			anim.SetInteger("count", 8);
			anim.SetBool("isClimbing", true);
		}

        if (isSleeping)
        {
            anim.SetInteger("count", 5);
        }
        else if (isAwaking)
        {
            anim.SetInteger("count", 6);
        }
        else if (isStopped)
        {
            anim.SetInteger("count", 2);
		}
		else if (isDancing)
		{
			anim.SetBool("isDancing", true);
			anim.SetInteger("count", 9);
		}
        else
        {
            anim.SetInteger("count", 1);
		}
	}

	void FixedUpdate () {		

		rb.velocity = movement;

		isGrounded = Physics2D.OverlapCircle (groundCheck.position,   0.1f,  groundLayer);
		isWalled   = Physics2D.OverlapCircle (wallCheck.position,     0.15f, groundLayer);
		isCeiled   = Physics2D.OverlapCircle (ceilingCheck.position,  0.2f,  groundLayer);
		isCeiled   = Physics2D.OverlapCircle (ceilingCheck2.position, 0.2f,  groundLayer);

		if(jump)
		{
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            AudioEventManager.TriggerPlaySoundID(jumpSoundId);
            jump = false;
            jumpClicked = false;
		}

	}

    void FireMissile()
    {
        // if no missile left there won't be any action
        if (missileCount > 0) 
        {
            //Sprite of the missile is obtained from the source files in Unity
            missilePrefab.GetComponent<SpriteRenderer>().sprite = missileSkin;

            // If the player is on the ground there will be running force to shoot the missile but otherwise force should be added
            if (!isGrounded)
            {
                //misille created and shot from players position onward
                missile = PhotonNetwork.Instantiate(missilePrefab.name, new Vector2(transform.position.x - 0.5f, transform.position.y - 0.1f), Quaternion.identity, 0);
                missile.GetComponent<SpriteRenderer>().sprite = missileSkin;
            }
            else
            {
                //missile creted and shot with added force
                missile = PhotonNetwork.Instantiate(missilePrefab.name, new Vector2(transform.position.x + 0.5f, transform.position.y), Quaternion.identity, 0);
                missile.GetComponent<SpriteRenderer>().sprite = missileSkin;
                missile.GetComponent<Rigidbody2D>().AddForce(new Vector2(missileForce, 0));
            }
            //Players missile count decreases.
            missileCount--;
            //Missile shooting sound is activated.
            AudioEventManager.TriggerPlaySoundID(missileSoundId);
            //If hit by a missile player should stop for a while
            OnMissiled(gameObject);
        }
        fireClicked = false;
    }

	void OnEnable() {
		
		FinishScript.OnFinished      += Finished;
		FinishScript.OnFinishWon     += OnRaceWon;
		StunTrapScript.OnStunned     += Stunning;
		MissileScript.OnStun         += Stunning;
		MissileBoxScript.OnDonate    += Donating;
        GameController.JumpClicked   += OnJumpClicked;
        GameController.FireClicked   += OnFireClicked;
	}

	void OnDisable() {
		
		FinishScript.OnFinished      -= Finished;
		FinishScript.OnFinishWon     -= OnRaceWon;
		StunTrapScript.OnStunned     -= Stunning;
		MissileScript.OnStun         -= Stunning;
		MissileBoxScript.OnDonate    -= Donating;
        GameController.JumpClicked   -= OnJumpClicked;
        GameController.FireClicked   -= OnFireClicked;
	}

	void Finished(GameObject go) {
		
		if(gameObject == go && !isDancing) {
			runSpeed   = 0;
			jumpForce  = 0;

			isStopped  = true;
			isAwaking  = false;
			isSleeping = false;
			isFinished = true;
		}
	}

	void OnRaceWon(GameObject go) {

		if(gameObject == go) {
			runSpeed   = 0;
			jumpForce  = 0;

			isStopped  = false;
			isAwaking  = false;
			isSleeping = false;
			isFinished = true;
			isDancing  = true;
		}
	}

	void Stunning(GameObject go) {
		
		if(gameObject == go) {
			if(isGrounded){
				runSpeed   = 0;
				jumpForce  = 0;

				isAwaking  = false;
				isSleeping = true;
			}
		}
	}

	void Recovering() {

		isSleeping = false;
		isAwaking  = true;

		runSpeed   = tempRun;
		jumpForce  = tempJump;
	}

	void Donating(GameObject go) {
		
		if(gameObject == go) {
			missileCount++;
		}
	}

    void OnJumpClicked()
    {
        jumpClicked = true;
    }

    void OnFireClicked()
    {
        fireClicked = true;
    }
          
}
