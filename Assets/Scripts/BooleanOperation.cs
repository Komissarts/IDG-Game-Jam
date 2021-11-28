using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Parabox.CSG;

public class BooleanOperation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Initialize two new meshes in the scene
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.localScale = Vector3.one * 1.3f;

        // Perform boolean operation
        Model result = CSG.Subtract(cube, sphere);

        // Create a gameObject to render the result
        var composite = new GameObject();
        composite.AddComponent<MeshFilter>().sharedMesh = result.mesh;
        composite.AddComponent<MeshRenderer>().sharedMaterials = result.materials.ToArray();
    }

    
}
