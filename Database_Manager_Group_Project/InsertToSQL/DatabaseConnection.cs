/**
 * @brief Namespace containing classes for inserting data into SQL database.
 */
namespace InsertToSQL
{
    /**
     * @brief Class for establishing connection with SQL database and performing data insertion operations.
     */
    public class DatabaseConnection
    {
        /**
         * @brief Property for SQL server name.
         */
        public string ServerName { get; } = "project-se.database.windows.net";
        /**
         * @brief Property for SQL database name.
         */
        public string DatabaseName { get; } = "ProjectSoftwareEngineering";
        /**
         * @brief Property for SQL username.
         */
        public string UserName { get; } = "project-se";
        /**
         * @brief Property for SQL password.
         */
        public string Password { get; } = "Software123!";
        /**
         * @brief SqlConnection object for database connection.
         */
        SqlConnection connection { get; set; }

        /**
         * @brief Destructor to close the database connection.
         */
        ~DatabaseConnection()
        {
            if (connection.State == System.Data.ConnectionState.Open)
            {
                connection.Close();
                Console.WriteLine("Connection Closed Successfully!");
            }
        }

        /**
         * @brief Retrieves gateway ID from the database based on location.
         * @param location Location of the gateway.
         * @return Gateway ID.
         */
        public int GetGatewayId(string location)
        {
            int gatewayId = 0;

            string connectionString = $"Data Source={ServerName};Initial Catalog={DatabaseName};User ID={UserName};Password={Password}";

            // Create SqlConnection
            using (connection = new SqlConnection(connectionString))
            {
                try
                {
                    // Open the connection
                    connection.Open();
                    Console.WriteLine("Connection For Get GatewayID Opened Successfully!");
                    string query = $"SELECT gateway_id FROM gateway WHERE location = '{location}';";
                    SqlCommand myCommand = new SqlCommand(query, connection);
                    SqlDataReader reader = myCommand.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        gatewayId = Convert.ToInt32(reader["gateway_id"]);
                    }
                    connection.Close();
                    Console.WriteLine("Connection Closed.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
            return gatewayId;
        }

        /**
         * @brief Retrieves device ID from the database based on device name.
         * @param device_name Name of the device.
         * @return Device ID.
         */
        public int GetDeviceId(string device_name)
        {
            int gatewayId = 0;

            string connectionString = $"Data Source={ServerName};Initial Catalog={DatabaseName};User ID={UserName};Password={Password}";

            // Create SqlConnection
            using (connection = new SqlConnection(connectionString))
            {
                try
                {
                    // Open the connection
                    connection.Open();
                    Console.WriteLine("Connection For Get DeviceID Opened Successfully!");
                    string query = $"SELECT device_id FROM device WHERE device_name = '{device_name}';";
                    SqlCommand myCommand = new SqlCommand(query, connection);
                    SqlDataReader reader = myCommand.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        gatewayId = Convert.ToInt32(reader["device_id"]);
                    }
                    connection.Close();
                    Console.WriteLine("Connection Closed.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
            return gatewayId;
        }

        /**
         * @brief Inserts a new gateway into the database.
         * @param gateway_name Name of the gateway.
         * @param latitude Latitude of the gateway.
         * @param longitude Longitude of the gateway.
         */
        public void InsertGateway(string gateway_name, string latitude, string longitude)
        {
            string connectionString = $"Data Source={ServerName};Initial Catalog={DatabaseName};User ID={UserName};Password={Password}";

            // Create SqlConnection
            using (connection = new SqlConnection(connectionString))
            {
                try
                {
                    // Open the connection
                    connection.Open();
                    Console.WriteLine("Connection For Insert Gateway Opened Successfully!");
                    string query = $"INSERT INTO gateway (latitude, longitude, location) VALUES ({latitude}, {longitude}, '{gateway_name}');";
                    SqlCommand myCommand = new SqlCommand(query, connection);
                    myCommand.ExecuteNonQuery();

                    var latest_gateway_id = "SELECT MAX(gateway_id) AS latest_id FROM gateway;";
                    var gateway_cmd = new SqlCommand(latest_gateway_id, connection);
                    var gateway_id = (int)gateway_cmd.ExecuteScalar();
                    connection.Close();
                    Console.WriteLine("Connection Closed.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }

        }

        /**
         * @brief Inserts a new device into the database.
         * @param device_name Name of the device.
         * @param bat_voltage Battery voltage of the device (optional, default is null).
         */
        public void InsertDevice(string device_name, string bat_voltage = "null")
        {
            string connectionString = $"Data Source={ServerName};Initial Catalog={DatabaseName};User ID={UserName};Password={Password}";

            // Create SqlConnection
            using (connection = new SqlConnection(connectionString))
            {
                try
                {
                    // Open the connection
                    connection.Open();
                    Console.WriteLine("Connection For Insert Device Opened Successfully!");
                    string query = $"INSERT INTO device (device_name, bat_voltage) VALUES ('{device_name}', {bat_voltage});";
                    SqlCommand myCommand = new SqlCommand(query, connection);
                    myCommand.ExecuteNonQuery();
                    connection.Close();
                    Console.WriteLine("Connection Closed.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }

        /**
         * @brief Inserts data into device readings table.
         * @param device_name Name of the device.
         * @param temp_outdoor Outdoor temperature.
         * @param gateway_name Name of the gateway.
         * @param latitude Latitude of the gateway (optional, default is null).
         * @param longitude Longitude of the gateway (optional, default is null).
         * @param ambient_light Ambient light (optional, default is null).
         * @param humidity Humidity (optional, default is null).
         * @param barometric_pressure Barometric pressure (optional, default is null).
         * @param temp_indoor Indoor temperature (optional, default is null).
         * @param bat_voltage Battery voltage (optional, default is null).
         */
        public void InsertToDeviceReadings(string device_name, string temp_outdoor, string gateway_name, string latitude = "null", string longitude = "null", string ambient_light = "null", string humidity = "null", string barometric_pressure = "null", string temp_indoor = "null", string bat_voltage = "null")
        {
            int gateway_id = 0;
            int device_id = 0;
            try
            {
                gateway_id = GetGatewayId(gateway_name);
                Console.WriteLine($"First call to GetGatewayId({gateway_id}): {DateTime.Now}");

                if (gateway_id == 0)
                {
                    InsertGateway(gateway_name, latitude, longitude);
                    gateway_id = GetGatewayId(gateway_name);
                    Console.WriteLine($"Second call to GetGatewayId({gateway_id}): {DateTime.Now}");
                }

                device_id = GetDeviceId(device_name);
                Console.WriteLine($"First call to GetDeviceId({device_id}): {DateTime.Now}");

                if (device_id == 0)
                {
                    InsertDevice(device_name, bat_voltage);
                    device_id = GetDeviceId(device_name);
                    Console.WriteLine($"Second call to GetDeviceId({device_id}): {DateTime.Now}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            string connectionString = $"Data Source={ServerName};Initial Catalog={DatabaseName};User ID={UserName};Password={Password}";

            // Create SqlConnection
            using (connection = new SqlConnection(connectionString))
            {
                try
                {
                    // Open the connection
                    connection.Open();
                    Console.WriteLine("Connection For Insert To Device Readings Opened Successfully!");

                    string query = $"INSERT INTO device_readings (ambient_light, humidity, barometric_pressure, device_id, gateway_id, temp_outdoor, temp_indoor, date_time) VALUES ({ambient_light}, {humidity}, {barometric_pressure}, {device_id}, {gateway_id}, {temp_outdoor}, {temp_indoor}, GETDATE());";
                    SqlCommand myCommand = new SqlCommand(query, connection);
                    myCommand.ExecuteNonQuery();
                    connection.Close();
                    Console.WriteLine("Connection Closed.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }

        /**
         * @brief Inserts data into airtime table.
         * @param device_name Name of the device.
         * @param gateway_name Name of the gateway.
         * @param snr Signal-to-noise ratio.
         * @param rssi Received signal strength indicator.
         * @param consumed_airtime Consumed airtime.
         * @param latitude Latitude of the gateway (optional, default is null).
         * @param longitude Longitude of the gateway (optional, default is null).
         * @param bat_voltage Battery voltage (optional, default is null).
         */
        public void InsertToAirtime(string device_name, string gateway_name, double snr, double rssi, double consumed_airtime, string latitude = "null", string longitude = "null", string bat_voltage = "null")
        {
            int gateway_id = 0;
            int device_id = 0;
            try
            {
                gateway_id = GetGatewayId(gateway_name);
                if (gateway_id == 0)
                {
                    InsertGateway(gateway_name, latitude, longitude);
                    gateway_id = GetGatewayId(gateway_name);
                }

                device_id = GetDeviceId(device_name);
                if (device_id == 0)
                {
                    InsertDevice(device_name, bat_voltage);
                    device_id = GetDeviceId(device_name);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            string connectionString = $"Data Source={ServerName};Initial Catalog={DatabaseName};User ID={UserName};Password={Password}";

            // Create SqlConnection
            using (connection = new SqlConnection(connectionString))
            {
                try
                {
                    // Open the connection
                    connection.Open();
                    Console.WriteLine("Connection For Insert To Airtime Opened Successfully!");

                    var query1 = "select MAX(consumed_airtime) from airtime";
                    var cmd1 = new SqlCommand(query1, connection);
                    var max_consumed_airtime = (double)cmd1.ExecuteScalar();

                    var query2 = "select MIN(consumed_airtime) from airtime";
                    var cmd2 = new SqlCommand(query2, connection);
                    var min_consumed_airtime = (double)cmd2.ExecuteScalar();

                    if (max_consumed_airtime < consumed_airtime)
                    {
                        max_consumed_airtime = consumed_airtime;
                    }
                    else if (min_consumed_airtime > consumed_airtime)
                    {
                        min_consumed_airtime = consumed_airtime;
                    }

                    string query = $"INSERT INTO airtime (device_id, gateway_id, snr, rssi, consumed_airtime, max_consumed_airtime, min_consumed_airtime, date_time) VALUES ({device_id}, {gateway_id}, {snr}, {rssi}, {consumed_airtime}, {max_consumed_airtime}, {min_consumed_airtime}, GETDATE());";
                    SqlCommand myCommand = new SqlCommand(query, connection);
                    myCommand.ExecuteNonQuery();
                    connection.Close();
                    Console.WriteLine("Connection Closed.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }
    }
}


