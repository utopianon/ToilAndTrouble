using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    public int score;
    private Player player;

    private static GameMaster _instance;
    public static GameMaster Instance
    {
        get
        {
            return _instance;
        }
    }

    void Awake()
    {

        if (GameObject.FindGameObjectsWithTag("GameMaster").Length > 1)
        {
            Destroy(this.gameObject);
        }

        _instance = this;
        DontDestroyOnLoad(this.gameObject);
        score = 0;
    }

    private void Update()
    {
            if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

            if (Input.GetMouseButtonDown(1))
        {
            if (player.heldItems >0)
            {
                player.activeAttachPoint.attachedItems[0].Drop();
            }
        }
    }

    public void SetPlayer(Player _player)
    {
        player = _player;
    }

    public void AddScore(float multiplier, int value)
    {
        int points = Mathf.CeilToInt(value * multiplier);
        score += points;
        Debug.Log("Score is: " + score);
    }

    private void Reset()
    {
        
    }
}
