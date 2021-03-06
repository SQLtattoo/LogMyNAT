using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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
            try
            {
                mynotifyicon.ContextMenuStrip = contextMenuStrip1;
                this.showToolStripMenuItem.Click += showToolStripMenuItem_Click;
                this.openLogFileToolStripMenuItem.Click += openLogFileToolStripMenuItem_Click;
                this.exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;

                startUpIP = GetPublicIpAddress();
                currentIP = startUpIP;
                mynotifyicon.Text = "LogMyNAT (Your public IP now is: " + currentIP + ")";
                mynotifyicon.ShowBalloonTip(5, "LogMyNAT - Starting up...", "Current IP: " + currentIP, ToolTipIcon.Info);

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
                                lblLastChange.Text = "Last logged change occurred @ " + DateTime.UtcNow.ToString("s") + "Z";
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
                        lblLastChange.Text = "Last logged change occurred @ " + DateTime.UtcNow.ToString("s") + "Z";
                    }
                }

                timer1.Start();
            }
            catch
            {

            }
            
        }

        private void CheckAndLog()
        {
            try
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
                    mynotifyicon.ShowBalloonTip(5, "LogMyNAT - IP changed!", "New IP: " + currentIP, ToolTipIcon.Warning);
                }
            }
            catch
            {

            }
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private static string GetPublicIpAddress()
        {
            string externalip="<>";
            try
            {
                externalip = new WebClient().DownloadString("http://ifconfig.me");
            }
            catch
            {

            }
            return externalip;
        }

        public static void Log(string logMessage, TextWriter w)
        {
            try
            {
                w.WriteLine(logMessage + ";" + $"{DateTime.UtcNow.ToString("s") + "Z"}");
            }
            catch
            {

            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            try
            {
                if (FormWindowState.Minimized == this.WindowState)
                {
                    SetToTray();
                }

                else if (FormWindowState.Normal == this.WindowState)
                {
                    mynotifyicon.Visible = false;
                }
            }
            catch
            {

            }
        }

        private void mynotifyicon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                {
                    mynotifyicon.Visible = false;
                    allowVisible = true;
                    this.Show();
                    this.WindowState = FormWindowState.Normal;
                }
            }
            catch
            {

            }
            
        }

        protected override void SetVisibleCore(bool value)
        {
            try
            {
                if (!allowVisible)
                {
                    value = false;
                    if (!this.IsHandleCreated) CreateHandle();
                }
                base.SetVisibleCore(value);
            }
            catch
            {

            }

        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                allowClose = true; //at some point we can make this a property but for now close it
                if (!allowClose)
                {
                    SetToTray();
                    e.Cancel = true;
                }
                base.OnFormClosing(e);
            }
            catch
            {

            }
            
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                allowVisible = true;
                Show();
                //showLog();
            }
            catch
            {

            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                allowClose = true;
                Application.Exit();
            }
            catch
            {

            }
        }

        private void openLogFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openLogFile();
        }

        private void lblOpenLog_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            openLogFile();
        }

        private void openLogFile()
        {
            try
            {
                System.Diagnostics.Process.Start(logFile);
            }
            catch
            {

            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            CheckAndLog();
        }

        private void SetToTray()
        {
            try
            {
                mynotifyicon.Visible = true;
                mynotifyicon.ShowBalloonTip(2, "I'm down here", "Still running in the background", ToolTipIcon.Info);
                this.Hide();
            }
            catch
            {

            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://github.com/SQLtattoo/LogMyNAT");
            }
            catch
            {

            }
            
        }

        private void mynotifyicon_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(currentIP);
            }
            catch
            {

            }
            
        }
    }
}
