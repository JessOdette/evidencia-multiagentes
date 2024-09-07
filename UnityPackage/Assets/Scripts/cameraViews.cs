using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class CameraViews : MonoBehaviour
{
    public Camera cam; // Assign your camera in the Unity editor
    public string serverURL = "http://127.0.0.1:5000/upload_image"; // Your Flask server URL

    void Start()
    {
        CaptureAndSendView(); // Capture and send the view when the game starts
    }

    public void CaptureAndSendView()
    {
        // Set the RenderTexture to match the screen resolution
        RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24);
        cam.targetTexture = rt;
        Texture2D screenShot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

        // Render the camera's view into the texture
        cam.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenShot.Apply();

        // Reset the camera target and render texture
        cam.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        // Encode the screenshot into PNG format
        byte[] imageBytes = screenShot.EncodeToPNG();

        // Save the file locally for reference (optional)
        string cameraName = cam.gameObject.name;
        string savePath = Application.dataPath + "/" + cameraName + "_CameraView.png";
        File.WriteAllBytes(savePath, imageBytes);
        Debug.Log("Screenshot saved to: " + savePath);

        // Start a coroutine to send the image to the server
        StartCoroutine(SendImageToServer(imageBytes, cameraName + "_CameraView.png"));
    }

    IEnumerator SendImageToServer(byte[] imageBytes, string fileName)
    {
        WWWForm form = new WWWForm();
        form.AddBinaryData("image", imageBytes, fileName, "image/png");

        UnityWebRequest www = UnityWebRequest.Post(serverURL, form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Image successfully uploaded");
        }
        else
        {
            Debug.LogError("Error uploading image: " + www.error);
        }
    }
}
