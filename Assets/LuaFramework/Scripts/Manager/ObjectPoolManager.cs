using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using PathologicalGames;
using Scripts;

namespace LuaFramework {
    /// <summary>
    /// 对象池管理器，分普通类对象池+资源游戏对象池
    /// </summary>
    public class ObjectPoolManager : Manager {
        private Dictionary<string, Object> m_loadObjDic;
        private SpawnPool m_spawnPool;
        private PrefabPool m_prefabPool;
        private Transform m_PoolRootObject = null;
        private Dictionary<string, object> m_ObjectPools = new Dictionary<string, object>();
        private Dictionary<string, GameObjectPool> m_GameObjectPools = new Dictionary<string, GameObjectPool>();

        void Awake()
        {
            m_instance = this;
            m_spawnPool = PoolManager.Pools[AppConst.PoolName];
            m_loadObjDic = new Dictionary<string, Object>();
        }

        internal GameObject LoadObjectByName(string prefabName, Transform parent, bool isLocal = false, int poolSize = 1)
        {
            Transform tmp = null;
            //GameObject obj = null;
            string key;
            string prefabUrl = prefabName.Split('/')[prefabName.Split('/').Length - 1];
            key = PreLoadObject(prefabName, isLocal, poolSize);

            //obj = Get(key);
            tmp = m_spawnPool.Spawn(key, parent);

            if (tmp == null)
            {
                Debug.LogError("Can't Load prefab from " + prefabName);
            }
            else
            {
                tmp.localPosition = new Vector3(0, 0, 10000);
                tmp.localScale = Vector3.one;
            }

            return tmp.gameObject;
        }

        internal string PreLoadObject(string prefabName, bool isLocal = false, int poolSize = 1)
        {
            isLocal = true;
            GameObject tmp = null;
            string key = prefabName.Split('/')[prefabName.Split('/').Length - 1];
            //if (!m_GameObjectPools.ContainsKey(key))
            if (m_spawnPool.prefabs.ContainsKey(key) == false)
            {
                if (GameStart.Instance.IsDebug || isLocal)
                {
                    tmp = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/" + prefabName + ".prefab", typeof(GameObject)) as GameObject;
                    //tmp = Resources.Load<GameObject>(prefabName);
                }
                else
                {
                    AssetBundle bundle = ResManager.LoadBundle(key);
                    tmp = Util.LoadAsset(bundle, key);
                }
                if (tmp == null)
                {
                    Debug.LogError("资源：" + prefabName + " 加载为空");
                }
                else if (!prefabName.StartsWith("Prefabs/FirstPackage") && !prefabName.StartsWith("Prefabs/SecondPackage"))
                {
                    CreatePrefabPool(tmp.transform);
                    //CreatePool(key, poolSize, 10, tmp);
                }
            }
            return key;
        }

        private void CreatePrefabPool(Transform tmp)
        {
            m_prefabPool = new PrefabPool(tmp);
            //默认初始化两个Prefab
            m_prefabPool.preloadAmount = 1;
            //初始化内存池
            m_spawnPool._perPrefabPoolOptions.Add(m_prefabPool);
            m_spawnPool.CreatePrefabPool(m_prefabPool);
        }

        internal AudioClip LoadAudioClipByName(string name, bool isLocal = true)
        {
            AudioClip tmp = null;
            string path = "Sounds/" + name;
            if (m_loadObjDic.ContainsKey(name) == false)
            {
                if (GameStart.Instance.IsDebug || isLocal)
                {
                    tmp = (AudioClip)Resources.Load(path, typeof(AudioClip));//调用Resources方法加载AudioClip资源
                }
                else
                {
                    AssetBundle bundle = ResManager.LoadBundle(name);
                    tmp = (bundle.LoadAsset(name, typeof(AudioSource)) as AudioSource).clip;
                }
                m_loadObjDic.Add(name, tmp);
            }
            else
            {
                tmp = (AudioClip)m_loadObjDic[name];
            }
            return tmp;
        }

        Transform PoolRootObject {
            get {
                if (m_PoolRootObject == null) {
                    var objectPool = new GameObject("ObjectPool");
                    objectPool.transform.SetParent(transform);
                    objectPool.transform.localScale = Vector3.one;
                    objectPool.transform.localPosition = Vector3.zero;
                    m_PoolRootObject = objectPool.transform;
                }
                return m_PoolRootObject;
            }
        }

        public GameObjectPool CreatePool(string poolName, int initSize, int maxSize, GameObject prefab) {
            var pool = new GameObjectPool(poolName, prefab, initSize, maxSize, PoolRootObject);
            m_GameObjectPools[poolName] = pool;
            return pool;
        }

        public GameObjectPool GetPool(string poolName) {
            if (m_GameObjectPools.ContainsKey(poolName)) {
                return m_GameObjectPools[poolName];
            }
            return null;
        }

        public GameObject Get(string poolName) {
            GameObject result = null;
            if (m_GameObjectPools.ContainsKey(poolName)) {
                GameObjectPool pool = m_GameObjectPools[poolName];
                result = pool.NextAvailableObject();
                if (result == null) {
                    Debug.LogWarning("No object available in pool. Consider setting fixedSize to false.: " + poolName);
                }
            } else {
                Debug.LogError("Invalid pool name specified: " + poolName);
            }
            return result;
        }

        internal void PoolDestroy(GameObject obj)
        {
            if (obj != null)
            {
                m_spawnPool.Despawn(obj.transform, m_spawnPool.transform);
            }
            else
            {
                Debug.Log("##########PoolDestroy error################" + obj.name);
            }
        }

        public void Release(string poolName, GameObject go) {
            if (m_GameObjectPools.ContainsKey(poolName)) {
                GameObjectPool pool = m_GameObjectPools[poolName];
                go.transform.SetParent(null);
                pool.ReturnObjectToPool(poolName, go);
            } else {
                Debug.LogWarning("No pool available with name: " + poolName);
            }
        }

        ///-----------------------------------------------------------------------------------------------

        public ObjectPool<T> CreatePool<T>(UnityAction<T> actionOnGet, UnityAction<T> actionOnRelease) where T : class {
            var type = typeof(T);
            var pool = new ObjectPool<T>(actionOnGet, actionOnRelease);
            m_ObjectPools[type.Name] = pool;
            return pool;
        }

        public ObjectPool<T> GetPool<T>() where T : class {
            var type = typeof(T);
            ObjectPool<T> pool = null;
            if (m_ObjectPools.ContainsKey(type.Name)) {
                pool = m_ObjectPools[type.Name] as ObjectPool<T>;
            }
            return pool;
        }

        public T Get<T>() where T : class {
            var pool = GetPool<T>();
            if (pool != null) {
                return pool.Get();
            }
            return default(T);
        }

        public void Release<T>(T obj) where T : class {
            var pool = GetPool<T>();
            if (pool != null) {
                pool.Release(obj);
            }
        }

        private static ObjectPoolManager m_instance;
        public static ObjectPoolManager Instance
        {
            get
            {
                return m_instance;
            }
        }
    }
}