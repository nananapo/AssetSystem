using UnityEngine;
using VisualScripting.Scripts.Graphs.Parts;
using VisualScripting.Scripts.Graphs.UI;
using VisualScripting.Scripts.Graphs.UI.Event;
using VisualScripting.Scripts.Graphs.UI.Unity;
using VisualScripting.Scripts.Graphs.UI.Value;

namespace VisualScripting.Scripts.Helper
{
    public static class GraphUIDependency
    {

        private static FixedUpdateGraphUI _updateGraphUIPrefab;
        public static FixedUpdateGraphUI UpdateGraphUIPrefab => _updateGraphUIPrefab ??= Resources.Load<FixedUpdateGraphUI>("GraphSystem/FixedUpdateGraph");

        private static ButtonProcessSenderGraphUI _buttonProcessSenderGraph;
        public static ButtonProcessSenderGraphUI ButtonGraphPrefab => _buttonProcessSenderGraph ??= Resources.Load<ButtonProcessSenderGraphUI>("GraphSystem/ButtonGraph");

        private static DebugTextGraphUI _debugTextGraphUIPrefab;
        public static DebugTextGraphUI DebugTextGraphUIPrefab => _debugTextGraphUIPrefab ??= Resources.Load<DebugTextGraphUI>("GraphSystem/DebugTextGraph");

        private static StringGraphUI _stringGraphUIPrefab;
        public static StringGraphUI StringGraphUIPrefab => _stringGraphUIPrefab ??= Resources.Load<StringGraphUI>("GraphSystem/StringGraph");

        private static SimpleGraphUI _simpleGraphUI;
        public static SimpleGraphUI SimpleGraphUIPrefab => _simpleGraphUI ??= Resources.Load<SimpleGraphUI>("GraphSystem/SimpleGraph");

        private static ProcessStreamer _processStreamerPrefab;
        public static ProcessStreamer ProcessStreamerPrefab => _processStreamerPrefab ??= Resources.Load<ProcessStreamer>("GraphSystem/Parts/ProcessStreamer");
        
        private static ItemStreamer _itemStreamerPrefab;
        public static ItemStreamer ItemStreamerPrefab => _itemStreamerPrefab ??= Resources.Load<ItemStreamer>("GraphSystem/Parts/ItemStreamer");
    }
}