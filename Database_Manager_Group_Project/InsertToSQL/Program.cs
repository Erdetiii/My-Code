/**
 * @file Program.cs
 * @version 1.0
 * @brief Main program for connecting to MQTT broker and processing messages.
 * 
 * This file contains the implementation of how to get the data from the TTN.
 * 
 * @date 12-1-2024
 * @copyright Clion License.
 */

using MQTTnet.Client;
using MQTTnet;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.Json;
using System.Runtime.InteropServices;
using InsertToSQL;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using test;


/**
 * @class Program
 * @brief Main program class for connecting to MQTT broker and processing messages.
 */
public class Program
{   
    /**
     * @brief Main entry point of the program.
     * @param args Command line arguments.
     */
    public static async Task Main(string[] args)
    {
        // List of TTNCredentials instances
        var credentialsList = new List<TTNCredentials>
        {
            new TTNCredentials
            {
                Username = "project-software-engineering@ttn",
                Password = "NNSXS.DTT4HTNBXEQDZ4QYU6SG73Q2OXCERCZ6574RVXI.CQE6IG6FYNJOO2MOFMXZVWZE4GXTCC2YXNQNFDLQL4APZMWU6ZGA"
            },
            new TTNCredentials
            {
                Username = "lora-sensors-projectse",
                Password = "NNSXS.OKCXJUXTPE2XFQDYALVDCKWFD4KVCG5THFBEDHQ.D6JEYGZGYHKDNV3Q634GFEUBZNVMNOIKH3TADQSHU4ONYV6MSHNQ"
            }
        };
        // Connect to MQTT broker for each set of credentials
        foreach (var credentials in credentialsList)
        {
            await Connect(credentials.Username, credentials.Password);
        }
        // Infinite loop to keep the program running
        while (true) { }
    }
    /**
   * @brief Connects to the MQTT broker using the provided credentials.
   * @param username MQTT username.
   * @param password MQTT password.
   * @return Asynchronous task.
   */
    public static async Task Connect(string username, string password)
    {
        // MQTT client setup
        var factory = new MqttFactory();
        var client = factory.CreateMqttClient();

        // MQTT broker settings
        var port = 1883;
        var broker = "eu1.cloud.thethings.network";

        // Connection options
        var options = new MqttClientOptionsBuilder()
            .WithTcpServer(broker, port)
            .WithCredentials(username, password)
            .Build();
        // Connect to the MQTT broker
        var result = await client.ConnectAsync(options);
        // Check connection result
        if (result.ResultCode == MqttClientConnectResultCode.Success)
        {
            Console.WriteLine($"connected with {username}");
        }
        else
        {
            Console.WriteLine($"connection failed for {username}");
            Environment.Exit(1);
        }


        // Subscribe to all topics
        await client.SubscribeAsync("#");
        // Event handler for processing received MQTT messages
        client.ApplicationMessageReceivedAsync += e =>
        {
            try
            {
                string jsonString = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

                var parsedJson = JObject.Parse(jsonString);

                string deviceId = parsedJson["end_device_ids"]["device_id"].ToString();

                if (deviceId == "mkr-tester" || deviceId == "mkr-wierden")
                {
                    double outdoorTemp = (double)parsedJson["uplink_message"]["decoded_payload"]["temperature"];
                    double light = (double)parsedJson["uplink_message"]["decoded_payload"]["light"];
                    double pressure = (double)parsedJson["uplink_message"]["decoded_payload"]["pressure"];
                    double humidity = (double)parsedJson["uplink_message"]["decoded_payload"]["humidity"];
                    string gatewayId = parsedJson["uplink_message"]["rx_metadata"][0]["gateway_ids"]["gateway_id"].ToString();
                    double latitude = (double)parsedJson["uplink_message"]["rx_metadata"][0]["location"]["latitude"];
                    double longitude = (double)parsedJson["uplink_message"]["rx_metadata"][0]["location"]["longitude"];
                    int rssi = (int)parsedJson["uplink_message"]["rx_metadata"][0]["rssi"];
                    int snr = (int)parsedJson["uplink_message"]["rx_metadata"][0]["snr"];
                    string consumedAirtime = parsedJson["uplink_message"]["consumed_airtime"].ToString();
                    string consumedAirtime_ = consumedAirtime.Substring(0, consumedAirtime.Length - 1);
                    double cons = Convert.ToDouble(consumedAirtime_);
                    DatabaseConnection dbcon = new DatabaseConnection();
                    dbcon.InsertToDeviceReadings(deviceId, outdoorTemp.ToString(), gatewayId, latitude.ToString(), longitude.ToString(), light.ToString(), humidity.ToString(), pressure.ToString());
                    dbcon.InsertToAirtime(deviceId, gatewayId, snr, rssi, cons, latitude.ToString(), longitude.ToString());
                    Console.WriteLine($"Device ID: {deviceId}");
                    Console.WriteLine($"Outdoor Temperature: {outdoorTemp}");
                    Console.WriteLine($"light : {light}");
                    Console.WriteLine($"pressure : {pressure}");
                    Console.WriteLine($"Humidity: {humidity}");
                    Console.WriteLine($"Gateway ID: {gatewayId}");
                    Console.WriteLine($"Latitude: {latitude}");
                    Console.WriteLine($"Longitude: {longitude}");
                    Console.WriteLine($"RSSI: {rssi}");
                    Console.WriteLine($"SNR: {snr}");
                    Console.WriteLine($"Consumed Airtime: {consumedAirtime}");
                }
                else if (deviceId == "arduino-lora-sensors-group12-2023")
                {
                    double humidity = (double)parsedJson["uplink_message"]["decoded_payload"]["decoded_payload"]["humidity"];
                    double light = (double)parsedJson["uplink_message"]["decoded_payload"]["decoded_payload"]["light"];
                    double pressure = (double)parsedJson["uplink_message"]["decoded_payload"]["decoded_payload"]["pressure"];
                    double temperature = (double)parsedJson["uplink_message"]["decoded_payload"]["decoded_payload"]["temperature"];
                    string gatewayId = parsedJson["uplink_message"]["rx_metadata"][0]["gateway_ids"]["gateway_id"].ToString();
                    double latitude = (double)parsedJson["uplink_message"]["rx_metadata"][0]["location"]["latitude"];
                    double longitude = (double)parsedJson["uplink_message"]["rx_metadata"][0]["location"]["longitude"];
                    double rssi = (double)parsedJson["uplink_message"]["rx_metadata"][0]["rssi"];
                    double snr = (double)parsedJson["uplink_message"]["rx_metadata"][0]["snr"];
                    string consumedAirtime = parsedJson["uplink_message"]["consumed_airtime"].ToString();
                    string consumedAirtime_ = consumedAirtime.Substring(0, consumedAirtime.Length - 1);
                    double cons = Convert.ToDouble(consumedAirtime_);
                    DatabaseConnection dbcon = new DatabaseConnection();
                    dbcon.InsertToDeviceReadings(deviceId, temperature.ToString(), gatewayId, latitude.ToString(), longitude.ToString(), light.ToString(), humidity.ToString(), pressure.ToString());
                    dbcon.InsertToAirtime(deviceId, gatewayId, snr, rssi, cons, latitude.ToString(), longitude.ToString());

                    Console.WriteLine($"Device ID: {deviceId}");
                    Console.WriteLine($"Humidity: {humidity}");
                    Console.WriteLine($"Illuminance: {light}");
                    Console.WriteLine($"Pressure: {pressure}");
                    Console.WriteLine($"Temperature: {temperature}");
                    Console.WriteLine($"Gateway ID: {gatewayId}");
                    Console.WriteLine($"Latitude: {latitude}");
                    Console.WriteLine($"Longitude: {longitude}");
                    Console.WriteLine($"RSSI: {rssi}");
                    Console.WriteLine($"SNR: {snr}");
                    Console.WriteLine($"Consumed Airtime: {consumedAirtime}");
                }
                else if (deviceId == "lht-gronau")
                {
                    double batV = (double)parsedJson["uplink_message"]["decoded_payload"]["BatV"];
                    double outdoorTemp = (double)parsedJson["uplink_message"]["decoded_payload"]["TempC_SHT"];
                    string pressure = "null";
                    double light = (double)parsedJson["uplink_message"]["decoded_payload"]["ILL_lx"];
                    double humidity = (double)parsedJson["uplink_message"]["decoded_payload"]["Hum_SHT"];
                    string gatewayId = parsedJson["uplink_message"]["rx_metadata"][0]["gateway_ids"]["gateway_id"].ToString();
                    double latitude = (double)parsedJson["uplink_message"]["rx_metadata"][0]["location"]["latitude"];
                    double longitude = (double)parsedJson["uplink_message"]["rx_metadata"][0]["location"]["longitude"];
                    int rssi = (int)parsedJson["uplink_message"]["rx_metadata"][0]["rssi"];
                    int snr = (int)parsedJson["uplink_message"]["rx_metadata"][0]["snr"];
                    string consumedAirtime = parsedJson["uplink_message"]["consumed_airtime"].ToString();
                    string consumedAirtime_ = consumedAirtime.Substring(0, consumedAirtime.Length - 1);
                    double cons = Convert.ToDouble(consumedAirtime_);
                    DatabaseConnection dbcon = new DatabaseConnection();
                    dbcon.InsertToDeviceReadings(deviceId, outdoorTemp.ToString(), gatewayId, latitude.ToString(), longitude.ToString(), light.ToString(), humidity.ToString(), pressure.ToString());
                    dbcon.InsertToAirtime(deviceId, gatewayId, snr, rssi, cons, latitude.ToString(), longitude.ToString());


                    Console.WriteLine($"Device ID: {deviceId}");
                    Console.WriteLine($"Battery Voltage: {batV}");
                    Console.WriteLine($"Outdoor Temperature: {outdoorTemp}");
                    Console.WriteLine($"Pressure: {pressure}");
                    Console.WriteLine($"Humidity: {humidity}");
                    Console.WriteLine($"Light: {light}");
                    Console.WriteLine($"Gateway ID: {gatewayId}");
                    Console.WriteLine($"Latitude: {latitude}");
                    Console.WriteLine($"Longitude: {longitude}");
                    Console.WriteLine($"RSSI: {rssi}");
                    Console.WriteLine($"SNR: {snr}");
                    Console.WriteLine($"Consumed Airtime: {consumedAirtime}");
                }
                else if (deviceId == "lht-saxion")
                {
                    double batV = (double)parsedJson["uplink_message"]["decoded_payload"]["BatV"];
                    double outdoorTemp = (double)parsedJson["uplink_message"]["decoded_payload"]["TempC_DS"];
                    double indoorTemp = (double)parsedJson["uplink_message"]["decoded_payload"]["TempC_SHT"];
                    string pressure = "null";
                    string light = "null";
                    double humidity = (double)parsedJson["uplink_message"]["decoded_payload"]["Hum_SHT"];
                    string gatewayId = parsedJson["uplink_message"]["rx_metadata"][0]["gateway_ids"]["gateway_id"].ToString();
                    double latitude = (double)parsedJson["uplink_message"]["rx_metadata"][0]["location"]["latitude"];
                    double longitude = (double)parsedJson["uplink_message"]["rx_metadata"][0]["location"]["longitude"];
                    int rssi = (int)parsedJson["uplink_message"]["rx_metadata"][0]["rssi"];
                    int snr = (int)parsedJson["uplink_message"]["rx_metadata"][0]["snr"];
                    string consumedAirtime = parsedJson["uplink_message"]["consumed_airtime"].ToString();
                    string consumedAirtime_ = consumedAirtime.Substring(0, consumedAirtime.Length - 1);
                    double cons = Convert.ToDouble(consumedAirtime_);
                    DatabaseConnection dbcon = new DatabaseConnection();
                    dbcon.InsertToDeviceReadings(deviceId, outdoorTemp.ToString(), gatewayId, latitude.ToString(), longitude.ToString(), light.ToString(), humidity.ToString(), pressure.ToString(), indoorTemp.ToString(), batV.ToString());
                    dbcon.InsertToAirtime(deviceId, gatewayId, snr, rssi, cons, latitude.ToString(), longitude.ToString(), batV.ToString());

                    Console.WriteLine($"Device ID: {deviceId}");
                    Console.WriteLine($"Battery Voltage: {batV}");
                    Console.WriteLine($"Outdoor Temperature: {outdoorTemp}");
                    Console.WriteLine($"Pressure: {pressure}");
                    Console.WriteLine($"Indoor Temperature: {indoorTemp}");
                    Console.WriteLine($"Humidity: {humidity}");
                    Console.WriteLine($"Light: {light}");
                    Console.WriteLine($"Gateway ID: {gatewayId}");
                    Console.WriteLine($"Latitude: {latitude}");
                    Console.WriteLine($"Longitude: {longitude}");
                    Console.WriteLine($"RSSI: {rssi}");
                    Console.WriteLine($"SNR: {snr}");
                    Console.WriteLine($"Consumed Airtime: {consumedAirtime}");
                }
                else
                {
                    double batV = (double)parsedJson["uplink_message"]["decoded_payload"]["BatV"];
                    double outdoorTemp = (double)parsedJson["uplink_message"]["decoded_payload"]["TempC_SHT"];
                    string pressure = "null";
                    double humidity = (double)parsedJson["uplink_message"]["decoded_payload"]["Hum_SHT"];
                    string gatewayId = parsedJson["uplink_message"]["rx_metadata"][0]["gateway_ids"]["gateway_id"].ToString();
                    double latitude = (double)parsedJson["uplink_message"]["rx_metadata"][0]["location"]["latitude"];
                    double longitude = (double)parsedJson["uplink_message"]["rx_metadata"][0]["location"]["longitude"];
                    int rssi = (int)parsedJson["uplink_message"]["rx_metadata"][0]["rssi"];
                    int snr = (int)parsedJson["uplink_message"]["rx_metadata"][0]["snr"];
                    string consumedAirtime = parsedJson["uplink_message"]["consumed_airtime"].ToString();
                    string consumedAirtime_ = consumedAirtime.Substring(0, consumedAirtime.Length - 1);
                    double cons = Convert.ToDouble(consumedAirtime_);
                    DatabaseConnection dbcon = new DatabaseConnection();
                    dbcon.InsertToDeviceReadings(deviceId, outdoorTemp.ToString(), gatewayId, latitude.ToString(), longitude.ToString(), "null", humidity.ToString(), pressure.ToString());
                    dbcon.InsertToAirtime(deviceId, gatewayId, snr, rssi, cons, latitude.ToString(), longitude.ToString());

                    Console.WriteLine($"Device ID: {deviceId}");
                    Console.WriteLine($"Battery Voltage: {batV}");
                    Console.WriteLine($"Outdoor Temperature: {outdoorTemp}");
                    Console.WriteLine($"Pressure: {pressure}");
                    Console.WriteLine($"Humidity: {humidity}");
                    Console.WriteLine($"Gateway ID: {gatewayId}");
                    Console.WriteLine($"Latitude: {latitude}");
                    Console.WriteLine($"Longitude: {longitude}");
                    Console.WriteLine($"RSSI: {rssi}");
                    Console.WriteLine($"SNR: {snr}");
                    Console.WriteLine($"Consumed Airtime: {consumedAirtime}");
                }

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            return Task.CompletedTask;
        };
    }

}

