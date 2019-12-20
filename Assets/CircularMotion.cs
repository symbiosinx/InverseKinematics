using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularMotion : MonoBehaviour {
	
	Vector3 startPos;

    void Start() {
		startPos = transform.position;
	}

	void Update() {
		transform.position = startPos + new Vector3(Mathf.Cos(Time.time)*1.5f, Mathf.Sin(Time.time*3f)*1f, Mathf.Sin(Time.time)*1.5f);
	}
}
