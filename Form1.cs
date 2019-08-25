using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace NetworkShareCopy
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnBrowseLocal_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            DialogResult dr=ofd.ShowDialog();
            if (dr == DialogResult.OK)
            {
                string str = ofd.FileName;
                txtLocalFile.Text = str;
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            string str = txtLocalFile.Text;
            try
            {
                if (!File.Exists(str))
                {
                    MessageBox.Show("Local file not exists");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            SNetworkFileCopier fileCopier = new SNetworkFileCopier();
            fileCopier.LocalFilePath = str;
            fileCopier.NetworkFolderPath = txtNetworkFolderPath.Text;
            fileCopier.Username = txtUsername.Text;
            fileCopier.Password = txtPassword.Text;

            Thread thr = new Thread(new ParameterizedThreadStart(CopyProc));
            thr.IsBackground = true;
            thr.Start(fileCopier);
        }

        private void CopyProc(object obj)
        {
            Console.WriteLine("CopyProc");
            SNetworkFileCopier copier = (SNetworkFileCopier)obj;
            try
            {
                Console.WriteLine("Connecting to remote");
                AppendToLog("Connecting to remote: "+copier.NetworkFolderPath);
                copier.ConnectToRemote();
                Console.WriteLine("Connected");
                AppendToLog("Connected to remote");
                string sfn = Path.GetFileName(copier.LocalFilePath);
                Console.WriteLine("Copying: " + sfn);
                string destination = copier.NetworkFolderPath + "\\" + sfn;
                AppendToLog("Copying " + copier.LocalFilePath + " to " + destination);
                File.Copy(copier.LocalFilePath, destination);
                Console.WriteLine("Copied");
                AppendToLog("Copied");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in CopyProc: " + ex.Message);
                AppendToLog(ex.Message);
            }
        }

        private void AppendToLog(string str)
        {
            Invoke((MethodInvoker)delegate ()
            {
                txtLog.AppendText(str + "\r\n");
            });
        }
    }
}
