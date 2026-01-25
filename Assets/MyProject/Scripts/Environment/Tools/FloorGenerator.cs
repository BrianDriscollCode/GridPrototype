using UnityEngine;
using System;

public class FloorGenerator : MonoBehaviour
{
    public GameObject FloorPiece;
    public GameObject OddFloorPiece;
    public int rowAmount;
    public int columnAmount;
    // Optional spacing multiplier (editable in the Inspector)
    public Vector2 spacing = Vector2.one;

    private void Start()
    {
        if (FloorPiece == null)
        {
            Debug.LogError("FloorPiece prefab is not assigned on " + name, this);
            return;
        }

        // Try to determine piece size from Renderer or Collider; fall back to (1,1,1)
        Vector3 pieceSize = Vector3.one;
        var renderer = FloorPiece.GetComponent<Renderer>();
        if (renderer != null)
        {
            pieceSize = renderer.bounds.size;
        }
        else
        {
            var collider = FloorPiece.GetComponent<Collider>();
            if (collider != null)
                pieceSize = collider.bounds.size;
        }

        // Compute offsets using spacing multipliers (Z used for rows)
        Vector3 offset = new Vector3(pieceSize.x * spacing.x, 0f, pieceSize.z * spacing.y);

        // Center generated grid on this GameObject's position
        Vector3 origin = transform.position - new Vector3((columnAmount - 1) * offset.x * 0.5f, 0f, (rowAmount - 1) * offset.z * 0.5f);

        for (int r = 0; r < rowAmount; r++)
        {
            for (int c = 0; c < columnAmount; c++)
            {
                System.Random random = new System.Random();
                int randomNumber = random.Next(1, 4);
                GameObject chosenFloorPiece = FloorPiece;

                if (randomNumber == 3)
                {
                    chosenFloorPiece = OddFloorPiece;
                }


                Vector3 pos = origin + new Vector3(c * offset.x, 0f, r * offset.z);
                var instance = Instantiate(chosenFloorPiece, pos, Quaternion.identity, transform);
                instance.name = $"{FloorPiece.name}_{r}_{c}";
            }
        }
    }
}