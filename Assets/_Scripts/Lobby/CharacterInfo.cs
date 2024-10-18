using Fusion;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterInfo", menuName = "ScriptableObject/Character Info")]
public class CharacterInfo : ScriptableObject
{
    [Header("# Character Informations")]
    [TextArea]
    [SerializeField] private string characterName;
    [SerializeField] private string characterConcept;
    [TextArea]
    [SerializeField] private string characterStory;
    [SerializeField] private RenderTexture characterRenderTexture;
    [SerializeField] private NetworkObject characterPrefab;

    public string CharacterName { get { return characterName; } }
    public string CharacterConcept { get { return characterConcept; } }
    public string CharacterStory { get { return characterStory; } }
    public RenderTexture CharacterRenderTexture { get { return characterRenderTexture; } }
    public NetworkObject CharacterPrefab { get { return characterPrefab; } }
}
