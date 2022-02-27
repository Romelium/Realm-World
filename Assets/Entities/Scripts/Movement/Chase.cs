using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Controller))]
public class Chase : MonoBehaviour
{
    public Transform target;
    public float chaseRadius = 10;
    public float stoppingDistance = 0.2f;
    public bool chasing;
    public bool sprint;
    public bool jump;

    private Controller controller;
    void Awake()
    {
        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player").transform;
        controller = GetComponent<Controller>();

        ActionDistance actionDistance;
        if (TryGetComponent<ActionDistance>(out actionDistance))
            actionDistance.target = target;
    }

    // Update is called once per frame
    void Update()
    {
        var dir = target.position - transform.position;
        chasing = dir.magnitude < chaseRadius && dir.magnitude > stoppingDistance;
        if (chasing)
            controller.Move(dir.normalized, sprint, jump);
        else
            controller.Move(Vector3.down, sprint, jump);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);
    }
}
