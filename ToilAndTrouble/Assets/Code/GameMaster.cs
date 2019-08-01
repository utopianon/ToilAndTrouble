using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{
    public int score;
    private Player player;
    public Text scoreText;
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
        if (scoreText == null)
        {
            Debug.Log("got here");
            scoreText = GameObject.FindGameObjectWithTag("Score").GetComponent<Text>();
        }
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
        scoreText.text = score.ToString();
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
