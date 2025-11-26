using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimContr : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current != null)
        {
            if (Keyboard.current.sKey.wasPressedThisFrame)
            {
                anim.Play("ShutDown");
            }

            if (Keyboard.current.wKey.wasPressedThisFrame)
            {
                anim.Play("WakeUp");
            }

            if (Keyboard.current.dKey.wasPressedThisFrame)
            {
                anim.Play("Destroyed");
            }
        }
    }
}
