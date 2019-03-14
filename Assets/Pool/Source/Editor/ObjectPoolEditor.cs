using UnityEngine;
using UnityEditor;
using System.Collections;

public class ObjectPoolEditor : EditorWindow {
	
	private static string m_filePath = "Whisper/";
	
	//[MenuItem ("GameObject/Whisper/Create Object Pool", false, 70)]
	//static void Init () {
	//	CreateObjectPool ();
	//}
	
	[MenuItem ("GameObject/Whisper/Create Object Pool", false, 70)]
	static void CreateObjectPool() {
		var objectPool = Resources.Load(m_filePath + "(Whisper)ObjectPoolManager", typeof(GameObject));
		if (objectPool == null) {
			Debug.LogError("Could not find Object Pool prefab. Please drag it into the scene yourself. It is located under Resources/Whisper.");
			return;
		}
		
		GameObject go = Instantiate (objectPool) as GameObject;
		Undo.RegisterCreatedObjectUndo(go, "Added Object Pool");
		go.name = "(Whisper)ObjectPoolManager";
		
		Debug.Log ("Object Pool instantiated!");
	}

	[MenuItem ("Window/Whisper/Create Object Pool Preset", false, 170)]
	static void CreateObjectPoolPreset() {
		ObjectPoolPreset preset = ScriptableObject.CreateInstance<ObjectPoolPreset> ();
		preset.Library = new Whisper.PoolLibrary ();

		AssetDatabase.CreateAsset (preset, "Assets/whisper_pool_preset.asset");
		AssetDatabase.SaveAssets();
		Debug.Log ("Object Pool Preset instantiated!");
	}
}
