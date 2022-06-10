using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using GraphConnectEngine;
using GraphConnectEngine.Graphs.Event;
using GraphConnectEngine.Nodes;
using MyWorldHub.Shared;

namespace VisualScripting.Scripts.Graphs.Unity
{
    /// <summary>
    /// Rootでのみ生成可能
    /// </summary>
    public class SpawnEventGraph : Graph
    {

        private readonly IProcessSender _processSender;
        
        public SpawnEventGraph(string id,IProcessSender sender) : base(id,false,true)
        {
            var resolverPlayer = new ItemTypeResolver(typeof(OperationalPlayer), "Player");
            AddNode(new OutItemNode(this, resolverPlayer, 0));
            _processSender = sender;
        }

        public async UniTask Update(object sender,OperationalPlayer player)
        {
            await _processSender.Fire(sender,this, new object[]{player});
        }

        /// <summary>
        /// parameters[0]にOperationalPlayerを格納して呼ぶ
        /// </summary>
        /// <param name="args"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override Task<ProcessCallResult> OnProcessCall(ProcessData args, object[] parameters)
        {
            return Task.FromResult(ProcessCallResult.Success(new []
            {
                parameters[0]
            }, OutProcessNodes[0]));
        }

        public override string GraphName => "SpawnEvent";
    }
}