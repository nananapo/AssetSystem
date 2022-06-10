using System.Collections.Generic;
using System.Linq;
using GraphConnectEngine.Variable;
using Newtonsoft.Json;
using UnityEngine;

namespace AssetSystem.Reference.Json
{
    [JsonObject]
    public class JsonObjectReference : IObjectReference
    {

        [JsonProperty] public string Position;

        [JsonProperty] public string Rotation;

        [JsonProperty] public string Scale;

        Vector3 IObjectReference.Position => Position.ToVector3();

        Quaternion IObjectReference.Rotation => Rotation.ToQuaternion();

        Vector3 IObjectReference.Scale => Scale.ToVector3();

        [JsonProperty]
        public List<string> ChildrenReferenceIds { get; set; }

        [JsonProperty]
        public Dictionary<string, string> LocalVariableString { get; set; }

        [JsonProperty]
        public Dictionary<string, int> LocalVariableInt{ get; set; }

        [JsonProperty]
        public Dictionary<string, float> LocalVariableFloat{ get; set; }

        [JsonProperty]
        public Dictionary<string, bool> LocalVariableBool{ get; set; }
        
        [JsonProperty]
        public Dictionary<string, string> GlobalVariableString{ get; set; }

        [JsonProperty]
        public Dictionary<string, int> GlobalVariableInt{ get; set; }

        [JsonProperty]
        public Dictionary<string, float> GlobalVariableFloat{ get; set; }

        [JsonProperty]
        public Dictionary<string, bool> GlobalVariableBool{ get; set; }
        
        public IList<string> GetChildrenReferenceIds() => ChildrenReferenceIds != null ? ChildrenReferenceIds.ToList() : new List<string>();
        
        public void InitializeGlobalVariable(IAsyncVariableHolder holder, IDictionary<string, string> changedIds)
        {
            if (GlobalVariableString != null)
            {
                foreach (var (name, value) in GlobalVariableString)
                {
                    var saveName = name;
                    foreach (var (bid,aid) in changedIds)
                    {
                        if (value.StartsWith(bid))
                        {
                            saveName = value.Substring(bid.Length) + aid;
                            break;
                        }
                    }
                    holder.CreateWithoutNotify(saveName, value);
                }
            }

            if (GlobalVariableInt != null)
            {
                foreach (var (name, value) in GlobalVariableInt)
                {
                    holder.CreateWithoutNotify(name, value);
                }
            }

            if (GlobalVariableFloat != null)
            {
                foreach (var (name, value) in GlobalVariableFloat)
                {
                    holder.CreateWithoutNotify(name, value);
                }
            }

            if (GlobalVariableBool != null)
            {
                foreach (var (name, value) in GlobalVariableBool)
                {
                    holder.CreateWithoutNotify(name, value);
                }
            }
        }
        
        public void InitializeLocalVariable(IVariableHolder holder, IDictionary<string, string> changedIds)
        {
            if (LocalVariableString != null)
            {
                foreach (var (name, value) in LocalVariableString)
                {
                    var saveName = name;
                    foreach (var (bid,aid) in changedIds)
                    {
                        if (value.StartsWith(bid))
                        {
                            saveName = value.Substring(bid.Length) + aid;
                            break;
                        }
                    }
                    holder.CreateWithoutNotify(saveName, value);
                }
            }

            if (LocalVariableInt != null)
            {
                foreach (var (name, value) in LocalVariableInt)
                    holder.CreateWithoutNotify(name, value);
            }

            if (LocalVariableFloat != null)
            {
                foreach (var (name, value) in LocalVariableFloat)
                    holder.CreateWithoutNotify(name, value);
            }

            if (LocalVariableBool != null)
            {
                foreach (var (name, value) in LocalVariableBool)
                    holder.CreateWithoutNotify(name, value);
            }
        }
    }
}