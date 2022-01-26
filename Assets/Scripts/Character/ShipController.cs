using System;
using Main;
using Network;
using UI;
using UnityEngine;
using UnityEngine.Networking;

namespace Character
{
    public class ShipController : NetworkMovableObject
    {
        [SerializeField] 
        private Transform _cameraAttach;
        private CameraOrbit _cameraOrbit;
        private PlayerLabel _playerLabel;
        private float _shipSpeed;
        private Rigidbody _rigidbody;

        [SyncVar(hook = "DisplayPlayerName")] 
        private string _playerName;

        [SyncEvent]
        public event Action EventOnSomethingHappend;

        protected override float Speed => _shipSpeed;

        public string PlayerName
        {
            get => _playerName;
            set => _playerName = value;
        }

        private void OnGUI()
        {
            if (_cameraOrbit == null)            
                return;
            
            _cameraOrbit.ShowPlayerLabels(_playerLabel);
        }

        public override void OnStartAuthority()
        {
            _rigidbody = GetComponent<Rigidbody>();
            if (_rigidbody == null)            
                return;
            
            //DisplayPlayerName(_playerName ?? "player");
            _cameraOrbit = FindObjectOfType<CameraOrbit>();
            _cameraOrbit.Initiate(_cameraAttach == null ? transform : _cameraAttach);
            _playerLabel = GetComponentInChildren<PlayerLabel>();
            base.OnStartAuthority();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            Debug.Log("OnStartClient");
        }

        public override void OnStartLocalPlayer()
        {
            CmdSendName(PlayerPrefs.GetString("PlayerName"));
            
            base.OnStartLocalPlayer();

            Debug.Log("OnStartLocalPlayer");
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            Debug.Log("OnStartServer");
        }

        protected override void HasAuthorityMovement()
        {
            var spaceShipSettings = SettingsContainer.Instance?.SpaceShipSettings;
            if (spaceShipSettings == null)            
                return;            

            var isFaster = Input.GetKey(KeyCode.LeftShift);
            var speed = spaceShipSettings.ShipSpeed;
            var faster = isFaster ? spaceShipSettings.Faster : 1.0f;

            _shipSpeed = Mathf.Lerp(_shipSpeed, speed * faster, spaceShipSettings.Acceleration);

            var currentFov = isFaster ? spaceShipSettings.FasterFov : spaceShipSettings.NormalFov;
            _cameraOrbit.SetFov(currentFov, spaceShipSettings.ChangeFovSpeed);

            var velocity = _cameraOrbit.transform.TransformDirection(Vector3.forward) * _shipSpeed;
            _rigidbody.velocity = velocity * (_updatePhase == UpdatePhase.FixedUpdate ? Time.fixedDeltaTime : Time.deltaTime);

            if (!Input.GetKey(KeyCode.C))
            {
                var targetRotation = Quaternion.LookRotation(Quaternion.AngleAxis(_cameraOrbit.LookAngle, -transform.right) * velocity);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speed);
            }
        }

        protected override void FromServerUpdate() { }
        protected override void SendToServer() { }

        [Command]
        private void CmdSendName(string playerName)
        {
            _playerName = playerName;
        }
        
        public void DisplayPlayerName(string newName)
        {
            Debug.Log("Player changed name to " + newName);

            _playerName = newName;
            gameObject.name = _playerName;
        }

        [Command]
        private void CmdCommandMethod()
        {

        }

        [ClientRpc]
        private void RpcMethod(int value)
        {
            _shipSpeed *= value;
        }

        [Client]
        private void ClientMethod()
        {

        }

        [ClientCallback]
        private void LateUpdate()
        {
            _cameraOrbit?.CameraMovement();
        }

        [Server]
        private void ServerMethod()
        {

        }

        [ServerCallback]
        private void ServerCalbackMethod()
        {

        }

        [TargetRpc]
        private void TargetMethod(NetworkConnection conn)
        {

        }

        [ServerCallback]
        public void OnTriggerEnter(Collider other)
        {
            if(hasAuthority)
            {

            }
            else
            {

            }
        }
    }
}
