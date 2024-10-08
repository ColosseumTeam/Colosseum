using Newtonsoft.Json.Bson;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObject/Player Data")]
public class PlayerData : ScriptableObject
{
    [SerializeField] private AudioClip leftStep;
    [SerializeField] private AudioClip rightStep;

    public int playerNumber;
    public float MaxHp;

    public AudioClip LeftStep { get { return leftStep; } }
    public AudioClip RightStep { get { return rightStep; } }
}
