using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMono : MonoBehaviour {
	// Use this for initialization
	void Start ()
    {
        Debug.Log(AssetBundleMgr.Instance.name);
        Debug.Log(AssetBundleMgr2.Instance.name);
        Debug.Log(AssetBundleMgr23.Instance.name);
    }
	
	// Update is called once per frame
	void Update () {
        PlayerControl.Instance.speed += Time.deltaTime * 2;
	}
}
