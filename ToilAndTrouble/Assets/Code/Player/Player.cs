using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    [Header("Controller options")]
    public bool platformerMovement;
    public bool variableJumping;


    CharacterController controller;
    Animator anim;

    [Space(10f)]
    [SerializeField] private float moveSpeed = 6;
    [SerializeField] private float minJumpHeight = 1;
    [SerializeField] private float medJumpHeight = 1;
    [SerializeField] private float maxJumpHeight = 4;
    [SerializeField] private float minJumpDist = 2;
    [SerializeField] private float medJumpDist = 2;
    [SerializeField] private float maxJumpDist = 6;
    [SerializeField] private float timeToJumpPeak = .4f;
    [SerializeField] private float jumpFallModifier = 2;
    [SerializeField] private float accelerationTimeInAir = .2f;
    [SerializeField] private float accelerationTimeGrounded = .1f;
    [SerializeField] private float minimumVelocity = 4;
    [SerializeField] private float maximumVelocity = 12;


    private float gravity;
    private float baseGravity;
    private float minJumpVelocity;
    private float maxJumpVelocity;
    private Vector3 velocity, oldPos;
    float movementSmoothing;
    bool won = false;

    private float _deathValueX = 0.0001f;
    bool died = false;

    // casting stuff
    Vector2 castSize;

    enum SpeedLevel { slow, medium, fast };
    SpeedLevel speedLevel;

    public int bufferSize = 12;     //How many frames the input buffer keeps checking for new inputs / The Size of the Buffer
    public InputBufferItem[] inputBuffer;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();

        //starts with medium speed
        speedLevel = SpeedLevel.slow;
        //calculate gravity and velocity from wanted jump height and time to peak
        gravity = -(2 * maxJumpHeight / Mathf.Pow(timeToJumpPeak, 2));
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpPeak;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
        baseGravity = gravity;
    

        inputBuffer = new InputBufferItem[bufferSize];
        for (int i = 0; i < inputBuffer.Length; i++)
        {
            inputBuffer[i] = new InputBufferItem();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (!died)
        {
            if (controller.collisions.above || controller.collisions.below)
            {
                velocity.y = 0;
            }

            //if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))
            //{
            //    if (controller.collisions.below)
            //    {
            //        FastFallingJump();
            //    }
            //}

            UpdateBuffer();
            UpdateCommand();

            if (variableJumping)
            {
                if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.UpArrow))
                {
                    if (velocity.y > (maxJumpVelocity / 2))
                        velocity.y = (maxJumpVelocity / 2);
                }
            }
        }

        //for test
        if (!platformerMovement)
            RunnerMovement();
        else
            StandartMovement();

        velocity.y += gravity * Time.deltaTime;
        oldPos = transform.position;

        // moves player
        controller.Move(velocity * Time.deltaTime);

        if (!died)
        {
            //DoDangerAndPickup();
            if (transform.position.x - oldPos.x < _deathValueX && velocity.y <= 0 && !won) Die();
        }
    }

    void UpdateBuffer()
    {
        if (Input.GetAxisRaw("Jump") > 0) { inputBuffer[inputBuffer.Length - 1].Hold(); }
        else { inputBuffer[inputBuffer.Length - 1].ReleaseHold(); }

        //Go through each Input Buffer item and copy the previous frame
        for (int i = 0; i < inputBuffer.Length - 1; i++)
        {
            inputBuffer[i].hold = inputBuffer[i + 1].hold;
            inputBuffer[i].used = inputBuffer[i + 1].used;
        }
    }

    public void UpdateCommand()
    {
        for (int i = 0; i < inputBuffer.Length; i++)
        {
            if (inputBuffer[i].CanExecute())
            {
                if (controller.collisions.below)
                {
                    FastFallingJump();
                    inputBuffer[i].Execute();
                    break;
                }
            }
        }
    }

    void SpeedUp()
    {
        //TODO
        //set rain vector
        speedLevel++;
       
        anim.SetFloat("animSpeed", (float)speedLevel + 1);
     
    }

    void SpeedDown()
    {
        //TODO
        //set rain vector
        speedLevel--;
       
        anim.SetFloat("animSpeed", (float)speedLevel + 1);
       

    }

    void StandartMovement()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        float targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref movementSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeInAir);

    }

    void VariableJumping()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (controller.collisions.below)
                velocity.y = maxJumpVelocity;
        }
        if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.UpArrow))
        {
            if (velocity.y > minJumpVelocity)
                velocity.y = minJumpVelocity;
        }
    }

    void FastFallingJump()
    {
        //calculate gravity and velocity from wanted jump height and distance
        float jumpVelocityX;
        float jumpHeight;
        float jumpDist;

        switch (speedLevel)
        {
            case SpeedLevel.slow:
                jumpHeight = minJumpHeight;
                jumpDist = minJumpDist;
                jumpVelocityX = minimumVelocity;
                break;
            case SpeedLevel.medium:
                jumpHeight = medJumpHeight;
                jumpDist = medJumpDist;
                jumpVelocityX = (minimumVelocity + maximumVelocity) / 2;
                break;
            case SpeedLevel.fast:
                jumpHeight = maxJumpHeight;
                jumpDist = maxJumpDist;
                jumpVelocityX = maximumVelocity;
                break;
            default:
                jumpHeight = (minJumpHeight + maxJumpHeight) / 2;
                jumpDist = (minJumpDist + minJumpDist) / 2;
                jumpVelocityX = (minimumVelocity + maximumVelocity) / 2;
                break;
        }

        maxJumpVelocity = (2 * (jumpHeight * jumpVelocityX) / (jumpDist / 2));
        gravity = (-2 * jumpHeight * Mathf.Pow(jumpVelocityX, 2)) / Mathf.Pow((jumpDist / 2), 2);
        baseGravity = gravity;
        timeToJumpPeak = (jumpDist / 2) / jumpVelocityX;
        velocity.y = maxJumpVelocity;
        StartCoroutine(DoFallJump());

    }

    void RunnerMovement()
    {
        float targetVelocityX;

        switch (speedLevel)
        {
            case SpeedLevel.slow:
                targetVelocityX = minimumVelocity;
                break;
            case SpeedLevel.medium:
                targetVelocityX = (minimumVelocity + maximumVelocity) / 2;
                break;
            case SpeedLevel.fast:
                targetVelocityX = maximumVelocity;
                break;
            default:
                targetVelocityX = velocity.x;
                break;
        }

        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref movementSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeInAir);
    }

   //public void DoDangerAndPickup()
   // {
   //     RaycastHit2D[] hits = Physics2D.BoxCastAll(oldPos, castSize, 0f, velocity.normalized, velocity.magnitude * Time.fixedDeltaTime);

   //     for (int i = 0; i < hits.Length; i++)
   //     {
   //         Pickup pickup = hits[i].collider.GetComponent<Pickup>();
   //         if (pickup != null)
   //         {
   //             if (pickup.victoryPickup)
   //             { 
   //                 //win level
   //                 won = true;
   //                 platformerMovement = true;
   //                 GameManager.GM.EndScreen(true);
   //             }
   //             GameManager.GM.currentScore += pickup.score;
   //             pickup.Die();
   //         }

   //         Danger danger = hits[i].collider.GetComponent<Danger>();
   //         if (danger != null)
   //         {
   //             died = true;
   //         }
   //     }

   //     if (died && !won) Die();
   // }

    void Die()
    {
       // controller.colliding = false;
      //  GameManager.GM.EndScreen(false);
    }

    private IEnumerator DoFallJump()
    {

        while (velocity.y > 0)
        {
            yield return null;
        }
        gravity = gravity * jumpFallModifier;
        while (!controller.collisions.below)
        {
            yield return null;

        }
        gravity = baseGravity;
        yield return null;
    }

}
