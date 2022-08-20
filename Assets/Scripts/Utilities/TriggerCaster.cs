using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCaster : MonoBehaviour
{
    [SerializeField]
    private Collider2D _trigger;

    public delegate void Function(Collider2D collision);

    public Function TriggerFunction { get; set; }

    private void Start()
    {
        _trigger.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (TriggerFunction != null)
        {
            TriggerFunction(collision);
        }
    }
}
