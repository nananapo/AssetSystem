using System.Collections.Generic;
using System.Linq;
using AssetSystem.Reference.Json;
using MyWorldHub.Unity;
using UnityEngine;

namespace AssetSystem.Unity
{
    public class Test_WorldContextInitializer : MonoBehaviour
    {

        public IWorldContext WorldContext;

        public Transform rootTransform;

        [SerializeField] public string mainId;
        [SerializeField] public List<Test_ManagedObjectType> objects;
        [SerializeField] public List<Test_ResourceType> resources;
        
        private async void Awake()
        {
            WorldContext = new WorldContext("SampleWorldID",
                new JsonMasterBundle
                {
                    MainReferenceId = mainId,
                    Objects = objects.ToDictionary(v=>v.id,v=>new JsonObjectReference
                    {
                        Position = v.position.Serialize(),
                        Rotation = v.rotation.Serialize(),
                        Scale = v.scale.Serialize(),
                        ChildrenReferenceIds = v.childrenIds,
                        GlobalVariableString = v.strGlobal.ToDictionary(k=>k.id,k=>k.value),
                        GlobalVariableInt = v.intGlobal.ToDictionary(k=>k.id,k=>k.value),
                        GlobalVariableFloat = v.floatGlobal.ToDictionary(k=>k.id,k=>k.value),
                        GlobalVariableBool = v.boolGlobal.ToDictionary(k=>k.id,k=>k.value),
                    }),
                    Resources = resources.ToDictionary(v=>v.id,v=>new JsonPropertyReference
                    {
                        Type = v.type,
                        Data = v.data.ToDictionary(k=>k.id,k=>k.value)
                    })
                });

            await WorldContext.LoadWorldResourcesAsync();
            await WorldContext.LoadWorldAsync(rootTransform);
        }
    }
}

/*
new JsonMasterBundle
{
    MainReferenceId = "fc24d5ab-4a27-4fe8-870f-31ccdb335320",
    Objects = new Dictionary<string, JsonObjectReference>
    {
        {"fc24d5ab-4a27-4fe8-870f-31ccdb335320",new JsonObjectReference
            {
                Position = Vector3.zero.Serialize(),
                Rotation = Quaternion.identity.Serialize(),
                Scale = Vector3.one.Serialize(),
                ChildrenReferenceIds = new List<string>
                {
                    "c8f2ce3a-e316-4e8c-a992-e05ddc1b76d4",
                    "4a351f25-e328-4ff0-9123-7856e78ba2cd",
                    "2c6dc2ab-1e3f-47aa-825e-6617f8ffd37f"
                },
                GlobalVariableString = new Dictionary<string, string>
                {
                    {"!object_name","Root"}
                }
            }
        },
        {"c8f2ce3a-e316-4e8c-a992-e05ddc1b76d4",new JsonObjectReference
            {
                Position = new Vector3(0,1,0).Serialize(),
                Rotation = Quaternion.identity.Serialize(),
                Scale = new Vector3(5,1,5).Serialize(),
                GlobalVariableString = new ()
                {
                    {"!object_name","Floor"},
                    
                    {$"!{ComponentSystem.DefinitionPrefix}_colliderId",BoxColliderComponent.ComponentName},
                    {"!colliderId_PhysicMaterialId","physicMaterial1"},
                    {"!colliderId_Center","0,0,0"},
                    {"!colliderId_Size","1,1,1"},
                    
                    {$"!{ComponentSystem.DefinitionPrefix}_meshFilterId",MeshFilterComponent.ComponentName},
                    {"!meshFilterId_MeshId","PrimitiveType:Cube?0"},
                    
                    {$"!{ComponentSystem.DefinitionPrefix}_meshRendererId",MeshRendererComponent.ComponentName},
                    {"!meshRendererId_MaterialIds","PrimitiveType:Cube?1"},
                },
                GlobalVariableBool = new()
                {
                    {"!colliderId_IsTrigger",false}
                }
            }
        },
        {"4a351f25-e328-4ff0-9123-7856e78ba2cd",new JsonObjectReference
            {
                Position = new Vector3(0,5,0).Serialize(),
                Rotation = Quaternion.identity.Serialize(),
                Scale = new Vector3(1,1,1).Serialize(),
                GlobalVariableString = new ()
                {
                    {"!object_name","Floor"},
                    
                    {$"!{ComponentSystem.DefinitionPrefix}_colliderId",SphereColliderComponent.ComponentName},
                    {"!colliderId_PhysicMaterialId","physicMaterial1"},
                    {"!colliderId_Center","0,0,0"},
                    {"!colliderId_Radius","1"},
                    
                    {$"!{ComponentSystem.DefinitionPrefix}_meshFilterId",MeshFilterComponent.ComponentName},
                    {"!meshFilterId_MeshId","PrimitiveType:Sphere?0"},
                    
                    {$"!{ComponentSystem.DefinitionPrefix}_meshRendererId",MeshRendererComponent.ComponentName},
                    {"!meshRendererId_MaterialIds","PrimitiveType:Sphere?1"},
                    
                    {$"!{ComponentSystem.DefinitionPrefix}_rigidbody",RigidbodyComponent.ComponentName},
                },
                GlobalVariableBool = new()
                {
                    {"!colliderId_IsTrigger",false},
                    {"!rigidbody_UseGravity",true},
                    {"!rigidbody_IsKinematic",false}
                }
            }
        },
        {"2c6dc2ab-1e3f-47aa-825e-6617f8ffd37f",new JsonObjectReference
            {
                Position = new Vector3(0,2,0).Serialize(),
                Rotation = Quaternion.identity.Serialize(),
                Scale = Vector3.one.Serialize(),
                GlobalVariableString = new Dictionary<string, string>
                {
                    {"!object_name","GraphTest"},
                    {"!"+GraphSystem.DefinitionPrefix+"_1","Updater"},
                    {"!"+GraphSystem.PositionPrefix+"_1","0,0,0"},
                    {"!"+GraphSystem.RotationPrefix+"_1","0,0,0,0"},
                    {"!"+GraphSystem.DefinitionPrefix+"_2","DebugText"},
                    {"!"+GraphSystem.PositionPrefix+"_2","1,0,0"},
                    {"!"+GraphSystem.RotationPrefix+"_2","0,0,0,0"},
                    {"!"+GraphSystem.DefinitionPrefix+"_3","ValueFunc<String>"},
                    {"!"+GraphSystem.PositionPrefix+"_3","0.5,-0.25,0"},
                    {"!"+GraphSystem.RotationPrefix+"_3","0,0,0,0"},
                    {"!3_Value","HelloWorld"},
                    {"!"+VarNodeConnector.ConnectionPrefix+"_1:1:0_2:0:0",""},
                    {"!"+VarNodeConnector.ConnectionPrefix+"_3:3:0_2:2:0",""},
                }
            }
        }
    },
    Resources = new Dictionary<string, JsonPropertyReference>
    {
        {"physicMaterial1",new JsonPropertyReference
            {
                Type = "PhysicMaterial",
                Data = new ()
                {
                    {"Name","moremoreBounce"},
                    {"DynamicFriction","0"},
                    {"StaticFriction","0"},
                    {"Bounciness","1"},
                    {"FrictionCombine","1"},
                    {"BounceCombine","3"},
                }
            }
        }
    }
}*/