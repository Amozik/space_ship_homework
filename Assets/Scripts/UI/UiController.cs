using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UI
{
    public class UiController : MonoBehaviour
    {
        [SerializeField] 
        private InputField _nameField;

        public event Action PlayerNameEntered;
        public void ActivateInputName()
        {
            _nameField.gameObject.SetActive(true); 
        }

        private void Start()
        {
            _nameField.gameObject.SetActive(false);
            _nameField.onEndEdit.AddListener(SetPlayerName);
        }

        private void SetPlayerName(string playerName)
        {
            PlayerPrefs.SetString("PlayerName", playerName);
            PlayerNameEntered?.Invoke();
            _nameField.gameObject.SetActive(false);
        }
    }
}