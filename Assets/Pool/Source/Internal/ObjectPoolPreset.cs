using UnityEngine;
using System.Collections;
using Whisper;

/// <summary>
/// Object Pool Preset. A container class that stores an Object Pool's library.
/// </summary>
public class ObjectPoolPreset : ScriptableObject {

	/// <summary>
	/// The pool library stored within the preset.
	/// </summary>
	[SerializeField]
	private PoolLibrary m_library;

	/// <summary>
	/// Gets or sets the pool library.
	/// </summary>
	/// <value>The library.</value>
	public PoolLibrary Library {
		get { return m_library; }
		set { m_library = value; }
	}
}
