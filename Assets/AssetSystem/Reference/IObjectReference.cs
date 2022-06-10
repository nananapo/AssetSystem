using System.Collections.Generic;
using GraphConnectEngine.Variable;
using UnityEngine;

namespace AssetSystem.Reference
{
    public interface IObjectReference
    {

        public Vector3 Position { get; }

        public Quaternion Rotation { get; }
        
        public Vector3 Scale { get; }
        
        public IList<string> GetChildrenReferenceIds();

        /// <summary>
        /// 渡されたVariableHolderを初期化する
        /// </summary>
        /// <param name="holder"></param>
        /// <param name="changedIds"></param>
        public void InitializeGlobalVariable(IAsyncVariableHolder holder,IDictionary<string,string> changedIds);

        /// <summary>
        /// 渡されたVariableHolderを初期化する
        /// </summary>
        /// <param name="holder"></param>
        /// <param name="changedIds"></param>
        public void InitializeLocalVariable(IVariableHolder holder,IDictionary<string,string> changedIds);
    }
}