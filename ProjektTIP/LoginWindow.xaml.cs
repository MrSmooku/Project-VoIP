﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Newtonsoft.Json;
using System.IO;
using System.Net.Sockets;
using System.Net;
using SharedClasses;

namespace ProjektTIP
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private UserLogin myLogin;

        private Random rand;

        public LoginWindow()
        {
            InitializeComponent();

            this.myLogin = new UserLogin();
            rand = new Random();
        }

        public string HashPassword(string ClientHashedPassword)
        {
            var byteArrayClientPassword = Encoding.UTF8.GetBytes(ClientHashedPassword);
            SHA256 sha = new SHA256Managed();
            sha.Initialize();
            var hashedPasswordBytes = sha.ComputeHash(byteArrayClientPassword);
            //var hashedPasswordString = Encoding.UTF8.GetString(hashedPasswordBytes);
            string result = "";
            foreach (var h in hashedPasswordBytes)
            {
                result += string.Format("{0:x2}", h);
            }

            sha.Clear();
            //Console.WriteLine(hashedPasss);
            return result;
        }

        private async void bLogin_Click(object sender, RoutedEventArgs e)
        {
            JsonClassRequest request = new JsonClassRequest()
            {
                RID = rand.Next(1000,3000),
                RequestCode = (int)RequestsCodes.HELLO,
                Parameters = new List<string>() { textNick.Text, HashPassword(passwordBox.Password) }
            };

            string json = JsonConvert.SerializeObject(request);
            var x = await ConnectToServer(json);
            if (x.Equals("error"))
            {
                MessageBox.Show("Brak połączenia z serwerem");
            }
            else
            {
                var login = JsonConvert.DeserializeObject<JsonClassResponse<UserLogin>>(x);
                UserLogin userLogin = login.Response;
                myLogin = userLogin;
                //MessageBox.Show(userLogin.Id.ToString());

                if (!userLogin.SessionID.Equals(new Guid()))
                {
                    this.DialogResult = true;

                }
                else
                {
                    lInfo.Content = "Podano błędne dane";
                }
            }
        }

        async Task<string> ConnectToServer(string json)
        {
            try { 
                using (var tcpClient = new TcpClient())
                {
                    await tcpClient.ConnectAsync(Settings.ServerAddress, Settings.ServerPort);
                    var writer = new StreamWriter(tcpClient.GetStream(), Encoding.UTF8);
                    writer.AutoFlush = true;
                    var reader = new StreamReader(tcpClient.GetStream(), Encoding.UTF8);
                    await writer.WriteLineAsync(json);
                    var responseString = await reader.ReadLineAsync();
                    return responseString;
                }
            }
            catch (SocketException e)
            {
                return "error";
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegistrationWindow();
            registerWindow.Show();
        }

        public UserLogin ShowLoginDialog()
        {
            this.ShowDialog();

            return myLogin;
        }
    }
}
