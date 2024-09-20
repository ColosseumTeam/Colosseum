using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "SceneData")]
public class SceneData : ScriptableObject
{
    // 0 = Fighter / 1 = Ranger
    public int winPlayer;
}
