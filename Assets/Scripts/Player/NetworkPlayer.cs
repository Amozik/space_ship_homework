#pragma warning disable 618
using UnityEngine;
using UnityEngine.Networking;
using static UnityEngine.Networking.NetworkServer;
#pragma warning restore 618

namespace Player
{
#pragma warning disable 618
    public class NetworkPlayer : NetworkBehaviour
#pragma warning restore 618
    {
        [SerializeField] 
        private GameObject _playerPrefab;
        
        private GameObject _playerCharacter;

        private void Start()
        {
            SpawnCharacter();
        }

        private void SpawnCharacter()
        {
            if (!isServer)
            {
                return;
            }

            _playerCharacter = Instantiate(_playerPrefab, transform.position, transform.rotation);
            

            SpawnWithClientAuthority(_playerCharacter, connectionToClient);
        }
    }
}
