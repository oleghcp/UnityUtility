#if !UNITY_2019_2_OR_NEWER || INCLUDE_UNITY_UI
using UnityEngine.UI;
using UnityEngine;

#pragma warning disable CS0649
namespace UnityUtility.UI
{
    [DisallowMultipleComponent]
    public class TabSelectorSubscribable : AbstractTabSelector
    {
        [SerializeField]
        private Button _button;

        protected override void OnAwake()
        {
            if (_button != null)
                _button.onClick.AddListener(OnClick);
        }

        public void OnButtonClick()
        {
            OnClick();
        }
    }
}
#endif
