using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class DroneViews : MonoBehaviour
{
    public Camera droneCam; // Assign your drone's camera in the Unity editor
    public string captureCheckURL = "http://127.0.0.1:5000/trigger_capture"; // Flask server URL for checking the capture trigger
    public string uploadURL = "http://127.0.0.1:5000/upload_drone_image"; // Flask server URL for uploading the drone image
    public string clearFoldersURL = "http://127.0.0.1:5000/clear_folders"; // Flask server URL for clearing the image folders
    public string processedImageURL = "http://127.0.0.1:5000/processed_image"; // Flask server URL for processed image

    private bool shouldCapture = false;
    private int captureCounter = 0; // Counter for captured images
    private int imagesToSkip = 10; // Number of images to skip

    public int nameCounter = 0;

    void Start()
    {
        // Clear folders before starting the simulation
        StartCoroutine(ClearImageFolders());

        // Start checking the Flask server for capture triggers
        StartCoroutine(CheckForCaptureTrigger());
    }

    IEnumerator ClearImageFolders()
    {
        UnityWebRequest www = UnityWebRequest.PostWwwForm(clearFoldersURL, "");
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Folders cleared successfully before starting simulation.");
        }
        else
        {
            Debug.LogError("Error clearing folders: " + www.error);
        }
    }

    IEnumerator CheckForCaptureTrigger()
    {
        while (true)
        {
            UnityWebRequest www = UnityWebRequest.Get(captureCheckURL);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                // If the Flask server indicates a capture trigger, enable the capture flag
                if (www.downloadHandler.text.Contains("success"))
                {
                    shouldCapture = true;
                    Debug.Log("Capture trigger received from Flask.");
                }
            }
            else
            {
                Debug.LogError("Error checking capture trigger: " + www.error);
            }

            // Check again every second (you can adjust this interval)
            yield return new WaitForSeconds(1f);
        }
    }

    void Update()
    {
        // Capture the drone view if Flask triggered it
        if (shouldCapture)
        {
            if (captureCounter >= imagesToSkip)
            {
                CaptureAndSendView();
            }
            else
            {
                captureCounter++; // Increment the counter and skip the capture
                Debug.Log("Skipping image capture " + captureCounter);
            }

            shouldCapture = false; // Reset the flag after handling the capture
        }
    }

    public void CaptureAndSendView()
    {
        // Set the RenderTexture to match the screen resolution
        RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24);
        droneCam.targetTexture = rt;
        Texture2D screenShot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

        // Render the drone camera's view into the texture
        droneCam.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenShot.Apply();

        // Reset the camera target and render texture
        droneCam.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        // Encode the screenshot into PNG format
        byte[] imageBytes = screenShot.EncodeToPNG();

        // Create a unique filename using the current timestamp
        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmssfff");  // Unique time-based filename
        string fileName = "DroneView_" + nameCounter.ToString() + ".png";
        nameCounter = nameCounter + 1;

        // Start a coroutine to send the image to the server
        StartCoroutine(SendImageToServer(imageBytes, fileName));
    }

    IEnumerator SendImageToServer(byte[] imageBytes, string fileName)
    {
        WWWForm form = new WWWForm();
        form.AddBinaryData("image", imageBytes, fileName, "image/png");

        UnityWebRequest www = UnityWebRequest.Post(uploadURL, form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Image successfully uploaded: " + fileName);

            // Request the processed image after successful upload
           // StartCoroutine(RequestProcessedImage());
        }
        else
        {
            Debug.LogError("Error uploading image: " + www.error);
        }
    }

        IEnumerator NotifySimulationComplete()
    {
        UnityWebRequest www = UnityWebRequest.PostWwwForm("http://127.0.0.1:5000/simulation_complete", "");
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Notified Flask server that simulation is complete.");
        }
        else
        {
            Debug.LogError("Error notifying Flask server: " + www.error);
        }
    }
}