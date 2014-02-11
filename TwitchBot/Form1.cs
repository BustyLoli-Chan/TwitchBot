using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Reflection;
using MediaCenter;

namespace TwitchBot
{
    public partial class Form1 : Form
    {
        MediaCenter.
        [DllImport("ole32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = false)]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        static extern string StringFromCLSID([MarshalAs(UnmanagedType.LPStruct)] Guid rclsid);

        public Form1()
        {
            InitializeComponent();
        }

        public IRCBot theBot;

        private void Form1_Load(object sender, EventArgs e)
        {
            MCRegister();

            theBot = new IRCBot();
            theBot.output = listBox1;
            theBot.Connect();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //theBot.Disconnect();
        }

        private void MCRegister()
        {
            //RegistryKey softKey = Registry.LocalMachine.OpenSubKey("Software", true);
            //softKey.CreateSubKey("J. River", RegistryKeyPermissionCheck.ReadWriteSubTree)
            //    .CreateSubKey("Media Center 18", RegistryKeyPermissionCheck.ReadWriteSubTree)
            //    .CreateSubKey("Plugins", RegistryKeyPermissionCheck.ReadWriteSubTree)
            //    .CreateSubKey("Interface", RegistryKeyPermissionCheck.ReadWriteSubTree)
            //    .CreateSubKey("TwitchBot", RegistryKeyPermissionCheck.ReadWriteSubTree);

            RegistryKey BaseKey = Registry.LocalMachine.CreateSubKey(@"Software\J. River\Media Center 18\Plugins\Interface\TwitchBot",RegistryKeyPermissionCheck.ReadWriteSubTree);
            
            Assembly asm = Assembly.GetExecutingAssembly();
            String CLSID = StringFromCLSID(asm.GetType().GUID);
            BaseKey.SetValue("CLSID", CLSID, RegistryValueKind.String);
            BaseKey.SetValue("IVersion", 1, RegistryValueKind.DWord);
            BaseKey.SetValue("Company", Application.CompanyName, RegistryValueKind.String);
            BaseKey.SetValue("Version", Application.ProductVersion, RegistryValueKind.String);
            BaseKey.SetValue("URL", "http://slushnet.dyndns.org", RegistryValueKind.String);
            BaseKey.SetValue("Copyright", "2013", RegistryValueKind.String);
            BaseKey.SetValue("nPluginMode", 1, RegistryValueKind.DWord);

        }

    }
}
