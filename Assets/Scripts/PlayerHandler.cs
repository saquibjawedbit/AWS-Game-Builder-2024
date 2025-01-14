using Mirror;
using UnityEngine;
using GoSystem;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class PlayerHandler : NetworkBehaviour
{
    public GameObject cameraPrefab;
    // Start is called before the first frame update
    public override void OnStartLocalPlayer()
    {
        // Spawn the camera for the local player
        GameObject cameraInstance = Instantiate(cameraPrefab);
        Cursor.lockState = CursorLockMode.Locked;
       

    }

   [ClientRpc]
    public void RpcUpdateCameraTargets()
    {
        if (!isLocalPlayer)
        {
            Transform headTransform = transform.Find("Bones").Find("Hips").Find("Spine").Find("Spine1").Find("Spine2").Find("Neck").Find("Head");
            print("Head Transform: " + headTransform);
            if (headTransform == null)
            {
                Debug.LogWarning("Head transform not found on player object!");
                return;
            }

            Camera.main.GetComponent<GoCameraSystem>().target = headTransform;
            Debug.Log("Camera target set to a remote player's Head.");
        }
    }

}
