using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartRoom : MonoBehaviour
{
    public Transform spawnPos;

    void Awake()
    {
        GameState.gameState.setSpawnPos(spawnPos);
    }
}
