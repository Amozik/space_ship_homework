using System.Collections;
using Newtonsoft.Json.Serialization;
using UnityEngine;
using UnityEngine.Networking;

namespace Mechanics
{
    public class BulletManager : MonoBehaviour
    {
        public NetworkHash128 AssetId => _assetId;

        [SerializeField] 
        private GameObject _bulletPrefab;
        private NetworkHash128 _assetId;

        void Start()
        {
            _assetId = _bulletPrefab.GetComponent<NetworkIdentity>().assetId;
            ClientScene.RegisterSpawnHandler(_assetId, SpawnBullet, UnSpawnBullet);
        }

        public GameObject SpawnBullet(Vector3 position)
        {
            return Instantiate(_bulletPrefab, position, Quaternion.identity);
        }

        public GameObject SpawnBullet(Vector3 position, NetworkHash128 assetId)
        {
            return SpawnBullet(position);
        }

        public void UnSpawnBullet(GameObject spawned)
        {
            Destroy(spawned);
        }
    }

    public class Player : NetworkBehaviour
    {
        private BulletManager bulletManager;

        private void Start()
        {
            bulletManager = GameObject.Find("BulletManager").GetComponent<BulletManager>();
        }

        private void Update()
        {
            if (!isLocalPlayer || !hasAuthority)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                CmdFire();
            }
        }

        [Command]
        private void CmdFire()
        {
            var bullet = bulletManager.SpawnBullet(transform.position + transform.forward);
            bullet.GetComponent<Rigidbody>().velocity = transform.forward * 4;

            NetworkServer.Spawn(bullet, bulletManager.AssetId);

            StartCoroutine(Destroy(bullet, 2.0f));
        }

        private IEnumerator Destroy(GameObject go, float timer)
        {
            yield return new WaitForSeconds(timer);
            bulletManager.UnSpawnBullet(go);
            NetworkServer.UnSpawn(go);
        }
    }
}