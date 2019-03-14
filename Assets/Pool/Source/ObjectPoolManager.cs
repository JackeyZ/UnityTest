using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Whisper;

/// <summary>
/// Object Pool Manager. Inherits from MonoBehaviour, controls all Object Pool behaviour in a scene.
/// </summary>
public class ObjectPoolManager : MonoBehaviour {

	#region PRIVATE MEMBER VARIABLES
	/// <summary>
	/// Determines whether or not the Object Pool will persist between scenes.
	/// 确定对象池是否在场景之间持久。
	/// </summary>
	[SerializeField]
	private bool m_persistent;

	/// <summary>
	/// Determines whether or not debug messages should be output in the editor.
	/// 确定是否应该在编辑器中输出调试消息。
	/// </summary>
	[SerializeField]
	private bool m_debug;

	/// <summary>
	/// Determines whether or not the Object Pool will use a preset pool asset or use the inspector.
	/// 确定对象池将使用预置池资产还是使用检查器。
	/// </summary>
	[SerializeField]
	private bool m_usePreset;

	/// <summary>
	/// Determines whether or not the Object Pool will instantiate new instances of objecs when they cannot be acquired.
	/// 确定当无法获取对象的新实例时，对象池是否将实例化它们。
	/// </summary>
	[SerializeField]
	private bool m_enforcePooling;
	
	/// <summary>
	/// Determines if the Object Pool will instantiate and destroy non-pooled objects.
	/// 确定对象池是否实例化和销毁非池化对象。
	/// </summary>
	[SerializeField]
	private bool m_allowInstantiation;

	/// <summary>
	/// Determines if the pooled objects are detached from the pool when they are acquired.
	/// 确定池对象在被获取时是否与池分离。
	/// </summary>
	[SerializeField]
	private bool m_detach;

	/// <summary>
	/// A reference to the Object Pool's presets, if used. Allows for multiple pools to share the same list without spending time rebuilding each list.
	/// 引用对象池的预置(如果使用)。允许多个池共享同一个列表，而不需要花费时间重新构建每个列表。
	/// </summary>
	[SerializeField]
	private List<ObjectPoolPreset> m_presets;

	/// <summary>
	/// The Pool Library. Holds a reference to each original GameObject and controls whether or not said object is eligible to be accessed at runtime. Sorted on initialization.
	/// 池的图书馆。保存对每个原始GameObject的引用，并控制所述对象是否有资格在运行时被访问。在初始化排序。
	/// </summary>
	[SerializeField]
	private PoolLibrary m_poolLibrary;

	#region UTILITY
	/// <summary>
	/// A list that holds all GameObjects that are to be released by a timer.
	/// 包含所有游戏对象的列表，这些对象将由计时器释放。
	/// </summary>
	private List<DelayedReleaseObject> m_timedReleaseObjects;
	#endregion
	#endregion

	#region PROTECTED MEMBER METHODS
	/// <summary>
	/// Start this instance, and initialize the internal data structures (Pool Library, object lists) within the pool.
	/// </summary>
	void Start () {

		if (m_persistent) {
			DontDestroyOnLoad (this.gameObject);
		}

		// Error handling. If the pool initializes and the Pool Library is null, notify the user.
		if (m_poolLibrary == null) {
			WhisperPoolException exception = new WhisperPoolException("Pool Library is null, something failed to initialize!");
			Debug.LogError(exception.Message);
			throw exception;
		}

		// Setup helper lists
		m_timedReleaseObjects = new List<DelayedReleaseObject> ();

		// Load preset if it is not null
		if (m_usePreset) {
			if (m_presets != null) {
				for (int i = 0; i < m_presets.Count; i++) {
					if (m_presets[i] != null) {
						m_poolLibrary.Nodes.AddRange(m_presets[i].Library.Nodes);
					}
				}
			}
		}

		// Sort the Pool Library
		m_poolLibrary.Pool = this.gameObject;
		m_poolLibrary.Sort ();
		m_poolLibrary.Initialize ();
	}

	/// <summary>
	/// Overrides the default MonoBehaviour OnDisable event, allows the pool to cleanup when disabled.
	/// </summary>
	void OnDisable () {
		m_poolLibrary.Shutdown ();
	}

