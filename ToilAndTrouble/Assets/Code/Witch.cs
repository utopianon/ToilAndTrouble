using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Witch : MonoBehaviour
{
    public Sprite[] possibleIngredients;
    List<Ingredient> ingredientsInScene;
    GameObject wantedSign;
    public IngredientType wantedIngredient;

    float timer;


    public void Start()
    {
        wantedSign = transform.GetChild(0).gameObject;
        LoadIngrediensInScene();
        ChooseIngredient();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            ChooseIngredient();
        }
    }

    private void LoadIngrediensInScene()
    {
        GameObject holder = GameObject.FindGameObjectWithTag("IngredientHolder");
        Ingredient[] ingredients = holder.GetComponentsInChildren<Ingredient>();
        ingredientsInScene = new List<Ingredient>();
        foreach (Ingredient i in ingredients)
        {
            ingredientsInScene.Add(i);
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
        if (ingredientsInScene.Count > 0)
        {
            int index = Random.Range(0, ingredientsInScene.Count);
            wantedIngredient = ingredientsInScene[index].type;
            ingredientsInScene.RemoveAt(index);
            wantedSign.GetComponent<SpriteRenderer>().sprite = possibleIngredients[(int)wantedIngredient];
        }
        else
        {
            //won scene
            wantedSign.SetActive(false);
            Debug.Log("won scene");
        }
    }

}
