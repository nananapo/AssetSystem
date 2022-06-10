using System;
using System.Net.Http;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using GraphConnectEngine;
using GraphConnectEngine.Nodes;
using UnityEngine;
using UnityEngine.UI;

namespace VisualScripting.Scripts.Graphs.Unity
{
    public class SetImageFromURLGraph : Graph
    {
        public SetImageFromURLGraph(string id) : base(id)
        {
            var resolver1 = new ItemTypeResolver(typeof(string), "URL");
            var resolver2 = new ItemTypeResolver(typeof(RawImage), "RawImage");
            
            AddNode(new InItemNode(this, resolver1));
            AddNode(new InItemNode(this, resolver2));
            
            AddNode(new OutItemNode(this, resolver1,0));
            AddNode(new OutItemNode(this, resolver2,1));
        }

        public override async Task<ProcessCallResult> OnProcessCall(ProcessData args, object[] parameters)
        {
            return await OnProcessCallAsync(args, parameters);
        }

        private async UniTask<ProcessCallResult> OnProcessCallAsync(ProcessData args, object[] parameters)
        {
            string url = parameters[0].ToString();
            RawImage rawImage = (RawImage) parameters[1];
            
            //Download image and read bytes
            var dwResult = false;
            byte[] imageData = null;

            await UniTask.Run(async () =>
            {
                try
                {
                    using (var client = new HttpClient())
                    using (var response = await client.GetAsync(url))
                    {
                        imageData = await response.Content.ReadAsByteArrayAsync();
                        dwResult = true;
                    }
                }
                catch (Exception) { }
            });

            if (!dwResult)
            {
                return ProcessCallResult.Fail();
            }
            await UniTask.Yield();

            var texture = new Texture2D(1, 1);
            texture.LoadImage(imageData);
            
            await UniTask.Yield();

            rawImage.texture = texture;

            //Set image
            return ProcessCallResult.Success(new object[] { url, rawImage }, OutProcessNodes[0]);
        }

        public static double GetUnixTime()
        {
            return DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        public override string GraphName => "UnityWebRequestTexture.GetTexture";
    }
}