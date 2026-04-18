using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public enum UnitState { Holding, Advancing, Fighting, Pursuing, Retreating  };

    public UnitState State = UnitState.Holding;

    GameObject spriteGO;
    SpriteRenderer sprite;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        spriteGO = transform.Find("sprite").gameObject;
        sprite = spriteGO.GetComponent<SpriteRenderer>();
        animator = spriteGO.GetComponent<Animator>();
        animator.speed = 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
