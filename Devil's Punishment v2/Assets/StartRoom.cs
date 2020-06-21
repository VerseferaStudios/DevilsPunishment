using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartRoom : MonoBehaviour
{
    public Transform spawnPos;

   

    void Start()
    {
        GameState.gameState.setSpawnPos(spawnPos);
    }
}
