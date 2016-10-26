using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine.UI;


public class GameScript : MonoBehaviour {


    public Sprite plyrOne; // Drag your first sprite here
    public Transform tOne;
    public Sprite plyrTwo; // Drag your first sprite here
    public Transform tTwo;


    string PlayerOneIP = "149.153.106.160";
    int portA = 8001;
    //public string Bip = "149.153.106.176";
    string PlayerTwoIP = "149.153.106.176";
    int portB = 8002;

    int socketPlayerOne;
    int socketPlayerTwo;

    bool isAorB;
    bool State; // true if state based, false is input


    string input;

    int myReiliableChannelId;
    int connectionID;
    ConnectionConfig config;
    HostTopology topology;


    // Use this for initialization
    void Start () {
        State = true;
        NetworkTransport.Init();
        config = new ConnectionConfig();
        myReiliableChannelId = config.AddChannel(QosType.Reliable);
        topology = new HostTopology(config, 10);
        
       
    }


    public void setupA()
    {
        socketPlayerOne = NetworkTransport.AddHost(topology, portA);
        //socketPlayerOne = NetworkTransport.AddHost(topology, portA);
        isAorB = true;
    }
    public void setupB()
    {
        socketPlayerTwo = NetworkTransport.AddHost(topology, portB);
        //socketPlayerTwo = NetworkTransport.AddHost(topology, portB);
        isAorB = false;
    }

    public void join()
    {
        if(!isAorB)
        {
            byte error = 0;
            connectionID = NetworkTransport.Connect(socketPlayerOne, PlayerOneIP, portA, 0, out error);
        }
        else
        {
          
            byte error = 0;
            connectionID = NetworkTransport.Connect(socketPlayerTwo, PlayerTwoIP, portB, 0, out error);
        }

    }

