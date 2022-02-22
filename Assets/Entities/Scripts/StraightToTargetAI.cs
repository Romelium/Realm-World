using UnityEngine;

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
            controller.Move(target.position - transform.position, false);
    }
}