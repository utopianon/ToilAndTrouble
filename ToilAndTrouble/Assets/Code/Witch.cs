using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Witch : MonoBehaviour
{
    public Material[] possibleIngredients;
    GameObject wantedSign;
    List<Ingredient> IngredientsInScene;
    public IngredientType wantedIngredient;

    float timer;


    public void Start()
    {
        wantedSign = transform.GetChild(0).gameObject;
        ChooseIngredient();    
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            ChooseIngredient();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject collidingObject = collision.gameObject;

        if (collidingObject.tag == "Ingredient")
        {
            if (collidingObject.GetComponent<Ingredient>().type == wantedIngredient)
            {
                Destroy(collidingObject);
                ChooseIngredient();
            }
        }
    }

    public void ChooseIngredient()
    {
        wantedIngredient = Utility.RandomEnumValue<IngredientType>();
        wantedSign.GetComponent<Renderer>().material = possibleIngredients[(int)wantedIngredient];
    }

}
