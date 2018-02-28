using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SenseSwitcher : MonoBehaviour {

    [SerializeField]
    private Material mSwitchMat;

    [SerializeField]
    private int mMaxSounds;

    [SerializeField]
    private GameObject mSoundPrefab;

    [SerializeField]
    private GameObject mPlayerRef;

    private List<GameObject> mObjectRefs;
    private List<Material> mOriginalMats;

    private bool mIsSight;
    private Material mActualMat, mOldSkybox;

    private ComputeBuffer mSoundPositions;
    private ComputeBuffer mSoundRadii;
    private int mNumSounds;
    private GameObject[] mCurrentSounds;

	// Use this for initialization
	void Start () {
        mCurrentSounds = new GameObject[mMaxSounds];
        mIsSight = true;
        mNumSounds = 0;
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

        Vector3[] soundInit = new Vector3[mMaxSounds];
        mSoundPositions = new ComputeBuffer(mMaxSounds, System.Runtime.InteropServices.Marshal.SizeOf(Vector3.zero), ComputeBufferType.Default);
        mSoundPositions.SetData(soundInit);

        float temp = 0;
        float[] soundRadiiInit = new float[mMaxSounds];
        mSoundRadii = new ComputeBuffer(mMaxSounds, System.Runtime.InteropServices.Marshal.SizeOf(temp), ComputeBufferType.Default);
        mSoundRadii.SetData(soundRadiiInit);

        mActualMat.SetBuffer("soundPositions", mSoundPositions);
        mActualMat.SetBuffer("soundRadii", mSoundRadii);
        mActualMat.SetInt("numPositions", mNumSounds);
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

        Vector3[] soundPos = new Vector3[mMaxSounds];
        mSoundPositions.GetData(soundPos);

        for (int i = 0; i < mNumSounds; i++)
        {
            soundPos[i] = mCurrentSounds[i].transform.position;
        }

        mSoundPositions.SetData(soundPos);
        mActualMat.SetInt("numPositions", mNumSounds);
    }

    public  void RemoveSound(GameObject reference)
    {
        int index = -1;
        for (int i = 0; i < mCurrentSounds.Length; i++)
        {
            if(mCurrentSounds[i] == reference)
            {
                index = i;
                break;
            }
        }

        if (index == -1)
            return;

        float[] soundRadii = new float[mMaxSounds];
        mSoundRadii.GetData(soundRadii);

        Vector3[] soundPos = new Vector3[mMaxSounds];
        mSoundPositions.GetData(soundPos);

        soundPos[index] = Vector3.zero;
        soundRadii[index] = 0;

        mSoundRadii.SetData(soundRadii);
        mSoundPositions.SetData(soundPos);

        Destroy(mCurrentSounds[index]);

        float[] radiiTemp = new float[mMaxSounds];
        Array.Copy(soundRadii, radiiTemp, mMaxSounds);
        Array.Sort(radiiTemp, soundPos);

        float[] radiiTemp2 = new float[mMaxSounds];
        Array.Copy(soundRadii, radiiTemp2, mMaxSounds);
        Array.Sort(radiiTemp2, mCurrentSounds);

        Array.Sort(soundRadii);

        Array.Reverse(soundRadii);
        Array.Reverse(soundPos);
        Array.Reverse(mCurrentSounds);

        mSoundPositions.SetData(soundPos);
        mSoundRadii.SetData(soundRadii);
        mNumSounds--;
    }

    public int AddNewSound(float radii, Vector3 pos)
    {
        if (mNumSounds >= mMaxSounds)
            return -1;

        float[] soundRadii = new float[mMaxSounds];
        mSoundRadii.GetData(soundRadii);

        Vector3[] soundPos = new Vector3[mMaxSounds];
        mSoundPositions.GetData(soundPos);

        int index = -1;
        for (int i = 0; i < mMaxSounds; i++)
        {
            if (soundRadii[i] == 0)
            {
                index = i;
                break;
            }
        }

        if (index == -1)
            return -1;

        soundRadii[index] = radii;
        soundPos[index] = pos;
        mNumSounds++;

        mSoundRadii.SetData(soundRadii);
        mSoundPositions.SetData(soundPos);

        mCurrentSounds[index] = GameObject.Instantiate(mSoundPrefab, pos, Quaternion.identity);
        NavMeshPath path = new NavMeshPath();
        if (mCurrentSounds[index].GetComponent<NavMeshAgent>().CalculatePath(mPlayerRef.transform.position, path))
        {
            mCurrentSounds[index].GetComponent<NavMeshAgent>().SetPath(path);
            mCurrentSounds[index].AddComponent<SenseMover>().beginMovement(index, this.gameObject);
        }

        return index;
    }

    private void OnDestroy()
    {
        mSoundPositions.Dispose();
        mSoundRadii.Dispose();
    }
}
