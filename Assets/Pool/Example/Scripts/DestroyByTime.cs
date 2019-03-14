using UnityEngine;
using System.Collections;
using Whisper;

public class DestroyByTime : MonoBehaviour {

	public float lifetime;

	void Start () {
		ObjectPoolManager.Instance.Release (this.gameObject, lifetime);
	}
}
