using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightRotate : MonoBehaviour {

    public Vector3 rotateAxis;
    public float rotateAngle;


	// Update is called once per frame
	void Update () {
        transform.Rotate(rotateAxis, rotateAngle * Time.deltaTime);
	}
}
