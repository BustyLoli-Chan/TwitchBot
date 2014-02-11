using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using MediaCenter;
using System.Runtime.InteropServices;

namespace TwitchBot
{
    public class IRCBot
    {
        public IRCBot()
        {
        }

        public Stack<String> performBuffer = new Stack<string>();
        public string Server = "";
        public int Port = 6667;
        private string User = "";
        private string Nick = "";
        private string Channel = "";
        private NetworkStream ns;
        public ListBox output = null;
        private System.Timers.Timer pingTimer;

        public byte[] readBuffer = new byte[1024];
        private IAsyncResult aResult;
        public AsyncCallback aCallBack;
        public Socket cSocket;

        public void Connect()
        {
            cSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            cSocket.Connect(Server, Port);

            if (cSocket.Connected)
            {
                WaitForData();
            }

            sendData("PASS " + "" + "\r\n");
            sendData("NICK " + Nick + "\r\n");

            pingTimer = new System.Timers.Timer(15000);
            //pingTimer.Start();
            pingTimer.Elapsed += pingTimer_Elapsed;
        }

        private void WaitForData()
        {
            try
            {
                if (aCallBack == null)
                {
                    aCallBack = new AsyncCallback(onDataRecieve);
                }
                aResult = cSocket.BeginReceive(readBuffer, 0, readBuffer.Length, SocketFlags.None, aCallBack, cSocket);
            }
            catch (SocketException se)
            {
                MessageBox.Show(se.Message);
            }
        }

        public void onDataRecieve(IAsyncResult asyncResult)
        {
            try
            {
                Socket rSocket = (Socket)asyncResult.AsyncState;
                int iRx = rSocket.EndReceive(asyncResult);


                if (iRx > 0)
                {
                    char[] chars = new char[iRx + 1];
                    System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
                    d.GetChars(readBuffer, 0, iRx, chars, 0);
                    String result = new String(chars);

                    foreach (String aResult in result.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (!string.IsNullOrWhiteSpace(aResult) || aResult != "\0")
                        {
                            Output("< " + aResult);

                            //logic
                            if (aResult.Contains("PRIVMSG"))
                            {
                                //chat commands
                                if (aResult.ToUpper().EndsWith("!Playing".ToUpper()))
                                {
                                    string joinee = aResult.Substring(1, aResult.IndexOf("!") - 1);
                                    sendData("PRIVMSG " + Channel + " :Slush.Net is currently playing the video game BLANK with musical accompaniement of \"BLANK\" by artist \"BLANK\"\r\n");
                                }
                                else if (aResult.ToUpper().EndsWith("!SongInfo".ToUpper()) || aResult.ToUpper().EndsWith("!Song".ToUpper()))
                                {
                                    sendData("PRIVMSG " + Channel + " : I am sorry, there is an indeterminable audio source at the time... \r\n");
                                }
                                else if (aResult.ToUpper().EndsWith("!Dance".ToUpper()))
                                {
                                    string joinee = aResult.Substring(1, aResult.IndexOf("!") - 1);
                                    sendData("PRIVMSG " + Channel + " :\u0001ACTION Does a beautiful dance\u0001\r\n");
                                }
                                else if (aResult.ToUpper().EndsWith(Nick.ToUpper()) || aResult.ToUpper().EndsWith(User.ToUpper()) || aResult.ToUpper().EndsWith("Omoika".ToUpper()))
                                {
                                    sendData("PRIVMSG " + Channel + " :\u0001ACTION " + Nick + " bows gracefully \u0001\r\n");
                                    sendData("PRIVMSG " + Channel + " :" + User + ", at your service \r\n");
                                }
                            }
                            else if (aResult.Contains("/MOTD"))
                            {
                                sendData("USER " + User + "\r\n");
                                sendData("MODE " + Nick + " +B\r\n");
                                sendData("JOIN " + Channel + "\r\n");

                                sendData("PRIVMSG " + Channel + " :\u0001ACTION " + Nick + " bows gracefully \u0001\r\n");
                                sendData("PRIVMSG " + Channel + " :" + User + ", at your service \r\n");
                            }
                            else if (aResult.EndsWith("JOIN " + Channel))
                            {
                                string joinee = aResult.Substring(1, aResult.IndexOf("!") - 1);
                                

                                sendData("NOTICE " + joinee + " :Welcome to Slush.Net, " + joinee + ". The current broadcast is of the game \"BLANK\" \r\n");
                            }
                        }
                    }
                }

                WaitForData();

            }
            catch (ObjectDisposedException)
            {
                System.Diagnostics.Debugger.Log(0, "1", "\nOnDataReceived: Socket has been closed\n");
            }
            catch (SocketException se)
            {
                MessageBox.Show(se.Message);
            }
        }

        public void sendData(String data)
        {
            Object objData = data;
            byte[] b = System.Text.Encoding.UTF8.GetBytes(objData.ToString());
            if (cSocket.Connected)
            {
                cSocket.Send(b);
            }
            Output("> " + data);
        }

        public void pingTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            sendData("PING : " + Server + "\r\n");
        }

        private delegate void del(String Output);
        private void Output(String Message)
        {
            if (output != null)
            {
                if (this.output.InvokeRequired)
                {
                    Delegate d = new del(Output);
                    output.Invoke(d, new object[] { Message });
                }
                else
                {
                    this.output.Items.Add(Message);
                    this.output.SelectedIndex = output.Items.Count - 1;
                    this.output.ClearSelected();
                }
            }
        }

        public void Disconnect()
        {
            if (cSocket.Connected)
            {
                cSocket.Disconnect(false);
            }
            cSocket.Close();
            cSocket.Dispose();
        }
    }
}
