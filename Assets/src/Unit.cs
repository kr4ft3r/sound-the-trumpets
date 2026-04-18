using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public enum UnitState { Holding, Advancing, Fighting, Pursuing, Retreating  };
    public enum UnitFightState { None, Pursuing, Retreating, Stalemate}

    public UnitState State = UnitState.Holding;
    public UnitState NextState = UnitState.Holding;
    public float NextStateTimer = 0.00f;
    public float groundTaken = 0;
    public int groundTakenPercent = 0;
    public int Strength = 0;
    public UnitFightState FightState = Unit.UnitFightState.None;
    public int FightDifference = 0;
    public float LastHealTime = 0;

    GameObject spriteGO;
    SpriteRenderer sprite;
    Animator animator;
    Transform strengthBar;
    SpriteRenderer strengthRenderer;
    // Start is called before the first frame update
    void Start()
    {
        spriteGO = transform.Find("sprite").gameObject;
        sprite = spriteGO.GetComponent<SpriteRenderer>();
        animator = spriteGO.GetComponent<Animator>();
        animator.speed = FixedValues.AnimationSpeedHolding;
        strengthBar = spriteGO.transform.Find("strengthBar");
        strengthRenderer = strengthBar.Find("Square").GetComponent<SpriteRenderer>();
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

    public void UpdatePosition(float originalX, float advanceDirection)
    {
        transform.position = new Vector2(originalX + groundTaken*advanceDirection, transform.position.y);
        groundTakenPercent = (int)Mathf.Round((groundTaken / FixedValues.GetFieldSize()) * 100);
    }
    public void UpdateStrengthHolding(float strength)
    {
        if (Strength < strength)
        {
            //Debug.Log("^ " + Strength);
            Strength += FixedValues.HoldingStrengthChangeRate;
        }
        else if (Strength > strength)
        {
            //Debug.Log("V " + Strength);
            Strength -= FixedValues.HoldingStrengthChangeRate;
        }
        if (Strength > strength) Strength = (int)strength;
        UpdateStrengthBar();
    }
    public void UpdateStrengthAdvancing(AnimationCurve curve, float strength, float traversed)
    {
        var traversedNormal = (traversed / FixedValues.GetFieldSize());
        //Debug.Log("!!!! " + traversedNormal);
        Strength = (int) Mathf.Round(strength * curve.Evaluate(traversedNormal));
        UpdateStrengthBar();
    }
    void UpdateStrengthBar()
    {
        strengthBar.localScale = new Vector3(Strength * .01f, 1, 1);
        if (strengthBar.localScale.x <= 1.0f)
        {
            strengthRenderer.color = Color.red;
        }
        else if (strengthBar.localScale.x >= 1.35f)
        {
            strengthRenderer.color = Color.white;
        }
        else
        {
            strengthRenderer.color = Color.green;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
