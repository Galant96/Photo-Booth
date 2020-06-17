using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class ObjectLoader : MonoBehaviour
{
	/// <summary>
	///  This is a structure for defining a mesh structure.
	/// </summary>
	private struct meshStruct
	{
		public string fileName;
		public Vector3[] vertices;
		public Vector3[] normals;
		public Vector2[] uv;
		public int[] triangles;
		public Vector3[] faceData;
	}

	// Use this for initialization
	/// <summary>
	/// This function allows to import data from the file and retutn its Mesh value.
	/// </summary>
	/// <param name="filePath"> The path where a file is stored. </param>
	/// <returns> Return a Mesh data by reference to the provided file.  </returns>
	public Mesh ImportFile(string filePath)
	{
		meshStruct newMesh = createMeshStruct(filePath);
		addMeshStruct(newMesh);

		Vector3[] newVerts = new Vector3[newMesh.faceData.Length];
		Vector2[] newUVs = new Vector2[newMesh.faceData.Length];
		Vector3[] newNormals = new Vector3[newMesh.faceData.Length];

		int i = 0;
		// The following foreach loops through the facedata and assigns the appropriate vertex, uv, or normal
        // for the appropriate Unity mesh array.
		foreach (Vector3 v in newMesh.faceData)
		{
			newVerts[i] = newMesh.vertices[(int)v.x - 1];
			if (v.y >= 1)
				newUVs[i] = newMesh.uv[(int)v.y - 1];

			if (v.z >= 1)
				newNormals[i] = newMesh.normals[(int)v.z - 1];
			i++;
		}

		Mesh mesh = new Mesh();

		mesh.vertices = newVerts;
		mesh.uv = newUVs;
		mesh.normals = newNormals;
		mesh.triangles = newMesh.triangles;

		mesh.RecalculateBounds();
		mesh.Optimize();

		return mesh;
	}

	/// <summary>
	/// Find out the number of particular members for a given structure and return this structure.
	/// </summary>
	/// <param name="filename"> The path where a file is stored. </param>
	/// <returns> Return a structure with its updated members. </returns>
	private static meshStruct createMeshStruct(string filename)
	{
		int triangles = 0;
		int vertices = 0;
		int vt = 0;
		int vn = 0;
		int face = 0;

		meshStruct mesh = new meshStruct();
		mesh.fileName = filename;
		StreamReader stream = File.OpenText(filename);
		string entireText = stream.ReadToEnd();
		stream.Close();

		// The C# using statement defines a boundary for the object outside of which, the object is automatically destroyed. 
		using (StringReader reader = new StringReader(entireText))
		{
			string currentText = reader.ReadLine();
			char[] splitIdentifier = { ' ' };
			string[] brokenString;
			while (currentText != null)
			{
				if (!currentText.StartsWith("f ") && !currentText.StartsWith("v ") && !currentText.StartsWith("vt ")
					&& !currentText.StartsWith("vn "))
				{
					currentText = reader.ReadLine();

					if (currentText != null)
					{
						currentText = currentText.Replace("  ", " ");
					}
				}
				else
				{
					currentText = currentText.Trim();                           //Trim the current line
					brokenString = currentText.Split(splitIdentifier, 50);      //Split the line into an array, separating the original line by blank spaces

					switch (brokenString[0])
					{
						case "v": // Vertices
							vertices++;
							break;
						case "vt": // Texture vertices
							vt++;
							break;
						case "vn": // Vertex normals
							vn++;
							break;
						case "f": // Faces
							face = face + brokenString.Length - 1;
							triangles = triangles + 3 * (brokenString.Length - 2); /* brokenString.Length is 3 or greater since a face must have at least
                                                                                     3 vertices.  For each additional vertice, there is an additional
                                                                                     triangle in the mesh (hence this formula).*/
							break;
					}

					currentText = reader.ReadLine();

					if (currentText != null)
					{
						currentText = currentText.Replace("  ", " ");
					}
				}
			}
		}
		mesh.triangles = new int[triangles];
		mesh.vertices = new Vector3[vertices];
		mesh.uv = new Vector2[vt];
		mesh.normals = new Vector3[vn];
		mesh.faceData = new Vector3[face];
		return mesh;
	}

	/// <summary>
	/// Assign read values to the particular elements of the members of the passed struct.
	/// </summary>
	/// <param name="mesh"> The structure, whose members store required for Mesh data. </param>
	private static void addMeshStruct(meshStruct mesh)
	{
		StreamReader stream = File.OpenText(mesh.fileName);
		string entireText = stream.ReadToEnd();
		stream.Close();

		using (StringReader reader = new StringReader(entireText))
		{
			string currentText = reader.ReadLine();

			char[] splitIdentifier = { ' ' };
			char[] splitIdentifier2 = { '/' };
			string[] brokenString;
			string[] brokenBrokenString;

			int f = 0;
			int f2 = 0;
			int v = 0;
			int vn = 0;
			int vt = 0;

			while (currentText != null)
			{
				if (!currentText.StartsWith("f ") && !currentText.StartsWith("v ") && !currentText.StartsWith("vt ") &&
					!currentText.StartsWith("vn ") && !currentText.StartsWith("g ") && !currentText.StartsWith("usemtl ") &&
					!currentText.StartsWith("mtllib ") && !currentText.StartsWith("vt1 ") && !currentText.StartsWith("vt2 ")
					&& !currentText.StartsWith("usemap "))
				{
					currentText = reader.ReadLine();
					if (currentText != null)
					{
						currentText = currentText.Replace("  ", " ");
					}
				}
				else
				{
					currentText = currentText.Trim();
					brokenString = currentText.Split(splitIdentifier, 50);

					switch (brokenString[0])
					{
						case "g": // Group name
							break;
						case "usemtl": // Material name
							break;
						case "usemap": // Texture map name
							break;
						case "mtllib": // Material library
							break;

						case "v": // Geometric vertices
							mesh.vertices[v] = new Vector3(System.Convert.ToSingle(brokenString[1]), System.Convert.ToSingle(brokenString[2]),
													 System.Convert.ToSingle(brokenString[3]));
							v++;
							break;
						case "vt": // Texture vertices
							mesh.uv[vt] = new Vector2(System.Convert.ToSingle(brokenString[1]), System.Convert.ToSingle(brokenString[2]));
							vt++;
							break;
						case "vn": // Vertex normals
							mesh.normals[vn] = new Vector3(System.Convert.ToSingle(brokenString[1]), System.Convert.ToSingle(brokenString[2]),
													System.Convert.ToSingle(brokenString[3]));
							vn++;
							break;
						case "f": // Faces

							int j = 1;
							List<int> intArray = new List<int>();

							while (j < brokenString.Length && ("" + brokenString[j]).Length > 0)
							{
								Vector3 temp = new Vector3();
								brokenBrokenString = brokenString[j].Split(splitIdentifier2, 3);    // Separate the face into individual components (vert, uv, normal)
								temp.x = System.Convert.ToInt32(brokenBrokenString[0]);
								if (brokenBrokenString.Length > 1)                                  // Some .obj files skip UV and normal
								{
									if (brokenBrokenString[1] != "")                                    // Some .obj files skip the uv and not the normal
									{
										temp.y = System.Convert.ToInt32(brokenBrokenString[1]);
									}
									temp.z = System.Convert.ToInt32(brokenBrokenString[2]);
								}
								j++;

								mesh.faceData[f2] = temp;
								intArray.Add(f2);
								f2++;
							}

							j = 1;

							while (j + 2 < brokenString.Length)     // Create triangles out of the face data.  There will generally be more than 1 triangle per face.
							{
								mesh.triangles[f] = intArray[0];
								f++;
								mesh.triangles[f] = intArray[j];
								f++;
								mesh.triangles[f] = intArray[j + 1];
								f++;

								j++;
							}
							break;
					}

					currentText = reader.ReadLine();

					if (currentText != null)
					{
						currentText = currentText.Replace("  ", " ");       // Some .obj files insert double spaces, this removes them.
					}
				}
			}
		}
	}
}
