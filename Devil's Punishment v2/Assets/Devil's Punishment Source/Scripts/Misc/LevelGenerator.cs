using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour {

	public Texture2D map;
	private bool[,] mapArray;
	public float offsetMultiplier = 2.0f;
	public Vector3 offset = Vector3.zero;

    private CorridorGenerator corridorGenerator;

	[System.Serializable]
	public class TileData {

		public GameObject prefab;
		public bool above;
		public bool below;
		public bool left;
		public bool right;

		public float yRotation;

		public bool CheckSurroundings(bool a, bool b, bool l, bool r) {
			return above == a && below == b && left == l && right == r;
		}

	}

	public List<TileData> tileData;

	public void GenerateLevel() {

		Debug.Log("Generated");

        //Image Generation

        corridorGenerator = GetComponent<CorridorGenerator>();

        corridorGenerator.GenerateMap();

        map = corridorGenerator.mapTexture;

        //Generation

        foreach (TileData t in tileData) {
			Debug.Log(t.prefab.name + "a/b/l/r" + t.above + t.below + t.left + t.right);
		}

		DestroyLevel (false);

		GenerateBoolMap();



		foreach (TileData t in tileData) {
			GameObject empty = new GameObject ("[" + t.prefab.name.ToUpper () + "]");
			empty.transform.localPosition = Vector3.zero;
			empty.transform.localRotation = Quaternion.identity;
			empty.transform.SetParent (transform);
			empty.isStatic = t.prefab.isStatic;

		}

		for (int x = 1; x < map.width-1; x++) {
			for (int y = 1; y < map.height-1; y++) {
				GenerateTile (x, y);
			}

		}

		DestroyLevel (true);
	}

	private void GenerateBoolMap() {

		mapArray = new bool[map.width, map.height];

		for(int x = 0; x < map.width; x++) {
			for(int y = 0; y < map.height; y++) {

				mapArray[x,y] = !(map.GetPixel(x,y).r>0);
			}
		}
	}

	public void DestroyLevel(bool onlyClean) {

		GameObject[] children = new GameObject[transform.childCount];

		for (int i = 0; i < transform.childCount; i++) {
			GameObject g = transform.GetChild (i).gameObject;
			children [i] = g;
		}

		foreach (GameObject child in children) {
			if (!onlyClean || onlyClean && child.transform.childCount <= 0) {
				GameObject.DestroyImmediate (child);
			}

		}
	}

	void GenerateTile(int x, int y) {

		bool center = mapArray[x,y];

		if (!center) {
			return;
		}

		bool above = mapArray[x, y+1];
		bool below = mapArray[x, y-1];
		bool left = mapArray[x-1, y];
		bool right = mapArray[x+1, y];

		int i = 0;

		foreach(TileData t in tileData) {

			if (t.CheckSurroundings(above,below,left,right)) {

				Debug.Log("fired!");

				Vector3 position = offset + t.prefab.transform.position + new Vector3 (x * offsetMultiplier, 0.0f, y * offsetMultiplier);

				GameObject g = Instantiate (t.prefab, position, Quaternion.Euler(new Vector3(0, t.yRotation, 0)));
				g.name = t.prefab.name.ToLower () + " ( " + x + " , " + y + " )";
				g.transform.SetParent (transform.GetChild(i));
			}

			i++;

		}

	}


}
