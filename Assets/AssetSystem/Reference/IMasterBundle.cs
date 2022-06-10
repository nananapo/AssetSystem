using System.Collections.Generic;

namespace AssetSystem.Reference
{
    /// <summary>
    /// 制約
    /// objectの参照はbundleで閉じてなければいけない
    /// </summary>
    public interface IMasterBundle
    {
        public string GetMainReferenceId();

        public IDictionary<string, IObjectReference> GetObjectReferences();

        public IDictionary<string, IResourceReference> GetResourceReferences();

        public string Encode();
    }
}