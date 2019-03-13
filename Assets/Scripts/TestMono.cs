using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMono : MonoBehaviour {
	// Use this for initialization
	void Start ()
    {
        Debug.LogError(AssetBundleMgr.Instance.name);
        Debug.LogError(AssetBundleMgr2.Instance.name);
        Debug.LogError(AssetBundleMgr23.Instance.name);
    }
	
	// Update is called once per frame
	void Update () {
        PlayerControl.Instance.speed += Time.deltaTime * 2;
	}
}
