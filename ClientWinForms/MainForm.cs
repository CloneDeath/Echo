//
//  PT SYNC
//  2013 Lee Brunjes
//
//  A one way file sync System. 
//
//
using System;
using System.Drawing;
using System.Windows.Forms;
using Shared;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Threading.Tasks;
using Amazon.S3.Model;
using Amazon.S3;
using Amazon;
using System.Diagnostics;
namespace ClientWinForms
{
	public class MainForm:Form {
		private Button SettingsButton = new Button();
		private Button StartSync = new Button();
		private Button Clear = new Button();
		private TextBox Console = new TextBox();
		private ProgressBar Progress = new ProgressBar();
        private BackgroundWorker backgroundWorker = null;
        private SettingsForm SettingsForm = null;

		public static int DownloadCount =0;
        private Button StartNS2;
        private TableLayoutPanel tableLayoutPanel1;
        private FlowLayoutPanel flowLayoutPanel1;
		public static int ErrorCount =0;


		public MainForm ()
		{
			try {
				this.Icon = new Icon ("echo.ico");
			} catch (Exception ex) {
				System.Console.WriteLine (ex.Message);
			}

            Settings.ReadConfigFile(Settings.CONFIG_FILE_CLIENT);

            SettingsForm = new SettingsForm();

            InitializeComponent();

            InitializeBackgroundWorker();

            if (!Settings.HaveReadSettingsFile) {
                MessageBox.Show("Settings File Not Found.\r\nPlease Edit Your settings and save them\r\n\r\nYou MUST change s3IDKey and s3SecretKey");
                SettingsForm.ShowDialog();
            }
        }

        private void InitializeComponent(){
            this.SettingsButton = new System.Windows.Forms.Button();
            this.StartSync = new System.Windows.Forms.Button();
            this.Clear = new System.Windows.Forms.Button();
            this.Console = new System.Windows.Forms.TextBox();
            this.Progress = new System.Windows.Forms.ProgressBar();
            this.StartNS2 = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // SettingsButton
            // 
            this.SettingsButton.Location = new System.Drawing.Point(269, 3);
            this.SettingsButton.Name = "SettingsButton";
            this.SettingsButton.Size = new System.Drawing.Size(96, 32);
            this.SettingsButton.TabIndex = 0;
            this.SettingsButton.Text = "Settings";
            // 
            // StartSync
            // 
            this.StartSync.Location = new System.Drawing.Point(3, 3);
            this.StartSync.Name = "StartSync";
            this.StartSync.Size = new System.Drawing.Size(260, 32);
            this.StartSync.TabIndex = 1;
            this.StartSync.Text = "Start Scan and Sync";
            // 
            // Clear
            // 
            this.Clear.Location = new System.Drawing.Point(371, 3);
            this.Clear.Name = "Clear";
            this.Clear.Size = new System.Drawing.Size(83, 32);
            this.Clear.TabIndex = 4;
            this.Clear.Text = "Clear";
            // 
            // Console
            // 
            this.Console.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Console.Location = new System.Drawing.Point(3, 67);
            this.Console.Multiline = true;
            this.Console.Name = "Console";
            this.Console.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.Console.Size = new System.Drawing.Size(560, 244);
            this.Console.TabIndex = 2;
            // 
            // Progress
            // 
            this.Progress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.Progress.Location = new System.Drawing.Point(3, 3);
            this.Progress.Name = "Progress";
            this.Progress.Size = new System.Drawing.Size(560, 16);
            this.Progress.TabIndex = 3;
            // 
            // StartNS2
            // 
            this.StartNS2.Location = new System.Drawing.Point(460, 3);
            this.StartNS2.Name = "StartNS2";
            this.StartNS2.Size = new System.Drawing.Size(96, 32);
            this.StartNS2.TabIndex = 5;
            this.StartNS2.Text = "Start NS2";
            this.StartNS2.Click += new System.EventHandler(this.StartNS2_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.Progress, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.Console, 0, 2);
            this.tableLayoutPanel1.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 41F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(566, 314);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.Controls.Add(this.StartSync);
            this.flowLayoutPanel1.Controls.Add(this.SettingsButton);
            this.flowLayoutPanel1.Controls.Add(this.Clear);
            this.flowLayoutPanel1.Controls.Add(this.StartNS2);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 26);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(560, 35);
            this.flowLayoutPanel1.TabIndex = 4;
            // 
            // MainForm
            // 
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(566, 316);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Echo";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		private void ClearConsole (Object o, MouseEventArgs args)
		{
			Console.Text ="";
		}

