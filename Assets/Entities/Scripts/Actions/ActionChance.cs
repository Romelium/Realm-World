using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class ActionChance : MonoBehaviour
{
    [System.Serializable]
    public class ActionProps
    {
        public float chance = 0.75f;
        public UnityEvent Action;
    }
    public float interval;
    public ActionProps[] actions;

    private float intervalDelta;

    public void Invoke()
    {
        intervalDelta += Time.deltaTime;
        if (intervalDelta > interval)
        {
            for (int i = 0; i < actions.Length; i++)
            {
                if (actions[i].chance > Random.value)
                {
                    actions[i].Action.Invoke();
                };
            }
            intervalDelta = 0f;
        }
    }
}