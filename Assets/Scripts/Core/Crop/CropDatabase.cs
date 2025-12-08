using System.Collections.Generic;
using UnityEngine;

public class CropDatabase : MonoBehaviour
{
    public List<Crop> crops = new List<Crop>();

#if UNITY_EDITOR
    // Only runs in the Unity Editor
    private void OnValidate()
    {
        LoadCrops();
    }

    private void LoadCrops()
    {
        crops.Clear();
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:Crop", new[] { "Assets/Crops" });

        foreach (string guid in guids)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            Crop crop = UnityEditor.AssetDatabase.LoadAssetAtPath<Crop>(path);
            if (crop != null)
                crops.Add(crop);
        }
    }
#endif
}