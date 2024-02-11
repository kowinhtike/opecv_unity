using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using OpenCvSharp;
using TMPro;
using UnityEngine;
using UnityEngine.Diagnostics;

public class Temp : MonoBehaviour
{

    WebCamTexture webCamTexture;
    Renderer renderer;

    CascadeClassifier cascade;
    public TextMeshProUGUI textMeshProUGUI;
    public TextMeshProUGUI textMeshProUGUI2;

    public GameObject modelObj;
    public Vector3 modelVector;
    public float rectangleRange;

    // Start is called before the first frame update
    void Start()
    {
        webCamTexture = new WebCamTexture();
        webCamTexture.Play();
        renderer = GetComponent<Renderer>();
        transform.rotation = Quaternion.Euler(0, -180, 0);
        renderer.material.mainTexture = webCamTexture;
        cascade = new CascadeClassifier("Assets/haarcascade_frontalface_default.xml");

    }

    void Update()
    {

        if(webCamTexture.didUpdateThisFrame) {
            textMeshProUGUI.text = "Camera is Running...";
            // Update the Mat with the current frame
            Mat frame = OpenCvSharp.Unity.TextureToMat(webCamTexture);
            var faces = cascade.DetectMultiScale(frame, 1.1, 2, HaarDetectionType.ScaleImage);
            //foreach (var face in faces)
            //{
                //frame.Rectangle(face, new Scalar(250, 0, 0), 2);
            //}
            if(faces.Length > 0)
            {
                var getFace = faces[0];
                frame.Rectangle(getFace, new Scalar(250, 0, 0), 8);
                textMeshProUGUI2.text = getFace.X + " & "+ getFace.Y;
                //add baryar
                modelObj.transform.position = new Vector3(modelVector.x +( getFace.X / rectangleRange), 0, modelVector.z + - (getFace.Y / rectangleRange));
            }
            Texture newTexture = OpenCvSharp.Unity.MatToTexture(frame);
            GetComponent<Renderer>().material.mainTexture = newTexture;
        }
        

    }

}
