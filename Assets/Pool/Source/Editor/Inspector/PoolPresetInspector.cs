using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Whisper;

/// <summary>
/// Object Pool Inspector.
/// </summary>
[CustomEditor(typeof(ObjectPoolPreset))]
public class PoolPresetInspector : Editor {
	
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
		ObjectPoolPreset preset = (ObjectPoolPreset)target;
		m_lib = preset.Library;
		m_nodes = m_lib.Nodes;
		
		// Begin preliminary GUI work
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		
		EditorGUILayout.HelpBox("Objects can be added or removed from the Pool Library below. Simply specify a number of objects to create at the start of the scene (maximum can be changed in the inspector code), set a unique name for the object, then link its prefab. The pool handles the rest.", MessageType.Info);
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
		
		if (m_nodes == null) {
			m_nodes = new List<PoolLibraryNode> ();
		}

		for (int i = 0; i < m_nodes.Count; i++) {
			
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			PoolLibraryNode node = m_nodes[i];
			node.Maximum = EditorGUILayout.IntSlider("Amount to Pool", node.Maximum, 0, 100);
			node.Name = EditorGUILayout.TextField("Object Name", node.Name);
			node.Prefab = (GameObject)EditorGUILayout.ObjectField ("Prefab", node.Prefab, typeof(GameObject), true);
		}
		
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
	}
}
