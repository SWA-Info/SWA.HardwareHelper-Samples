using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Xps.Serialization;
using Microsoft.VisualBasic;
using SWA.HardwareHelper;

namespace SWA.HardwareHelper_Samples
{
  internal class MainWindowViewModel : ViewModelBase
  {
    public SerialPortSampleViewModel SerialPortSampleViewModel { get; set; } = new SerialPortSampleViewModel();
    public SiemensS7PLCSampleViewModel SiemensS7PLCSampleViewModel { get; set; } = new SiemensS7PLCSampleViewModel();
  }
}
