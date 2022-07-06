#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

public class CreateHealingPotion
{
    [MenuItem("Assets/Create/Item/HealingPotion")]
    public static void CreateMyAsset()
    {
        HealingPotion asset = ScriptableObject.CreateInstance<HealingPotion>();

        AssetDatabase.CreateAsset(asset, EditorUtilities.GetSelectedPathOrFallback() + "/NewHealingPotion.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}

public class CreateLongevityPotion
{
    [MenuItem("Assets/Create/Item/LongevityPotion")]
    public static void CreateMyAsset()
    {
        LongevityPotion asset = ScriptableObject.CreateInstance<LongevityPotion>();

        AssetDatabase.CreateAsset(asset, EditorUtilities.GetSelectedPathOrFallback() + "/NewLongevityPotion.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}

public class CreateStatsChangePotion
{
    [MenuItem("Assets/Create/Item/StatsChangingPotion")]
    public static void CreateMyAsset()
    {
        HealingPotion asset = ScriptableObject.CreateInstance<HealingPotion>();

        AssetDatabase.CreateAsset(asset, EditorUtilities.GetSelectedPathOrFallback() + "/NewStatsChangingPotion.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}

public class CreateCollectableItem
{
    [MenuItem("Assets/Create/Item/Collectable")]
    public static void CreateMyAsset()
    {
        CollectableItem asset = ScriptableObject.CreateInstance<CollectableItem>();

        AssetDatabase.CreateAsset(asset, EditorUtilities.GetSelectedPathOrFallback() + "/NewCollectable.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}

#endif
