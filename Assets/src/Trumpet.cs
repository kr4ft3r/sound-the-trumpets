using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Trumpet : MonoBehaviour
{
    public enum TrumpetState { Ready, NotReady }

    public TrumpetState State = TrumpetState.Ready;
    public float EnableTimer = 0.00f;
    GameObject spriteGO;
    SpriteRenderer sprite;
    Animator animator;
    TextMeshPro key;
    public bool Deactivated = false;
    // Start is called before the first frame update
    void Start()
    {
        spriteGO = transform.Find("sprite").gameObject;
        sprite = spriteGO.GetComponent<SpriteRenderer>();
        animator = spriteGO.GetComponent<Animator>();
        key = transform.Find("number").GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetKey(int number, float xMod)
    {
        if (number == 10) number = 0;
        var key = transform.Find("number").GetComponent<TextMeshPro>();
        key.text = number.ToString();
        key.transform.localPosition += (Vector3.right * -0.8f * xMod);
    }

    public void Blow(float enableTime)
    {
        //Debug.Log("Blow");
        State = TrumpetState.NotReady;
        animator.SetTrigger("TrumpetBlowing");
        EnableTimer = enableTime;
    }
    public void Heard()
    {
        //Debug.Log("Heard");
        animator.SetTrigger("TrumpetNotAvailable");
    }
    public void Ready()
    {
        //Debug.Log("Ready");
        State = TrumpetState.Ready;
        animator.SetTrigger("TrumpetAvailable");
        EnableTimer = 0.00f;
    }
    public void Deactivate()
    {
        //Debug.Log("Deactivate");
        State = TrumpetState.NotReady;
        animator.SetTrigger("TrumpetNotAvailable");
        Deactivated = true;
    }
}
