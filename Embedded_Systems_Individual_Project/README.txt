## I2C Example: Arduino Uno I2C Bus Master with IO Expander and temperature register

### Description:
This code demonstrates how to implement an Arduino Uno I2C Bus Master with an IO expander and temperature register. It utilizes the Atmel AVR Studio and Arduino IDE(to see the serial monitor). The code was last updated in January 2024.

### Components:
- Arduino Uno
- IO expander (MCP23008)
- Temperature sensor (DS1621)

### Dependencies:
- avr/io.h
- util/delay.h
- compat/twi.h

### I2C Configuration:
- **MCP23008 IO Expander**
  - Device Identifier: 0x40
  - Device Address: 0x02
  - Registers:
    - IODIR (I/O Direction Register): 0x00
    - GPIO (General Purpose I/O Register): 0x09
    - OLAT (Output Latch Register): 0x0A
- **DS1621 Temperature Sensor**
  - Device Identifier: 0x90
  - Device Address: 0x00
  - Temperature Register: 0xAA
  - Command to Start Conversion: 0xEE

### Functions given by the assignment:
- `i2c_transmit(unsigned char type)`: Function to perform I2C transmission based on the specified type (start, data, data with ACK, stop).
- `i2c_start(unsigned int dev_id, unsigned int dev_addr, unsigned char rw_type)`: Initiates I2C start condition for a given device ID, address, and read/write type.
- `i2c_stop(void)`: Sends I2C stop condition.
- `i2c_write(char data)`: Writes data to the I2C bus.
- `i2c_read(char *data, char ack_type)`: Reads data from the I2C bus with optional acknowledgment.

### Functions that I created:
- `Write_MCP23008(unsigned char reg_addr, unsigned char data)`: Writes data to a specified register of the MCP23008 IO expander.
- `Read_MCP23008(unsigned char reg_addr)`: Reads data from a specified register of the MCP23008 IO expander.
- `i2c_init(void)`: Initializes the TWI/I2C peripheral.
- `USART_Init()`: Initializes UART communication.
- `UARTsendchar(unsigned char data)`: Sends a character over UART.
- `temp_start_conversion()`: Starts temperature conversion on the DS1621 temperature sensor.
- `read_temp()`: Reads temperature from the DS1621 temperature sensor.
- `display_temp(uint8_t temperature)`: Displays temperature on UART.
- `main()`: The main function initializes peripherals, starts temperature conversion, reads temperature, displays it on UART, converts temperature to binary, and displays it on a 7-segment display.

### Usage:
1. Connect the Arduino Uno with the IO expander (MCP23008) and the temperature sensor (DS1621) via I2C bus.
2. Upload this code to the Arduino Uno using the Atmel AVR Studio or Arduino IDE.
3. Monitor the UART output for temperature readings.
4. LED display will show the units digit of the temperature in binary format.

### Notes:
- Adjust the baud rate in `USART_Init()` function according to your communication settings.
- Modify the delay in `display_temp()` function to suit your application requirements.
- This project was made with Atmel AVR Studio and is saved as an atmel project. You can also make it work with Arduino IDE by just copying the code of main.c to it. The main.c file is in the Final-assignment directory
