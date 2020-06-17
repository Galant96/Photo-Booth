using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramManager : MonoBehaviour
{
	// Available modes
	private enum Mode
	{
		rotation,
		translation
	}

	[SerializeField, Header("The mode in which the user is operating now.")]
	Mode currentMode = new Mode();

	public static ProgramManager Instance { get; private set; }

	private ObjectLoader objectLoader = null;

	[SerializeField]
	private GameObject helpPanel = null;

	[SerializeField]
	private GameObject takingPhotoPanel = null;

	private bool isProgramPaused = true; // Set true if the program is paused.

	[SerializeField]
	public List<Model> models; // Store the loaded models.

	int currentModelIndex = 0; // Store the current mode.

	private void Awake()
	{
		// Set up Singelton pattern.
		// If instance of the object is null then keep the previous object,
		// else destroy the new object and keep "the old" one.
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	// Start is called before the first frame update
	private void Start()
    {
		takingPhotoPanel.SetActive(false);

		// Get the object loader
		objectLoader = GetComponentInChildren<ObjectLoader>();

		// Set the current mode.
		currentMode = Mode.translation;

		// Load and assign .obj files to models.
		foreach (Model model in models)
		{
			model.ModelGameObject = model.GetModelGameObject(objectLoader);
		}

		// At the beginning, generate a random model from the collection and display it.
		currentModelIndex = Random.Range(0, models.Count);
		models[currentModelIndex].SetModelActive(true);
	}

    // Update is called once per frame
    private void Update()
	{
		// Allow for user input if the program is not paused.
		if (isProgramPaused != true)
		{
			HandleUserInput();

			// Check the current mode.
			if (currentMode == Mode.rotation)
			{
				models[currentModelIndex].Rotate();
			}

			if (currentMode == Mode.translation)
			{
				models[currentModelIndex].Translate();
			}
		}
	}

	/// <summary>
	/// This function handles user input.
	/// </summary>
	private void HandleUserInput()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			// Call static function from PhotoHandler script, which is responsible for taking pictures.
			PhotoHandler.TakePicture_Static(Screen.width, Screen.height);
		}

		if (Input.GetKeyDown(KeyCode.R)) // Rotate.
		{
			currentMode = Mode.rotation;
		}

		if (Input.GetKeyDown(KeyCode.T)) // Translate.
		{
			currentMode = Mode.translation;
		}

		if (Input.GetKeyDown(KeyCode.N)) // Load next model.
		{
			LoadNextModel();
		}

		if (Input.GetKeyDown(KeyCode.P)) // Load previous model.
		{
			LoadPreviousModel();
		}

		if (Input.GetKeyDown(KeyCode.H)) // Display help panel.
		{
			DisplayHelpPanel(true);
		}

		if (Input.GetKeyDown(KeyCode.U)) // Reset rotation.
		{
			models[currentModelIndex].ResetModelRotation();
		}
	}

	/// <summary>
	/// This function is used to change the mode.
	/// </summary>
	public void ChangeMode()
	{
		if (currentMode == Mode.translation)
		{
			currentMode = Mode.rotation;
		}
		else
		{
			currentMode = Mode.translation;
		}
	}

	/// <summary>
	/// This function loads the next model from the collection.
	/// </summary>
	public void LoadNextModel()
	{
		// Deactivate the current model.
		models[currentModelIndex].SetModelActive(false);
		models[currentModelIndex].ResetModelToOriginalPosition();

		// Go to the index of a next model.
		currentModelIndex++;

		// Make sure that a new index value is not over the size of the collection.
		if (currentModelIndex > models.Count - 1)
		{
			currentModelIndex = 0;
		}

		// Active a new model.
		models[currentModelIndex].SetModelActive(true);
	}

	/// <summary>
	/// This function loads the previous model from the collection.
	/// </summary>
	public void LoadPreviousModel()
	{
		// Deactivate the current model.
		models[currentModelIndex].SetModelActive(false);
		models[currentModelIndex].ResetModelToOriginalPosition();

		// Go to the index of a previous model.
		currentModelIndex--;

		// Make sure that new index value is not over the minimal limit of the collection.
		if (currentModelIndex < 0)
		{
			currentModelIndex = models.Count - 1;
		}

		// Active a new model.
		models[currentModelIndex].SetModelActive(true);
	}

	/// <summary>
	/// This function is used to active or deactive the help panel.
	/// </summary>
	/// <param name="isDisplayed"> Pass true or false to display the panel. </param>
	public void DisplayHelpPanel(bool isDisplayed)
	{
		if (helpPanel != null)
		{
			isProgramPaused = isDisplayed;
			helpPanel.SetActive(isDisplayed);
		}
		else
		{
			Debug.Log("The help panel is not assigned!");
		}
	}

	public static void DisplayTakingPhotoPanel(float time)
	{
		Instance.StartCoroutine(Instance.DisplayTakingPhotoPanel_Coroutine(time));
	}

	private IEnumerator DisplayTakingPhotoPanel_Coroutine(float time)
	{
		takingPhotoPanel.SetActive(true);
		yield return new WaitForSeconds(time);
		takingPhotoPanel.SetActive(false);
	}

}