    public void send(string s)
    {
        
        byte error = 0;
        byte[] buffer = new byte[100];
        System.IO.Stream stream = new MemoryStream(buffer);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, s);
        if (isAorB)
        {
            NetworkTransport.Send(socketPlayerOne, connectionID, myReiliableChannelId, buffer, (int)stream.Position, out error);
        }
        else
        {
            NetworkTransport.Send(socketPlayerTwo, connectionID, myReiliableChannelId, buffer, (int)stream.Position, out error);
        }


    }
    public void sendState(string s)
    {

        byte error = 0;
        byte[] buffer = new byte[100];
        System.IO.Stream stream = new MemoryStream(buffer);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, s);
        if (isAorB)
        {
            NetworkTransport.Send(socketPlayerOne, connectionID, myReiliableChannelId, buffer, (int)stream.Position, out error);
        }
        else
        {
            NetworkTransport.Send(socketPlayerTwo, connectionID, myReiliableChannelId, buffer, (int)stream.Position, out error);
        }


    }


    public void recieve()
    {
        int remoteSocketId, remoteConnectionId, remoteChannelId, bufferSize = 500, dataSize;
        if (State)
        {
            if (isAorB)
            {
                byte[] recBuffer = new byte[500];
                byte error = 0;
                NetworkEventType receivedData = NetworkTransport.Receive(out remoteSocketId, out remoteConnectionId, out remoteChannelId, recBuffer, bufferSize, out dataSize, out error);
                switch (receivedData)
                {
                    case NetworkEventType.Nothing:         //1
                        break;
                    case NetworkEventType.ConnectEvent:    //2
                        Debug.Log("Connect event A");
                        break;
                    case NetworkEventType.DataEvent:       //3

                        Stream stream = new MemoryStream(recBuffer);
                        BinaryFormatter formatter = new BinaryFormatter();
                        input = formatter.Deserialize(stream) as string;

                        Debug.Log("Message received: " + input);
                        if ((input == "true") || (input == "false"))
                        {
                            if (input == "true")
                            {
                                State = true;
                            }
                            else
                            {
                                State = false;
                            }

                        }
                        else
                        {
                            tTwo.position = DeserializeVector3Array(input);
                        }



                        break;

                    case NetworkEventType.DisconnectEvent: //4
                        break;
                }
            }
            else
            {
                byte[] recBuffer = new byte[500];
                byte error = 0;
                NetworkEventType receivedData = NetworkTransport.Receive(out remoteSocketId, out remoteConnectionId, out remoteChannelId, recBuffer, bufferSize, out dataSize, out error);
                switch (receivedData)
                {
                    case NetworkEventType.Nothing:         //1
                        break;
                    case NetworkEventType.ConnectEvent:    //2
                        Debug.Log("Connect event B");
                        break;
                    case NetworkEventType.DataEvent:       //3

                        Stream stream = new MemoryStream(recBuffer);
                        BinaryFormatter formatter = new BinaryFormatter();
                        input = formatter.Deserialize(stream) as string;
                        Debug.Log("Message received: " + input);
                        if ((input == "true") || (input == "false"))
                        {
                            if (input == "true")
                            {
                                State = true;
                            }
                            else
                            {
                                State = false;
                            }

                        }
                        else
                        {
                            tOne.position = DeserializeVector3Array(input);
                        }



                        break;

                    case NetworkEventType.DisconnectEvent: //4
                        break;
                }
            }
        }
        else
        {
            if (isAorB)
            {
                byte[] recBuffer = new byte[500];
                byte error = 0;
                NetworkEventType receivedData = NetworkTransport.Receive(out remoteSocketId, out remoteConnectionId, out remoteChannelId, recBuffer, bufferSize, out dataSize, out error);
                switch (receivedData)
                {
                    case NetworkEventType.Nothing:         //1
                        break;
                    case NetworkEventType.ConnectEvent:    //2
                        Debug.Log("Connect event A");
                        break;
                    case NetworkEventType.DataEvent:       //3

                        Stream stream = new MemoryStream(recBuffer);
                        BinaryFormatter formatter = new BinaryFormatter();
                        input = formatter.Deserialize(stream) as string;

                        Debug.Log("Message received: " + input);
                        if ((input == "true") || (input == "false"))
                        {
                            if (input == "true")
                            {
                                State = true;
                            }
                            else
                            {
                                State = false;
                            }

                        }
                        else
                        {
                            inputbasedMovement(input);
                        }



                        break;

                    case NetworkEventType.DisconnectEvent: //4
                        break;
                }
            }
            else
            {
                byte[] recBuffer = new byte[500];
                byte error = 0;
                NetworkEventType receivedData = NetworkTransport.Receive(out remoteSocketId, out remoteConnectionId, out remoteChannelId, recBuffer, bufferSize, out dataSize, out error);
                switch (receivedData)
                {
                    case NetworkEventType.Nothing:         //1
                        break;
                    case NetworkEventType.ConnectEvent:    //2
                        Debug.Log("Connect event B");
                        break;
                    case NetworkEventType.DataEvent:       //3

                        Stream stream = new MemoryStream(recBuffer);
                        BinaryFormatter formatter = new BinaryFormatter();
                        input = formatter.Deserialize(stream) as string;
                        Debug.Log("Message received: " + input);
                        if ((input == "true") || (input == "false"))
                        {
                            if (input == "true")
                            {
                                State = true;
                            }
                            else
                            {
                                State = false;
                            }

                        }
                        else
                        {
                            inputbasedMovement(input);
                        }


                        break;

                    case NetworkEventType.DisconnectEvent: //4
                        break;
                }
            }
        }
        
      
    }

    // Update is called once per frame
    void Update () {
        getKeyInput();
        recieve();
    }

    void getKeyInput()
    {
        if (State)
        {
            if (isAorB)
            {
                Vector3 temp = new Vector3(tOne.position.x, tOne.position.y, tOne.position.z);
                if (Input.GetKeyDown("w"))
                {
                    tOne.position =
                    Vector3.MoveTowards(tOne.position, new Vector3(1, 3, 100), .4f);
                    send(SerializeVector3Array(tOne.position));
                }
                if (Input.GetKeyDown("a"))
                {
                    tOne.position =
                    Vector3.MoveTowards(tOne.position, new Vector3(15, 3, 100), .4f);
                    send(SerializeVector3Array(tOne.position));
                }
                if (Input.GetKeyDown("s"))
                {

                    tOne.position =
                    Vector3.MoveTowards(tOne.position, new Vector3(1, 3, 100), .4f);
                    send(SerializeVector3Array(tOne.position));
                }
                if (Input.GetKeyDown("d"))
                {

                    tOne.position =
                    Vector3.MoveTowards(tOne.position, new Vector3(1, 3, 100), .4f);
                    send(SerializeVector3Array(tOne.position));
                }
                if (Input.GetKeyDown("p"))
                {
                    Debug.Log("State Input Switch");
                    State = false;
                    sendState("false");

                }
            }
            else
            {
                Vector3 temp = new Vector3(tTwo.position.x, tTwo.position.y, tTwo.position.z);
                if (Input.GetKeyDown("w"))
                {

                    tTwo.position =
                    Vector3.MoveTowards(tTwo.position, new Vector3(1, 3, 100), .4f);
                    send(SerializeVector3Array(tTwo.position));
                }
                if (Input.GetKeyDown("a"))
                {
                    tTwo.position =
                    Vector3.MoveTowards(tTwo.position, new Vector3(15, 3, 100), .4f);
                    send(SerializeVector3Array(tTwo.position));
                }
                if (Input.GetKeyDown("s"))
                {

                    tTwo.position =
                    Vector3.MoveTowards(tTwo.position, new Vector3(1, 3, 100), .4f);
                    send(SerializeVector3Array(tTwo.position));
                }
                if (Input.GetKeyDown("d"))
                {

                    tTwo.position =
                    Vector3.MoveTowards(tTwo.position, new Vector3(1, 3, 100), .4f);
                    send(SerializeVector3Array(tTwo.position));
                }
                if (Input.GetKeyDown("p"))
                {
                    Debug.Log("State Input Switch");
                    State = false;
                    sendState("false");
                }
            }
        }
        else
        {
            if (isAorB)
            {
                Vector3 temp = new Vector3(tOne.position.x, tOne.position.y, tOne.position.z);
                if (Input.GetKeyDown("w"))
                {
                    tOne.position =
                    Vector3.MoveTowards(tOne.position, new Vector3(1, 3, 100), .4f);
                    send("w");
                }
                if (Input.GetKeyDown("a"))
                {
                    tOne.position =
                    Vector3.MoveTowards(tOne.position, new Vector3(15, 3, 100), .4f);
                    send("a");
                }
                if (Input.GetKeyDown("s"))
                {

                    tOne.position =
                    Vector3.MoveTowards(tOne.position, new Vector3(1, 3, 100), .4f);
                    send("s");
                }
                if (Input.GetKeyDown("d"))
                {

                    tOne.position =
                    Vector3.MoveTowards(tOne.position, new Vector3(1, 3, 100), .4f);
                    send("d");
                }
                if (Input.GetKeyDown("p"))
                {
                    Debug.Log("State Input Switch");
                    State = true;
                    sendState("true");
                }
            }
            else
            {
                Vector3 temp = new Vector3(tTwo.position.x, tTwo.position.y, tTwo.position.z);
                if (Input.GetKeyDown("w"))
                {

                    tTwo.position =
                    Vector3.MoveTowards(tTwo.position, new Vector3(1, 3, 100), .4f);
                    send("w");
                }
                if (Input.GetKeyDown("a"))
                {
                    tTwo.position =
                    Vector3.MoveTowards(tTwo.position, new Vector3(15, 3, 100), .4f);
                    send("a");
                }
                if (Input.GetKeyDown("s"))
                {

                    tTwo.position =
                    Vector3.MoveTowards(tTwo.position, new Vector3(1, 3, 100), .4f);
                    send("s");
                }
                if (Input.GetKeyDown("d"))
                {

                    tTwo.position =
                    Vector3.MoveTowards(tTwo.position, new Vector3(1, 3, 100), .4f);
                    send("d");
                }
                if (Input.GetKeyDown("p"))
                {
                    Debug.Log("State Input Switch");
                    State = true;
                    sendState("true");
                }
            }
        }
        
    }



    private static string SerializeVector3Array(Vector3 aVector)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(aVector.x).Append(" ").Append(aVector.y).Append(" ").Append(aVector.z).Append("|");

        sb.Remove(sb.Length - 1, 1);
        return sb.ToString();
    }



    private static Vector3 DeserializeVector3Array(string aData)
    {

        Vector3 result = new Vector3();

        string[] values = aData.Split(' ');
        if (values.Length != 3)
            {
                throw new System.FormatException("component count mismatch. Expected 3 components but got " + values.Length);
            }
        result = new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
        return result;
    }

    public void inputbasedMovement(string c)
    {
        if (isAorB)
        {
            Vector3 temp = new Vector3(tOne.position.x, tOne.position.y, tOne.position.z);
            if (c == "w")
            {
                tOne.position =
                Vector3.MoveTowards(tOne.position, new Vector3(1, 3, 100), .4f);
                send(SerializeVector3Array(tOne.position));
            }
            if(c == "a")
            {
                tOne.position =
                Vector3.MoveTowards(tOne.position, new Vector3(15, 3, 100), .4f);
                send(SerializeVector3Array(tOne.position));
            }
            if (c == "s")
                {

                tOne.position =
                Vector3.MoveTowards(tOne.position, new Vector3(1, 3, 100), .4f);
                send(SerializeVector3Array(tOne.position));
            }
            if  (c == "d")
                {

                tOne.position =
                Vector3.MoveTowards(tOne.position, new Vector3(1, 3, 100), .4f);
                send(SerializeVector3Array(tOne.position));
            }
        }
        else
        {
            Vector3 temp = new Vector3(tTwo.position.x, tTwo.position.y, tTwo.position.z);
            if (c == "w")
            {

                tTwo.position =
                Vector3.MoveTowards(tTwo.position, new Vector3(1, 3, 100), .4f);
                send(SerializeVector3Array(tTwo.position));
            }
            if (c == "a")
            {
                tTwo.position =
                Vector3.MoveTowards(tTwo.position, new Vector3(15, 3, 100), .4f);
                send(SerializeVector3Array(tTwo.position));
            }
            if (c == "s")
            {

                tTwo.position =
                Vector3.MoveTowards(tTwo.position, new Vector3(1, 3, 100), .4f);
                send(SerializeVector3Array(tTwo.position));
            }
            if (c == "d")
            {

                tTwo.position =
                Vector3.MoveTowards(tTwo.position, new Vector3(1, 3, 100), .4f);
                send(SerializeVector3Array(tTwo.position));
            }
        }
    }


}
