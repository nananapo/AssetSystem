using System;
using System.Collections.Generic;
using UnityEngine;
using GraphConnectEngine.Graphs.Event;
using GraphConnectEngine.Graphs.Operator;
using GraphConnectEngine.Graphs.Statement;
using VisualScripting.Scripts.Graphs.UI;
using VisualScripting.Scripts.Graphs.UI.Event;
using VisualScripting.Scripts.Graphs.UI.Unity;
using VisualScripting.Scripts.Graphs.UI.Value;
using VisualScripting.Scripts.Graphs.Unity;
using GraphConnectEngine;
using GraphConnectEngine.Nodes;
using GraphConnectEngine.Graphs;
using GraphConnectEngine.Graphs.Value;

namespace VisualScripting.Scripts.Helper
{
    public class GraphCreator
    {

        Dictionary<string, Func<string, GraphSystem, IGraph>> simpleGraphs = new();

        Dictionary<string, GraphUI> specialGraphUIs = new();

        /// <summary>
        /// ID / Connector
        /// </summary>
        Dictionary<string, Func<string,INodeConnector,GraphSystem,IGraph>> specialGraphs = new();
/*
        public GraphCreator()
        {
            simpleGraphs["IfStatement"] = (id,_) => new IfStatementGraph(id);

            simpleGraphs["AdditionOperator"] = (id, _) => new AdditionOperatorGraph(id);

            simpleGraphs["ModulusOperator"] = (id, _) => new ModulusOperatorGraph(id);

            simpleGraphs["GreaterThanOperator"] = (id, _) => new GreaterThanOperatorGraph(id);

            simpleGraphs["EqualOperator"] = (id, _) => new EqualOperatorGraph(id);

            simpleGraphs["Vector3"] = (id, _) => new Vector3Graph(id);

            simpleGraphs["Instantiate"] = (id, _) => new InstantiateGraph(id, new GameObject());//TODO
            
            simpleGraphs["Cast<Int32>"] = (id, _) => new CastGraph<int>(id);

            simpleGraphs["Cast<Single>"] = (id, _) => new CastGraph<float>(id);

            simpleGraphs["GameObject.GetComponent<Image>"] = (id, _) => new GameObjectGetComponentImageGraph(id);

            simpleGraphs["UnityWebRequestTexture.GetTexture"] = (id, _) => new SetImageFromURLGraph(id);

            simpleGraphs["Delay"] = (id, _) => new DelayGraph(id);

            simpleGraphs["OnCollisionEnter"] = (id, parent) =>
            {
                var graph = new ParameterOutGraph(id, new[] { "Collision" }, new[] { typeof(Collision) });
                var prop = parent.gameObject.TryGetOrAddComponent<RigidbodyActionPropagator>();

                //TODO Dispose
                prop.OnCollisionEnterAction += collision => graph.InvokeWithoutCheck(ProcessData.Fire(this, parent.Connector), true, new object[] { collision });
                return graph;
            };

            simpleGraphs["OnCollisionExit"] = (id, parent) =>
            {
                var graph = new ParameterOutGraph(id, new[] { "Collision" }, new[] { typeof(Collision) });
                var prop = parent.gameObject.TryGetOrAddComponent<RigidbodyActionPropagator>();

                prop.OnCollisionExitAction += collision => graph.InvokeWithoutCheck(ProcessData.Fire(this, parent.Connector), true, new object[] { collision });
                return graph;
            };

            simpleGraphs["OnTriggerEnter"] = (id, parent) =>
            {
                var graph = new ParameterOutGraph(id, new[] { "Collider" }, new[] { typeof(Collider) });
                var prop = parent.gameObject.TryGetOrAddComponent<RigidbodyActionPropagator>();

                prop.OnTriggerEnterAction += collision => graph.InvokeWithoutCheck(ProcessData.Fire(this, parent.Connector), true, new object[] { collision });
                return graph;
            };

            simpleGraphs["OnTriggerExit"] = (id, parent) =>
            {
                var graph = new ParameterOutGraph(id, new[] { "Collider" }, new[] { typeof(Collider) });
                var prop = parent.gameObject.TryGetOrAddComponent<RigidbodyActionPropagator>();

                prop.OnTriggerExitAction += collision => graph.InvokeWithoutCheck(ProcessData.Fire(this, parent.Connector), true, new object[] { collision });
                return graph;
            };

            specialGraphUIs["FixedUpdate"] = Resources.Load<FixedUpdateGraphUI>("GraphSystem/FixedUpdateGraph");
            specialGraphUIs["ButtonImpulse"] = Resources.Load<ButtonProcessSenderGraphUI>("GraphSystem/ButtonImpulseGraph");
            specialGraphUIs["DebugText"] = Resources.Load<DebugTextGraphUI>("GraphSystem/DebugTextGraph");
            specialGraphUIs["Int"] = Resources.Load<Int32GraphUI>("GraphSystem/IntGraph");
            specialGraphUIs["Single"] = Resources.Load<SingleGraphUI>("GraphSystem/SingleGraph");
            specialGraphUIs["Boolean"] = Resources.Load<BooleanGraphUI>("GraphSystem/BooelanGraph");
            specialGraphUIs["String"] = Resources.Load<StringGraphUI>("GraphSystem/StringGraph");
            specialGraphUIs["GameObject"] = Resources.Load<GameObjectGraphUI>("GraphSystem/IntGraph");
            specialGraphUIs["GetLocalVariable"] = Resources.Load<GetVariableGraphUI>("GraphSystem/GetVariableGraph");
            specialGraphUIs["SetLocalVariable"] = Resources.Load<SetVariableGraphUI>("GraphSystem/SetVariableGraph");

            specialGraphs["FixedUpdate"] = (id, connector,_) =>
            {
                var updater = new UpdaterGraph(id, new ProcessSender(connector, 1, true));
                updater.IntervalType = UpdaterGraph.Type.Update;
                return updater;
            };

            specialGraphs["ButtonImpulse"] = (id, connector, _) =>
            {
                var updater = new UpdaterGraph(id, new AllProcessSender(connector));
                updater.IntervalType = UpdaterGraph.Type.Update;
                return updater;
            };

            specialGraphs["DebugText"] = (id, connector, _) =>
            {
                var graph = new DebugTextGraph(id);
                return graph;
            };

            specialGraphs["Int"] = (id, connector, _) =>
            {
                return new ValueFuncGraph<int>(id);

                //TODO 誰の仕事？
                await SettingVariables.Remove(ValueVariable);
            };

            specialGraphs["GameObject"] = (id,connector, system) => 
            {
                return new ValueGraph<GameObject>(id, system.gameObject);
            };
        }

        public GraphUI CreateUI(string graphName, GraphSystemMonoBehaviour parentObject, string graphId, Transform instantiateParent, Vector3 position, Quaternion rotation)
        {
            GraphUI graph;
            if (simpleGraphs.ContainsKey(graphName))
            {
                graph = simpleGraphs
            }
        }

        private GraphUI Instantiate(GraphUI prefab,Transform parent,Vector3 position,Quaternion rotation)
        {
            var obj = GameObject.Instantiate(prefab,position,rotation);

            obj.transform.parent = parent;
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.transform.localScale = Vector3.one;

            return obj;
        }
    */
        
    }
}
