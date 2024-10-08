using Newtonsoft.Json.Bson;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "PlayerData")]
public class PlayerData : ScriptableObject
{
    public int playerNumber;
    public float MaxHp;
}
