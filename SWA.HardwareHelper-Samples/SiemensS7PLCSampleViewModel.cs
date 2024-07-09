using S7.Net;
using SWA.HardwareHelper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SWA.HardwareHelper_Samples
{
  internal class SiemensS7PLCSampleViewModel : ViewModelBase
  {
    #region Private Fields
    private S7PLC _plc;
    #endregion

    #region Properties - Connection
    public IEnumerable<S7PLC_CpuType> CpuTypes
    {
      get { return Enum.GetValues(typeof(S7PLC_CpuType)).Cast<S7PLC_CpuType>(); }
    }
    public S7PLC_CpuType CpuType { get; set; } = S7PLC_CpuType.S7_1500;
    public string IP { get; set; } = "192.168.1.10";
    public short Rack { get; set; } = 0;
    public short Slot { get; set; } = 1;

    public bool IsDisconnectButtonEnabled {  get; set; } = false;
    public string ConnectionStatusString { get; set; } = "";
    #endregion

    #region Properties - Monitoring
    public bool IsMonitoring 
    {
      get => (null == _plc) ? false : _plc.IsMonitoring;
      set
      {
        if (null != _plc)
        {
          if (true == value) _plc.StartMonitoring(); else _plc.StopMonitoring();
        }
        OnPropertyChanged(nameof(IsMonitoring));
      }
    }
    public string Item_NameString { get; set; } = "";
    public IEnumerable<S7PLC_AreaType> Item_AreaTypes
    {
      get { return Enum.GetValues(typeof(S7PLC_AreaType)).Cast<S7PLC_AreaType>(); }
    }
    public S7PLC_AreaType Item_AreaType { get; set; } = S7PLC_AreaType.DataBlock;
    public int Item_AreaIndex { get; set; } = 1;
    public IEnumerable<S7PLC_VarType> Item_VarTypes
    {
      get { return Enum.GetValues(typeof(S7PLC_VarType)).Cast<S7PLC_VarType>(); }
    }
    public S7PLC_VarType Item_VarType { get; set; } = S7PLC_VarType.Int;
    public int Item_Address {  get; set; } = 0;
    public uint Item_Count { get; set; } = 1;
    //public short Item_BitIndex { get; set; } = 0;
    public IEnumerable<S7PLC_BitIndex> Item_BitIndexs
    {
      get { return Enum.GetValues(typeof(S7PLC_BitIndex)).Cast<S7PLC_BitIndex>(); }
    }
    public S7PLC_BitIndex Item_BitIndex { get; set; } = S7PLC_BitIndex.Ignore;
    public IEnumerable<S7PLC_SubscribeMode> Item_SubscribeModes
    {
      get { return Enum.GetValues(typeof(S7PLC_SubscribeMode)).Cast<S7PLC_SubscribeMode>(); }
    }
    public S7PLC_SubscribeMode Item_SubscribeMode { get; set; } = S7PLC_SubscribeMode.BothWay;
    public object? Item_SubscribedValue { get; set; } = 0;
    public string SubscribeEventsMessage { get; set; } = "";
    public string MonitoringMessage { get; set; } = "";
    public ObservableCollection<PLC_DataItem> MonitoringItems => (_plc == null)? null: _plc.DataItems;
    #endregion

    #region XAML Property & Method
    public void ButtonConnect_Click(object sender, RoutedEventArgs e)
    {
      if(null == _plc)
      {
        try
        {
          _plc = new S7PLC(CpuType, IP, Rack, Slot);
          _plc.OnConnectionStatusChanged += OnConnectionStatusChanged;
          _plc.OnSubscribedDataValueChanged += OnSubscribedDataValueChanged;
        }
        catch { }
      }
    }

    public void ButtonMonitoringByNameString_Click(object sender, RoutedEventArgs e)
    {
      if (null == _plc) { MonitoringMessage = "PLC not avaliable."; return; }

      try
      {
        PLC_DataItem item = new(this.Item_NameString, Item_SubscribeMode);
        if (null != item) 
        {
          item.SubscribedValue = S7PLC_Helper.GetCSharpTypeValue(this.Item_VarType, this.Item_SubscribedValue, (int)this.Item_Count);
          item.ItemCount = this.Item_Count;
          _plc.DataItems.Add(item);
          OnPropertyChanged(nameof(MonitoringItems));
        }
      }
      catch (Exception ex)
      {
        MonitoringMessage = ex.Message;
        OnPropertyChanged(nameof(MonitoringMessage));
      }
    }
    public void ButtonMonitoringByParameters_Click(object sender, RoutedEventArgs e) 
    {
      if (null == _plc) { MonitoringMessage = "PLC not avaliable."; return; }
      Thread.Sleep(200); //enum para in wpf will cause a delay when value changed.wait for value change done.
      try
      {
        PLC_DataItem item;
        if (Item_VarType == S7PLC_VarType.Bit)
          item = new(Item_AreaType, Item_AreaIndex, Item_VarType, Item_Address, S7PLC_BitIndex.Bit_0, Item_SubscribeMode);
        else
          item = new(Item_AreaType, Item_AreaIndex, Item_VarType, Item_Address, Item_SubscribeMode);

        if (null != item)
        {
          //item.SubscribedValue = S7PLC_Helper.GetCSharpTypeValue(this.Item_VarType, this.Item_SubscribedValue, (int)this.Item_Count);
          item.ItemCount = this.Item_Count;
          _plc.DataItems.Add(item);
          OnPropertyChanged(nameof(MonitoringItems));
        }
      }
      catch (Exception ex)
      {
        MonitoringMessage = ex.Message;
        OnPropertyChanged(nameof(MonitoringMessage));
      }
    }
    #endregion

    #region Event Handler
    private void OnConnectionStatusChanged()
    {
      ConnectionStatusString = _plc.StatusString + " (" + _plc.StatusByte.ToString() + ")";
      OnPropertyChanged(nameof(ConnectionStatusString));

      IsDisconnectButtonEnabled = _plc.IsConnected;
      OnPropertyChanged(nameof(IsDisconnectButtonEnabled));
    }
    private void OnSubscribedDataValueChanged(PLC_DataItem dataItem, S7PLC_ValueChangeType valueChangeType, object? valueFrom, object? valueTo)
    {
      if (valueChangeType == S7PLC_ValueChangeType.ValueIn)
      {
        SubscribeEventsMessage += string.Format("Value In Event: {0}{1} {2} address:{3} Subscribed:{{4}} From:{{5}} To:{{6}}\r\n"
        , dataItem.AreaType, dataItem.AreaIndex, dataItem.VarType, dataItem.Address, dataItem.SubscribedValue, valueFrom, valueTo);
      }
      else
      {
        SubscribeEventsMessage += string.Format("Value Out Event: {0}{1} {2} address:{3} Subscribed:{{4}} From:{{5}} To:{{6}}\r\n"
        , dataItem.AreaType, dataItem.AreaIndex, dataItem.VarType, dataItem.Address, dataItem.SubscribedValue, valueFrom, valueTo);
      }
      OnPropertyChanged(nameof(SubscribeEventsMessage));
    }
    #endregion
  }
}
