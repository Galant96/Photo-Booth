using UnityEngine;

/* This class is responsible for defining a loaded model 
 * and providing the usable functionalities for it.  */

[System.Serializable]
public class Model
{
	[SerializeField]
	private string modelName = "Change This";

	[SerializeField]
	private string fileName = ".....obj";

	private string modelDirectory = "";

	private GameObject modelGameObject;
	public GameObject ModelGameObject { get => modelGameObject; set => modelGameObject = value; }

	[SerializeField]
	private Material modelMaterial; // Set a material for the model to be visible on the scene.
	public Material Material { get => modelMaterial; set => modelMaterial = value; }

	[SerializeField]
	private bool isEnabled = false; // Set if the model is active on the scene.

	[SerializeField]
	private float transformationSpeed = 1f;

	[SerializeField]
	private float transformationSpeedBoost = 5f;

	/// <summary>
	/// This function prepares and returns a loaded model as the game object.
	/// </summary>
	/// <param name="objectLoader"> Refference to the object loader. </param>
	/// <returns> Return a loaded model as the game object. </returns>
	public GameObject GetModelGameObject(ObjectLoader objectLoader)
	{
		// Set the file's directory
		modelDirectory = Application.dataPath + "/Input/" + fileName;

		Mesh meshModel = objectLoader.ImportFile(modelDirectory);

		PrepareGameObjectForModel(meshModel);

		return modelGameObject;
	}

	/// <summary>
	/// This function prepares the game object from the loaded mesh model.
	/// </summary>
	/// <param name="meshModel"> Refference to the loaded mesh. </param>
	private void PrepareGameObjectForModel(Mesh meshModel)
	{
		// Create a new game object.
		modelGameObject = new GameObject(modelName);

		// Set its active to false
		modelGameObject.SetActive(isEnabled);

		// Add required components to the game object
		modelGameObject.AddComponent<MeshRenderer>();

		modelGameObject.GetComponent<MeshRenderer>().material = modelMaterial;

		modelGameObject.AddComponent<MeshFilter>();

		// Assign the mesh model to game object's mesh filter
		modelGameObject.GetComponent<MeshFilter>().mesh = meshModel;
	}

	/// <summary>
	/// This function gives the ability to rotate the model.
	/// </summary>
	public void Rotate()
	{
		float speed = GetTransformationSpeed();

		Transform modelTransform = modelGameObject.transform;

		if (Input.GetKey(KeyCode.W)) // Rotate up along x axis
		{
			modelTransform.RotateAround(modelTransform.position, Vector3.right, speed);
		}

		if (Input.GetKey(KeyCode.A)) // Rotate anticlockwise along y axis
		{
			modelTransform.RotateAround(modelTransform.position, Vector3.up, speed);
		}

		if (Input.GetKey(KeyCode.S)) // Rotate down along x axis
		{
			modelTransform.RotateAround(modelTransform.position, Vector3.left, speed);
		}

		if (Input.GetKey(KeyCode.D)) // Rotate clockwise along y axis
		{
			modelTransform.RotateAround(modelTransform.position, Vector3.down, speed);
		}
	}


	public void Translate()
	{
		Transform modelTransform = modelGameObject.transform;

		float speed = GetTransformationSpeed();

		if (Input.GetKey(KeyCode.W)) // Move up
		{
			modelTransform.Translate(Vector3.up * speed * Time.deltaTime, Space.World);
		}

		if (Input.GetKey(KeyCode.A)) // Move left
		{
			modelTransform.Translate(Vector3.left * speed * Time.deltaTime, Space.World);
		}

		if (Input.GetKey(KeyCode.S)) // Move down
		{
			modelTransform.Translate(Vector3.down * speed * Time.deltaTime, Space.World);
		}

		if (Input.GetKey(KeyCode.D)) // Move right
		{
			modelTransform.Translate(Vector3.right * speed * Time.deltaTime, Space.World);
		}

		if (Input.GetKey(KeyCode.E)) // Move forward
		{
			modelTransform.Translate(Vector3.forward * speed * Time.deltaTime, Space.World);
		}

		if (Input.GetKey(KeyCode.C)) // Move backward
		{
			modelTransform.Translate(Vector3.back * speed * Time.deltaTime, Space.World);
		}

	}

	/// <summary>
	/// This function is responsible for manipulating of the model's transformation speed
	/// </summary>
	/// <returns> Return the transformation speed. </returns>
	private float GetTransformationSpeed()
	{
		float speed = transformationSpeed;

		// Hold the left shift key to boost the speed of transformation.
		if (Input.GetKey(KeyCode.LeftShift))
		{
			speed = transformationSpeedBoost;
		}
		else
		{
			speed = transformationSpeed;
		}

		return speed;
	}

	/// <summary>
	/// This function is used to set this very model active on the scene.
	/// </summary>
	/// <param name="isActive"> Pass true or false to set the model active or not. </param>
	public void SetModelActive(bool isActive)
	{
		isEnabled = isActive;
		modelGameObject.SetActive(isEnabled);
	}

	/// <summary>
	/// This function resets the model to the original position.
	/// </summary>
	public void ResetModelToOriginalPosition()
	{
		modelGameObject.transform.position = Vector3.zero;
		modelGameObject.transform.rotation = Quaternion.identity;
	}

	/// <summary>
	/// This function resets the model rotation.
	/// </summary>
	public void ResetModelRotation()
	{
		modelGameObject.transform.rotation = Quaternion.identity;
	}
}
