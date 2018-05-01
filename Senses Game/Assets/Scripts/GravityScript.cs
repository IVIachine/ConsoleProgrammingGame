using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityScript : MonoBehaviour
{
    [SerializeField]
    private float mZOffset;

    [SerializeField]
    private float mChangeSpeed;

    [SerializeField]
    private float mMaxAlpha;

    [SerializeField]
    private int mPulseCount;

    [SerializeField]
    private float mPulseWidth;

    [SerializeField]
    private float mOffset;

    private float mCurrentAlpha;
    private List<float> mPulses;

    /// <summary>
    /// Set up pulse positions based on developer input
    /// </summary>
    void Start()
    {
        mCurrentAlpha = 0.0f;

        mPulses = new List<float>();
        float actualOffset = 0;
        if (mOffset == 0)
            actualOffset = 1;
        else
            actualOffset = mOffset;
        for (int i = 0; i < mPulseCount * 2; i += 2)
        {
            mPulses.Add(GetComponent<MeshFilter>().mesh.bounds.extents.y - (i / 2 * actualOffset * ((GetComponent<MeshFilter>().mesh.bounds.size.y + mPulseWidth) / mPulseCount)) + mOffset);
            mPulses.Add(GetComponent<MeshFilter>().mesh.bounds.extents.y - (i / 2 * actualOffset * ((GetComponent<MeshFilter>().mesh.bounds.size.y + mPulseWidth) / mPulseCount)) + mPulseWidth + mOffset);
        }


        GetComponent<MeshRenderer>().material.SetVector("center", GetComponent<MeshFilter>().mesh.bounds.center);
    }

    /// <summary>
    /// Move the positions downward and set variables in shader. Wrap if necessary
    /// </summary>
    void Update()
    {
        for (int i = 0; i < mPulses.Count; i += 2)
        {
            mPulses[i] = mPulses[i] - mChangeSpeed * Time.deltaTime;
            mPulses[i + 1] = mPulses[i + 1] - mChangeSpeed * Time.deltaTime;

            if (mPulses[i + 1] < -GetComponent<MeshFilter>().mesh.bounds.extents.y - mOffset)
            {
                mPulses[i] = GetComponent<MeshFilter>().mesh.bounds.extents.y + mOffset;
                mPulses[i + 1] = GetComponent<MeshFilter>().mesh.bounds.extents.y + mPulseWidth + mOffset;

            }
        }

        Color old = GetComponent<MeshRenderer>().material.color;
        GetComponent<MeshRenderer>().material.SetFloatArray("rectBounds", mPulses.ToArray());
        GetComponent<MeshRenderer>().material.color = new Color(old.r, old.g, old.b, mCurrentAlpha);
    }
}
