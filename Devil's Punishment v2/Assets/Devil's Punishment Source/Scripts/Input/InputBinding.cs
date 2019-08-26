using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InputBinding
{
    public BindingType type;

    public KeyCode key;

    public int mouseButton;

    public Vector2 scrollDirection;

    public InputBinding(KeyCode key)
    {
        type = BindingType.Keyboard;
        this.key = key;
        mouseButton = -1;
        scrollDirection = Vector2.zero;
    }

    public InputBinding(int mouseButton)
    {
        type = BindingType.MouseButton;
        key = KeyCode.None;
        this.mouseButton = mouseButton;
        scrollDirection = Vector2.zero;
    }

    public InputBinding(Vector2 scrollDirection)
    {
        type = BindingType.MouseScroll;
        key = KeyCode.None;
        mouseButton = -1;
        this.scrollDirection = scrollDirection;
    }

    public bool GetKeyDown()
    {
        switch (type)
        {
            case BindingType.Keyboard:
                return Input.GetKeyDown(key);
            case BindingType.MouseButton:
                return Input.GetMouseButtonDown(mouseButton);
            case BindingType.MouseScroll:
                return Vector2.Dot(Input.mouseScrollDelta, scrollDirection) > 0f;
        }
        Debug.LogError("Looks like you added a new input type and forgot to wire up the Fire() method.");
        return false;
    }

    public override string ToString()
    {
        switch (type)
        {
            case BindingType.Keyboard:
                return key.ToString();
            case BindingType.MouseButton:
                switch (mouseButton)
                {
                    case 0: return "Left Mouse";
                    case 1: return "Right Mouse";
                    case 2: return "MWheel Click";
                    default: return "Error - Invalid Mouse Button";
                }
            case BindingType.MouseScroll:
                if (scrollDirection.y > 0)
                    return "M Scroll Up";
                if (scrollDirection.y < 0)
                    return "M Scroll Down";
                if (scrollDirection.x > 0)
                    return "M Scroll Right";
                if (scrollDirection.x < 0)
                    return "M Scroll Left";
                return "Error - Invalid Mouse Scroll";
        }
        return "Error - Invalid Input BindingType";
    }
}
