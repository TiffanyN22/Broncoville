using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using MongoDB.Driver;
// using MongoDB.Bson;

public class ButtonUI_Connect : MonoBehaviour
{
    [SerializeField] private string my_name = "bob";
    public void AtlasConnect()
    {
        Debug.Log("MongoDB Atlas Connected!" + my_name);
    }

    /*
    public void Start()
    {
        const string connectionUri = "mongodb+srv://lleong:KB6TnbaoBZlKB5jR@broncoville.kvdsv3v.mongodb.net/?retryWrites=true&w=majority&appName=broncoville";

        var settings = MongoClientSettings.FromConnectionString(connectionUri);

        // Set the ServerApi field of the settings object to set the version of the Stable API on the client
        settings.ServerApi = new ServerApi(ServerApiVersion.V1);

        // Create a new client and connect to the server
        var client = new MongoClient(settings);

        // Send a ping to confirm a successful connection
        try
        {
            var result = client.GetDatabase("admin").RunCommand<BsonDocument>(new BsonDocument("ping", 1));
            Console.WriteLine("Pinged your deployment. You successfully connected to MongoDB!");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
    */

    
}
