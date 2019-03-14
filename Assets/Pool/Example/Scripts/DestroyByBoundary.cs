using UnityEngine;
using System.Collections;
using Whisper;

public class DestroyByBoundary : MonoBehaviour
{
	void OnTriggerExit (Collider other) 
	{
		ObjectPoolManager.Instance.Release(other.gameObject);
	}
}