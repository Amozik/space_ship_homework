using System;

using UnityEngine;
using UnityEngine.Networking;

namespace Network
{
#pragma warning disable 618
    public abstract class NetworkMovableObject : NetworkBehaviour
#pragma warning restore 618
    {
        protected abstract float Speed { get; }
        protected Action OnUpdateAction { get; set; }
        protected Action OnFixedUpdateAction { get; set; }
        protected Action OnLateUpdateAction { get; set; }
        protected Action OnPreRenderActionAction { get; set; }
        protected Action OnPostRenderAction { get; set; }

        protected UpdatePhase _updatePhase;

#pragma warning disable 618
        [SyncVar] 
        protected Vector3 _serverPosition;
        [SyncVar] 
        protected Vector3 _serverEuler;
#pragma warning restore 618

        public override void OnStartAuthority()
        {
            Initiate();
        }

        protected virtual void Initiate(UpdatePhase updatePhase = UpdatePhase.Update)
        {
            _updatePhase = updatePhase;
            switch (_updatePhase)
            {
                case UpdatePhase.Update:
                    OnUpdateAction += Movement;
                    break;
                case UpdatePhase.FixedUpdate:
                    OnFixedUpdateAction += Movement;
                    break;
                case UpdatePhase.LateUpdate:
                    OnLateUpdateAction += Movement;
                    break;
                case UpdatePhase.PostRender:
                    OnPostRenderAction += Movement;
                    break;
                case UpdatePhase.PreRender:
                    OnPreRenderActionAction += Movement;
                    break;
            }
        }

        private void Update()
        {
            OnUpdateAction?.Invoke();
        }

        private void LateUpdate()
        {
            OnLateUpdateAction?.Invoke();
        }
        private void FixedUpdate()
        {
            OnFixedUpdateAction?.Invoke();
        }
        private void OnPreRender()
        {
            OnPreRenderActionAction?.Invoke();
        }
        private void OnPostRender()
        {
            OnPostRenderAction?.Invoke();
        }

        protected virtual void Movement()
        {
            if (hasAuthority)
            {
                HasAuthorityMovement();
            }
            else
            {
                FromServerUpdate();
            }
        }

        protected abstract void HasAuthorityMovement();

        protected abstract void FromServerUpdate();

        protected abstract void SendToServer();
    }
}
