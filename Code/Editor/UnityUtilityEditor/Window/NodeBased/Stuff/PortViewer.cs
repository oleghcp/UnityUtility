using UnityEngine;

namespace UnityUtilityEditor.Window.NodeBased.Stuff
{
    internal enum PortType : byte
    {
        In,
        Out
    }

    internal class PortViewer
    {
        private const float X_OFFSET = 8f;

        private PortType _type;
        private GraphEditorWindow _window;
        private NodeViewer _node;
        private GUIStyle _style;
        private Rect _screenRect;
        private int _screenRectVersion;

        public NodeViewer Node => _node;
        public PortType Type => _type;

        public Rect ScreenRect
        {
            get
            {
                UpdateRect();
                return _screenRect;
            }
        }

        public PortViewer(NodeViewer node, PortType type, GraphEditorWindow window)
        {
            _node = node;
            _type = type;
            _window = window;
            _screenRect = new Rect(0, 0, 16f, 24f);
            _style = type == PortType.In ? GraphEditorStyles.Styles.InPort : GraphEditorStyles.Styles.OutPort;
        }

        public void Draw()
        {
            UpdateRect();

            if (GUI.Button(_screenRect, GraphEditorStyles.Styles.RightTriangle, _style))
                _window.OnClickOnPort(this);
        }

        private void UpdateRect()
        {
            if (_screenRectVersion != _window.OnGuiCounter)
            {
                Rect nodeScreenRect = _node.ScreenRect;
                _screenRect.y = nodeScreenRect.y + (nodeScreenRect.height * 0.5f) - _screenRect.height * 0.5f;

                switch (_type)
                {
                    case PortType.In:
                        _screenRect.x = nodeScreenRect.x - _screenRect.width + X_OFFSET;
                        break;

                    case PortType.Out:
                        _screenRect.x = nodeScreenRect.x + nodeScreenRect.width - X_OFFSET;
                        break;
                }

                _screenRectVersion = _window.OnGuiCounter;
            }
        }
    }
}
