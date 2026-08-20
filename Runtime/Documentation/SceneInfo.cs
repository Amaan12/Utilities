using UnityEngine;
using VInspector;

namespace Utilities.Documentation
{
    /// <summary>
    /// Scene-level documentation component.
    /// 
    /// Recommended Usage:
    /// - Create an empty GameObject at scene root
    /// - Name it "_SCENE_INFO" or "_README"
    /// - Attach this component
    /// 
    /// Purpose:
    /// - Explain scene purpose
    /// - Document gameplay flow
    /// - Leave setup instructions
    /// - Track known issues
    /// - Help onboarding for other developers
    /// 
    /// This component contains no runtime logic.
    /// </summary>
    public class SceneInfo : MonoBehaviour
    {
        [TextArea(3, 10)]
        public string sceneDescription = @"Describe the purpose of this scene, its role in the game, and any relevant context.";

        [Header("Known Issues")]
        [TextArea(3, 10)]
        public string knownIssues = @"List bugs, limitations, or temporary hacks.";
        
        [Header("TODO")]
        [TextArea(3, 10)]
        public string todo = @"Future improvements or pending tasks.";
    }
}