using UnityEngine;
using System.Collections;

namespace Whisper {
	
	/// <summary>
	/// Pooled Game Object. A class that stores a reference to a currently pooled GameObject and determines whether or not it has been acquired.
	/// </summary>
	[System.Serializable]
	public class PooledGameObject {
	
		#region PRIVATE MEMBER VARIABLES
		/// <summary>
		/// Determines whether or not the GameObject reference in this class is currently acquired.
		/// </summary>
		[SerializeField]
		private bool m_acquired;
	
		/// <summary>
		/// The GameObject reference.
		/// </summary>
		[SerializeField]
		private GameObject m_gameObject;
		#endregion
	
		#region CONSTRUCTOR
		/// <summary>
		/// Initializes a new instance of the <see cref="PooledGameObject"/> class.
		/// </summary>
		/// <param name="obj">Object.</param>
		public PooledGameObject (GameObject obj) {
			m_acquired = false;
			m_gameObject = obj;
		}
		#endregion
	
		#region ACCESSORS AND MUTATORS
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="PooledGameObject"/> is currently acquired.
		/// </summary>
		/// <value><c>true</c> if acquired; otherwise, <c>false</c>.</value>
		public bool Acquired {
			get { return m_acquired; }
			set { m_acquired = value; }
		}
	
		/// <summary>
		/// Gets or sets the GameObject reference.
		/// </summary>
		/// <value>The game object.</value>
		public GameObject GameObject {
			get { return m_gameObject; }
			set { m_gameObject = value; }
		}
		#endregion
	}
}