using UnityEngine;
using System;
using System.Collections;

using client;

public class u3d_binding : MonoBehaviour {

	// Use this for initialization
	void Start () {
        _client = new client.client();
        _client.connect_server("127.0.0.1", 3236);

        tick = Environment.TickCount;
        tickcount = 0;

        _client.call_logic("helloworld", "call_helloworld", "hello world!");
    }
	
	// Update is called once per frame
	void Update () {
        Int64 tmptick = (Environment.TickCount & UInt32.MaxValue);
        if (tmptick < tick)
        {
            tickcount += 1;
            tmptick = tmptick + tickcount * UInt32.MaxValue;
        }
        tick = tmptick;

        _client.poll(tick);
    }

    private Int64 tickcount;
    private Int64 tick;
    private client.client _client;

}
