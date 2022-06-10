using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AssetSystem.Resource;
using AssetSystem.Unity;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace AssetSystem.Reference.Json
{
    public class JsonReferenceConverter : IReferenceConverter
    {
        UniTask<(bool result, IBundleDescription data)> IReferenceConverter.DecodeWorldDescription(string name,string bundleId,string authorId,long createdTime,long updatedTime)
        {
            return UniTask.FromResult((true,(IBundleDescription)new JsonBundleDescription
            {
                Name = name,
                BundleId = bundleId,
                AuthorId = authorId,
                CreatedTime = createdTime,
                UpdatedTime = updatedTime
            }));
        }

        UniTask<(bool result, IMasterBundle data)> IReferenceConverter.DecodeMasterBundle(string data)
        {
            try
            {
                var reference = JsonConvert.DeserializeObject<JsonMasterBundle>(data);
                return UniTask.FromResult((reference != null, (IMasterBundle) reference));
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return UniTask.FromResult<(bool result, IMasterBundle data)>((false, null));
            }
        }

        // TODO 安全な辞書を使う 
        public (bool result, IResourceReference data) ConvertPropertyToResource(IPropertyReference reference)
        {
            var type = reference.GetPropertyType();
            var properties = reference.GetProperties();

            if (type == "PhysicMaterial")
            {
                return (true,new PhysicMaterialResource
                {
                    Name            = properties["Name"],
                    DynamicFriction = float.Parse(properties["DynamicFriction"]),
                    StaticFriction  = float.Parse(properties["StaticFriction"]),
                    Bounciness      = float.Parse(properties["Bounciness"]),
                    FrictionCombine = int.Parse(properties["FrictionCombine"]),
                    BounceCombine   = int.Parse(properties["BounceCombine"]),
                });
            }
            
            return (false,null);
        }

        public async Task<IMasterBundle> CreateMasterBundleAsync(IManagedObject rootObject)
        {
            var objects = new Dictionary<string, JsonObjectReference>();
            var resources = new Dictionary<string, JsonPropertyReference>();
            var rootId = await PackReferenceAsync(rootObject, objects, resources);

            return new JsonMasterBundle
            {
                MainReferenceId = rootId,
                Objects = objects,
                Resources = resources
            };
        }

        Task<IBundleDescription> IReferenceConverter.CreateBundleDescription(string name,string bundleId,string authorId,long createdTime,long updatedTime)
        {
            return Task.FromResult((IBundleDescription)new JsonBundleDescription
            {
                Name = name,
                BundleId = bundleId,
                AuthorId = authorId,
                CreatedTime = createdTime,
                UpdatedTime = updatedTime
            });
        }

        /// <summary>
        ///  Nullは追加しない(NullObjectにする)
        /// </summary>
        /// <param name="target"></param>
        /// <param name="allObjects"></param>
        /// <param name="allResources"></param>
        /// <returns>オブジェクトのID</returns>
        private async Task<string> PackReferenceAsync(IManagedObject target,IDictionary<string,JsonObjectReference> allObjects,IDictionary<string,JsonPropertyReference> allResources)
        {
            if(allObjects.ContainsKey(target.UniqueId))
            {
                Debug.LogWarning("circular reference");
                return null;
            }
            allObjects[target.UniqueId] = null;
            
            // 子を探索する
            var children = new HashSet<string>(); 
            foreach (var child in target.GetChildren())
            {
                var childId = await PackReferenceAsync(child,allObjects,allResources);
                if (!string.IsNullOrEmpty(childId))
                {
                    children.Add(childId);
                }
            }

            //依存しているResourceを追加
            foreach (var resourceId in target.GetResourceDependencies())
            {
                if (!allResources.ContainsKey(resourceId))
                {
                    var obj = target.WorldContext.GetResource(resourceId);
                    if (obj)
                    {
                        allResources[resourceId] = CastObjectToJson(obj);
                    }
                }
            }

            Dictionary<string, string> gs = new();
            Dictionary<string, int> gi = new();
            Dictionary<string, float> gf = new();
            Dictionary<string, bool> gb = new();

            foreach (var key in await target.GlobalVariable.GetKeysAsync())
            {
                var valueResult = await target.GlobalVariable.TryGetAsync(key);
                
                if (!valueResult.IsSucceeded)
                    continue;

                switch (valueResult.Value)
                {
                    case string str:
                        gs[key] = str;
                        break;
                    case int integer:
                        gi[key] = integer;
                        break;
                    case float single:
                        gf[key] = single;
                        break;
                    case bool boo:
                        gb[key] = boo;
                        break;
                    default:
                        Debug.LogWarning($"global variable[{key}] : type {valueResult.Value.GetType()} will not be saved.");
                        break;
                }
            }

            var self = new JsonObjectReference
            {
                Position = target.LocalPosition.Serialize(),
                Rotation = target.LocalRotation.Serialize(),
                Scale = target.Scale.Serialize(),
                ChildrenReferenceIds = children.ToList(),
                LocalVariableString = new (),
                LocalVariableInt = new (),
                LocalVariableFloat = new (),
                LocalVariableBool = new (),
                GlobalVariableString = gs,
                GlobalVariableInt = gi,
                GlobalVariableFloat = gf,
                GlobalVariableBool = gb
            };

            allObjects[target.UniqueId] = self;

            return target.UniqueId;
        }

        private JsonPropertyReference CastObjectToJson(UnityEngine.Object obj)
        {
            return obj switch
            {
                PhysicMaterial material => new JsonPropertyReference
                {
                    Type = "PhysicMaterial",
                    Data = new (){
                        {"Name",material.name},
                        {"DynamicFriction",material.dynamicFriction.ToString(CultureInfo.InvariantCulture)},
                        {"StaticFriction",material.staticFriction.ToString(CultureInfo.InvariantCulture)},
                        {"Bounciness",material.bounciness.ToString(CultureInfo.InvariantCulture)},
                        {"FrictionCombine",((int)material.frictionCombine).ToString()},
                        {"BounceCombine",((int)material.bounceCombine).ToString()}
                    }
                },
                _ => null
            };
        }
    }
}