        // Set up the BackgroundWorker object by attaching event handlers.
        private void InitializeBackgroundWorker ()
		{
			backgroundWorker = new BackgroundWorker ();
			backgroundWorker.WorkerReportsProgress = true;
			backgroundWorker.WorkerSupportsCancellation = true;
			backgroundWorker.DoWork += new DoWorkEventHandler (ThreadedSyncAndScan);
			backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler (SyncComplete);
			backgroundWorker.ProgressChanged += new ProgressChangedEventHandler (SyncProgressChanged);

        }

		public void RunSyncAndScan(Object o, MouseEventArgs args)
		{
            if (!StartSync.Enabled)
            {
                this.Console.Text += "cannot sync now";
                return;
            }

            backgroundWorker.RunWorkerAsync();
            StartSync.Enabled = false;
          
        }

        private delegate void StringDelegate(string s);
        private void AddTextToConsole(string text)
        {
            if (this.Console.InvokeRequired)
            {
                StringDelegate sd = new StringDelegate(AddTextToConsole);
                this.Console.Invoke(sd, new object[] { text });
                return;
            }

            this.Console.AppendText(text);
        }


        private void ThreadedSyncAndScan(object sender, DoWorkEventArgs e)
        {   
            // Get the BackgroundWorker that raised this event.
            BackgroundWorker worker = sender as BackgroundWorker;

            Dictionary<string, SyncItem> serverhashes;

            //get the server hashlist
            AddTextToConsole("Getting server hashes (" + Settings.HashServer + ")" + Environment.NewLine);
            try
            {
                serverhashes = Http.GetHashList();

            }
            catch (Exception ex)
            {
                AddTextToConsole("Error Reading Remote Hashes:" + ex.Message + Environment.NewLine);
                return;
            }

            //hash all teh local files.
            AddTextToConsole("Checking Local hashes" + Environment.NewLine);
            SyncList LocalData = new SyncList(Settings.LocalDirectory);

            List<string> FilesToDownload = new List<string>();
			LocalData.saveSyncList (Settings.HashCache);
            //get the delta list
            foreach (KeyValuePair<string, SyncItem> kvp in serverhashes)
            {

                //ignore metadata.
                if (kvp.Key == "__Server" || kvp.Key == "__DateGeneratedUTC")
                {
                    ;
                }
                else
                {
                    //Queue downloads
                    if (!LocalData.HashList.ContainsKey(kvp.Key))
                    {
                        //downlaod files that dont exist.
                        FilesToDownload.Add(kvp.Key);
                    }
                    else
                    {
                        if (kvp.Value.Hash != LocalData.HashList[kvp.Key].Hash)
                        {
                            //download files the dont mathc the hash
                            FilesToDownload.Add(kvp.Key);
                        }
                        else
                        {
                            //if we get here the file exist and is right.
                            //get it out of the hash list so we dont delrte it.
                            LocalData.HashList.Remove(kvp.Key);
                        }
                    }
                }

            }

            //Remove Files if set to
            if (Settings.RemoveLocalFileIfNoRemoteFile)
            {
                AddTextToConsole("Generating list of local files to remove" + Environment.NewLine);
                bool shouldDelete = true;
                //ensure we didnt delte everything accidentally.
                if (LocalData.HashList.Count > Settings.numFilesToRemoveWithNoWarning)
                {
                    shouldDelete = false;
                    if (MessageBox.Show(LocalData.HashList.Count + " Files Are flagged for deletion." + Environment.NewLine + " Delete them?", "Continue?", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    {
                        shouldDelete = true;
                        AddTextToConsole("Deleting Files" + Environment.NewLine);
                    }
                    else
                    {
                        AddTextToConsole("Files Will NOT be Removed" + Environment.NewLine);
                    }


                }


                //remove files
				if (shouldDelete && LocalData.HashList.Count >0)
                {
                    foreach (KeyValuePair<string, SyncItem> kvp in LocalData.HashList)
                    {
						AddTextToConsole("  "+ kvp.Key + Environment.NewLine);
                    
                        File.Delete(Settings.LocalDirectory + kvp.Key);
                    }
                }
            }


          
		

            //DOWNLOADS
			AddTextToConsole ("Need to download: " + FilesToDownload.Count +" from " +Settings.DownloadType +Environment.NewLine);
			ErrorCount =0;




			int tasklimit =10;

			int nextFile =0;
			GetObjectRequest request;
			AmazonS3Client s3 = new  AmazonS3Client (Settings.s3IDKey, Settings.s3SecretKey);
			while(nextFile < FilesToDownload.Count || DownloadCount > 0){
				if(DownloadCount < tasklimit && nextFile < FilesToDownload.Count){
					request = new GetObjectRequest();
					request.BucketName = Settings.s3Bucket;
					request.Key = FilesToDownload[nextFile].Substring (1);//use substring so we elminate the /
					request.Timeout  =1000;//wait 1 minute for a  response.
					AddTextToConsole("  "+(nextFile +1)+"/"+FilesToDownload.Count +" "+request.Key+Environment.NewLine);

					s3.BeginGetObject(request,DownloadFile,s3);
					nextFile++;
					DownloadCount++;
					worker.ReportProgress((int)(((float)nextFile) / ((float)FilesToDownload.Count) * 100.0f));
           
				}

			}
               

            
			if (nextFile-ErrorCount < FilesToDownload.Count)
            {
                AddTextToConsole("WARNING: NOT ALL FILES WERE SUCCESSFULLY DOWNLOADED" + Environment.NewLine + Environment.NewLine);
            }
			AddTextToConsole("Download queue processed. Check log for any errors"+Environment.NewLine);
        }

        private void SyncComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            // First, handle the case where an exception was thrown. 
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else if (e.Cancelled)
            {
                // Next, handle the case where the user canceled the operation. 
            }
            else
            {
                // Finally, handle the case where the operation succeeded.
            }

            StartSync.Enabled = true;
        }

        private void SyncProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Progress.Value = e.ProgressPercentage;
        }

