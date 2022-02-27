using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActionDistance : MonoBehaviour
{
    public Transform target;
    public float actionDistance = 1.8f;
    public UnityEvent action;

    // Update is called once per frame
    void Update()
    {
        if ((target.position - transform.position).magnitude < actionDistance)
        {
            action.Invoke();
        }
    }
}
