
using OpenCvSharp;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    WebCamTexture webcamTexture;
    
    CascadeClassifier cascade;
    OpenCvSharp.Rect rectangle;

    // Start is called before the first frame update
    // https://www.youtube.com/watch?v=lXvt66A0i3Q
    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        webcamTexture = new WebCamTexture(devices[0].name);
        webcamTexture.Play();
        cascade = new CascadeClassifier("Assets/haarcascade_frontalface_default.xml");
        Debug.Log("Good");
        transform.rotation = Quaternion.Euler(0, -180, 0);
    }

    // Update is called once per frame
    void Update()
    {
      // GetComponent<Renderer>().material.mainTexture = webcamTexture;

        Mat frame = OpenCvSharp.Unity.TextureToMat(webcamTexture);
        findNewFace(frame);
        display(frame);


    }

    void findNewFace(Mat frame)
    {
        var faces = cascade.DetectMultiScale(frame,1.1,2,HaarDetectionType.ScaleImage);
        if(faces.Length >= 1)
        {
            Debug.Log(faces[0].Location);
            rectangle = faces[0];
        }
    }

    void display(Mat frame)
    {
        if(rectangle != null)
        {
            frame.Rectangle(rectangle,new Scalar(250,0,0),2);
        }
        Texture newTexture = OpenCvSharp.Unity.MatToTexture(frame);
        GetComponent<Renderer>().material.mainTexture = newTexture;
    }

}
