using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Controller))]
public class StraightToTargetAI : MonoBehaviour
{
    public Transform target;
    private Controller controller;
    void Awake()
    {
        controller = GetComponent<Controller>();
    }
    void Update()
    {
        if (target != null)
            controller.Move((target.position - transform.position).normalized, false, false);
        else
            controller.Move(Vector2.zero, false, false);
    }
}