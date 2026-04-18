using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public enum UnitState { Holding, Advancing, Fighting, Pursuing, Retreating  };

    public UnitState State = UnitState.Holding;
    public UnitState NextState = UnitState.Holding;
    public float NextStateTimer = 0.00f;

    GameObject spriteGO;
    SpriteRenderer sprite;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        spriteGO = transform.Find("sprite").gameObject;
        sprite = spriteGO.GetComponent<SpriteRenderer>();
        animator = spriteGO.GetComponent<Animator>();
        animator.speed = FixedValues.AnimationSpeedHolding;
    }

    public void ToNextState()
    {
        NextStateTimer = 0.00f;
        State = NextState;
        switch (State)
        {
            case UnitState.Holding:
                StartHold();
                break;
            case UnitState.Advancing:
                StartAdvance();
                break;
            case UnitState.Fighting:
                StartFight();
                break;
        }
    }
    void StartAdvance()
    {
        animator.speed = FixedValues.AnimationSpeedMoving;
    }
    void StartHold()
    {
        animator.speed = FixedValues.AnimationSpeedHolding;
    }
    void StartFight()
    {
        animator.speed = FixedValues.AnimationSpeedFighting;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
