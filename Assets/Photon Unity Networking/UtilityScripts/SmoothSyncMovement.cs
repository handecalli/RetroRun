using UnityEngine;
using System.Collections;

public class SmoothSyncMovement : Photon.MonoBehaviour
{
    public float SmoothingDelay = 5;
    Rigidbody2D rb;

    private Vector2 correctPlayerPos = Vector2.zero; //We lerp towards this
    //private Quaternion correctPlayerRot = Quaternion.identity; //We lerp towards this

    public void Awake()
    {       
        rb = GetComponent<Rigidbody2D>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //We own this player: send the others our data
            stream.SendNext(rb.position);
            // stream.SendNext(transform.rotation); 
        }
        else
        {
            //Network player, receive data
            correctPlayerPos = (Vector2)stream.ReceiveNext();
            // correctPlayerRot = (Quaternion)stream.ReceiveNext();
        }
    }



    public void Update()
    {
        if (!photonView.isMine)
        {
            //Update remote player (smooth this, this looks good, at the cost of some accuracy)
            rb.position = Vector2.Lerp(rb.position, correctPlayerPos, Time.deltaTime * this.SmoothingDelay);
        }
    }

}