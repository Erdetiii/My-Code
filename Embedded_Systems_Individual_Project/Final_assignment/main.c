//***************************************************************************
//  File Name	 : main.c
//  Version	 	 : 1.0
//  Description  : AVR I2C Bus Master with IO expander and temperature sensor
//  IDE          : Atmel AVR Studio
//               :
//  Last Updated : January 2024
//***************************************************************************
#include <avr/io.h>
#include <util/delay.h>
#include <compat/twi.h>
#define MAX_TRIES 50
#define MCP23008_ID    0x40   // MCP23008 Device Identifier
#define MCP23008_ADDR  0x02   // MCP23008 Device Address
#define IODIR 0x00            // MCP23008 I/O Direction Register
#define GPIO  0x09            // MCP23008 General Purpose I/O Register
#define OLAT  0x0A            // MCP23008 Output Latch Register
#define MCP9800_ADDR 0x90     // I2C address of MCP9800
#define MCP9800_TEMP_REG 0x05 // Temperature register address
#define I2C_START 0
#define I2C_DATA 1
#define I2C_DATA_ACK 2
#define I2C_STOP 3
#define ACK 1
#define NACK 0
#define DATASIZE 32

#define DS1621_ID 0x90          // DS1621 Device Identifier
#define DS1621_ADDR 0x00        // DS1621 Device Address
#define TEMP_REG 0xAA			      // Temperature Register
#define CMD_START_CONVERT 0xEE  // Initiates Temperature Conversion

#define F_CPU 16000000UL
#define BAUD 9600
#define UBRR_VAL ((F_CPU / 16 / BAUD) - 1)

/* START I2C Routine */
unsigned char i2c_transmit(unsigned char type) {
	switch(type) {
		case I2C_START:    // Send Start Condition
		TWCR = (1 << TWINT) | (1 << TWSTA) | (1 << TWEN);
		break;
		case I2C_DATA:     // Send Data with No-Acknowledge
		TWCR = (1 << TWINT) | (1 << TWEN);
		break;
		case I2C_DATA_ACK: // Send Data with Acknowledge
		TWCR = (1 << TWEA) | (1 << TWINT) | (1 << TWEN);
		break;
		case I2C_STOP:     // Send Stop Condition
		TWCR = (1 << TWINT) | (1 << TWEN) | (1 << TWSTO);
		return 0;
	}
	// Wait for TWINT flag set on Register TWCR
	while (!(TWCR & (1 << TWINT)));
	// Return TWI Status Register, mask the prescaler bits (TWPS1,TWPS0)
	return (TWSR & 0xF8);
}

char i2c_start(unsigned int dev_id, unsigned int dev_addr, unsigned char rw_type)
{
	unsigned char n = 0;
	unsigned char twi_status;
	char r_val = -1;
	i2c_retry:
	if (n++ >= MAX_TRIES) return r_val;
	// Transmit Start Condition
	twi_status=i2c_transmit(I2C_START);

	// Check the TWI Status
	if (twi_status == TW_MT_ARB_LOST) goto i2c_retry;
	if ((twi_status != TW_START) && (twi_status != TW_REP_START)) goto i2c_quit;
	// Send slave address (SLA_W)
	TWDR = (dev_id & 0xF0) | (dev_addr & 0x0E) | rw_type;
	// Transmit I2C Data
	twi_status=i2c_transmit(I2C_DATA);
	// Check the TWSR status
	if ((twi_status == TW_MT_SLA_NACK) || (twi_status == TW_MT_ARB_LOST)) goto i2c_retry;
	if (twi_status != TW_MT_SLA_ACK) goto i2c_quit;
	r_val=0;
	i2c_quit:
	return r_val;
}

void i2c_stop(void)
{
	unsigned char twi_status;
	// Transmit I2C Data
	twi_status=i2c_transmit(I2C_STOP);
}

char i2c_write(char data)
{
	unsigned char twi_status;
	char r_val = -1;
	// Send the Data to I2C Bus
	TWDR = data;
	// Transmit I2C Data
	twi_status=i2c_transmit(I2C_DATA);
	// Check the TWSR status
	if (twi_status != TW_MT_DATA_ACK) goto i2c_quit;
	r_val=0;
	i2c_quit:
	return r_val;
}

char i2c_read(char *data,char ack_type)
{
	unsigned char twi_status;
	char r_val = -1;

	if (ack_type) {
		// Read I2C Data and Send Acknowledge
		twi_status=i2c_transmit(I2C_DATA_ACK);
		if (twi_status != TW_MR_DATA_ACK) goto i2c_quit;
		} else {
		// Read I2C Data and Send No Acknowledge
		twi_status=i2c_transmit(I2C_DATA);
		if (twi_status != TW_MR_DATA_NACK) goto i2c_quit;
	}
	// Get the Data
	*data=TWDR;
	r_val=0;
	i2c_quit:
	return r_val;
}