	/// <summary>
	/// Overrides the Late Update event to manage releasing GameObjects with a delay.
	/// </summary>
	void LateUpdate () {

		for (int i = m_timedReleaseObjects.Count - 1; i >= 0; i--) {

			DelayedReleaseObject obj = m_timedReleaseObjects[i];
			obj.CurrentTime += Time.deltaTime;

			if (obj.CurrentTime >= obj.Delay) {
				Release (obj.GameObject);
				m_timedReleaseObjects.RemoveAt(i);
			}
		}
	}
	#endregion

	#region PUBLIC MEMBER METHODS
	/// <summary>
	/// Acquire an instance of the pooled object based on the specified name. If an object cannot be acquired, a new one can be instantiated. Note that while using the Acquire method to load objects from resources is still available, this feature is no longer reccomended. An instantiation method has been added to support this behavior.
	/// </summary>
	/// <param name="name">Name.</param>
	public GameObject Acquire (string name) {

		GameObject go = m_poolLibrary[name].Acquire (this.transform.position, this.transform.rotation);
		
		if (go == null) {
			
			if (!m_enforcePooling) {
				
				// Attempt to instantiate a new instance of an otherwise pooled object.
				if (m_poolLibrary[name] != null) {
					go = (GameObject)GameObject.Instantiate(m_poolLibrary[name].Prefab, this.transform.position, this.transform.rotation);
				}
			}

			#if UNITY_EDITOR
			else {
				if (m_debug) {
					Debug.Log ("Attempting to overdraw from the pool, but pooling is enforced! A null object will be returned.");
				}
			}
			#endif
		}

		#if UNITY_EDITOR
		else {
			if (m_debug) {
				Debug.Log ("Acquired an object from the pool.");
			}
		}
		#endif
		
		// Attempt to instantiate an object from the Resources folder -- assume "name in parameters" refers to an asset path. These can be removed using the pool.
		if (go == null && m_allowInstantiation) {
			go = (GameObject)GameObject.Instantiate(Resources.Load(name), this.transform.position, this.transform.rotation);
		}
		
		// Should this object be detached from the Pool's tree?
		if (go != null && m_detach) {
			go.transform.parent = null;
		}
		
		// Throw an exception if there is an internal object acquisition error.
		if (go == null && !m_enforcePooling) {
			WhisperPoolException exception = new WhisperPoolException("Object could not be acquired or instantiated!");
			Debug.LogError(exception.Message);
			throw exception;	
		}
		
		return go;
	}

	/// <summary>
	/// Acquire an instance of the pooled object based on the specified name, then set its position and rotation. If an object cannot be acquired, a new one can be instantiated. Note that while using the Acquire method to load objects from resources is still available, this feature is no longer reccomended. An instantiation method has been added to support this behavior.
	/// </summary>
	/// <param name="name">Name.</param>
	/// <param name="position">Position.</param>
	/// <param name="rotation">Rotation.</param>
	public GameObject Acquire (string name, Vector3 position, Quaternion rotation) {

		GameObject go = m_poolLibrary[name].Acquire (position, rotation);
		
		if (go == null) {

			if (!m_enforcePooling) {

				// Attempt to instantiate a new instance of an otherwise pooled object.
				if (m_poolLibrary[name] != null) {
					go = (GameObject)GameObject.Instantiate(m_poolLibrary[name].Prefab, position, rotation);
					go.name = name;

					#if UNITY_EDITOR
					if (m_debug) {
						Debug.Log ("Overdrawing from the pool--instantiating a new instance of " + name);
					}
					#endif
				}
			}

			#if UNITY_EDITOR
			else {
				if (m_debug) {
					Debug.Log ("Attempting to overdraw from the pool, but pooling is enforced! A null object will be returned.");
				}
			}
			#endif
		}

		#if UNITY_EDITOR
		else {
			if (m_debug) {
				Debug.Log ("Acquired an object from the pool.");
			}
		}
		#endif
		
		// Attempt to instantiate an object from the Resources folder -- assume "name in parameters" refers to an asset path. These can be removed using the pool.
		if (go == null && m_allowInstantiation) {
			go = (GameObject)GameObject.Instantiate(Resources.Load(name), position, rotation);
		}

		if (go == null && !m_allowInstantiation) {
			#if UNITY_EDITOR
			if (m_debug) {
				Debug.Log ("Instantiation after start up is not allowed! A null object will be returned.");
			}
			#endif
		}

		// Should this object be detached from the Pool's tree?
		if (go != null && m_detach) {
			go.transform.parent = null;
		}

		// Throw an exception if there is an internal object acquisition error.
		if (go == null && !m_enforcePooling) {
			WhisperPoolException exception = new WhisperPoolException ("Object could not be acquired or instantiated!");
			Debug.LogError(exception.Message);
			throw exception;	
		}

		return go;
	}

