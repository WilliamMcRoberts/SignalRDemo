using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRDemoClient.NetMaui.Models;

public class MessageModel : INotifyPropertyChanged
{
    string _messageText;
    public string MessageText
    {
        get => _messageText;
        set
        {
            if (_messageText == value)
                return;

            _messageText = value;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MessageText)));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
}
