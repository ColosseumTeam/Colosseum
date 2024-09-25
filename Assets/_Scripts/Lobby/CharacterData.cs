using Fusion;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "ScriptableObject/Character Data")]
public class CharacterData : ScriptableObject
{
    [Header("# Character Explanation")]
    [TextArea]
    [SerializeField] private string characterDesciption;
    [SerializeField] private string characterName;
    [SerializeField] private string characterConcept;
    [TextArea]
    [SerializeField] private string characterStory;
    [SerializeField] private NetworkObject characterPrefab;

    public string CharacterDesciption { get { return characterDesciption; } }
    public string CharacterName { get { return characterName; } }
    public string CharacterConcept { get { return characterConcept; } }
    public string CharacterStory { get { return characterStory; } }
    public NetworkObject CharacterPrefab { get { return characterPrefab; } }
}
