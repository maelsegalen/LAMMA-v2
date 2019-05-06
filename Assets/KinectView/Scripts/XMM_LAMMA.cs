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

    float footDistanceThreshold = 0.1; //To be adapted.
    float[] prevLegCoords = new float[9];
    float[] legCoords = new float[9]; //Will hold the coordinates of the foot, the ankle and the knee.


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
        updateLegCoords();

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

        
        if (record || filter)
        {
            //To be added
            //Vector3 jointPosition = BodySourceView.GetJointLocalPosition(Kinect.JointType.FootRight);


            //Should be modified to get the distance of the foot only
            if (distanceLeg(legCoords, prevLegCoords) > footDistanceThreshold)
            {
                prevLegCoords[0] = legCoords[0];
                prevLegCoords[1] = legCoords[1];
                prevLegCoords[2] = legCoords[2];
                prevLegCoords[3] = legCoords[3];
                prevLegCoords[4] = legCoords[4];
                prevLegCoords[5] = legCoords[5];
                prevLegCoords[6] = legCoords[6];
                prevLegCoords[7] = legCoords[7];
                prevLegCoords[8] = legCoords[8];

                if (record)
                {
                    phrase.Add(legDelta[0]);
                    phrase.Add(legDelta[1]);
                    phrase.Add(legDelta[2]);
                    phrase.Add(legDelta[3]);
                    phrase.Add(legDelta[4]);
                    phrase.Add(legDelta[5]);
                    phrase.Add(legDelta[6]);
                    phrase.Add(legDelta[7]);
                    phrase.Add(legDelta[8]);

                }
                else
                { //filter
                    hhmm.Filter(legDelta);
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
        //Kinect coordinates are already updated at the beginning of the "update" loop.
        prevLegCoords = legCoords;
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
        //Kinect coordinates are already updated at the beginning of the "update" loop.
        prevLegCoords = legCoords;
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

    //Computes the difference between the previous leg coordinates and the current one,
    //and returns the distance made by the foot, in order to trigger the system.
    private float distanceLeg(float[] newPos, float[] prevPos)
    {
        legDelta[0] = newPos[0] - prevPos[0];
        legDelta[1] = newPos[1] - prevPos[1];
        legDelta[2] = newPos[2] - prevPos[2];
        legDelta[3] = newPos[3] - prevPos[3];
        legDelta[4] = newPos[4] - prevPos[4];
        legDelta[5] = newPos[5] - prevPos[5];
        legDelta[6] = newPos[6] - prevPos[6];
        legDelta[7] = newPos[7] - prevPos[7];
        legDelta[8] = newPos[8] - prevPos[8];

        //Returns only the distance of the foot to trigger.
        return (float)Math.Sqrt(legDelta[0] * legDelta[0] +
                                legDelta[1] * legDelta[1] +
                                legDelta[2] * legDelta[2]);
    }

    private void logLabels()
    {
        string[] labels = ts.GetLabels();
        Debug.Log("nb of labels : " + labels.Length);
        for (int i = 0; i < labels.Length; ++i)
        {
            Debug.Log("label " + i + " : " + labels[i]);
        }
    }

    private void updateLegCoords()
    {
        //Get the coordinates of every points of the right leg.
        Vector3 jointPositionFootRight = BodySourceView.GetJointLocalPosition(Kinect.JointType.FootRight)
        Vector3 jointPositionAnkleRight = BodySourceView.GetJointLocalPosition(Kinect.JointType.AnkleRight)
        Vector3 jointPositionKneeRight = BodySourceView.GetJointLocalPosition(Kinect.JointType.KneeRight)
        Vector3 jointPositionHipRight = BodySourceView.GetJointLocalPosition(Kinect.JointType.HipRight)

        legCoords[0] = jointPositionFootRight.localPosition[0] - jointPositionHipRight.localPosition[0];
        legCoords[1] = jointPositionFootRight.localPosition[1] - jointPositionHipRight.localPosition[1];
        legCoords[2] = jointPositionFootRight.localPosition[2] - jointPositionHipRight.localPosition[2];
        legCoords[3] = jointPositionAnkleRight.localPosition[0] - jointPositionHipRight.localPosition[0]
        legCoords[4] = jointPositionAnkleRight.localPosition[1] - jointPositionHipRight.localPosition[1]
        legCoords[5] = jointPositionAnkleRight.localPosition[2] - jointPositionHipRight.localPosition[2]
        legCoords[6] = jointPositionKneeRight.localPosition[0] - jointPositionHipRight.localPosition[0]
        legCoords[7] = jointPositionKneeRight.localPosition[1] - jointPositionHipRight.localPosition[1]
        legCoords[8] = jointPositionKneeRight.localPosition[2] - jointPositionHipRight.localPosition[2]

    }
}
