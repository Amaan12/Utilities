using UnityEngine;
using TMPro;

namespace Utilities.VersionNumbering
{
    [DefaultExecutionOrder(-10)]
    public class VersionNumber : MonoBehaviour
    {
        [Header("UI Reference")]
        [Tooltip("The TextMeshPro component to display the version info. If left empty, it will try to get it on this GameObject.")]
        [SerializeField] private TMP_Text versionText;

        [Header("Toggles")]
        [SerializeField] private bool showVersion = true;
        [SerializeField] private bool showUnityVersion = false;
        [SerializeField] private bool showPlatform = false;
        [SerializeField] private bool showBuildType = false;
        [SerializeField] private bool showDeviceModel = false;
        [SerializeField] private bool showOperatingSystem = false;
        [SerializeField] private bool showDateTime = false;

        [Header("Formatting Settings")]
        [Tooltip("Prefix to display before the version info (e.g. 'v' or 'Version: ').")]
        [SerializeField] private string prefix = "v";
        
        [Tooltip("If true, each enabled option will be printed on a new line. If false, they will be separated by the Separator string.")]
        [SerializeField] private bool useNewLines = false;
        
        [Tooltip("The separator used between info elements if 'Use New Lines' is false.")]
        [SerializeField] private string separator = " | ";
        
        [Tooltip("Custom suffix to display at the very end.")]
        [SerializeField] private string customSuffix = "";

        private void Awake()
        {
            if (versionText == null)
            {
                versionText = GetComponent<TMP_Text>();
            }
        }

        private void Start()
        {
            UpdateVersionText();
        }

        [ContextMenu("Update Text Now")]
        public void UpdateVersionText()
        {
            if (versionText == null)
            {
                return;
            }

            System.Text.StringBuilder builder = new System.Text.StringBuilder();

            // Add prefix if defined
            if (!string.IsNullOrEmpty(prefix))
            {
                builder.Append(prefix);
            }

            bool hasContent = false;
            string itemSeparator = useNewLines ? "\n" : separator;

            // 1. Build/Project Version
            if (showVersion)
            {
                builder.Append(Application.version);
                hasContent = true;
            }

            // 2. Unity Version
            if (showUnityVersion)
            {
                AppendInfo(builder, ref hasContent, itemSeparator, $"Unity {Application.unityVersion}");
            }

            // 3. Platform
            if (showPlatform)
            {
                AppendInfo(builder, ref hasContent, itemSeparator, Application.platform.ToString());
            }

            // 4. Build Type (Debug / Release)
            if (showBuildType)
            {
                string buildType = Debug.isDebugBuild ? "Debug Build" : "Release Build";
                AppendInfo(builder, ref hasContent, itemSeparator, buildType);
            }

            // 5. Device Model
            if (showDeviceModel)
            {
                AppendInfo(builder, ref hasContent, itemSeparator, SystemInfo.deviceModel);
            }

            // 6. Operating System
            if (showOperatingSystem)
            {
                AppendInfo(builder, ref hasContent, itemSeparator, SystemInfo.operatingSystem);
            }

            // 7. Date & Time
            if (showDateTime)
            {
                AppendInfo(builder, ref hasContent, itemSeparator, System.DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
            }

            // Add suffix if defined
            if (!string.IsNullOrEmpty(customSuffix))
            {
                AppendInfo(builder, ref hasContent, itemSeparator, customSuffix);
            }

            versionText.text = builder.ToString();
        }

        private void AppendInfo(System.Text.StringBuilder builder, ref bool hasContent, string separatorToUse, string content)
        {
            if (string.IsNullOrEmpty(content)) return;

            if (hasContent)
            {
                builder.Append(separatorToUse);
            }
            builder.Append(content);
            hasContent = true;
        }

        private void OnValidate()
        {
            if (versionText == null)
            {
                versionText = GetComponent<TMP_Text>();
            }

            // Only update text in Editor if the component exists and is not inside a prefab asset (which can throw warnings)
            if (versionText != null && !gameObject.scene.IsValid())
            {
                // Do not update text on prefab assets, but update on prefab instances in scene or regular scene objects
            }
            else if (versionText != null)
            {
                UpdateVersionText();
            }
        }
    }
}

