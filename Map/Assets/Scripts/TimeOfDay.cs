using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeOfDay : MonoBehaviour {

    float xAngle = 0;
    float timeConstant = 5;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        xAngle = timeConstant * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, 0, xAngle) * transform.rotation;
	}
}