	/// <summary>
	/// Release the specified object. If it was acquired from the pool, it is released. If it was instantiated, it will be destroyed. Otherwise, the object will either be left alone or destroyed.
	/// </summary>
	/// <param name="obj">Object to release or destroy.</param>
	public void Release (GameObject obj) {

		if (m_poolLibrary[obj.name] != null) {

			bool released = m_poolLibrary[obj.name].Release (obj, this.transform.position, this.transform.rotation);

			if (!released) {

				if (m_enforcePooling) {
					WhisperPoolException exception = new WhisperPoolException ("Failed to release object. Object not destroyed as pooling is enforced. A pool overdraw has occured. Ensure that no new objects were instantiated by the pool if \"Enforce Pooling\" was at one point disabled during runtime.");
					Debug.LogError(exception.Message);
					throw exception;	
				}

				else {

					#if UNITY_EDITOR
					if (m_debug) {
						Debug.Log ("Object instantiated after start up by the pool, destroying it instead.");
					}
					#endif

					Destroy (obj);
				}
			}

			#if UNITY_EDITOR
			else {
				if (m_debug) {
					Debug.Log ("Released an object back into the pool.");
				}
			}
			#endif
		}

		else {

			if (m_allowInstantiation) {

				#if UNITY_EDITOR
				if (m_debug) {
					Debug.Log ("Object not in pool passed to the release method--destroying object.");
				}
				#endif

				Destroy (obj);
			}

			#if UNITY_EDITOR
			else {
				if (m_debug) {
					Debug.Log ("Object not in pool passed to the release method, but external object removal has not been enabled in editor. To destroy external objects, enable \"Allow Instantiations\" in the editor.");
				}
			}
			#endif
		}
	}

	/// <summary>
	/// Release the specified object. If it was acquired from the pool, it is released. If it was instantiated, it will be destroyed. Otherwise, the object will either be left alone or destroyed.
	/// </summary>
	/// <param name="obj">Object to release or destroy.</param>
	/// <param name="time">Delay.</param>
	public void Release(GameObject obj, float time) {
		m_timedReleaseObjects.Add (new DelayedReleaseObject (obj, time));
	}

	/// <summary>
	/// Add a new object to the pool at runtime. This process will add an object to the pool, sort the Pool Library, then instantiate pooled instances. As such, this process will cause a performance hitch, so it should be used sparingly (for instance, during a load screen).
	/// </summary>
	/// <param name="prefab">Prefab reference.</param>
	/// <param name="name">Name of the prefab.</param>
	/// <param name="amount">Amount to initialize.</param>
	public void Add (GameObject prefab, string name, int amount) {

		// Add a new object to the Pool Library
		m_poolLibrary.Add (prefab, name, amount);

		// Sort the Pool Library
		m_poolLibrary.Sort ();
	}

	/// <summary>
	/// Removes and object from the Object Pool based on its name. Since the Pool Library is sorted, the process is much faster than adding a new object at runtime (although it is still a slow operation).
	/// </summary>
	/// <param name="name">Name.</param>
	public void Remove (string name) {

		// Find the node
		PoolLibraryNode node = m_poolLibrary [name];

		// Remove it from the Pool Library (shutdown handled in the Pool Library itself)
		m_poolLibrary.Remove (node);
	}
	#endregion

	#region PUBLIC UNITY ENGINE WRAPPER FUNCTIONS
	/// <summary>
	/// Instantiate the specified prefab at the origin.
	/// </summary>
	/// <param name="obj">Prefab.</param>
	public GameObject Instantiate (GameObject obj) {
		return (GameObject)GameObject.Instantiate (obj);
	}
	
	/// <summary>
	/// Instantiate the specified prefab at the supplied position and rotation.
	/// </summary>
	/// <param name="obj">Prefab.</param>
	/// <param name="position">Position.</param>
	/// <param name="rotation">Rotation.</param>
	public GameObject Instantiate (GameObject obj, Vector3 position, Quaternion rotation) {
		return (GameObject)GameObject.Instantiate (obj, position, rotation);
	}
	
