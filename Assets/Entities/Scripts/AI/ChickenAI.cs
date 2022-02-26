using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller))]
public class ChickenAI : MonoBehaviour
{
    public float wanderRadius = 5;
    public float wanderTime = 10;

    private float wanderTimeDelta;
    private Vector3 target;

    private Controller controller;
    void Awake()
    {
        controller = GetComponent<Controller>();
    }
    void Start()
    {
        wanderTimeDelta = wanderTime;
    }
    void Update()
    {
        wanderTimeDelta += Time.deltaTime;
        if (wanderTimeDelta > wanderTime)
        {
            var random = Random.insideUnitCircle * wanderRadius;
            target = transform.position + new Vector3(random.x, 0, random.y);

            wanderTimeDelta = 0;
        }
        var dir = target - transform.position;
        controller.Move(dir.magnitude > 1 ? dir.normalized : dir, wanderTime / wanderTimeDelta > 0.5, false);
    }
}
