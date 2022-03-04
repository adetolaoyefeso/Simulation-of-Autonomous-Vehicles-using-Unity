using UnityEngine;
//using System.Net;
using System.Net.Sockets;
using System;

public class SocketFloat : MonoBehaviour
{
    public string ip = "127.0.0.1";
    public int port = 60000;
    private Socket client;
    [SerializeField]
    private float[] dataOut, dataIn; //debugging
    public string message;
    public CameraSensor cs;

    float time;
    float[] tempMessage = {2.0f,5.0f,6.0f};



    //camsensor object
    // public CameraSensor cs;


//unitystart function//can also use awake**
    public void Start()
    {
    	client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        client.Connect(ip, port);

        // if (!client.Connected) {
        //     Debug.LogError("Connection Failed");
        //     return null; 
        // }
    }

    public void Update(){

    	//dont do anything
    	if (!client.Connected) {
            Debug.LogError("Connection Failed");
            return; 
        }

        if(cs == null){
        	return;
        }

        time += Time.deltaTime;

        //do every 10 seconds
        if(time >= 10.0){

        	// in here
        	//call function to send/recieve messages
        	//in that function see hint
        	SendAndReceive();
        }



    }

    ///Hint
    //look into logger for savecam function
    //similar process only that the images arent being stored
    //send byte data to python


    /// <summary>
    /// Helper function for sending and receiving.
    /// </summary>
    /// <param name="dataOut">Data to send</param>
    /// <returns></returns>
    protected float[] ServerRequest(float[] dataOut)
    {
        //print("request");
        this.dataOut = dataOut; //debugging
        // this.dataIn = SendAndReceive(dataOut); //debugging
        return this.dataIn;
    }

    /// <summary> 
    /// Send data to port, receive data from port.
    /// </summary>
    /// <param name="dataOut">Data to send</param>
    /// <returns></returns>
    private float[] SendAndReceive()//float[] dataOut)
    {
        //initialize socket
        float[] floatsReceived;
       
   

        Texture2D image = cs.GetImage();


        //convert floats to bytes, send to port
        var byteArray = new byte[dataOut.Length * 4];
        Buffer.BlockCopy(dataOut, 0, byteArray, 0, byteArray.Length);
        client.Send(image.EncodeToPNG());

        //allocate and receive bytes
        byte[] bytes = new byte[4000];
        int idxUsedBytes = client.Receive(bytes);
        //print(idxUsedBytes + " new bytes received.");

        //convert bytes to floats
        floatsReceived = new float[idxUsedBytes/4];
        Buffer.BlockCopy(bytes, 0, floatsReceived, 0, idxUsedBytes);

        // client.Close();
        return floatsReceived;
    }
}