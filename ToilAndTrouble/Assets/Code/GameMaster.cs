using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    private void Update()
    {
            if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
