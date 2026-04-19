using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Trumpet;

public class Cannon : MonoBehaviour
{
    public enum CannonState { Ready, NotReady }

    public CannonState State = CannonState.Ready;
    public float EnableTimer = 0.00f;
    public float FuseTimer = 0.00f;
    GameObject spriteGO;
    SpriteRenderer sprite;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        spriteGO = transform.Find("sprite").gameObject;
        sprite = spriteGO.GetComponent<SpriteRenderer>();
        animator = spriteGO.GetComponent<Animator>();
    }

    public void Fuse(float fuseTime)
    {
        State = CannonState.NotReady;
        animator.SetTrigger("CannonFuse");
        FuseTimer = fuseTime;
    }
    public void Fired(float enableTime)
    {
        //Debug.Log("Heard");
        FuseTimer = 0;
        EnableTimer = enableTime;
        animator.SetTrigger("CannonNotAvailable");
    }

    public void Ready()
    {
        //Debug.Log("Ready");
        State = CannonState.Ready;
        animator.SetTrigger("CannonAvailable");
        EnableTimer = 0.00f;
    }

    public void Deactivate()
    {
        //Debug.Log("Deactivate");
        State = CannonState.NotReady;
        animator.SetTrigger("CannonNotAvailable");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
