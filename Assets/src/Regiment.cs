using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Regiment
{
    public readonly int Order;
    public Color Color;
    public Unit Unit;
    public Regiment(int order, Color color)
    {
        Order = order;
        Color = color;
    }
    public string GetName()
    {
        string num = "th";
        if (Order == 1) num = "st";
        else if (Order == 2) num = "nd";
        else if (Order == 3) num = "rd";
        
        return Order+num+" Regiment";
    }
}
