using UnityEngine;

namespace Utilities.Documentation
{
    [AddComponentMenu("Documentation/Game Object Info")]
    public class GameObjectInfo : MonoBehaviour
    {
#if UNITY_EDITOR
        [TextArea(8, 10)]
        [SerializeField] string notes = "Describe what this GameObject is responsible for.";
#endif
    }
}