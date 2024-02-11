using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using OpenCvSharp;
using UnityEngine;
using UnityEngine.Diagnostics;

public class AndroidCamera : MonoBehaviour
{

    WebCamTexture webCamTexture;
    Renderer renderer;

    CascadeClassifier cascade;
    OpenCvSharp.Rect rectangle;

    

    // Start is called before the first frame update
    void Start()
    {
        webCamTexture = new WebCamTexture();
        webCamTexture.Play();
        renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = webCamTexture;
        cascade = new CascadeClassifier("Assets/haarcascade_frontalface_default.xml");

    }

    void Update()
    {
        // Update the Mat with the current frame
        Mat frame = OpenCvSharp.Unity.TextureToMat(webCamTexture);
        findNewFace(frame);
        display(frame);
    }

    void findNewFace(Mat frame)
    {
        var faces = cascade.DetectMultiScale(frame, 1.1, 2, HaarDetectionType.ScaleImage);
        if (faces.Length >= 1)
        {
            Debug.Log(faces[0].Location);
            rectangle = faces[0];
            

        }
    }

    void display(Mat frame)
    {
        if (rectangle != null)
        {
            frame.Rectangle(rectangle, new Scalar(250, 0, 0), 2);
        }
        Texture newTexture = OpenCvSharp.Unity.MatToTexture(frame);
        GetComponent<Renderer>().material.mainTexture = newTexture;
       
    }
}
