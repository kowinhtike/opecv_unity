using UnityEngine;
using System.Collections.Generic; // For List<>
using OpenCvSharp; // Assuming OpenCV+Unity package uses OpenCvSharp or similar
using System.Linq;
using UnityEngine.Diagnostics;

public class ObjectDetect : MonoBehaviour
{
    public WebCamTexture webcamTexture;
    private Mat frame;
    private Scalar lowYellow = new Scalar(24, 110, 120); // Adjust based on your specific needs
    private Scalar highYellow = new Scalar(29, 255, 255); // Adjust based on your specific needs

    void Start()
    {
        // Initialize webcam texture
        webcamTexture = new WebCamTexture();
        GetComponent<Renderer>().material.mainTexture = webcamTexture;
        webcamTexture.Play();

        // Initialize frame Mat
        frame = new Mat(webcamTexture.height, webcamTexture.width, MatType.CV_8UC3);
    }

    void Update()
    {
        if (webcamTexture.didUpdateThisFrame && webcamTexture.isPlaying)
        {
             // Convert WebCamTexture to Mat
            frame = OpenCvSharp.Unity.TextureToMat(webcamTexture);

            Mat hsvFrame = new Mat();
            Cv2.CvtColor(frame, hsvFrame, ColorConversionCodes.BGR2HSV); // Convert frame to HSV

            Mat redMask = new Mat();
            Cv2.InRange(hsvFrame, lowYellow,highYellow, redMask); // Create mask for red colors

            // Find contours
            Cv2.FindContours(redMask, out Point[][] contours, out HierarchyIndex[] hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple);

            if (contours.Length > 0)
            {
                // Sort contours by area and get the largest one
                var largestContour = contours.OrderByDescending(c => Cv2.ContourArea(c)).First();
                OpenCvSharp.Rect boundingRect = Cv2.BoundingRect(largestContour); // Get bounding rect for the largest contour

                Cv2.Rectangle(frame, boundingRect, new Scalar(0, 0, 255), 2); // Draw rectangle around detected object

                int yMedium = (boundingRect.Top + boundingRect.Bottom) / 2;
                int xMedium = (boundingRect.Left + boundingRect.Right) / 2;

                Cv2.Line(frame, new Point(xMedium, 0), new Point(xMedium, frame.Rows), new Scalar(0, 255, 0), 2); // Draw vertical line
                Cv2.Line(frame, new Point(0, yMedium), new Point(frame.Cols, yMedium), new Scalar(0, 255, 0), 2); // Draw horizontal line
            }

            // Convert Mat back to Texture2D and apply to material
            Texture newTexture = OpenCvSharp.Unity.MatToTexture(frame);
            GetComponent<Renderer>().material.mainTexture = newTexture;
            // Flip texture horizontally
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                Vector2 scale = renderer.material.mainTextureScale;
                scale.x = -1; // Use -1 to flip horizontally, use scale.y = -1 to flip vertically
                renderer.material.mainTextureScale = scale;
            }
        }
    }

    void OnDestroy()
    {
        if (webcamTexture != null)
        {
            webcamTexture.Stop();
        }
    }
}
