using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using Kinect = Windows.Kinect;


public class XMM_LAMMA : MonoBehaviour
{
    private Dictionary<Kinect.JointType, Kinect.JointType> _BoneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
    {
        { Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft },
        { Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft },
        { Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },
        { Kinect.JointType.HipLeft, Kinect.JointType.SpineBase },
        
        { Kinect.JointType.FootRight, Kinect.JointType.AnkleRight },
        { Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight },
        { Kinect.JointType.KneeRight, Kinect.JointType.HipRight },
        { Kinect.JointType.HipRight, Kinect.JointType.SpineBase },
        
        { Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.HandLeft, Kinect.JointType.WristLeft },
        { Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft },
        { Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },
        { Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder },
        
        { Kinect.JointType.HandTipRight, Kinect.JointType.HandRight },
        { Kinect.JointType.ThumbRight, Kinect.JointType.HandRight },
        { Kinect.JointType.HandRight, Kinect.JointType.WristRight },
        { Kinect.JointType.WristRight, Kinect.JointType.ElbowRight },
        { Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },
        { Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder },
        
        { Kinect.JointType.SpineBase, Kinect.JointType.SpineMid },
        { Kinect.JointType.SpineMid, Kinect.JointType.SpineShoulder },
        { Kinect.JointType.SpineShoulder, Kinect.JointType.Neck },
        { Kinect.JointType.Neck, Kinect.JointType.Head },
    };
    float mouseDistanceThreshold = 2;
    float[] prevMouseCoords = new float[2];
    float[] mouseCoords = new float[2];
    float[] mouseDelta = new float[2];

    float legDistanceThreshold = 0.5; //To be adapted
    float[] prevLegCoords = new float[9];
    float[] legCoords = new float[9]; //Will hold the coordinates of the foot, the ankle and the knee
    float[] legDelta = new float[9];

    List<float> phrase;
    bool recordEnabled = false;
    bool record = false;
    bool filter = false;
    string label = "";
    string likeliest = "";
    float[] likelihoods = new float[0];
    private XmmTrainingSet ts = new XmmTrainingSet();
    private XmmModel hhmm = new XmmModel("hhmm");

    // Use this for initialization
    void Start()
    {
        hhmm.SetStates(10);
        hhmm.SetLikelihoodWindow(5);
        hhmm.SetRelativeRegularization(0.01f);
        hhmm.SetGaussians(1);
    }

    // Update is called once per frame
    void Update()
    {
        //Set the current label
        if (Input.anyKey)
        {
            if (!String.IsNullOrEmpty(Input.inputString))
            {
                char c = Input.inputString[0];
                label = (c >= 'a' && c <= 'z' && c != 'r') ? c.ToString() : label;

                if (c == 'r' && !Input.GetMouseButton(0))
                {
                    recordEnabled = !recordEnabled;
                }
            }
        }

        //Record/Perform using the mouse
        if (Input.GetMouseButtonDown(0))
        {
            if (recordEnabled)
            {
                startRecording();
            }
            else
            {
                startFiltering();
            }
        }

        //Stop recording/performing using the mouse
        if (Input.GetMouseButtonUp(0))
        {
            if (recordEnabled)
            {
                stopRecording();
            }
            else
            {
                stopFiltering();
            }
        }

        //
        if (record || filter)
        {

            List<string> Liste = new List<string>();
            for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
            {
                Kinect.Joint sourceJoint = body.Joints[jt];
                Kinect.Joint? targetJoint = null;
                
                Transform jointObj = bodyObject.transform.Find(jt.ToString());
                jointObj.localPosition = GetVector3FromJoint(sourceJoint);
                
                if (jt == Kinect.JointType.FootRight || jt == Kinect.JointType.AnkleRight || jt == Kinect.JointType.KneeRight || jt == Kinect.JointType.HipRight)
                {

                    //TODO : Compute the difference between the points to avoid some orientation problem
                    print(jt.ToString());
                    print(jointObj.localPosition);
                    Liste.Add(jointObj.localPosition[0].ToString());
                    Liste.Add(jointObj.localPosition[1].ToString());
                    Liste.Add(jointObj.localPosition[2].ToString());
                    if (Liste.Count==12)
                    {
                        Liste.ForEach(item => print(item));
                        Liste.Clear();
                    }
                }
            }

            mouseCoords[0] = Input.mousePosition[0];
            mouseCoords[1] = Input.mousePosition[1];

            //Should be modified to get the distance of the foot only
            if (distance(mouseCoords, prevMouseCoords) > mouseDistanceThreshold)
            {
                prevMouseCoords[0] = mouseCoords[0];
                prevMouseCoords[1] = mouseCoords[1];

                if (record)
                {
                    phrase.Add(mouseDelta[0]);
                    phrase.Add(mouseDelta[1]);
                }
                else
                { //filter
                    hhmm.Filter(mouseDelta);
                    likeliest = hhmm.GetLikeliest();
                    likelihoods = hhmm.GetLikelihoods();
                }
            }
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 200, 50),
                  "recording " + (recordEnabled ? "enabled" : "disabled"));
        GUI.Label(new Rect(10, 30, 200, 50),
                  "current label : " + label);
        GUI.Label(new Rect(10, 50, 200, 50), "nb of models : " + hhmm.GetNbOfModels());
        GUI.Label(new Rect(10, 70, 200, 50), "nb of phrases : " + ts.Size());
        GUI.Label(new Rect(10, 90, 200, 50), "likeliest : " + likeliest);

        string l = "";
        for (int i = 0; i < likelihoods.Length; ++i)
        {
            l += likelihoods[i] + " ";
        }
        GUI.Label(new Rect(10, 110, 200, 50), "likelihoods : " + l);
    }

    //Modify it to get the input of the three axis of the leg. Check the dimensions.
    private void startRecording()
    {
        record = true;
        phrase = new List<float>();
        prevMouseCoords[0] = Input.mousePosition[0];
        prevMouseCoords[1] = Input.mousePosition[1];
    }

    //Idem
    private void stopRecording()
    {
        record = false;
        float[] p = phrase.ToArray();
        string[] colNames = { "mouseX", "mouseY" };
        ts.AddPhraseFromData(label, colNames, p, 2, 0);
        hhmm.Train(ts);
        hhmm.Reset();
    }

    //Idem
    private void startFiltering()
    {
        filter = true;
        prevMouseCoords[0] = Input.mousePosition[0];
        prevMouseCoords[1] = Input.mousePosition[1];
    }

    //Idem
    private void stopFiltering()
    {
        filter = false;
        hhmm.Reset();
    }

    private float distance(float[] newPos, float[] prevPos)
    {
        mouseDelta[0] = newPos[0] - prevPos[0];
        mouseDelta[1] = newPos[1] - prevPos[1];

        return (float)Math.Sqrt(mouseDelta[0] * mouseDelta[0] +
                                mouseDelta[1] * mouseDelta[1]);
    }

    //WTF ?
    private void logLabels()
    {
        string[] labels = ts.GetLabels();
        Debug.Log("nb of labels : " + labels.Length);
        for (int i = 0; i < labels.Length; ++i)
        {
            Debug.Log("label " + i + " : " + labels[i]);
        }
    }
}
