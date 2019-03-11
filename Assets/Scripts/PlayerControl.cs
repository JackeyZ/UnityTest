using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {
    CharacterController cc;
    public float speed;
	// Use this for initialization
	void Start () {
        cc = gameObject.GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {
        cc.Move(transform.forward * Time.deltaTime * Input.GetAxis("Vertical") * speed);
        cc.Move(transform.right * Time.deltaTime * Input.GetAxis("Horizontal") * speed);
    }
}
