using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AssetSystem.Unity;
using Cysharp.Threading.Tasks;
using GraphConnectEngine;
using GraphConnectEngine.Graphs;
using GraphConnectEngine.Graphs.Event;
using GraphConnectEngine.Graphs.Value;
using GraphConnectEngine.Nodes;
using VisualScripting.Scripts.Graphs.Unity;
using VisualScripting.Scripts.Helper;

namespace VisualScripting.Scripts.Graphs
{
    public static class GraphInitializer
    {

        /// <summary>
        /// id,(notnull,nullable)
        /// </summary>
        private static readonly Dictionary<string, (Func<string, INodeConnector, IManagedObject,IGraph> InstantiateGraphFunc,
            Func<string, IManagedObject,UniTask<bool>> CreateVariableFunc)> _graphs = new()
        {
            {"Updater", ((id, c,obj) => new UpdaterGraph(id, new UI.Event.ProcessSender(c, 1, true)), null)},
            {"ButtonGraph",((id,c,_)=>new ButtonProcessGraph(id,new UI.Event.ProcessSender(c, 1, true)),null)},
            {"DebugText", ((id, c,obj) => new DebugTextGraph(id),null)},
            {"ValueFunc<String>", ((id,c,obj)=>new ValueFuncGraph<string>(id, CreateVariableValueFunc<string>(obj.HiddenVariableManager,id)), async (id, managedObject) => await managedObject.HiddenVariableManager.TryCreateIfNotExist(id, "Value", ""))},
            {"GameObject", ((id,c,obj) => new GameObjectGraph(id,obj as ManagedObject),null)},
            {"GetPosition", ((id,_,_) => new GameObjectGetPositionGraph(id),null)},
            {"SetPosition", ((id,_,_) => new GameObjectSetPositionGraph(id),null)},
            {"Mathf.Sin", ((id,_,_) => new MathfSinGraph(id),null)},
            {"Mathf.Cos", ((id,_,_) => new MathfCosGraph(id),null)},
            {"Time.time", ((id,_,_) => new TimeTimeGraph(id),null)},
            {"Vector3.SetX", ((id,_,_) => new Vector3SetXGraph(id),null)},
            {"Vector3.SetY", ((id,_,_) => new Vector3SetYGraph(id),null)},
            {"Vector3.SetZ", ((id,_,_) => new Vector3SetZGraph(id),null)}
        };

        /// <summary>
        /// グラフを作成する
        /// </summary>
        /// <param name="graphId"></param>
        /// <param name="graphType"></param>
        /// <param name="connector"></param>
        /// <param name="managedObject"></param>
        /// <returns></returns>
        public static IGraph Instantiate(string graphId,string graphType,INodeConnector connector,IManagedObject managedObject)
        {
            if (!_graphs.ContainsKey(graphType))
            {
                return null;
            }
            return _graphs[graphType].InstantiateGraphFunc(graphId, connector, managedObject);
        }

        /// <summary>
        /// グラフに使う変数を作成する
        ///
        /// GraphDefinitionとかは無し
        /// </summary>
        /// <param name="graphId"></param>
        /// <param name="graphType"></param>
        /// <param name="managedObject"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static UniTask<bool> CreateVariable(string graphId, string graphType, IManagedObject managedObject)
        {
            if (!_graphs.ContainsKey(graphType))
            {
                return UniTask.FromResult(false);
            }

            if (_graphs[graphType].CreateVariableFunc == null)
            {
                return UniTask.FromResult(true);
            }

            return _graphs[graphType].CreateVariableFunc(graphId, managedObject);
        }

        /// <summary>
        /// グラフの種類をすべて取得
        /// </summary>
        /// <returns></returns>
        public static IList<string> GetGraphTypes()
        {
            return _graphs.Keys.ToList();
        }

        private static Func<Task<ValueResult<T>>> CreateVariableValueFunc<T>(IHiddenVariableManager variableManager,string graphId)
        {
            // TODO "Value"がハードコードしてしまっている
            return () => variableManager.Get<T>(graphId, "Value");
        } 
    }
}