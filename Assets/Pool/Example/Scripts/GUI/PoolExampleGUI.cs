using UnityEngine;
using System.Collections;
using Whisper;

public class PoolExampleGUI : MonoBehaviour {

	public GameObject asteroid;
	private ObjectPoolManager m_pool;
	private bool m_paused;
	private int m_posY;

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start () {
	
		m_posY = 50;
		m_paused = false;
		m_pool = ObjectPoolManager.Instance;
	}
	
	/// <summary>
	/// Update this instance. Handles key presses and Object Pool manipulation.
	/// </summary>
	void Update () {
	
		if (Input.GetKeyDown(KeyCode.Escape)) {

			m_paused = !m_paused;

			if (m_paused) {
				Time.timeScale = 0;
			}

			else {
				Time.timeScale = 1;
			}
		}

		if (Input.GetKeyDown(KeyCode.T)) {

			m_pool.EnforcePooling = !m_pool.EnforcePooling;
		}

		if (Input.GetKeyDown(KeyCode.C)) {
			
			m_pool.UseDebug = !m_pool.UseDebug;
		}


		if (Input.GetKeyDown(KeyCode.G)) {
			
			m_pool.AllowInstantiation = !m_pool.AllowInstantiation;
		}

		if (Input.GetKeyDown(KeyCode.X)) {
			
			m_pool.Detatch = !m_pool.Detatch;
		}

		if (Input.GetKeyDown(KeyCode.Q)) {

			for (int i = -6; i < 8; i += 2) {
				Vector3 spawnPosition = new Vector3 (i, 0, 16);
				Quaternion spawnRotation = Quaternion.identity;
				ObjectPoolManager.Instance.Acquire ("Asteroid 01", spawnPosition, spawnRotation);
			}
		}

		if (Input.GetKeyDown(KeyCode.E)) {
			
			for (int i = -6; i < 8; i += 2) {
				Vector3 spawnPosition = new Vector3 (i, 0, 16);
				Quaternion spawnRotation = Quaternion.identity;
				Instantiate (asteroid, spawnPosition, spawnRotation);
			}
		}
	}

	/// <summary>
	/// Raises the GUI event. Draws a simple control panel.
	/// </summary>
	void OnGUI () {

		// Make a background box
		GUI.Box(new Rect(10, m_posY + 10, 300, 170), "Object Pool Menu");
		GUI.Label(new Rect(20, m_posY + 30, 300, 90), "Press Escape to pause!");

		GUI.Toggle (new Rect (20, m_posY + 50, 300, 90), m_pool.UseDebug, " Use debug? C toggles.");
		GUI.Toggle (new Rect (20, m_posY + 70, 300, 90), m_pool.EnforcePooling, " Enforce pooling? T toggles.");
		GUI.Toggle (new Rect (20, m_posY + 90, 300, 90), m_pool.AllowInstantiation, " Allow instantiations? G toggles.");
		GUI.Toggle (new Rect (20, m_posY + 110, 300, 90), m_pool.Detatch, " Detatch Objects? X toggles.");

		GUI.Label(new Rect(20, m_posY + 130, 300, 90), "Press Q to acquire pooled objects!");
		GUI.Label(new Rect(20, m_posY + 150, 300, 90), "Press E to instantiate objects!");
	}
}