void Write_MCP23008(unsigned char reg_addr,unsigned char data)
{
	// Start the I2C Write Transmission
	i2c_start(MCP23008_ID,MCP23008_ADDR,TW_WRITE);
	// Sending the Register Address
	i2c_write(reg_addr);
	// Write data to MCP23008 Register
	i2c_write(data);
	// Stop I2C Transmission
	i2c_stop();
}

unsigned char Read_MCP23008(unsigned char reg_addr)
{
	char data;
	// Start the I2C Write Transmission
	i2c_start(MCP23008_ID,MCP23008_ADDR,TW_WRITE);
	// Read data from MCP23008 Register Address
	i2c_write(reg_addr);
	// Stop I2C Transmission
	i2c_stop();

	// Re-Start the I2C Read Transmission
	i2c_start(MCP23008_ID,MCP23008_ADDR,TW_READ);
	i2c_read(&data,NACK);

	// Stop I2C Transmission
	i2c_stop();

	return data;
}

void i2c_init(void)
{
	// Initial ATMega328P TWI/I2C Peripheral
	TWSR = 0x00;         // Select Prescaler of 1
	// SCL frequency = 11059200 / (16 + 2 * 48 * 1) = 98.743 kHz
	TWBR = 0x30;        // 48 Decimal
}

void USART_Init() {
	// Set baud rate
	UBRR0H = (uint8_t)(UBRR_VAL >> 8);
	UBRR0L = (uint8_t)UBRR_VAL;

	// Enable receiver and transmitter
	UCSR0B = (1 << RXEN0) | (1 << TXEN0);

	// Set frame format: 8 data bits, 1 stop bit, no parity
	UCSR0C = (1 << UCSZ01) | (1 << UCSZ00);
}
void UARTsendchar(unsigned char data) {
	while  ( !( UCSR0A & (1<<UDRE0)) );
	UDR0 = data;
}

// Function to start temperature conversion on the DS1621 temperature sensor
void temp_start_conversion() {
	// Start I2C communication with DS1621 device for writing
	i2c_start(DS1621_ID, DS1621_ADDR, TW_WRITE);
	// Write the command to start temperature conversion
	i2c_write(CMD_START_CONVERT);
	// Stop I2C communication
	i2c_stop();
}

// Function to read temperature from the DS1621 temperature sensor
char read_temp() {
	char temp;
	// Start I2C communication with DS1621 device for writing
	i2c_start(DS1621_ID, DS1621_ADDR, TW_WRITE);
	// Write the address of the temperature register to read from
	i2c_write(TEMP_REG);
	// Start I2C communication with DS1621 device for reading
	i2c_start(DS1621_ID, DS1621_ADDR, TW_READ);
	// Read the temperature data from the register
	i2c_read(&temp, NACK);
	// Stop I2C communication
	i2c_stop();
	// Return the temperature data
	return temp;
}

// Function to display temperature on UART
void display_temp(uint8_t temperature) {
	// Temporary buffer to store the temperature string
	char buffer[20];
	// Convert the temperature to a string and store it in the buffer
	snprintf(buffer, sizeof(buffer), "%u\n", temperature);
	// Loop through the buffer and send each character over UART
	for (int i = 0; buffer[i] != '\0'; i++) {
		UARTsendchar(buffer[i]);
	}
}

// Main function
int main(void)
{
	// Initialize variables and peripherals
	i2c_init();
	USART_Init();
	Write_MCP23008(IODIR,0b00000000);   // pinMode()
	Write_MCP23008(GPIO,0b00000000);    // Reset all the Output Port - digitalWrite()
	
	// Start temperature conversion
	temp_start_conversion();
	_delay_ms(1); // Add a delay for the conversion to complete

	// Binary values representing the 7-segment display for each digit 0-9
	const uint8_t binaryValues[] = {
		0b10000001, // 0
		0b11001111, // 1
		0b10010010, // 2
		0b10000110, // 3
		0b11001100, // 4
		0b10100100, // 5
		0b10100000, // 6
		0b10001111, // 7
		0b10000000, // 8
		0b10000100  // 9
	};
	
	// Main loop
	while(1) {
		// Read temperature
		unsigned char temperature = read_temp();
		
		// Display temperature on UART
		display_temp(temperature);

		// Convert temperature to an integer
		int temperature_number = (int)temperature;

		// Extract units digit
		if(unitsDigit >= 10){
			//Show only the lower number on the 7-segment display
			int unitsDigit = temperature_number % 10;
		}else{
			int unitsDigit = temperature_number;
		}
		
		
		// Convert temperature to binary and display on 7-segment display
		Write_MCP23008(GPIO, binaryValues[unitsDigit]);

		// Delay before next iteration
		_delay_ms(1000);
	}
	// Return 0 to indicate successful program execution
	return 0;
}

//endoffile