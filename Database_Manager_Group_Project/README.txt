## MQTT Data Processing

### Description:
This code connects to an MQTT broker to process messages received from various LoRa devices. It retrieves data from The Things Network (TTN) and inserts it into an SQL database. The code utilizes the MQTTnet library for MQTT client functionality and Newtonsoft.Json for JSON parsing. It was last updated on 12-1-2024.

### Dependencies:
- MQTTnet.Client
- MQTTnet
- Newtonsoft.Json

### MQTT Broker Configuration:
- Broker: eu1.cloud.thethings.network
- Port: 1883

### TTNCredentials:
- **project-software-engineering@ttn**
  - Username: project-software-engineering@ttn
  - Password: NNSXS.DTT4HTNBXEQDZ4QYU6SG73Q2OXCERCZ6574RVXI.CQE6IG6FYNJOO2MOFMXZVWZE4GXTCC2YXNQNFDLQL4APZMWU6ZGA
- **lora-sensors-projectse**
  - Username: lora-sensors-projectse
  - Password: NNSXS.OKCXJUXTPE2XFQDYALVDCKWFD4KVCG5THFBEDHQ.D6JEYGZGYHKDNV3Q634GFEUBZNVMNOIKH3TADQSHU4ONYV6MSHNQ

### TTNCredentials List:
- **project-software-engineering@ttn**
- **lora-sensors-projectse**

### Functions:
- `Main(string[] args)`: Main entry point of the program. Connects to MQTT broker for each set of credentials and subscribes to all topics.
- `Connect(string username, string password)`: Connects to the MQTT broker using the provided credentials. Subscribes to all topics and processes received MQTT messages.
- `InsertToDeviceReadings(...)`: Inserts data into the device_readings table of the SQL database based on device type.
- `InsertToAirtime(...)`: Inserts data into the airtime table of the SQL database based on device type.
- `GetGatewayId(string location)`: Retrieves gateway ID from the database based on location.
- `GetDeviceId(string device_name)`: Retrieves device ID from the database based on device name.
- `InsertGateway(...)`: Inserts a new gateway into the database.
- `InsertDevice(...)`: Inserts a new device into the database.

### Usage:
1. Configure MQTT broker settings and TTNCredentials.
2. Run the program to connect to the MQTT broker and start processing messages.
3. Ensure the SQL database is configured with the necessary tables.

### Notes:
- Modify the MQTT broker settings and TTNCredentials according to your configuration.
- Ensure the SQL database connection details are correctly set in the DatabaseConnection class.
- Make sure to handle exceptions and errors appropriately for robust operation.
- This code uses asynchronous programming with async/await for efficient message processing.
- The database Entity-Relationship Diagram is in this directory as a png.
- The database is no longer up on the azure cloud so it can no longer be accessed.
