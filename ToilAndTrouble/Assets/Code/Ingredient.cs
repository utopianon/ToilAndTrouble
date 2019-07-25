using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum IngredientType
{
    A,
    B,
    C,
    D
}

public class Ingredient : GrabbableObject
{

    public IngredientType type;

}

