using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameElementsSynchronizer : NetworkBehaviour
{
    public class SyncTag : MonoBehaviour
    {
        public int id;
        public GameElementsSynchronizer gameElementsSynchronizer;

        private void Start()
        {            
            Debug.Log($"Sync tag started {id}");
        }

        private void OnDestroy()
        {

            gameElementsSynchronizer.RPC_DespawnProjectile(id);
            Debug.Log($"Sync tag destroyed {id}");
        }
    }

    public struct ProjectileData
    {
        public int id;
        public float firedTime;
        public string prefabName;
        public Vector3 velocity;
        public Vector3 firedPosition;
    }

    Dictionary<int, ProjectileData> projectilesData = new Dictionary<int, ProjectileData>();
    Dictionary<int, GameObject> projectiles = new Dictionary<int, GameObject>();
    Dictionary<string, GameObject> projectilePrefabs = new Dictionary<string, GameObject>();

    public float elapsedTime => Runner.SimulationTime;

    private void Awake()
    {
        var prefabs = Resources.LoadAll<GameObject>("Projectiles");
        for (int i = 0; i < prefabs.Length; i++)
        {
            projectilePrefabs.Add(prefabs[i].name, prefabs[i]);
        }
        Instantiate(projectilePrefabs["TestProjectile"], Vector3.one * 9999, Quaternion.identity).name = "Awake test";
        //StartCoroutine(C_Spawned());
        Invoke("TestSpawn", 10);
    }

    int testId;
    //public IEnumerator C_Spawned()
    //{
    //    yield return new WaitForSeconds(10f);
    //    testId = Guid.NewGuid().GetHashCode();
    //    Instantiate(projectilePrefabs["TestProjectile"], Vector3.one * 9999, Quaternion.identity).name = "Spawned test";
    //    SpawnProjectile(testId, "TestProjectile", Vector3.forward, Vector3.right * 5f);
    //}

    private void TestSpawn()
    {
        testId = Guid.NewGuid().GetHashCode();
        Instantiate(projectilePrefabs["TestProjectile"], Vector3.one * 9999, Quaternion.identity).name = "Spawned test";
        SpawnProjectile(testId, "TestProjectile", Vector3.forward, Vector3.right * 5f); 
    }


    public void SpawnProjectile(int id, string prefabName, Vector3 velocity, Vector3 firedPosition)
    {
        Debug.Log("TrySpawnProjectile");
        RPC_SpawnProjectile(id, prefabName, velocity, firedPosition);
    }

    public void DespawnProjectile(GameObject gameObject)
    {
        if (gameObject.TryGetComponent(out SyncTag tag))
        {
            RPC_DespawnProjectile(tag.id);
        }
    }


    [Rpc(RpcSources.All, RpcTargets.All)]
    void RPC_SpawnProjectile(int id, string prefabName, Vector3 velocity, Vector3 firedPosition)
    {
        Debug.Log("Rpc - TrySpawnProjectile");

        ProjectileData projectile = new ProjectileData
        {
            id = id,
            firedTime = elapsedTime,
            prefabName = prefabName,
            velocity = velocity,
            firedPosition = firedPosition
        };
        projectilesData.Add(projectile.id, projectile);
    }


    [Rpc(RpcSources.All, RpcTargets.All)]
    void RPC_DespawnProjectile(int id)
    {
        projectilesData.Remove(id);
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        UpdateProjectiles();
    }

    private void UpdateProjectiles()
    {
        // Spawn / Despawn 등을 할때마다 갱신된 projectilesDAta 를 전체 순회하면서 projectile 을 업데이트한다.
        foreach (var projectileData in projectilesData)
        {
            // Data 는 있고 Projectile 객체도 있을때
            if (projectiles.TryGetValue(projectileData.Key, out GameObject projectile))
            {
                projectile.transform.position = projectileData.Value.firedPosition + (elapsedTime - projectileData.Value.firedTime) * projectileData.Value.velocity;
            }
            // Data는 있는데 Projectile 객체 없어서 새로 생성해야할때
            else
            {
                projectile = Instantiate(projectilePrefabs[projectileData.Value.prefabName], projectileData.Value.firedPosition, Quaternion.LookRotation(projectileData.Value.velocity));
                projectile.AddComponent<SyncTag>().id = projectileData.Key;

                SyncTag synTag = projectile.AddComponent<SyncTag>();
                synTag.gameElementsSynchronizer = GetComponent<GameElementsSynchronizer>();                

                projectiles.Add(projectileData.Key, projectile);
                Debug.Log("Instantiated projectile");
            }
        }

        // 생성된 객체를 전체 탐색하고, 데이터가 없어지면 생성되어있으면 안되니까 파괴
        var ids = projectiles.Keys.ToList();

        foreach (var id in ids)
        {
            if (projectilesData.TryGetValue(id, out ProjectileData projectileData))
            {
                // nothing to do
            }
            else
            {
                Destroy(projectiles[id]);
                projectiles.Remove(id);
                Debug.Log("Destroyed projectile");

            }
        }
    }
}
