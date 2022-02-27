using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterController))]
public class Controller : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Move speed of the character in m/s")]
    public float MoveSpeed = 2.0f;
    [Tooltip("Sprint speed of the character in m/s")]
    public float SprintSpeed = 5.335f;
    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.12f;
    [Tooltip("Acceleration and deceleration")]
    public float SpeedChangeRate = 10.0f;

    [Space(10)]
    [Tooltip("The height the player can jump")]
    public float JumpHeight = 1.2f;
    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    public float Gravity = -15.0f;

    [Space(10)]
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    public float JumpTimeout = 0.50f;
    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    public float FallTimeout = 0.15f;

    [Header("Grounded")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    public bool Grounded = true;
    [Tooltip("Useful for rough ground")]
    public float GroundedOffset = -0.14f;
    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.28f;
    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;
    [Header("Animation")]
    public string SpeedParamName = "Speed";
    public string GroundedParamName = "Grounded";
    public string JumpParamName = "Jump";
    public string FreeFallParamName = "FreeFall";
    public string MotionSpeedParamName = "MotionSpeed";

    // player
    private float _speed;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;
    private Vector3 _frictionVelocity;

    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    private CharacterController _controller;
    private Animator _animator;
    private bool _hasAnimator;
    private float _animationBlend;
    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;
    private HashSet<int> _validParameters = new HashSet<int>();

    private void Start()
    {
        _hasAnimator = TryGetComponent(out _animator);
        _controller = GetComponent<CharacterController>();

        if (_hasAnimator)
            AnimationInit();

        // reset our timeouts on start
        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;
    }
    void Update()
    {
        AnimationInit();
    }

    private void AnimationInit()
    {
        _animIDSpeed = Animator.StringToHash(SpeedParamName);
        _animIDGrounded = Animator.StringToHash(GroundedParamName);
        _animIDJump = Animator.StringToHash(JumpParamName);
        _animIDFreeFall = Animator.StringToHash(FreeFallParamName);
        _animIDMotionSpeed = Animator.StringToHash(MotionSpeedParamName);
        for (int i = 0; i < _animator.parameters.Length; i++)
        {
            _validParameters.Add(_animator.parameters[i].nameHash);
        }
    }

    public void Move(Vector3 move, bool sprint, bool jump)
    {
        Move(new Vector2(move.x, move.z), sprint, jump);
    }
    public void Move(Vector2 move, bool sprint, bool jump)
    {
        JumpAndGravity(jump);
        GroundedCheck();
        Movement(move, sprint);
    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        var others = Physics.OverlapSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
        if (others.Length != 0)
        {
            Grounded = true;
            var rb = others[0].attachedRigidbody;
            _frictionVelocity = rb == null ? Vector3.zero : rb.velocity;
        }
        else
        {
            Grounded = false;
            _frictionVelocity = Vector3.zero;
        }

        // update animator if using character
        if (_hasAnimator && _validParameters.Contains(_animIDGrounded))
        {
            _animator.SetBool(_animIDGrounded, Grounded);
        }
    }

    private void Movement(Vector2 move, bool sprint)
    {
        // set target speed based on move speed, sprint speed and if sprint is pressed
        float targetSpeed = sprint ? SprintSpeed : MoveSpeed;

        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

        // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is no input, set the target speed to 0
        if (move == Vector2.zero) targetSpeed = 0.0f;

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x - _frictionVelocity.x, 0.0f, _controller.velocity.z - _frictionVelocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = move.magnitude;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

            // round speed to 3 decimal places
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        // normalise input direction
        Vector3 inputDirection = new Vector3(move.x, 0.0f, move.y).normalized;

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (move != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);

            // rotate to face input direction relative to camera position
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }


        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
        Vector3 velocity = targetDirection.normalized * _speed + new Vector3(0.0f, _verticalVelocity, 0.0f) + _frictionVelocity;

        // move the player
        _controller.Move(velocity * Time.deltaTime);

        // update animator if using character
        if (_hasAnimator)
        {
            if (_validParameters.Contains(_animIDSpeed))
            {
                _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
                _animator.SetFloat(_animIDSpeed, _animationBlend);
            }
            if (_validParameters.Contains(_animIDMotionSpeed))
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
        }
    }

    private void JumpAndGravity(bool jump)
    {
        if (Grounded)
        {
            // reset the fall timeout timer
            _fallTimeoutDelta = FallTimeout;

            // stop our velocity dropping infinitely when grounded
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            // Jump
            if (jump && _jumpTimeoutDelta <= 0.0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                // update animator if using character
                if (_hasAnimator && _validParameters.Contains(_animIDJump))
                {
                    _animator.SetBool(_animIDJump, true);
                }
            }
            else if (_hasAnimator && _validParameters.Contains(_animIDJump))
                // update animator if using character
                _animator.SetBool(_animIDJump, false);

            // jump timeout
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }

            // update animator if using character
            if (_hasAnimator && _validParameters.Contains(_animIDJump))
            {
                _animator.SetBool(_animIDFreeFall, false);
            }
        }
        else
        {
            // reset the jump timeout timer
            _jumpTimeoutDelta = JumpTimeout;

            // fall timeout
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else if (_hasAnimator && _validParameters.Contains(_animIDJump))
                // update animator if using character
                _animator.SetBool(_animIDFreeFall, true);
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += Gravity * Time.deltaTime;
        }
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (Grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
    }
}