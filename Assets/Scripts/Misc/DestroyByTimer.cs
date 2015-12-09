using UnityEngine;
using System.Collections;

public class DestroyByTimer : MonoBehaviour {
    public float timeLeft;
	// Use this for initialization
	void Start () {
        Destroy(gameObject, timeLeft);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
