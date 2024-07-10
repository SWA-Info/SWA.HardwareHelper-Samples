using SWA.HardwareHelper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SWA.HardwareHelper_Samples
{
  internal class SerialPortSampleViewModel : ViewModelBase
  {
    #region Private Fields
    private ObservableCollection<SerialPortInfo> _SerialPortInfosAll = new ObservableCollection<SerialPortInfo>();
    private ObservableCollection<SerialPortInfo> _SerialPortInfosIgnoreVirtual = new ObservableCollection<SerialPortInfo>();

    private ObservableCollection<SerialPortInfo> _SerialPortInfos = new ObservableCollection<SerialPortInfo>();

    private SerialPort _serialPort;
    #endregion

    #region Properties
    public bool Radio_RecieveMode_FlagByte_Checked { get; set; } = true;
    public string FlagByteHexString { get; set; } = "04";
    public int FixedLengthInByte { get; set; } = 1;
    public IEnumerable<SerialPortDataRecieveMode> SerialPortDataRecieveModes
    {
      get { return Enum.GetValues(typeof(SerialPortDataRecieveMode)).Cast<SerialPortDataRecieveMode>(); }
    }
    public SerialPortDataRecieveMode SerialPortDataRecieveMode { get; set; } = SerialPortDataRecieveMode.FlagByte;
    public IEnumerable<SerialPortBaudRate> SerialPortBaudRates
    {
      get { return Enum.GetValues(typeof(SerialPortBaudRate)).Cast<SerialPortBaudRate>(); }
    }
    public SerialPortBaudRate SerialPortBaudRate { get; set; } = SerialPortBaudRate.BPS_9600;
    public IEnumerable<SerialPortParity> SerialPortParities
    {
      get { return Enum.GetValues(typeof(SerialPortParity)).Cast<SerialPortParity>(); }
    }
    public SerialPortParity SerialPortParity { get; set; } = SerialPortParity.None;
    public IEnumerable<SerialPortDataBitLength> SerialPortDataBitLengths
    {
      get { return Enum.GetValues(typeof(SerialPortDataBitLength)).Cast<SerialPortDataBitLength>(); }
    }
    public SerialPortDataBitLength SerialPortDataBitLength { get; set; } = SerialPortDataBitLength.Bits_8;
    public IEnumerable<SerialPortStopBitsLength> SerialPortStopBitsLengthes
    {
      get { return Enum.GetValues(typeof(SerialPortStopBitsLength)).Cast<SerialPortStopBitsLength>(); }
    }
    public SerialPortStopBitsLength SerialPortStopBitsLength { get; set; } = SerialPortStopBitsLength.One;

    public ObservableCollection<SerialPortInfo> SerialPortInfosAll => _SerialPortInfosAll;
    public ObservableCollection<SerialPortInfo> SerialPortInfosIgnoreVirtual => _SerialPortInfosIgnoreVirtual;

    public ObservableCollection<SerialPortInfo> SerialPortInfos => _SerialPortInfos;
    public SerialPortInfo SerialPortInfoSelected { get; set; }

    public bool IsCloseButtonEnabled { get; set; } = false;

    public string StringTobeSend { get; set; } = "";
    public string RecievedDataString { get; set; } = "";
    public string RecievedErrorString { get; set; } = "";
    #endregion

    #region XAML Method
    public void ButtonRefreshSerialPortsAll_Click(object sender, RoutedEventArgs e)
    {
      _SerialPortInfosAll.Clear();
      List<SerialPortInfo> serials = SerialPortHelper.GetSerialPortInfo(false);
      if (null != serials)
      {
        foreach (SerialPortInfo serial in serials)
          _SerialPortInfosAll.Add(serial);
      }

      OnPropertyChanged(nameof(SerialPortInfosAll));

      _SerialPortInfos = _SerialPortInfosAll;
      OnPropertyChanged(nameof(SerialPortInfos));
    }
    public void ButtonRefreshSerialPortsIgnoreVirtual_Click(object sender, RoutedEventArgs e)
    {
      _SerialPortInfosIgnoreVirtual.Clear();
      List<SerialPortInfo> serials = SerialPortHelper.GetSerialPortInfo(true);
      if (null != serials)
      {
        foreach (SerialPortInfo serial in serials)
          _SerialPortInfosIgnoreVirtual.Add(serial);
      }

      OnPropertyChanged(nameof(SerialPortInfosIgnoreVirtual));

      _SerialPortInfos = SerialPortInfosIgnoreVirtual;
      OnPropertyChanged(nameof(SerialPortInfos));
    }

    public void ButtonRefreshSerialPortOpen_Click(object sender, RoutedEventArgs e)
    {
      if (null != _serialPort)
        _serialPort.Close();

      if (Radio_RecieveMode_FlagByte_Checked == true)
      {
        byte[] flagBytes = Utilities.HexStrToBytes(FlagByteHexString);
        if (flagBytes.Length > 0)
        {
          _serialPort = new SerialPort(SerialPortInfoSelected, flagBytes[0]);
        }
      }
      else
      {
        _serialPort = new SerialPort(SerialPortInfoSelected, (uint)FixedLengthInByte);
      }
      int baudrate = (int)SerialPortBaudRate;
      if (null != _serialPort)
      {
        RecievedDataString = "";
        _serialPort.EventReceiveDataHandle += EventSerialPortDataArrivedHandler;
        _serialPort.EventReceiveErrorHandle += EventSerialPortErrorArrivedHandler;

        if (_serialPort.Open(SerialPortBaudRate, SerialPortParity, SerialPortDataBitLength, SerialPortStopBitsLength))
        { IsCloseButtonEnabled = true; OnPropertyChanged(nameof(IsCloseButtonEnabled)); }
      }
      else
      {
        { IsCloseButtonEnabled = false; OnPropertyChanged(nameof(IsCloseButtonEnabled)); }
      }
    }
    public void ButtonRefreshSerialPortClose_Click(object sender, RoutedEventArgs e)
    {
      if (null != _serialPort)
      {
        _serialPort.Close();
      }

      IsCloseButtonEnabled = false; OnPropertyChanged(nameof(IsCloseButtonEnabled));
    }
    public void ButtonRefreshSerialPortSend_Click(object sender, RoutedEventArgs e)
    {
      if (null != _serialPort)
      {
        if (_serialPort.IsOpen && StringTobeSend.Length > 0) { _serialPort.Send(StringTobeSend); }
      }
    }
    #endregion

    #region Event Handler
    private void EventSerialPortDataArrivedHandler(byte[] Data)
    {
      RecievedDataString += Encoding.ASCII.GetString(Data);
      OnPropertyChanged(nameof(RecievedDataString));
    }
    private void EventSerialPortErrorArrivedHandler(SerialPortError error, string msg)
    {
      RecievedErrorString = string.Format("Error: {0}, Message: {1}", nameof(error), msg);
      OnPropertyChanged(nameof(RecievedErrorString));
    }
    #endregion
  }
}
