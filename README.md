# HardwareHelper

## Overview
HardwareHelper aims to provide a .net-based class library to facilitate the development of communication programs with hardware.

### Supported Hardware：
* Serial Port
* Siemens S7 PLCs

Samples can be found in [SWA_HardwareHelper_Samples](https://github.com/SWA-Info/SWA_HardwareHelper_Samples)

### Features

New in 2.1.0
* Extend S7 varTypes, new var types:
  *LWORD* (64 bits)
  *SInt* (8 bits)
  *LInt* (64 bits)
  *USInt* (8 bits)
  *UInt* (16 bits)
  *UDInt* (32 bits)
  *ULInt* (64 bits)

#### Serial Port
* Scan the serial ports on the PC, and choose whether to include virtual serial ports.
* Get serial port information, such as name, port number, status, etc.
* Establish a serial port connection and select the data receiving mode (end flag or specified length) to trigger the DataArrived event.

#### Siemens S7 PLC (Based on [S7NetPlus](https://github.com/S7NetPlus/s7netplus))
* Provides standardized data object classes, storage of values ​​and update timestamps, and provides a collection of historical data records of values.
* Provides value monitoring function and synchronizes PLC data in real time.
* Provides value change notification events and supports subscription of rising and falling edges of value changes.

## Documents
* [Serial Port](SWA.HardwareHelper/Documents/SerialPort.md)
* [S7 PLC](SWA.HardwareHelper/Documents/S7Plc.md)