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
            // 객체가 파괴되기 전에 gameElementsSynchronizer의 RPC 호출
            if (gameElementsSynchronizer != null)
            {
                gameElementsSynchronizer.RPC_DespawnProjectile(id);
            }
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
        Invoke("TestSpawn", 10);
    }

    int testId;

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
        // 갱신된 projectilesData를 순회하면서 projectile을 업데이트
        foreach (var projectileData in projectilesData)
        {
            // Data가 있고 Projectile 객체도 있을 때
            if (projectiles.TryGetValue(projectileData.Key, out GameObject projectile))
            {
                if (projectile != null)
                {
                    projectile.transform.position = projectileData.Value.firedPosition +
                        (elapsedTime - projectileData.Value.firedTime) * projectileData.Value.velocity;
                }
            }
            // Data는 있는데 Projectile 객체가 없을 때 새로 생성
            else
            {
                projectile = Instantiate(projectilePrefabs[projectileData.Value.prefabName], projectileData.Value.firedPosition, Quaternion.LookRotation(projectileData.Value.velocity));

                SyncTag syncTag = projectile.AddComponent<SyncTag>();
                syncTag.id = projectileData.Key;
                syncTag.gameElementsSynchronizer = this;

                projectiles.Add(projectileData.Key, projectile);
                Debug.Log("Instantiated projectile");
            }
        }

        // 생성된 객체 탐색 후 데이터가 없으면 파괴
        var ids = projectiles.Keys.ToList();

        foreach (var id in ids)
        {
            if (!projectilesData.ContainsKey(id))
            {
                if (projectiles.TryGetValue(id, out GameObject projectile))
                {
                    if (projectile != null)
                    {
                        Destroy(projectile);
                    }
                    projectiles.Remove(id);
                    Debug.Log("Destroyed projectile");
                }
            }
        }
    }
}
