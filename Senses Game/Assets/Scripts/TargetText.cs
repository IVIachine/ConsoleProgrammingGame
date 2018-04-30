using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetText : MonoBehaviour {

    [SerializeField]
    GameObject playerObject;

	// Update is called once per frame
	void Update () {
        transform.forward = -(playerObject.transform.position - transform.position).normalized;
	}
}