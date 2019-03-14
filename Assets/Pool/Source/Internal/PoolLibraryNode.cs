using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Whisper {

	/// <summary>
	/// Pool Library Node. Stores the pooled GameObject, its name, and each pooled instance.
	/// </summary>
	[System.Serializable]
	public class PoolLibraryNode {

		#region PRIVATE MEMBER VARIABLES
		/// <summary>
		/// The maximum number of instances to be instantiated when the pool is created.
		/// </summary>
		[SerializeField]
		private int m_maximum;

		/// <summary>
		/// The object's name.
		/// </summary>
		[SerializeField]
		private string m_name;

		/// <summary>
		/// The prefab reference. A number of instances based on this prefab will be instantiated when the Object Pool is initialized.
		/// </summary>
		[SerializeField]
		private GameObject m_prefab;

		/// <summary>
		/// A helper GameObject that organizes the pooled game objects.
		/// </summary>
		private GameObject m_hierarchyNode;

		/// <summary>
		/// The pooled instances of each added object.
		/// </summary>
		private List<PooledGameObject> m_instances;
		#endregion

		#region CONSTRUCTOR
		/// <summary>
		/// Initializes a new instance of the <see cref="PoolLibraryNode"/> class. Each instance of this class controls how the Object Pool handles the acquisition, release, instantiation, or destruction of objects.
		/// </summary>
		/// <param name="maximum">Maximum number of objects in the pool.</param>
		/// <param name="name">Name of the prefab.</param>
		/// <param name="prefab">Prefab reference.</param>
		public PoolLibraryNode(int maximum = 0, string name = "", GameObject prefab = null) {

			m_maximum = maximum;
			m_name = name;
			m_prefab = prefab;
		}
		#endregion

		#region PUBLIC MEMBER METHODS
		/// <summary>
		/// Initialize the pooled object instances once setup is complete.
		/// </summary>
		public void Initialize () {

			// Instantiate objects
			m_instances = new List<PooledGameObject>();
			
			for (int i = 0; i < m_maximum; i++) {
				
				GameObject go = (GameObject)GameObject.Instantiate(m_prefab);
				go.name = m_name;
				go.SetActive(false);
				go.transform.parent = m_hierarchyNode.transform;

				m_instances.Add (new PooledGameObject (go));
			}
		}

		/// <summary>
		/// Shuts down this pooled object and destroys all instantiated objects.
		/// </summary>
		public void Shutdown () {

			for (int i = 0; i < m_maximum; i++) {

				GameObject.Destroy(m_instances[i].GameObject);
			}

			m_instances = null;
		}

		/// <summary>
		/// Acquire an instance of the pooled object based on the specified name, then set its position and rotation. If an object cannot be acquired, a new one can be instantiated.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="position">Position.</param>
		/// <param name="rotation">Rotation.</param>
		public GameObject Acquire(Vector3 position = new Vector3(), Quaternion rotation = new Quaternion()) {
			
			GameObject go = null;

			foreach (PooledGameObject o in m_instances) {
				
				if (!o.Acquired) {
					GameObject obj = o.GameObject;
					obj.SetActive(true);
					obj.transform.position = position;
					obj.transform.rotation = rotation;

					foreach (MonoBehaviour c in obj.GetComponentsInChildren<MonoBehaviour> (true)) {
						c.enabled = true;
					}

					obj.SendMessage ("Start", SendMessageOptions.DontRequireReceiver);
					
					o.Acquired = true;
					go = obj;
					break;
				}
			}
			
			return go;
		}

		/// <summary>
		/// Release the object specified in the method arguments. The object will be reset to the pool's default position and rotation.
		/// </summary>
		/// <param name="go">GameObject to release.</param>
		/// <param name="position">Position of the Object Pool.</param>
		/// <param name="rotation">Rotation of the Object Pool.</param>
		public bool Release(GameObject go, Vector3 position, Quaternion rotation) {

			foreach (PooledGameObject o in m_instances) {

				if (o.GameObject == go) {
					go.SetActive (false);
					go.transform.position = position;
					go.transform.rotation = rotation;
					go.transform.parent = m_hierarchyNode.transform;

					foreach (MonoBehaviour c in go.GetComponentsInChildren<MonoBehaviour> (true)) {
						c.enabled = false;
					}

					o.Acquired = false;
					return true;
				}
			}

			return false;
		}
		#endregion

		#region ACCESSORS AND MUTATORS
		/// <summary>
		/// Gets or sets the maximum number of pooled objects.
		/// </summary>
		/// <value>The maximum number of pooled instances of the referenced prefab.</value>
		public int Maximum {
			get { return m_maximum; }
			set { m_maximum = value; }
		}

		/// <summary>
		/// Gets or sets the name of the object.
		/// </summary>
		/// <value>The name of the object.</value>
		public string Name {
			get { return m_name; }
			set { m_name = value; }
		}

		/// <summary>
		/// Gets or sets the prefab reference.
		/// </summary>
		/// <value>The prefab reference.</value>
		public GameObject Prefab {
			get { return m_prefab; }
			set { m_prefab = value; }
		}

		/// <summary>
		/// Gets or sets the hierarchy node.
		/// </summary>
		/// <value>The hierarchy node.</value>
		public GameObject HierarchyNode {
			get { return m_hierarchyNode; }
			set { m_hierarchyNode = value; }
		}
		#endregion
	}
}