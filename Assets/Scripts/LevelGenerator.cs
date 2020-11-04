using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour{

    public LayerMask solidMask;
    public float blockSize = 3f;

    [Header("Terrain")]
    public Texture2D terrainMap;
    public PixelToObject[] pixelToTerrainMappings;
    private Dictionary<Color, GameObject> colorToTerrainMap = new Dictionary<Color, GameObject>();

    [Header("Objects")]
    public Texture2D objectMap;
    public PixelToObject[] pixelToObjectMappings;
    private Dictionary<Color, GameObject> colorToObjectMap = new Dictionary<Color, GameObject>();

    private Color pixelColor;

    private void Awake() {
        buildMap(pixelToTerrainMappings, colorToTerrainMap);
        buildMap(pixelToObjectMappings, colorToObjectMap);
        GenerateTerrain();
        GenerateObjects();
    }

    private void buildMap(PixelToObject[] mappings, Dictionary<Color, GameObject> map) {
        // Put the pixel to object mappings into a dictionary for nicer efficiency.
        foreach (PixelToObject pto in mappings) {
            // If the color is clear, log error and ignore.
            if (pto.color.a < 1) {
                Debug.LogError("Color " + pto.color + " is not opaque. Its mapping to " + pto.prefab.name + " will be ignored!");
                continue;
            }

            // If the color is already in the dictionary, log error and ignore.
            if (map.ContainsKey(pto.color)) {
                Debug.LogError("Color " + pto.color + " is already registered. Its mapping to " + pto.prefab.name + " will be ignored!");
                continue;
            }

            map.Add(pto.color, pto.prefab);
        }
    }

    private void GenerateTerrain() {
        for (int i = 0; i < terrainMap.width; i++) {
            for (int j = 0; j < terrainMap.height; j++) {
                pixelColor = terrainMap.GetPixel(i, j);
                // If pixel is not opaque, skip, it's empty.
                if (pixelColor.a < 1) {
                    continue;
                }

                if (!colorToTerrainMap.ContainsKey(pixelColor)) {
                    Debug.LogWarning("Color " + pixelColor + " found in terrain map but it has no prefab mapped to it. Ignoring.");
                    continue;
                }

                Instantiate(colorToTerrainMap[pixelColor], new Vector3(i * blockSize, 0, j * blockSize), Quaternion.identity, transform);
            }
        }
    }

    private void GenerateObjects() {
        for (int i = 0; i < objectMap.width; i++) {
            for (int j = 0; j < objectMap.height; j++) {
                pixelColor = objectMap.GetPixel(i, j);
                // If pixel is not opaque, skip, it's empty.
                if (pixelColor.a < 1) {
                    continue;
                }

                if (!colorToObjectMap.ContainsKey(pixelColor)) {
                    Debug.LogWarning("Color " + pixelColor + " found in object map but it has no prefab mapped to it. Ignoring.");
                    continue;
                }

                // Determine where the floor is by shooting a raycast from above downwards.
                Vector3 position = new Vector3(i * blockSize, 50, j * blockSize);
                RaycastHit hit = Helpers.RaycastWithDebug(position, Vector3.down, 100f, solidMask);
                if (hit.collider == null) {
                    Debug.LogWarning("No terrain detected for object at " + i + "," + j + ". Ignoring.");
                    continue;
                }
                position.y = hit.point.y;
                Instantiate(colorToObjectMap[pixelColor], position, Quaternion.identity, transform);
            }
        }
    }

    [System.Serializable]
    public class PixelToObject {
        public Color color;
        public GameObject prefab;
    }
}
