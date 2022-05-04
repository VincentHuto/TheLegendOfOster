using Mirror;
using System;
using TMPro;
using UnityEngine;

public class ChatBehavior : NetworkBehaviour
{
    [SerializeField] private GameObject chatUI = null;
    [SerializeField] private TMP_Text chatText = null;
    [SerializeField] private TMP_InputField inputField = null;
    [SerializeField] public InputHandler inputHandler;

   private static event Action<string> OnMessage;



    private void Update()
    {
        if (inputField.isFocused)
        {
            inputHandler.inputActions.Disable();
        }
        else
        {
            inputHandler.inputActions.Enable();
        }
    }

    public void ToggleWindow()
    {
        chatUI.gameObject.SetActive(!chatUI.activeSelf);
    }

    public override void OnStartAuthority()
    {
        chatUI.SetActive(true);

        OnMessage += HandleNewMessage;
        chatUI.SetActive(false);

    }

    [ClientCallback]
    private void OnDestroy()
    {
        if (!hasAuthority) { return; }

        OnMessage -= HandleNewMessage;
    }

    private void HandleNewMessage(string message)
    {
        chatText.text += message;
    }

    [Client]
    public void Send(TMP_InputField tMP_InputField)
    {

        if (!inputHandler.send_chat_Input) { return; }

        string message = tMP_InputField.text ;
        if (string.IsNullOrWhiteSpace(message)) { return; }

        CmdSendMessage(message);

        inputField.text = string.Empty;
    }

    [Command]
    private void CmdSendMessage(string message)
    {
        Debug.Log($"[{connectionToClient.connectionId}]: {message}");
        RpcHandleMessage($"[{connectionToClient.connectionId}]: {message}");
    }

    [ClientRpc]
    private void RpcHandleMessage(string message)
    {
        OnMessage?.Invoke($"\n{message}");
    }

   
}
