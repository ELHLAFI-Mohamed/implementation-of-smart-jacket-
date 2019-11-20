

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEngine;


public class Manager : MonoBehaviour
{
    string CurrentValue = System.String.Empty;
    Thread thread;
    bool StateClient;
    public GameObject target; // is the gameobject to

    //float acc_normalizer_factor = 0.00025f;
    float gyro_normalizer_factor = 1.0f / 32768.0f;   // 32768 is max value captured during test on imu

    float curr_angle_x = 0;
    float curr_angle_y = 0;
    float curr_angle_z = 0;

    float curr_offset_x = 0;
    float curr_offset_y = 0;
    float curr_offset_z = 0;

    // Increase the speed/influence rotation
    public float factor = 7;


    public bool enableRotation;
    //  public bool enableTranslation;


    void Start()
    {
        //This will do the network stuff
        Begin("192.168.137.131", 80);
    }



    void Update()
    {
         
        if (EditorApplication.isPlaying==false)
        {
            Debug.Log("Play this please");
        }

        string dataString = CurrentValue;
        Debug.Log(dataString);
        char splitChar = ';';
        string[] dataRaw = dataString.Split(splitChar);
        Debug.Log(dataRaw);

        // normalized accelerometer values
        /*  float ax = Int32.Parse(dataRaw[0]) * acc_normalizer_factor;
          float ay = Int32.Parse(dataRaw[1]) * acc_normalizer_factor;
          float az = Int32.Parse(dataRaw[2]) * acc_normalizer_factor;*/

        // normalized gyrocope values
        float gx = Int32.Parse(dataRaw[3]) * gyro_normalizer_factor;
        float gy = Int32.Parse(dataRaw[4]) * gyro_normalizer_factor;
        float gz = Int32.Parse(dataRaw[5]) * gyro_normalizer_factor;

        // prevent
        /* if (Mathf.Abs(ax) - 1 < 0) ax = 0;
         if (Mathf.Abs(ay) - 1 < 0) ay = 0;
         if (Mathf.Abs(az) - 1 < 0) az = 0;


         curr_offset_x += ax;
         curr_offset_y += ay;
         curr_offset_z += 0; // The IMU module have value of z axis of 16600 caused by gravity
         */

        // prevent little noise effect
        if (Mathf.Abs(gx) < 0.025f) gx = 0f;
        if (Mathf.Abs(gy) < 0.025f) gy = 0f;
        if (Mathf.Abs(gz) < 0.025f) gz = 0f;

        curr_angle_x += gx;
        curr_angle_y += gy;
        curr_angle_z += gz;   

        //if (enableTranslation) target.transform.position = new Vector3(curr_offset_x, curr_offset_z, curr_offset_y);
        if (enableRotation) target.transform.rotation = Quaternion.Euler(curr_angle_x * factor, curr_angle_y * factor, curr_angle_z * factor + 100);



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

            var stream = new StreamReader(client.GetStream());

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
     thread.Abort();
    }

}
