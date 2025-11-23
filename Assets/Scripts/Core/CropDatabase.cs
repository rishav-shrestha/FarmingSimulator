using UnityEditor;
using UnityEngine;

public class CropDatabase : MonoBehaviour
{
    public Crop[] crops;

    void OnValidate()
    {
        string[] guids = AssetDatabase.FindAssets("t:Crop", new[] { "Assets/Crops" });
        crops = new Crop[guids.Length];

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            crops[i] = AssetDatabase.LoadAssetAtPath<Crop>(path);
        }
        Debug.Log("Loaded " + crops.Length + " crops from Assets/Crops/");
        
    }
}
