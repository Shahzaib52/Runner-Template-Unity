using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class char_controller : MonoBehaviour
{
    public enum type
    {
        Human, AI
    }

    public type charType;

    [Space(10)]
    public float xMoveLimit;
    public float min_yMoveLimit, max_yMoveLimit;
    public bool isTransitioning;
    public float targetPosition;
    private float side = 0;
    private float prevSide = 0;

    public delegate void charDelegate();
    public charDelegate OnSideWallHit, OnFrontHit, OnDead;

    public delegate void charPosDelegate(float pos);
    public charPosDelegate OnCharPositionChanged;


    [Space(10)]
    public Rigidbody rigidbody;
    public float speed = 5;
    private float maxSpeed;
    public float forwardSpeed = 10;

    [Range(0, 1)]
    public float transitionValue;
    public bool isDead;

    public bool isGrounded;

    public int lives = 3;

    public List<string> obstacleTags;

    [Header("AI Paramters: ")]
    public char_controller _Target;
    public float rayDistance = 3;
    public LayerMask mask;
    public float minZ, maxZ;
    public float _keepDistance;
    private Coroutine coroutineRef;

    private float playerPos;
    // Start is called before the first frame update
    void Start()
    {
        maxSpeed = speed;

        if (charType == type.AI)
        {
            _Target.OnSideWallHit += OnTargetHit;
            _Target.OnDead += OnTargetDead;
            _Target.OnCharPositionChanged += OnTargetChangedPosition;
        }
    }

    #region AI Callbacks
    void OnTargetHit()
    {
        Mathf.Clamp(forwardSpeed += 10, 50, 70);
    }

    void OnTargetDead()
    {
        forwardSpeed = 0;
    }

    void OnTargetChangedPosition(float pos)
    {
        if (coroutineRef == null)
        {
            coroutineRef = StartCoroutine(changePosition(pos));
        }
    }

    IEnumerator changePosition(float pos)
    {
        yield return new WaitForSeconds(0.1f);
        playerPos = pos;
        coroutineRef = null;
    }
    #endregion

    void FixedUpdate()
    {
        if (isDead)
        {
            if (!rigidbody.isKinematic)
            {
                rigidbody.isKinematic = true;

                if (charType == type.Human)
                {
                    if (OnDead != null)
                    {
                        OnDead();
                    }
                }
            }
            return;
        }

        targetPosition = Mathf.MoveTowards(targetPosition, side, speed * Time.fixedDeltaTime);
        var reqPos = new Vector3(targetPosition, transform.position.y, transform.position.z);

        if (transform.position.x != targetPosition)
        {
            rigidbody.MovePosition(reqPos);
        }

        var forceVector = new Vector3(transform.position.x, transform.position.y, forwardSpeed);
        rigidbody.AddForce(forceVector, ForceMode.Acceleration);
        rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, forwardSpeed / 2);
    }

    void LateUpdate()
    {
        if (charType == type.AI)
        {
            minZ = _Target.transform.position.z - _keepDistance;
            maxZ = _Target.transform.position.z - 2f;
            transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, min_yMoveLimit, max_yMoveLimit), Mathf.Clamp(transform.position.z, minZ, maxZ));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (charType == type.AI)
        {
            ObstacleAvoidance();
        }
        else
        {
            var left = (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) && !isTransitioning;
            var right = (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && !isTransitioning;

            if (left)
            {
                prevSide = side;
                if (side == xMoveLimit)
                {
                    side = 0;
                }
                else
                {
                    side -= 3;
                }
            }
            else if (right)
            {
                prevSide = side;
                if (side == -xMoveLimit)
                    side = 0;
                else
                    side += 3;
            }

            if (OnCharPositionChanged != null)
                OnCharPositionChanged(side);
        }
        Movement();
    }

    #region AI [EXPERIMENTAL]
    void ObstacleAvoidance()
    {
        if (isTransitioning || _Target.isDead)
            return;

        var hit = new RaycastHit();

        var isRightBlocked = false;
        var isLeftBlocked = false;
        var isFrontBlocked = false;

        if (side != playerPos)
        {
            if (playerPos == xMoveLimit)
            {
                // TODO: Player is on Rightmost lane
                isRightBlocked = Physics.Raycast(transform.position, transform.right, out hit, rayDistance, mask);
                if (isRightBlocked)
                {
                    // just keep going forward
                }
                else
                {
                    prevSide = side;
                    side = playerPos;
                    return;
                }
            }
            else if (playerPos == -xMoveLimit)
            {
                // TODO: Player is on Leftmost lane

                isLeftBlocked = Physics.Raycast(transform.position, -transform.right, out hit, rayDistance, mask);
                if (isLeftBlocked)
                {
                    // just keep going forward
                }
                else
                {
                    prevSide = side;
                    side = playerPos;
                    return;
                }
            }
            else
            {
                // Player is in dead center

                // if we have have something on the front
                isFrontBlocked = Physics.Raycast(transform.position, transform.forward, out hit, rayDistance, mask);

                // if the side player is currently on, has something that we could collide with shall we move to that side
                var BlockageAhead = Physics.Raycast(_Target.transform.position, -_Target.transform.forward, out hit, rayDistance * 2.5f, mask);;
                
                if (playerPos == 0)
                {
                    if(BlockageAhead && !isFrontBlocked)
                    {
                        return;
                    }

                    isLeftBlocked = Physics.Raycast(transform.position, -transform.right, out hit, rayDistance, mask);

                    if (isLeftBlocked)
                    {
                        isRightBlocked = Physics.Raycast(transform.position, transform.right, out hit, rayDistance, mask);
                        if (isRightBlocked)
                        {
                            // TODO: cater a scenario where we have no side left to move to, could be an interesting place to code duck or jump logic for AI
                        }
                        else
                        {
                            prevSide = side;
                            if (side == xMoveLimit || side == -xMoveLimit)
                            {
                                side = 0;
                            }
                            else
                                side += 3;
                        }
                    }
                    else
                    {
                        prevSide = side;
                        if (side == xMoveLimit || side == -xMoveLimit)
                        {
                            side = 0;
                        }
                        else
                            side -= 3;
                    }
                }
            }
        }
    }
    #endregion

    #region Movement
    void Movement()
    {
        isTransitioning = side != targetPosition;

        side = Mathf.Clamp(side, -xMoveLimit, xMoveLimit);
        var value = Mathf.Clamp(targetPosition, -xMoveLimit, xMoveLimit);
        targetPosition = value;

        var transVal = 0f;
        if (targetPosition == 0)
            transVal = prevSide;
        else
            transVal = targetPosition;

        transitionValue = Mathf.Abs(1 - (transform.position.x) / xMoveLimit);
    }
    #endregion

    void OnCollisionEnter(Collision other)
    {
        if (tag == "platform" || tag == "ground")
        {
            isGrounded = true;
        }
    }

    public void OnHit(Collider col, sensor _sensor)
    {
        if (obstacleTags.Contains(col.tag))
        {
            switch (_sensor.sensorType)
            {
                case sensor.type.front:
                    lives = -1;

                    if (charType == type.AI)
                        ObstacleAvoidance();
                    break;
                case sensor.type.back:
                    lives = -1;
                    break;
                default:
                    side = prevSide;
                    lives--;
                    break;
            }

            if (charType == type.Human)
            {
                if (OnSideWallHit != null)
                    OnSideWallHit();
            }

            if (lives < 0)
            {
                lives = 0;
                switch (charType)
                {
                    case type.AI:

                        break;
                    default:
                        isDead = true;
                        GameManager.instance.OnPlayerDied();
                        break;
                }
            }
        }
    }
}
