using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

public class AIPatrolScript : MonoBehaviour {

    [SerializeField]
    List<Vector3> mPatrolLocations;

    [SerializeField]
    private float mWaitTime;

    private float mCurrentWait;
    private int mCurrentIndex;
    private bool mPathfinding;
	// Use this for initialization
	void Start () {
        mCurrentIndex = 0;
        mCurrentWait = 0.0f;
        mPathfinding = false;
	}
	
	// Update is called once per frame
	void Update () {
        if(!mPathfinding && mCurrentWait <= 0.0f)
        {
            mCurrentIndex++;
            if (mCurrentIndex >= mPatrolLocations.Count)
                mCurrentIndex = 0;

            NavMeshPath path = new NavMeshPath();
            if (GetComponent<NavMeshAgent>().CalculatePath(mPatrolLocations[mCurrentIndex], path))
            {
                GetComponent<NavMeshAgent>().SetPath(path);
            }
            else
            {
                Debug.LogError(name + " Failed to find path");
            }

            mPathfinding = true;
        }
        else if(!mPathfinding)
            mCurrentWait -= Time.deltaTime;

        if (GetComponent<NavMeshAgent>().remainingDistance <= .025f && mPathfinding)
        {
            mPathfinding = false;
            mCurrentWait = mWaitTime;
        }
    }
}
