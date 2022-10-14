using Microsoft.AspNetCore.SignalR.Client;
using SignalRDemoClient.NetMaui.Models;
using System.Collections.ObjectModel;

namespace SignalRDemoClient.NetMaui;

public partial class MainPage : ContentPage
{
    HubConnection hubConnection;
    public ObservableCollection<MessageModel> MessageList { get; } = new();

    public MainPage()
    {
        InitializeComponent();
        messages.ItemsSource = MessageList;
        hubConnection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7294/chathub")
            .WithAutomaticReconnect()
            .Build();

        hubConnection.Reconnecting += async (sender) =>
        {

            var newMessage = "Attempting to reconnect...";
            MessageList.Add(new MessageModel { MessageText = newMessage});
        };

        hubConnection.Reconnected += async (sender) =>
        {

                var newMessage = "Reconnected to the server";
            MessageList.Clear();
            MessageList.Add(new MessageModel { MessageText = newMessage });
        };

        hubConnection.Closed += async (sender) =>
        {

            var newMessage = "Connection closed";
            MessageList.Add(new MessageModel { MessageText = newMessage });
            openConnection.IsEnabled = true;
            sendMessage.IsEnabled = false;
        };
    }

    private async void sendMessage_Clicked(object sender, EventArgs e)
    {
        try
        {
            await hubConnection.InvokeAsync(
                "SendMessage", ".NET MAUI Client", messageInput.Text);
        }
        catch (Exception ex)
        {
            MessageList.Add(new MessageModel { MessageText = ex.Message });
        }
    }

    private async void openConnection_Clicked(object sender, EventArgs e)
    {
        // "ReceiveMessage" is the event indicator used by the server for identifying messages
        hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
        {
            var newMessage = $"{user}: {message}";
            MessageList.Add(new MessageModel { MessageText = newMessage });
        });

        try
        {
            await hubConnection.StartAsync();
            MessageList.Add(new MessageModel { MessageText = "Connection Started" });
            openConnection.IsEnabled = false;
            sendMessage.IsEnabled = true;
        }
        catch (Exception ex)
        {
            MessageList.Add(new MessageModel { MessageText = ex.Message });
        }
    }
}