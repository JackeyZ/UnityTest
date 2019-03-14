using UnityEngine;
using System.Collections;
using Whisper;

public class WeaponController : MonoBehaviour
{
	public GameObject shot;
	public Transform shotSpawn;
	public float fireRate;
	public float delay;

	private float m_timer = 0;

	void Update () {
		m_timer += Time.deltaTime;

		if (m_timer >= fireRate) {
			Fire();
			m_timer = 0;
		}
	}

	void Fire () {
		ObjectPoolManager.Instance.Acquire(shot.name, shotSpawn.position, shotSpawn.rotation);
		GetComponent<AudioSource>().Play();
	}
}
