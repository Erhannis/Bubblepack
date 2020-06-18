using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle
{
    public Vector2 pos;
    public float radius;
    public Color color;

    public Circle(Vector2 pos, Color color) {
        this.pos = pos;
        this.radius = 0f;
        this.color = color;
    }

    public bool touching(Circle b) {
        if ((this.pos - b.pos).magnitude <= (this.radius + b.radius)) {
            return true;
        } else {
            return false;
        }
    }

    public float area() {
        return Mathf.PI * radius * radius;
    }
}
