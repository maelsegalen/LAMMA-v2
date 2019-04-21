using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;


public class DisableOnStart : MonoBehaviour {
 
    // Use this for initialization
    void Start () 
    {
        gameObject.SetActive (false);
}
}
