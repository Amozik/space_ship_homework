using System.Collections.Generic;
using Character;
using UI;
using UnityEngine;
using UnityEngine.Networking;

namespace Main
{
    public class SolarSystemNetworkManager : NetworkManager
    {
        [SerializeField] 
        private string _playerName;

        [SerializeField] 
        private UiController _uiController;

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {
            var spawnTransform = GetStartPosition();

            var player = Instantiate(playerPrefab, spawnTransform.position, spawnTransform.rotation);
            //player.GetComponent<ShipController>().PlayerName = _playerName;
            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);

        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            PlayerPrefs.DeleteAll();
            _uiController.ActivateInputName();
            _uiController.PlayerNameEntered += ResumeConnection;

            void ResumeConnection()
            {
                base.OnClientConnect(conn);
                _uiController.PlayerNameEntered -= ResumeConnection;
            }
        }
    }
}
