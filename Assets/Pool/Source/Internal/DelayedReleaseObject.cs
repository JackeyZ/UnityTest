using UnityEngine;
using System.Collections;

/// <summary>
/// Delayed Release Object. A helper class that holds a reference to a GameObject that should be released when a timer has expired.
/// </summary>
[System.Serializable]
public class DelayedReleaseObject {

	#region PRIVATE MEMBER VARIABLES
	/// <summary>
	/// The delay to wait.
	/// </summary>
	[SerializeField]
	private float m_delay;

	/// <summary>
	/// The current time past since Release was called.
	/// </summary>
	[SerializeField]
	private float m_currentTime;

	/// <summary>
	/// The GameObject reference.
	/// </summary>
	[SerializeField]
	private GameObject m_gameObject;
	#endregion

	#region CONSTRUCTOR
	/// <summary>
	/// Initializes a new instance of the <see cref="DelayedReleaseObject"/> class.
	/// </summary>
	/// <param name="obj">Object.</param>
	/// <param name="time">Time.</param>
	public DelayedReleaseObject (GameObject obj, float time) {
		m_gameObject = obj;
		m_delay = time;
		m_currentTime = 0;
	}
	#endregion

	#region ACCESSORS AND MUTATORS
	/// <summary>
	/// Gets or sets the time delay.
	/// </summary>
	/// <value>The delay.</value>
	public float Delay {
		get { return m_delay; }
		set { m_delay = value; }
	}

	/// <summary>
	/// Gets or sets the current time elapsed since Release was called.
	/// </summary>
	/// <value>The current time.</value>
	public float CurrentTime {
		get { return m_currentTime; }
		set { m_currentTime = value; }
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
