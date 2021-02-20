using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogMyNATPubIP
{
    public partial class Form1 : Form
    {
        private bool allowVisible;
        private bool allowClose;
        private static string startUpIP;
        private static string currentIP;
        private static string[] lastLoggedIP;
        private string logFile = "LogMyNAT.log";

        public Form1()
        {
            InitializeComponent();

            mynotifyicon.ContextMenuStrip = contextMenuStrip1;
            this.showToolStripMenuItem.Click += showToolStripMenuItem_Click;
            this.openLogFileToolStripMenuItem.Click += openLogFileToolStripMenuItem_Click;
            this.exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            
            startUpIP = GetPublicIpAddress();
            currentIP = startUpIP;
            mynotifyicon.Text = "LogMyNAT (the public IP now is: " + currentIP + ")";
            mynotifyicon.ShowBalloonTip(1000, "LogMyNAT - Starting up...", "Current IP: " + currentIP,ToolTipIcon.Info);

            lblCurrPubIP.Text = currentIP;

            if (File.Exists(logFile))
            {
                var lastLine = File.ReadLines(logFile).Last();
                lastLoggedIP = lastLine.Split(';');
                if (lastLoggedIP.Count() > 0)
                {
                    if (lastLoggedIP[0] != currentIP)
                    {
                        using (StreamWriter w = File.AppendText(logFile))
                        {
                            Log(currentIP, w);
                        }
                    }
                    else
                    {
                        lblLastChange.Text = "Last logged change occurred @ " + lastLoggedIP[1];
                    }
                    
                }
            }
            else
            {
                using (StreamWriter w = File.AppendText(logFile))
                {
                    w.WriteLine("IP;Timestamp"); //first time created, add headers
                    Log(currentIP, w);
                }
            }

            timer1.Start();
        }

        private void CheckAndLog()
        {
            string probeIP = GetPublicIpAddress();

            if (currentIP != probeIP)
            {
                currentIP = probeIP;
                using (StreamWriter w = File.AppendText(logFile))
                {
                    Log(currentIP, w);
                    lblCurrPubIP.Text = currentIP;
                    lblLastChange.Text = "Last logged change occurred @ " + lastLoggedIP[1];
                }
                mynotifyicon.ShowBalloonTip(1000, "LogMyNAT - IP changed!", "New IP: " + currentIP, ToolTipIcon.Warning);
            }
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private static string GetPublicIpAddress()
        {
            string externalip = new WebClient().DownloadString("http://ifconfig.me");
            return externalip;
        }

        private void showLog()
        {
            //open log
        }

        public static void Log(string logMessage, TextWriter w)
        {
            w.WriteLine(logMessage + ";" + $"{DateTime.UtcNow.ToString("s") + "Z"}");
        }

        public static void DumpLog(StreamReader r)
        {
            string line;
            while ((line = r.ReadLine()) != null)
            {
                Console.WriteLine(line);
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            //if (FormWindowState.Minimized == this.WindowState)
            //{
            //    mynotifyicon.Visible = true;
            //    mynotifyicon.ShowBalloonTip(10,"I'm down here","Still running in the background",ToolTipIcon.Info);
            //    this.Hide();
            //}

            //else if (FormWindowState.Normal == this.WindowState)
            //{
            //    mynotifyicon.Visible = false;
            //}
        }

        private void mynotifyicon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //{
            //    mynotifyicon.Visible = false;
            //    this.Show();
            //    this.WindowState = FormWindowState.Normal;
            //}
        }

        protected override void SetVisibleCore(bool value)
        {
            if (!allowVisible)
            {
                value = false;
                if (!this.IsHandleCreated) CreateHandle();
            }
            base.SetVisibleCore(value);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!allowClose)
            {
                this.Hide();
                e.Cancel = true;
            }
            base.OnFormClosing(e);
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            allowVisible = true;
            Show();
            //showLog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            allowClose = true;
            Application.Exit();
        }

        private void openLogFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            allowClose = true;
            Application.Exit();
        }

        private void lblOpenLog_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            openLogFile();
        }

        private void openLogFile()
        {
            System.Diagnostics.Process.Start(logFile);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            CheckAndLog();
        }

    }
}
