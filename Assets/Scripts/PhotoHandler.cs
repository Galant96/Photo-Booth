using UnityEngine;

[RequireComponent(typeof(Camera))] // Use only if it is a camera.
public class PhotoHandler : MonoBehaviour
{
	private static PhotoHandler Insatnce;

	[SerializeField]
	private Camera screenshotCamera = null;

	private bool takePictureOnNextFrame = false; // If yes take a picture

	private void Awake()
	{
		// Set the instance.
		Insatnce = this;
		screenshotCamera = GetComponent<Camera>();
	}

	/// <summary>
	/// OnPostRender is called after a camera finished rendering the Scene. 
	/// Accordingly, taking the picture is performed at the end of a frame.
	/// </summary>
	private void OnPostRender()
	{
		if (takePictureOnNextFrame) // Proceed if the flag is set.
		{
			// Reset the flag.
			takePictureOnNextFrame = false;

			// Get the render texture.
			RenderTexture renderTexture = screenshotCamera.targetTexture;

			// Store the data of a rendered picture. Make it transparent by using ARGB32 format and do not use a bitmap.
			Texture2D renderPicture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
			// Create a rectangel to save the picture
			Rect rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
			renderPicture.ReadPixels(rect, 0, 0);

			// Read bytes and encode to a PNG.
			byte[] byteArray = renderPicture.EncodeToPNG();
			string fileName = GetPictureName(renderTexture.width, renderTexture.height);
			// Save the file.
			System.IO.File.WriteAllBytes(fileName, byteArray);
			Debug.Log("The picture has been taken!");

			// Release a temporary render texture allocated to the camera.
			RenderTexture.ReleaseTemporary(renderTexture);
			screenshotCamera.targetTexture = null;

			ProgramManager.DisplayTakingPhotoPanel(1f);

			// Refresh the folder to see the updated conent.
			UnityEditor.AssetDatabase.Refresh();
		}
	}

	/// <summary>
	/// This function sets a name for the photo and returns the path of the folder where photos are stored.
	/// </summary>
	/// <param name="width"> The resolution width of the picture. </param>
	/// <param name="height"> The resolution height of the picture. </param>
	/// <returns> Get the path of the folder where the photo will be stored. </returns>
	string GetPictureName(int width, int height)
	{
		return string.Format("{0}/Output/photo_{1}x{2}_{3}.png",
			Application.dataPath,
			width,
			height,
			System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")); // Add date and time when the picture was taken
	}

	/// <summary>
	/// Initialise taking a picture of the arranged scene;
	/// </summary>
	/// <param name="width"> The resolution width of the picture. </param>
	/// <param name="height"> The resolution height of the picture. </param>
	private void InitialiseTakingPicture(int width, int height)
	{
		//	Add a temporary render texture for storing the camera's view.
		screenshotCamera.targetTexture = RenderTexture.GetTemporary(width, height, 16);

		takePictureOnNextFrame = true;
	}

	/// <summary>
	/// This function is static to allow calling it from the other scripts. 
	/// Its main function is to start the taking photo process.
	/// </summary>
	/// <param name="width"> The resolution width of the picture. </param>
	/// <param name="height"> The resolution height of the picture. </param>
	public static void TakePicture_Static(int width, int height)
	{
		Insatnce.InitialiseTakingPicture(width, height);
	}

	/// <summary>
	/// This function is used to taking a picture on the button click event.
	/// </summary>
	public void TakePictureOnClick()
	{
		InitialiseTakingPicture(Screen.width, Screen.height);
	}
}
