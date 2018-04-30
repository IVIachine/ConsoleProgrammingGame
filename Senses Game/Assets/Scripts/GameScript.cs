using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScript : MonoBehaviour {

    [SerializeField]
    float mDistRequired = 3.0f;

    [SerializeField]
    Transform mGoalTransform;

	// Update is called once per frame
	void Update () {
		if(Vector3.Distance(mGoalTransform.position, transform.position) <= mDistRequired)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
	}
}
