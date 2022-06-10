using System;
using System.Collections.Generic;
using System.Text;
using AssetSystem.Reference;
using Cysharp.Threading.Tasks;
using Firebase.Firestore;
using Firebase.Storage;
using UnityEngine;
using UnityEngine.Networking;

namespace AssetSystem.Repository
{
    public class FirebaseBundleDataRepository : IBundleDataRepository
    {

        private readonly IReferenceConverter _referenceConverter;
        
        private readonly FirebaseStorage _storage;
        
        private readonly FirebaseFirestore _firestore;

        private readonly Dictionary<string, IMasterBundle> _bundleCache = new();

        public FirebaseBundleDataRepository(IReferenceConverter referenceConverter)
        {
            _referenceConverter = referenceConverter;
            _firestore = FirebaseFirestore.DefaultInstance;
            _storage = FirebaseStorage.DefaultInstance;
        }

        async UniTask<(bool result,IMasterBundle data)> IBundleDataRepository.GetMasterBundle(string bundleId)
        {
            // キャッシュのチェック
            if (_bundleCache.ContainsKey(bundleId))
            {
                return (true,_bundleCache[bundleId]);
            }
            
            var reference = _storage.GetReference("bundles/" + bundleId);

            // URIを取得する
            Uri uri;
            try
            {
                uri = await reference.GetDownloadUrlAsync();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return (false, null);
            }
            
            // ダウンロードする
            string text;
            try
            {
                using var webRequest = UnityWebRequest.Get(uri);
                
                Debug.Log("Downloading Bundle....");
                await webRequest.SendWebRequest();

                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.Success:
                        Debug.Log("Download Succeeded");
                        text = webRequest.downloadHandler.text;
                        break;
                    default:
                        Debug.Log("Failed to download bundle.");
                        return (false, null);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return (false, null);
            }
            
            //読み込む
            var (result, bundle) = await _referenceConverter.DecodeMasterBundle(text);
            if (!result)
            {
                return (false, null);
            }

            _bundleCache[bundleId] = bundle;
            return (true,bundle);
        }

        async UniTask<List<(string ObjectId, IBundleDescription Description)>> IBundleDataRepository.GetObjectList(long createdTime,bool start,bool at,int limit)
        {
            var query = _firestore.Collection("objects")
                .OrderBy("CreatedTime")
                .Limit(limit);

            if (start)
            {
                query = at ? query.StartAt(createdTime) : query.StartAfter(createdTime);
            }
            else
            {
                query = at ? query.EndAt(createdTime) : query.EndBefore(createdTime);
            }

            var querySnapshot = await query.GetSnapshotAsync();
            var list = new List<(string, IBundleDescription)>();
            foreach (var documentSnapshot in querySnapshot.Documents)
            {
                var (result,desc) =  await GetDescription(documentSnapshot);
                if (result)
                {
                    list.Add((documentSnapshot.Id,desc));
                }
            }

            return list;
        }

        async UniTask<List<(string worldId, IBundleDescription Description)>> IBundleDataRepository.GetWorldList(long createdTime,bool start,bool at,int limit)
        {
            var query = _firestore.Collection("worlds")
                .OrderBy("CreatedTime")
                .Limit(limit);
            
            if (start)
            {
                query = at ? query.StartAt(createdTime) : query.StartAfter(createdTime);
            }
            else
            {
                query = at ? query.EndAt(createdTime) : query.EndBefore(createdTime);
            }

            var querySnapshot = await query.GetSnapshotAsync();
            var list = new List<(string, IBundleDescription)>();
            foreach (var documentSnapshot in querySnapshot.Documents)
            {
                var (result,desc) =  await GetDescription(documentSnapshot);
                if (result)
                {
                    list.Add((documentSnapshot.Id,desc));
                }
            }

            return list;
        }

        public async UniTask<(bool result, IBundleDescription data)> GetObjectDescription(string objectId)
        {
            var snapshot = await _firestore.Collection("objects").Document(objectId).GetSnapshotAsync();
            if (!snapshot.Exists)
            {
                return (false, null);
            }
            return await GetDescription(snapshot);
        }

        async UniTask<(bool result,IBundleDescription data)> IBundleDataRepository.GetWorldDescription(string worldId)
        {
            var snapshot = await _firestore.Collection("worlds").Document(worldId).GetSnapshotAsync();
            if (!snapshot.Exists)
            {
                return (false, null);
            }
            return await GetDescription(snapshot);
        }

        private UniTask<(bool result,IBundleDescription data)> GetDescription(DocumentSnapshot snapshot)
        {
            var name = snapshot.GetValue<string>("Name") ?? "Unknown";
            var bundleId = snapshot.GetValue<string>("BundleId");
            var authorId = snapshot.GetValue<string>("AuthorId") ?? "Unknown";
            var createdTime = snapshot.GetValue<long>("CreatedTime");
            var updatedTime = snapshot.GetValue<long>("UpdatedTime");
            
            if (bundleId == null)
            {
                return UniTask.FromResult<(bool,IBundleDescription)>((false,null));
            }
            return _referenceConverter.DecodeWorldDescription(name, bundleId, authorId, createdTime, updatedTime);
        }

        public async UniTask<string> GetModelDataUrl(string id)
        {
            var uri = await _storage.GetReference(id).GetDownloadUrlAsync();
            return uri?.OriginalString;
        }

        async UniTask<bool> IBundleDataRepository.SaveObjectDescription(string objectId, IBundleDescription bundle)
        {
            var reference = _firestore.Collection("objects").Document(objectId);

            try
            {
                await reference.SetAsync(new Dictionary<string, object>
                {
                    {"Name",bundle.Name}, 
                    {"BundleId", bundle.BundleId},
                    {"AuthorId", bundle.AuthorId},
                    {"CreatedTime", bundle.CreatedTime},
                    {"UpdatedTime", bundle.UpdatedTime},
                });
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }
                
            Debug.Log($"Saved Description : firestore/objects/{objectId}");
            return true;
        }

        async UniTask<bool> IBundleDataRepository.SaveWorldDescription(string worldId, IBundleDescription bundle)
        {
            var reference = _firestore.Collection("worlds").Document(worldId);

            try
            {
                await reference.SetAsync(new Dictionary<string, object>
                {
                    {"Name",bundle.Name}, 
                    {"BundleId", bundle.BundleId},
                    {"AuthorId", bundle.AuthorId},
                    {"CreatedTime", bundle.CreatedTime},
                    {"UpdatedTime", bundle.UpdatedTime},
                });
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }
                
            Debug.Log($"Saved Description : firestore/worlds/{worldId}");
            return true;
        }

        async UniTask<bool> IBundleDataRepository.SaveBundle(string id, IMasterBundle masterBundle)
        {
            var reference = _storage.GetReference("bundles/" + id);

            try
            {
                await reference.PutBytesAsync(Encoding.UTF8.GetBytes(masterBundle.Encode()));
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }
            
            Debug.Log($"Saved MasterBundle : storage/bundle/{id}");
            return true;
        }
    }
}