#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

public class CreateAbilityObject
{
    [MenuItem("Assets/Create/MovementChangeAbility")]
    public static void CreateMyAsset()
    {
        MovementChangeAbility asset = ScriptableObject.CreateInstance<MovementChangeAbility>();

        AssetDatabase.CreateAsset(asset, "Assets/NewMovementChangeAbility.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}

#endif
