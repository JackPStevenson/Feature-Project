using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Inputs : MonoBehaviour
{
    public static Inputs instance;

    [Header("Movement")]
    public InputActionReference MoveBind;
    public static InputActionReference Move;
    public static Vector2 moveDir
    {
        get { return Move.action.ReadValue<Vector2>(); }
    }


    [Header("Jumping")]
    public InputActionReference JumpBind;
    public static InputActionReference Jump;
    public static bool isJumping
    {
        get { return Jump.action.IsPressed(); }
    }


    [Header("Attack")]
    public InputActionReference AttackBind;
    public static InputActionReference Attack;
    public static bool attack
    {
        get { return Attack.action.IsPressed(); }
    }


    [Header("Mouse")]
    public InputActionReference MouseBind;
    public static InputActionReference Mouse;
    public static Vector2 mouseDelta
    {
        get {
            Vector2 delta = Mouse.action.ReadValue<Vector2>();
            delta = new Vector2(-delta.y, delta.x);
            
            return delta;
        }
    }

    [Header("Ctrl")]
    public InputActionReference CtrlBind;
    public static InputActionReference Ctrl;
    public static bool ctrl
    {
        get { return Ctrl.action.IsPressed(); }
    }

    [Header("Arrow Keys")]
    public InputActionReference ArrowKeysBind;
    public static InputActionReference ArrowKeys;
    public static Vector2 arrowKeys
    {
        get { return ArrowKeys.action.ReadValue<Vector2>(); }
    }


    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Input system already exists. Disabling...");
            gameObject.SetActive(false);

            return;
        }
        instance = this;

        // Movement
        Move = MoveBind;
        Jump = JumpBind;

        // Attack
        Attack = AttackBind;

        // Mouse
        Mouse = MouseBind;

        // Control
        Ctrl = CtrlBind;

        // Arrow Keys
        ArrowKeys = ArrowKeysBind;
    }
}
