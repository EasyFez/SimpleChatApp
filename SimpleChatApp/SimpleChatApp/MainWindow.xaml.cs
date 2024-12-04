using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WebSocket4Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace SimpleChatApp
{
    public sealed partial class MainWindow : Window
    {
        private WebSocket _webSocket;
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void SendMessage(object sender, RoutedEventArgs e)
        {
            if(_webSocket.State == WebSocketState.Open)
            {
                if(MessageBox.Text.Length == 0)
                {
                    SendMessageButton.Content = "Message Box is Empty";
                    return;
                }
                else
                {
                    _webSocket.Send(MessageBox.Text);
                    SendMessageButton.Content = "Message Sent!";
                    MessageBox.Text = "";
                }
            }
            if (_webSocket.State == WebSocketState.Closed)
            {
                DispatcherQueue.TryEnqueue(() =>

                SendMessageButton.Content = "Connection Error");
            }
        }

        void disconnectServer(object sender, RoutedEventArgs e)
        {
            if (_webSocket != null && _webSocket.State == WebSocketState.Open)
            {
                _webSocket.Close();
                ConnectServerButton.Content = "Connection Closed";
            }
            else
            {
                ConnectServerButton.Content = "Connection is already closed";
            }
        }

        private void connectToServer(object sender, RoutedEventArgs e)
        {
            string serverAddress = ServerIP.Text;
            int serverPort = (int)PortNumber.Value;

            if(string.IsNullOrWhiteSpace(serverAddress))
            {
                ConnectServerButton.Content = "Server Address is Empty";
                return;
            }


            try
            {
                if (_webSocket != null && _webSocket.State == WebSocketState.Open)
                {
                    DispatcherQueue.TryEnqueue(() =>
                    
                        ConnectServerButton.Content = "Already Connected");
                        return;
                    
                }

                    //Again This is Hardcoded to Localhost because for now it's going to be running server on my local machine
                    _webSocket = new WebSocket($"ws://{serverAddress}:{serverPort}/chat");

                    _webSocket.Opened += Websocket_Opened;
                    _webSocket.MessageReceived += Websocket_MessageReceived;
                    _webSocket.Closed += Websocket_Closed;
                    _webSocket.Error += WebSocket_Error;

                    _webSocket.Open();
                
            }
            catch (Exception ex)
            {
                ConnectServerButton.Content = ex.Message;
            }

        }

        private void Websocket_Opened(object sender, EventArgs e)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                ConnectServerButton.Content = "Connection Opened";
                _webSocket.Send("Connected to " );
        });
        }

        private void Websocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Console.WriteLine($"Message Received: {e.Message}");
            DispatcherQueue.TryEnqueue(() =>
                UserChatBox.Text += e.Message + "\n");
        }

        private void WebSocket_Error(object sender,  SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            DispatcherQueue.TryEnqueue(() => 
                SendMessageButton.Content = "Connection Error");
        }

        private void Websocket_Closed(object sender, EventArgs e)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                ConnectServerButton.Content = "Connection Closed";
            });

            Console.WriteLine($"Client Disconnected: {e}");


        }


    }
}
