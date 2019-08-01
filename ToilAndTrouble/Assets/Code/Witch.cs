using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Witch : MonoBehaviour
{
    public Sprite[] possibleIngredients;
    List<Ingredient> ingredientsInScene;
    int receivedIngredients = 0;
    GameObject wantedSign;
    bool thinking = false;
    public IngredientType wantedIngredient;
    bool receivingInmgredients;
    public float comboTimer = 0.5f;
    public int scorePerIngredient = 20;

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
            Ingredient temp = collidingObject.GetComponent<Ingredient>();
            if (temp.type == wantedIngredient && !temp.grabbed)
            {
                receivedIngredients++;
                ingredientsInScene.Remove(temp);
                Destroy(collidingObject);
                if (!thinking)
                {                    
                    thinking = true;
                    StartCoroutine(NewIngredientTimer());
                }
            }
        }
    }

    public IEnumerator NewIngredientTimer()
    {
        yield return new WaitForSeconds(0.5f);
        UseIngredients();
        ChooseIngredient();
    }

    void UseIngredients()
    {
        float multiplier = 0.5f;

        for (int i = receivedIngredients; i > 0; i--)
        {
            multiplier += 0.5f;           
        }
        GameMaster.Instance.AddScore(multiplier, receivedIngredients * scorePerIngredient);
        Debug.Log("Multiplier is: " + multiplier + " ingredients received is: " + receivedIngredients);
        receivedIngredients = 0;
    }
    public void ChooseIngredient()
    {
        if (ingredientsInScene.Count > 0)
        {
            int index = Random.Range(0, ingredientsInScene.Count);
            wantedIngredient = ingredientsInScene[index].type;
            ingredientsInScene.RemoveAt(index);
            wantedSign.GetComponent<SpriteRenderer>().sprite = possibleIngredients[(int)wantedIngredient];
            thinking = false;
        }
        else
        {
            //won scene
            wantedSign.SetActive(false);
            Debug.Log("won scene");
        }
    }

}
