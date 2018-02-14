using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SenseSwitcher : MonoBehaviour {

    [SerializeField]
    private Material mSwitchMat;

    private List<GameObject> mObjectRefs;
    private List<Material> mOriginalMats;

    private bool mIsSight;
    private Material mActualMat, mOldSkybox;

	// Use this for initialization
	void Start () {
        mIsSight = true;
        mActualMat = Material.Instantiate(mSwitchMat);
        mOldSkybox = RenderSettings.skybox;

        mObjectRefs = new List<GameObject>();
        mOriginalMats = new List<Material>();
        MeshRenderer[] renderers = GameObject.FindObjectsOfType<MeshRenderer>();

        foreach(MeshRenderer rend in renderers)
        {
            mObjectRefs.Add(rend.gameObject);
            mOriginalMats.Add(rend.material);
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.G))
        {
            mIsSight = !mIsSight;

            if(mIsSight)
            {
                int index = 0;
                foreach(GameObject obj in mObjectRefs)
                {
                    obj.GetComponent<MeshRenderer>().material = mOriginalMats[index];
                    index++;
                }

                RenderSettings.skybox = mOldSkybox;
            }
            else
            {
                foreach (GameObject obj in mObjectRefs)
                {
                    obj.GetComponent<MeshRenderer>().material = mActualMat;
                }

                RenderSettings.skybox = null;
            }
        }
	}
}
