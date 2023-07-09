using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Colors
{
    public static Type RandomColor() {
        return (Type)Random.Range(0, System.Enum.GetValues(typeof(Type)).Length);
    }

    public enum Type {
        Red, Green, Blue, Purple
    }
}
