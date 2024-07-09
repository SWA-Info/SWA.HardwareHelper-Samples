# SWA.HardwareHelper Siemens S7 PLC

## Quick Start

### 1. Connection && Status

#### 1.1 Connect PLC

```csharp
#region Private Fields
private S7PLC _plc;
#endregion
private void NewS7PLC()
{
    _plc = new S7PLC(CpuType, IP, Rack, Slot);
    _plc.OnConnectionStatusChanged += OnConnectionStatusChanged;
    _plc.OnSubscribedDataValueChanged += OnSubscribedDataValueChanged;
    ...
}
```

#### 1.2 Status

```csharp
/// <summary>
/// indicate plc connection status.
/// </summary>
public bool IsConnected { get => _isConnected; private set { _isConnected = value; } }
/// <summary>
/// indicate plc communication ready.
/// </summary>
public bool IsAvaliable { get => _isAvaliable; private set { _isAvaliable = value; } }
```

#### 1.3 Connection Changed Event

```csharp
private void NewS7PLC()
{
    ...
    _plc.OnConnectionStatusChanged += OnConnectionStatusChanged;
    ...
}
//Event Handler
private void OnConnectionStatusChanged()
{
  ConnectionStatusString = _plc.StatusString + " (" + _plc.StatusByte.ToString() + ")";
}
```

### 2. Monitoring Item

class PLC_DataItem is used to specify monitoring item(s).
PLC_DataItem can be used not only single value but a value sequence in same vartype.

#### 2.1 New by NameString

NameString is known as a string used to represent the location and variable type of PLC value, For example: DB1.DBB100, C10, etc.

```csharp
{
    PLC_DataItem item = new(this.Item_NameString);
    _plc.DataItems.Add(item);   //Add to DataItems to monitoring
    ...
}
```

If a value sequnce is wanted, set the ItemCount property following next.

```csharp
{
    PLC_DataItem item1 = new("DB1.DBX5.4");
    item1.ItemCount = 4; //Bit(bool) 5.4/5.5/5.6/5.7 @DataBlock 1 will be read as bool[]
    _plc.DataItems.Add(item1);   //Add to DataItems to monitoring

    PLC_DataItem item2 = new("DB1.DBX5.6");
    item2.ItemCount = 4; //Bit(bool) 5.6/5.7/6.0/6.1 @DataBlock 1 will be read as bool[]
    _plc.DataItems.Add(item2);   //Add to DataItems to monitoring
    ...
}
```

#### 2.2 New by Parameter

Bit(s)

```csharp
{
    PLC_DataItem itembit = new(Item_AreaType, Item_AreaIndex, Item_VarType, Item_Address, S7PLC_BitIndex.Bit_0);
    ...

    PLC_DataItem itembits = new(Item_AreaType, Item_AreaIndex, Item_VarType, Item_Address, S7PLC_BitIndex.Bit_0);
    itembits.ItemCount = 8;
    ...
}
```

Other types

```csharp
{
    PLC_DataItem item1 = new(Item_AreaType, Item_AreaIndex, Item_VarType, Item_Address);
    ...

    PLC_DataItem item2 = new(Item_AreaType, Item_AreaIndex, Item_VarType, Item_Address);
    itembits.ItemCount = 10;
    ...
}
```

#### 2.3 Start / Stop Monitoring

**Note: Monitoring is OFF when PLC initialized**

*IsMonitoring Property*

>_plc.IsMonitoring;

*Start Monitoring*

>_plc.StartMonitoring();

*Stop Monitoring*

>_plc.StopMonitoring();

#### 2.4 DataItem Collection

*DataItem(s) Property*

```csharp
public class S7PLC : IDisposable
{
    public ObservableCollection<PLC_DataItem> DataItems { get; set; }
    ...
}
```

### 3. Value & History

When monitoring is turned on, the value will be synchronized with the PLC in real time, and the update time of the value will be recorded in Property *ValueUpdateDT* of PLC_DataItem.

```csharp
/// <summary>
/// DataItem for value/history storage, also sucscribe.
/// </summary>
public class PLC_DataItem
{
    ...
    /// <summary>
    /// Value of PLC_DataItem.
    /// </summary>
    public object? Value { get; set; } = null;
    /// <summary>
    /// timestamp of value read
    /// </summary>
    public System.DateTime ValueUpdateDT { get; set; } = System.DateTime.MinValue;
    ...
}
```

History value will be recorded in Property *HistoryValue* of PLC_DataItem when value has changed during Monitoring.

```csharp
/// <summary>
/// Value history item
/// </summary>
public struct PLC_HistoryValue
{
  /// <summary>
  /// Value
  /// </summary>
  public object? Value;
  /// <summary>
  /// DateTime recorded
  /// </summary>
  public System.DateTime? RecordDT;
}

/// <summary>
/// DataItem for value/history storage, also sucscribe.
/// </summary>
public class PLC_DataItem
{
    ...
    /// <summary>
    /// History values
    /// </summary>
    public List<PLC_HistoryValue> HistoryValue { get; set; } = new();
    ...
}
```

### 4. Subscribe

Subscription is used to obtain specific behavior changes of the monitored value, which can be the value changing to the subscribed value, the value changing from the subscribed value to another value, or the bidirectional subscription triggering the corresponding event.

#### 4.1 Mode

The Subscribe *Mode* and *Value* can be specified when creating a new *DataItem*, or modified by properties *SubscribeMode* and *SubscribedValue* of *DataItem* after the item is created.

```csharp
/// <summary>
/// value changed event trigger mode
/// </summary>
[TypeConverter(typeof(SWAEnumConverter))]
public enum S7PLC_SubscribeMode
{
  /// <summary>
  /// No subscribe
  /// </summary>
  [Description("No subscribe")]
  None,
  /// <summary>
  /// Trigger on changed into subscribed Value
  /// </summary>
  [Description("Trigger In")]
  ValueIn,
  /// <summary>
  /// Trigger on changed out from subscribed Value
  /// </summary>
  [Description("Trigger Out")]
  ValueOut,
  /// <summary>
  /// Trigger BothWay
  /// </summary>
  [Description("Trigger BothWay")]
  BothWay,
}
```

#### 4.2 Value Changed Event

```csharp
private void NewS7PLC()
{
    ...
    _plc.OnSubscribedDataValueChanged += OnSubscribedDataValueChanged;
    ...
}

//Event Handler
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
}
```
