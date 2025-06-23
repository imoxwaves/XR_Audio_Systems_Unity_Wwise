using UnityEngine;
using UnityEditor; // Necesario para Editor scripts

public class MeshToBoxColliderConverter : MonoBehaviour
{
    [MenuItem("Tools/Convert Selected Mesh Colliders to Box")]
    public static void ConvertSelectedMeshToBoxColliders()
    {
        // Obtener todos los objetos seleccionados
        GameObject[] selectedObjects = Selection.gameObjects;

        foreach (GameObject obj in selectedObjects)
        {
            MeshCollider meshCollider = obj.GetComponent<MeshCollider>();
            if (meshCollider != null)
            {
                // Calcular los límites (bounds) del Mesh
                Bounds bounds = obj.GetComponent<Renderer>().bounds;

                // Eliminar Mesh Collider
                DestroyImmediate(meshCollider);

                // Añadir Box Collider y ajustar su tamaño
                BoxCollider boxCollider = obj.AddComponent<BoxCollider>();
                boxCollider.center = bounds.center - obj.transform.position;
                boxCollider.size = bounds.size;
            }
        }
        Debug.Log("Conversión completada para " + selectedObjects.Length + " objetos.");
    }
}