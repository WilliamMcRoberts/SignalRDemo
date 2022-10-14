using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace SignalRDemoClient.WPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    HubConnection hubConnection;
    HubConnection counterConnection;

    public MainWindow()
    {
        InitializeComponent();

        // Builds the connection
        hubConnection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7294/chathub")
            .WithAutomaticReconnect()
            .Build();

        // Builds the connection
        counterConnection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7294/counterhub")
            .WithAutomaticReconnect()
            .Build();

        hubConnection.Reconnecting += async (sender) =>
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                var newMessage = "Attempting to reconnect...";
                messages.Items.Add(newMessage);
            });
        };

        hubConnection.Reconnected += async (sender) =>
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                var newMessage = "Reconnected to the server";
                messages.Items.Clear();
                messages.Items.Add(newMessage);
            });
        };

        hubConnection.Closed += async (sender) =>
        {
            await this.Dispatcher.InvokeAsync(() =>
            {
                var newMessage = "Connection closed";
                messages.Items.Add(newMessage);
                openConnection.IsEnabled = true;
                sendMessage.IsEnabled = false;
            });
        };
    }

    private async void openConnection_Click(object sender, RoutedEventArgs e)
    {
        // "ReceiveMessage" is the event indicator used by the server for identifying messages
        hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
        {
            this.Dispatcher.Invoke(() =>
            {
                var formattedMessage = $"{user}: {message}";
                messages.Items.Add(formattedMessage);
            });
        });

        try
        {
            await hubConnection.StartAsync();
            messages.Items.Add("Connection started");
            openConnection.IsEnabled = false;
            sendMessage.IsEnabled = true;
        }
        catch (Exception ex)
        {
            messages.Items.Add(ex.Message);
        }
    }

    private async void sendMessage_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await hubConnection.InvokeAsync(
                "SendMessage", "WPF Client", messageInput.Text);
        }
        catch (Exception ex)
        {
            messages.Items.Add(ex.Message);
        }
    }

    private async void openCounter_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await counterConnection.StartAsync();
            openCounter.IsEnabled = false;
        }
        catch (Exception ex)
        {
            messages.Items.Add(ex.Message);
        }
    }

    private async void incrementCounter_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await counterConnection.InvokeAsync("AddToTotal", "WPF Client", 1);
        }
        catch (Exception ex)
        {
            messages.Items.Add(ex.Message);
        }
    }
}
