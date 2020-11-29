using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxRotator : MonoBehaviour {
    
    public float speed = 0.2f;
    private Material skyMat;

    private void Start() {
        skyMat = RenderSettings.skybox;
    }

    void Update(){
        skyMat.SetFloat("_Rotation", skyMat.GetFloat("_Rotation") + Time.deltaTime * speed);
    }
}
