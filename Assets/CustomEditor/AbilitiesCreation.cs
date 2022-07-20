#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

public class CreateMovementChangeAbility
{
    [MenuItem("Assets/Create/Ability/MovementChangeAbility")]
    public static void CreateMyAsset()
    {
        MovementChangeAbility asset = ScriptableObject.CreateInstance<MovementChangeAbility>();

        AssetDatabase.CreateAsset(asset, EditorUtilities.GetSelectedPathOrFallback() + "/NewMovementChangeAbility.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}

public class CreateProjectileChangeAbility
{
    [MenuItem("Assets/Create/Ability/ProjectileChangeAbility")]
    public static void CreateMyAsset()
    {
        ProjectileChangeAbility asset = ScriptableObject.CreateInstance<ProjectileChangeAbility>();

        AssetDatabase.CreateAsset(asset, EditorUtilities.GetSelectedPathOrFallback() + "/NewProjectileChangeAbility.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}

public class CreateStatChangeAbility
{
    [MenuItem("Assets/Create/Ability/StatChangeAbility")]
    public static void CreateMyAsset()
    {
        StatsChangeAbility asset = ScriptableObject.CreateInstance<StatsChangeAbility>();

        AssetDatabase.CreateAsset(asset, EditorUtilities.GetSelectedPathOrFallback() + "/NewStatChangeAbility.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}

#endif
