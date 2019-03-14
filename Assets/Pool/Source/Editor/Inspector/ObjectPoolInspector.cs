using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Whisper;

/// <summary>
/// Object Pool Inspector.
/// </summary>
[CustomEditor(typeof(ObjectPoolManager))]
public class ObjectPoolInspector : Editor {

	/// <summary>
	/// The name of the preset to be generated.
	/// </summary>
	private string m_presetName = "preset";

	/// <summary>
	/// A reference to the Pool Library.
	/// </summary>
	private PoolLibrary m_lib;

	/// <summary>
	/// A reference to the Pool Library's nodes.
	/// </summary>
	private List<PoolLibraryNode> m_nodes;

	/// <summary>
	/// Raises the inspector GUI event. Defines a custom inspector for the Object Pool
	/// </summary>
	public override void OnInspectorGUI() {
	
		// Cache useful references
		ObjectPoolManager pool = (ObjectPoolManager)target;
		m_lib = pool.PoolLibrary;
		m_nodes = m_lib.Nodes;

		// Begin preliminary GUI work
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		EditorGUILayout.BeginHorizontal (EditorStyles.objectFieldThumb);
		EditorGUILayout.BeginVertical ();
		EditorGUILayout.LabelField("Default Settings", EditorStyles.boldLabel);
		EditorGUILayout.Space ();

		// Define toggles
		EditorGUILayout.HelpBox("Enabling \"Use Debug\" will allow the Object Pool to produce debug messages in the console. These areas of code are removed through the use of preprocessor directives when the game is compiled.", MessageType.None);
		pool.UseDebug = EditorGUILayout.Toggle ("Use Debug", pool.UseDebug);
		EditorGUILayout.Space ();
		
		EditorGUILayout.HelpBox("Enabling \"Persistent\" will allow the Object Pool to persist through scene load.", MessageType.None);
		pool.Persistent = EditorGUILayout.Toggle ("Persistent", pool.Persistent);
		EditorGUILayout.Space ();

		EditorGUILayout.HelpBox("Enabling \"Enforce Pooling\" will ensure that the Object Pool never instantiates or destroys objects when an object cannot normally be acquired. If disabled, the pool will create (and destroy) instances of pooled objects when they cannot normally be acquired.", MessageType.None);
		pool.EnforcePooling = EditorGUILayout.Toggle ("Enforce Pooling", pool.EnforcePooling);
		EditorGUILayout.Space ();

		EditorGUILayout.HelpBox("Enabling \"Allow Instantiation\" will allow the Object Pool to instantiate non-pooled objects through calls to Instantiate. The name argument is treated as a path in the resource folder.", MessageType.None);
		pool.AllowInstantiation = EditorGUILayout.Toggle ("Allow Instantiation", pool.AllowInstantiation);
		EditorGUILayout.Space ();

		EditorGUILayout.HelpBox("Enabling \"Detatch in Hierarchy\" will cause the Object Pool to move acquired objects to the root of the scene hierarchy. Useful for organization and debugging.", MessageType.None);
		pool.Detatch = EditorGUILayout.Toggle ("Detatch in Hierarchy", pool.Detatch);
		EditorGUILayout.Space ();

		EditorGUILayout.EndVertical ();
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		EditorGUILayout.BeginHorizontal (EditorStyles.objectFieldThumb);
		EditorGUILayout.BeginVertical ();
		EditorGUILayout.LabelField("Pool Presets", EditorStyles.boldLabel);
		EditorGUILayout.Space ();

		EditorGUILayout.HelpBox("Enabling \"Use Presets\" will cause the Object Pool to load its pool list from a Scriptable Object in the Assets folder.", MessageType.None);
		pool.UsePreset = EditorGUILayout.Toggle ("Use Presets", pool.UsePreset);
		EditorGUILayout.Space ();

		// Define Pool Library structure
		if (pool.UsePreset) {

			EditorGUILayout.Space ();
	
			EditorGUILayout.HelpBox("The buttons below will allow you to add references to pool presets in the Asset library.", MessageType.None);
			EditorGUILayout.BeginHorizontal ();
			
			if (GUILayout.Button("Add Preset")) {
				pool.Presets.Add(null);
			}
			
			if (GUILayout.Button("Remove Preset")) {
				
				if (pool.Presets.Count > 0) {
					pool.Presets.RemoveAt(pool.Presets.Count - 1);
				}
				
				else {
					Debug.LogWarning ("There aren't any presets to remove!");
				}
			}
	
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			for (int i = 0; i < pool.Presets.Count; i++) {
				pool.Presets[i] = (ObjectPoolPreset)EditorGUILayout.ObjectField ("Preset", pool.Presets[i], typeof(ScriptableObject), true);
			}
			EditorGUILayout.Space ();
		}

		EditorGUILayout.EndVertical ();
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		EditorGUILayout.BeginHorizontal (EditorStyles.objectFieldThumb);
		EditorGUILayout.BeginVertical ();
		EditorGUILayout.LabelField("Pool Library", EditorStyles.boldLabel);
		EditorGUILayout.Space ();

		EditorGUILayout.HelpBox("Objects can be added or removed from the Pool Library below. Simply specify a number of objects to create at the start of the scene (maximum can be changed in the inspector code), set a unique name for the object, then link its prefab. The pool handles the rest.", MessageType.None);
		EditorGUILayout.BeginHorizontal ();
		
		if (GUILayout.Button("Add Object")) {
			m_nodes.Add(new PoolLibraryNode());
		}
		
		if (GUILayout.Button("Remove Object")) {
			
			if (m_nodes.Count > 0) {
				m_nodes.RemoveAt(m_nodes.Count - 1);
			}
			
			else {
				Debug.LogWarning ("There aren't any nodes to remove!");
			}
		}
		EditorGUILayout.EndHorizontal ();
		
		for (int i = 0; i < m_nodes.Count; i++) {
			
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			PoolLibraryNode node = m_nodes[i];
			node.Maximum = EditorGUILayout.IntSlider("Amount to Pool", node.Maximum, 0, 100);
			node.Name = EditorGUILayout.TextField("Object Name", node.Name);
			node.Prefab = (GameObject)EditorGUILayout.ObjectField ("Prefab", node.Prefab, typeof(GameObject), true);
		}

		if (m_nodes.Count > 0) {

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			GUI.backgroundColor = Color.cyan;
			EditorGUILayout.BeginHorizontal (EditorStyles.objectFieldThumb);
			EditorGUILayout.BeginVertical ();
			EditorGUILayout.LabelField("Preset Generation", EditorStyles.boldLabel);
			EditorGUILayout.Space ();

			EditorGUILayout.HelpBox("Clicking \"Create Preset\" will create a (Whisper) Object Pool preset that can be used in other pools. The user can specify a name for the preset.", MessageType.None);
			EditorGUILayout.BeginHorizontal ();
			m_presetName = EditorGUILayout.TextField ("Preset Name", m_presetName);
			EditorGUILayout.EndHorizontal ();
			
			EditorGUILayout.BeginHorizontal ();
			if (GUILayout.Button("Create Preset", GUILayout.Width (120))) {
				
				ObjectPoolPreset preset = ScriptableObject.CreateInstance<ObjectPoolPreset> ();

				preset.Library = m_lib;
				preset.Library.Nodes = m_nodes;

				string path = "Assets/";
				AssetDatabase.CreateAsset (preset, path + m_presetName + ".asset");
				AssetDatabase.SaveAssets();
				Debug.Log ("Object Pool Preset instantiated!");
			}
			
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.Space ();
			EditorGUILayout.EndVertical ();
			EditorGUILayout.EndHorizontal ();
		}
	
		EditorGUILayout.Space ();
		EditorGUILayout.EndVertical ();
		EditorGUILayout.EndHorizontal ();
	}
}
