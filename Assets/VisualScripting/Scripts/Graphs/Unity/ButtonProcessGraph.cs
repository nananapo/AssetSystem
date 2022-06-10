using System;
using System.Threading.Tasks;
using GraphConnectEngine;
using GraphConnectEngine.Graphs.Event;

namespace VisualScripting.Scripts.Graphs.Unity
{
    public class ButtonProcessGraph : Graph
    {
        private IProcessSender _processSender;
        
        public ButtonProcessGraph(string id, IProcessSender processSender) : base(id, false, true)
        {
            _processSender = processSender;
        }

        public override Task<ProcessCallResult> OnProcessCall(ProcessData args, object[] parameters)
        {
            return Task.FromResult(ProcessCallResult.Success(Array.Empty<object>(), OutProcessNodes[0]));
        }
        
        public async Task<bool> Update(object sender)
        {
            await _processSender.Fire(sender,this, null);
            return false;
        }

        public override string GraphName => "Button";
    }
}