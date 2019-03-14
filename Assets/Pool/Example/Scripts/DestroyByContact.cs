using UnityEngine;
using System.Collections;
using Whisper;

public class DestroyByContact : MonoBehaviour
{
	public GameObject explosion;
	public GameObject playerExplosion;
	public int scoreValue;
	private GameController gameController;

	void Start ()
	{
		GameObject gameControllerObject = GameObject.FindGameObjectWithTag ("GameController");
		if (gameControllerObject != null)
		{
			gameController = gameControllerObject.GetComponent <GameController>();
		}
		if (gameController == null)
		{
			Debug.Log ("Cannot find 'GameController' script");
		}
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Boundary" || other.tag == "Enemy")
		{
			return;
		}

		if (explosion != null)
		{
			ObjectPoolManager.Instance.Acquire(explosion.name, transform.position, transform.rotation);
		}

		if (other.tag == "Player")
		{
			ObjectPoolManager.Instance.Acquire(playerExplosion.name, other.transform.position, other.transform.rotation);
			gameController.GameOver();
		}
		
		gameController.AddScore(scoreValue);
		ObjectPoolManager.Instance.Release (other.gameObject);
		ObjectPoolManager.Instance.Release (gameObject);
	}
}