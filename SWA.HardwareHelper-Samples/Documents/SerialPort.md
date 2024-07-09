# SWA.HardwareHelper Serial Port

## Quick Start

### 1. Get Serial Ports

include *virtual ports*
> List<SerialPortInfo> serials = SerialPortHelper.GetSerialPortInfo(false);

exclude *virtual ports*
> List<SerialPortInfo> serials = SerialPortHelper.GetSerialPortInfo(true);

```csharp
/// <summary>
/// Get SerialPortInfos on device.
/// </summary>
/// <param name="ignoreVirtualPort">ignore virtual port.</param>
/// <returns>List of serial ports, error result is null. </returns>
public static List<SerialPortInfo> GetSerialPortInfo(bool ignoreVirtualPort = true)
{... }
```

### 2. New Serial Port Instance

#### 2.1 New in Flag Byte Mode

```csharp
#region Private Fields
SerialPort _serialPort;
#endregion
private void NewSerialPort()
{
  byte flagByte = 0x04;
  ...

  //RecieveData event will be raised when flaByte arrived.
  _serialPort = new SerialPort(SerialPortInfoSelected, flagByte);
  ...
}
```

#### 2.2 New in Fixed Length Mode

```csharp
#region Private Fields
SerialPort _serialPort;
#endregion
private void NewSerialPort()
{
  uint fixedLengthInByte = 100;
  ...

  //RecieveData event will be raised when data recieved length equal to fixedLengthInByte.
  _serialPort = new SerialPort(SerialPortInfoSelected, fixedLengthInByte);
  ...
}
```

#### 2.3 Properties

```csharp
/// <summary>
/// current SerialPortInfo
/// </summary>
public SerialPortInfo PortInfo => _portInfo;
/// <summary>
/// DataRecieve event trigger mode.
/// </summary>
public SerialPortDataRecieveMode RecieveMode => _recieveMode;
/// <summary>
/// indicate SerialPort open state.
/// </summary>
public bool IsOpen  { get; }

/// <summary>
/// BaudRate setting.
/// </summary>
public SerialPortBaudRate PortBaudRate { get; set; } = SerialPortBaudRate.BPS_9600;
/// <summary>
/// Parity setting.
/// </summary>
public SerialPortParity PortParity { get; set; } = SerialPortParity.None;
/// <summary>
/// DataBitLength setting.
/// </summary>
public SerialPortDataBitLength PortDataBitLength { get; set; } = SerialPortDataBitLength.Bits_8;
/// <summary>
/// StopBits setting.
/// </summary>
public SerialPortStopBitsLength PortStopBits { get; set; } = SerialPortStopBitsLength.One;

/// <summary>
/// control protocol
/// </summary>
public Handshake PortHandshake { get; set; } = Handshake.None;
/// <summary>
/// ReadTimeout
/// </summary>
public int PortReadTimeout { get; set; } = 500;
/// <summary>
/// WriteTimeout
/// </summary>
public int PortWriteTimeout { get; set; } = 500;
/// <summary>
/// specify bytes count in buffer before take.
/// must less than minimun lenght of data.
/// </summary>
public int PortReceivedBytesThreshold { get; set; } = 1;
/// <summary>
/// specify data encoding
/// </summary>
public System.Text.Encoding PortDataEncoding { get; set; } = System.Text.Encoding.ASCII;
/// <summary>
/// Dtr On/Off
/// </summary>
public bool PortDtrEnable { get; set; } = false;
/// <summary>
/// Rts On/Off
/// </summary>
public bool PortRtsEnable { get; set; } = false;
/// <summary>
/// BytesToRead.
/// </summary>
/// <returns>-1 = error.</returns>
public int PortBytesToRead { get; }
/// <summary>
/// BytesToWrite.
/// </summary>
/// <returns>-1 = error.</returns>
public int PortBytesToWrite { get; }
```

### 3. Event Handler

#### 3.1 Recieve Data Event

```csharp
#region Private Fields
SerialPort _serialPort;
#endregion
private void NewSerialPort()
{
...

_serialPort.EventReceiveDataHandle += EventSerialPortDataArrivedHandler;
...
}

private void EventSerialPortDataArrivedHandler(byte[] Data)
{
  ...
}
```

#### 3.2 Recieve Error Event

```csharp
#region Private Fields
SerialPort _serialPort;
#endregion
private void NewSerialPort()
{
...

_serialPort.EventReceiveErrorHandle += EventSerialPortErrorArrivedHandler;
...
}

private void EventSerialPortErrorArrivedHandler(SerialPortError error, string msg)
{
  ...
}
```

### 4. Open/Close/Send

#### 4.1 Open
```csharp
/// <summary>
/// Open Serial Port, default setting.
/// </summary>
/// <returns>true = open succeeded, false = open failed. </returns>
public bool Open()
{...}

/// <summary>
/// Open Serial Port.
/// </summary>
/// <param name="baudRate">enum SerialPortBaudRate</param>
/// <param name="parity">enum SerialPortParity</param>
/// <param name="dataBitLength">enum SerialPortDataBitLength</param>
/// <param name="stopBits">enum SerialPortStopBitsLength</param>
/// <returns>true = open succeeded, false = open failed. </returns>
public bool Open(SerialPortBaudRate baudRate, SerialPortParity parity, SerialPortDataBitLength dataBitLength, SerialPortStopBitsLength stopBits)
{...}
```

#### 4.1 Close
```csharp
/// <summary>
/// Close Serial Port.
/// </summary>
/// <returns>true = close succeeded, false = close failed. </returns>
public bool Close()
{...}
```

#### 4.1 Send
```csharp
/// <summary>
/// send string.
/// </summary>
/// <param name="data">string data</param>
/// <returns>true = Send succeeded, false = Send failed. </returns>
public bool Send(string data)
{...}

/// <summary>
/// send bytes.
/// </summary>
/// <param name="data">bytes data</param>
/// <returns>true = Send succeeded, false = Send failed. </returns>
public bool Send(byte[] data)
{...}

/// <summary>
/// send chars.
/// </summary>
/// <param name="data">chars data</param>
/// <returns>true = Send succeeded, false = Send failed. </returns>
public bool Send(char[] data)
{...}
```
