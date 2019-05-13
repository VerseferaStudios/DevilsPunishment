using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manager for all of the Resources in the game
/// </summary>
public class ResourceManager : MonoBehaviour
{
	/// <summary>
	/// Singleton instance of the resource manager
	/// </summary>
	public static ResourceManager instance;

	[Header("Prefabs")]
	[SerializeField]
	[Tooltip("Collection of Prefabs")]
	private GameObject[] gameObjects;

	private Dictionary<string, GameObject> gameObjectDictionary;

	private void Awake()
	{
		//Maintain singleton instance
		if (instance == null)
			instance = this;
		//Ensure there is only one instance of the SoundManager
		else if (instance != this)
			Destroy(gameObject);
		//Persist between scenes
		DontDestroyOnLoad(gameObject);
		gameObjectDictionary = new Dictionary<string, GameObject>(gameObjects.Length);
		foreach (GameObject prefab in gameObjects)
		{
			gameObjectDictionary.Add(prefab.name, prefab);
			Debug.Log(prefab.name);
			// Do Something to the prefab?
		}
	}


	// For the functions below, can C# generate them with a "macro" based on the "keys" in the "gameObjectDictionary"? Creating (and maintaining) these getters seems like busy work...
	public GameObject getResource(string ResourceName)
	{		
		return gameObjectDictionary[ResourceName];
	}
}
