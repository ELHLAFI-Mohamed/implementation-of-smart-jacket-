using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class Wifi_filter : MonoBehaviour
{
    string CurrentValue = System.String.Empty;
    Thread thread;
    bool StateClient;
    public GameObject target; // is the gameobject to
    float gx, gy, gz;

    void Start()
    {
        //This will do the network stuff
        Begin("192.168.137.127", 80);
    }

      

    void Update()
    {


        string dataString = CurrentValue;
        Debug.Log(dataString);
        char splitChar = ';';
        string[] dataRaw = dataString.Split(splitChar);
        gx = float.Parse(dataRaw[0]);
        gy = float.Parse(dataRaw[1]);
        gz = float.Parse(dataRaw[2]);
        target.transform.eulerAngles = new Vector3(gx, gy, gz);

    }

    public void Begin(string ipAddress, int port)
    {
        //Give the network stuff its own special thread
        thread = new Thread(() =>
        {
            //This class makes it super easy to do network stuff
            var client = new TcpClient();

            //Change this to your real device address
            client.Connect(ipAddress, port);

            var stream = new System.IO.StreamReader(client.GetStream());

            //We'll read values and buffer them up in here
            var buffer = new List<byte>();

            StateClient = client.Connected;
            while (StateClient)
            {
                //Read the next byte
                var read = stream.Read();

                //We split readings with a carriage return, so check for it
                Debug.Log("he want to enter\n");
                if (read == 10)
                {

                    //Once we have a reading, convert our buffer to a string, since the values are comming as strings
                    var str = Encoding.ASCII.GetString(buffer.ToArray());

                    Debug.Log("Unity has read this value:" + str + "\n");

                    CurrentValue = str;
                    Debug.Log(CurrentValue.Length);
                    //Clear the buffer ready for another reading
                    buffer.Clear();

                }
                else
                    //if this was not the end of a reading, then just add this new byte to our buffer
                    buffer.Add((byte)read);


                Debug.Log("i'm still retrieving data :" + buffer + "\n");
            }

        });
        thread.Start();

    }


    void OnDestroy()
    {
        StateClient = false;
    }
     
}