	/// <summary>
	/// Instantiate a prefab specified by the resources path.
	/// </summary>
	/// <param name="path">Resources path.</param>
	public GameObject Instantiate (string path) {
		return (GameObject)GameObject.Instantiate (Resources.Load (path));
	}
	
	/// <summary>
	/// Instantiate a prefab specified by the resources path at the supplied position and rotation.
	/// </summary>
	/// <param name="path">Resources path.</param>
	/// <param name="position">Position.</param>
	/// <param name="rotation">Rotation.</param>
	public GameObject Instantiate (string path, Vector3 position, Quaternion rotation) {
		return (GameObject)GameObject.Instantiate (Resources.Load (path), position, rotation);
	}
	
	/// <summary>
	/// Destroy the specified GameObject.
	/// </summary>
	/// <param name="obj">Object.</param>
	public void Destroy (GameObject obj) {
		GameObject.Destroy (obj);
	}
	
	/// <summary>
	/// Destroy the specified GameObject after a specified time has past.
	/// </summary>
	/// <param name="obj">Object.</param>
	/// <param name="time">Time.</param>
	public void Destroy (GameObject obj, float time) {
		GameObject.Destroy (obj, time);
	}
	#endregion

	#region ACCESSORS AND MUTATORS
	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="ObjectPoolManager"/> is persistent through scene changes.
	/// </summary>
	/// <value><c>true</c> if persistent; otherwise, <c>false</c>.</value>
	public bool Persistent {
		get { return m_persistent; }
		set { m_persistent = value; }
	}

	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="ObjectPoolManager"/> outputs debug messages.
	/// </summary>
	/// <value><c>true</c> if use debug; otherwise, <c>false</c>.</value>
	public bool UseDebug {
		get { return m_debug; }
		set { m_debug = value; }
	}

	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="ObjectPoolManager"/> enforces pooling.
	/// </summary>
	/// <value><c>true</c> if pooling is enforced; otherwise, <c>false</c>.</value>
	public bool EnforcePooling {
		get { return m_enforcePooling; }
		set { m_enforcePooling = value; }
	}

	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="ObjectPoolManager"/> allows object instantiation.
	/// </summary>
	/// <value><c>true</c> if instantiation is allowed; otherwise, <c>false</c>.</value>
	public bool AllowInstantiation {
		get { return m_allowInstantiation; }
		set { m_allowInstantiation = value; }
	}

	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="ObjectPoolManager"/> detatches acquired objects in the hierarchy.
	/// </summary>
	/// <value><c>true</c> if objects should detatch; otherwise, <c>false</c>.</value>
	public bool Detatch {
		get { return m_detach; }
		set { m_detach = value; }
	}

	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="ObjectPoolManager"/> uses a preset pool list.
	/// </summary>
	/// <value><c>true</c> if the pool uses a preset list; otherwise, <c>false</c>.</value>
	public bool UsePreset {
		get { return m_usePreset; }
		set { m_usePreset = value; }
	}

	/// <summary>
	/// Gets or sets the preset list (optional).
	/// </summary>
	/// <value>The preset list.</value>
	public List<ObjectPoolPreset> Presets {
		get { return m_presets; }
		set { m_presets = value; }
	}

	/// <summary>
	/// An accessor to the Pool Library. Used only by the Unity Inspector.
	/// </summary>
	/// <value>The pool library.</value>
	public PoolLibrary PoolLibrary {
		get { return m_poolLibrary; }
	}
	#endregion

	#region SINGLETON
	/// <summary>
	/// The singleton instance of the object pool. Since static objects are not serialized by the Unity Editor, a singleton is the next best design to use.
	/// </summary>
	private static ObjectPoolManager m_singleton;

	/// <summary>
	/// A read only accessor to the Object Pool Manager singleton. Only one pool should exist per scene (and if multiple scenes are used, then only one pool per group).
	/// </summary>
	/// <value>The Object Pool Manager singleton.</value>
	public static ObjectPoolManager Instance {

		get {

			if (m_singleton == null) {

				m_singleton = GameObject.Find("(Whisper)ObjectPoolManager").GetComponent<ObjectPoolManager>();
		
				if (m_singleton == null) {
					WhisperPoolException exception = new WhisperPoolException("No Object Pool object found in scene, but another behaviour still attempted to access it!");
					Debug.LogError(exception.Message);
					throw exception;
				}
			}

			return m_singleton;
		}
	}
	#endregion
}