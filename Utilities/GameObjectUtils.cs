using UnityEngine;

namespace GoAwayMonkeys.Utilities;

public static class GameObjectUtils
{
    public static GameObject? GetObject(string name)
    {
        foreach (GameObject go in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (go.name.ToLower() == name.ToLower()) return go;
        }
        Plugin.Log.WriteLine("GetObject failed");
        return null;
    }
}
