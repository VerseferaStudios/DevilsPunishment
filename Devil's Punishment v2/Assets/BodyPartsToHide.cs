using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartsToHide : MonoBehaviour
{
	public GameObject[] bodyParts;
	bool enabled = false;
	void awake()
	{
		foreach (GameObject part in bodyParts)
		{
			//part.SetActive(enabled);
			part.transform.GetComponent<SkinnedMeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
		}
	}
    // Start is called before the first frame update	
    void Start()
	{
		awake();
	}

	// Update is called once per frame
	void Update()
	{
		awake();
	}
}
