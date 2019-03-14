using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Whisper {

	/// <summary>
	/// Pool Library. A data structure that holds all of the objects managed by the (Whisper) Object Pool
	/// </summary>
	[System.Serializable]
	public class PoolLibrary {
		
		#region PRIVATE MEMBER VARIABLES
		/// <summary>
		/// A reference to the Object Pool's game object.
		/// </summary>
		private GameObject m_pool;

		/// <summary>
		/// The master list of nodes in the Pool Library. Each node is a data structure holding its own array of pooled GameObjects.
		/// </summary>
		[SerializeField]
		private List<PoolLibraryNode> m_nodes;
		#endregion

		#region CONSTRUCTOR
		/// <summary>
		/// Initializes a new instance of the <see cref="PoolLibrary"/> class.
		/// </summary>
		public PoolLibrary () {

			m_nodes = new List<PoolLibraryNode> ();
		}
		#endregion

		#region PUBLIC MEMBER METHODS
		/// <summary>
		/// Initializes the Pool Library and instantiates the objects contained in each of its nodes.
		/// </summary>
		public void Initialize () {
		
			foreach (PoolLibraryNode node in m_nodes) {

				GameObject go = new GameObject();
				go.name = node.Name;
				go.transform.parent = m_pool.transform;

				node.HierarchyNode = go;
				node.Initialize ();
			}
		}

		/// <summary>
		/// Shuts down the Pool Library by individually shutting down each of its nodes.
		/// </summary>
		public void Shutdown () {

			foreach (PoolLibraryNode node in m_nodes) {

				node.Shutdown ();
				GameObject.Destroy(node.HierarchyNode);
			}
		}

		/// <summary>
		/// Add a new pooled prefab at runtime, specifiying its name and amount in the method arguments.
		/// </summary>
		/// <param name="prefab">Prefab to pool.</param>
		/// <param name="name">Name of the pooled object.</param>
		/// <param name="amount">Amount to hold.</param>
		public void Add (GameObject prefab = null, string name = "", int amount = 0) {
			PoolLibraryNode node = new PoolLibraryNode (amount, name, prefab);
			m_nodes.Add (node);
			node.Initialize();
		}

		/// <summary>
		/// Shuts down and removes the specified node.
		/// </summary>
		/// <param name="node">Node to remove.</param>
		public void Remove (PoolLibraryNode node) {
			node.Shutdown ();
			m_nodes.Remove (node);
		}

		/// <summary>
		/// Sort the list of nodes based on the name value stored in each node.
		/// </summary>
		public void Sort () {

			// Uses a delegate to define a sorting method
			m_nodes.Sort (delegate (PoolLibraryNode a, PoolLibraryNode b) {
				return a.Name.CompareTo (b.Name);
			});
		}

		/// <summary>
		/// Finds the pooled object based on the string taken as a parameter.
		/// </summary>
		/// <param name="name">Name of the pooled object.</param>
		public PoolLibraryNode Find (string name) {

			PoolLibraryNode node = null;

			foreach (PoolLibraryNode n in m_nodes) {

				if (n.Name == name) {
					node = n;
					break;
				}
			}

			return node;
		}
		#endregion

		#region OPERATOR OVERLOADS
		/// <summary>
		/// Gets the <see cref="PoolLibraryNode"/> with the specified name. An operator overload for the "[]" operator.
		/// </summary>
		/// <param name="name">GameObject name.</param>
		public PoolLibraryNode this [string name] {

			get {
				return Find (name);
			}
		}
		#endregion

		#region ACCESSORS AND MUTATORS
		/// <summary>
		/// An accessor to the pool reference.
		/// </summary>
		/// <value>The pool reference.</value>
		public GameObject Pool {
			get { return m_pool; }
			set { m_pool = value; }
		}

		/// <summary>
		/// An accessor to the Pool Library list. This is for internal, inspector use only.
		/// </summary>
		/// <value>The Pool Library list structure.</value>
		public List<PoolLibraryNode> Nodes {
			get { return m_nodes; }
			set { m_nodes = value; }
		}
		#endregion
	}
}