using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Controller : MonoBehaviour
{
    public float movingTurnSpeed = 360;
    public float stationaryTurnSpeed = 180;
    public float jumpPower = 12f;
    public float gravityMultiplier = 2f;
    public float moveSpeedMultiplier = 1f;
    public float groundCheckDistance = 0.1f;

    public bool m_IsGrounded;
    public float m_TurnAmount;
    public float m_ForwardAmount;
    public Vector3 m_GroundNormal;

    Rigidbody m_Rigidbody;

    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();

        m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }


    public void Move(Vector3 move, bool jump)
    {

        // convert the world relative moveInput vector into a local-relative
        // turn amount and forward amount required to head in the desired
        // direction.
        move.Normalize();
        move = transform.InverseTransformDirection(move);
        CheckGroundStatus();
        move = Vector3.ProjectOnPlane(move, m_GroundNormal);
        m_TurnAmount = Mathf.Atan2(move.x, move.z);
        m_ForwardAmount = move.z;

        ApplyExtraTurnRotation();

        // control and velocity handling is different when grounded and airborne:
        if (m_IsGrounded)
        {
            HandleGroundedMovement(jump);
        }
        else
        {
            HandleAirborneMovement();
        }

        // send input and other state parameters to the animator
        if (m_IsGrounded && Time.deltaTime > 0)
        {
            Vector3 v = transform.forward * m_ForwardAmount * moveSpeedMultiplier;
            v.y = m_Rigidbody.velocity.y;
            m_Rigidbody.velocity = v;
        }
    }

    void HandleAirborneMovement()
    {
        // apply extra gravity from multiplier:
        Vector3 extraGravityForce = (Physics.gravity * gravityMultiplier) - Physics.gravity;
        m_Rigidbody.AddForce(extraGravityForce);
    }


    void HandleGroundedMovement(bool jump)
    {
        // check whether conditions are right to allow a jump:
        if (jump)
        {
            // jump!
            m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, jumpPower, m_Rigidbody.velocity.z);
            m_IsGrounded = false;
            groundCheckDistance = 0.1f;
        }
    }

    void ApplyExtraTurnRotation()
    {
        // help the character turn faster (this is in addition to root rotation in the animation)
        float turnSpeed = Mathf.Lerp(stationaryTurnSpeed, movingTurnSpeed, m_ForwardAmount);
        transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
    }

    void CheckGroundStatus()
    {
        RaycastHit hitInfo;
#if UNITY_EDITOR
        // helper to visualise the ground check ray in the scene view
        Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * groundCheckDistance));
#endif
        // 0.1f is a small offset to start the ray from inside the character
        // it is also good to note that the transform position in the sample assets is at the base of the character
        if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, groundCheckDistance))
        {
            m_GroundNormal = hitInfo.normal;
            m_IsGrounded = true;
        }
        else
        {
            m_IsGrounded = false;
            m_GroundNormal = Vector3.up;
        }
    }
}