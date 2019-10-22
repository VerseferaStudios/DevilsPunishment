//Author: David Bird
    //Date: Monday 10/21/19



using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentRandomPopulation : MonoBehaviour
{
    public GameObject[] set1;
    public GameObject[] set2;
    public GameObject[] set3;
    public GameObject[] set4;
    float timer = 2;

    // Start is called before the first frame update
    void Awake()
    {
        set1[Random.Range(0, set1.Length)].SetActive(true);
        set2[Random.Range(0, set2.Length)].SetActive(true);
        set3[Random.Range(0, set3.Length)].SetActive(true);
        set4[Random.Range(0, set4.Length)].SetActive(true);
    }
}