		public void ShowSettings(object a, MouseEventArgs e)
        {
			SettingsForm.ShowDialog();
		}


		protected void DownloadFile (IAsyncResult Result)
		{
			string file ="unknown";
			try {
				AmazonS3 s3 = Result.AsyncState as AmazonS3;
				GetObjectResponse r = s3.EndGetObject (Result);
				System.Console.WriteLine(r.Key);
				string dir = Path.GetDirectoryName (Settings.LocalDirectory +"/" + r.Key);
				if (!Directory.Exists (dir)) {

					Directory.CreateDirectory (dir);
				}
				r.WriteResponseStreamToFile (Settings.LocalDirectory +"/"+ r.Key);
			} catch (Exception ex) {
				ErrorCount++;
				System.Console.WriteLine(ex.Message);
				AddTextToConsole("ERROR:"+ex.Message+Environment.NewLine);
			}
			DownloadCount--;

		}

        private Process NS2Process;

        private void StartNS2_Click(object sender, EventArgs e) {

            string ns2 = Path.Combine(Settings.LocalDirectory, "ns2.exe");

            if (!File.Exists(ns2)){
                MessageBox.Show(string.Format("Cannot find ns2.exe in directory {0}", Settings.LocalDirectory));
		       return;
	        }

            ProcessStartInfo startInfo = new ProcessStartInfo {
                WorkingDirectory = Settings.LocalDirectory,
                FileName = "ns2.exe"
            };

            NS2Process = Process.Start(startInfo);
        }
	}
}

