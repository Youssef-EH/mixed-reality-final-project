using System;
using UnityEngine;
using Button = UnityEngine.UI.Button;

namespace Harpia.CampingPack
{
    public class CampingPackCanvas : MonoBehaviour
    {

        [NonSerialized]
        private bool ShowedIconCreatorWindow;
        
        public Button iconCreatorButton;
        public Button PrefabBrushButton;
        public Button LowPolyColorChangerButton;
        public Button QuickAnimationEvents;
        
        
        public const string iconCreatorLink = "https://assetstore.unity.com/packages/tools/game-toolkits/icon-creator-generate-fast-easy-complete-icons-generator-198488?aid=1100lACye&utm_campaign=unity_affiliate&utm_medium=affiliate&utm_source=partnerize-linkmaker";
        public const string prefabBrushLink = "https://prf.hn/click/camref:1100lACye/destination:https://assetstore.unity.com/packages/tools/utilities/prefab-brush-easy-object-placement-tool-level-designer-260560";
        public const string lowPolyColorChangerLink = "https://assetstore.unity.com/packages/tools/level-design/low-poly-color-changer-easy-color-changing-variations-248562?aid=1100lACye";
        public const string quickAnimationEventsLink = "https://assetstore.unity.com/packages/tools/animation/quick-animation-events-manage-animation-events-easily-311920?clickref=1101lAncWS49&utm_source=partnerize&utm_medium=affiliate&utm_campaign=unity_affiliate";

        private void Start()
        {
            iconCreatorButton.onClick.AddListener(() => OpenLink(iconCreatorLink));
            PrefabBrushButton.onClick.AddListener(() => OpenLink(prefabBrushLink));
            LowPolyColorChangerButton.onClick.AddListener(() => OpenLink(lowPolyColorChangerLink));
            QuickAnimationEvents.onClick.AddListener(() => OpenLink(quickAnimationEventsLink));
        }

        private void OpenLink(string url) => Application.OpenURL(url);
        
    }
}