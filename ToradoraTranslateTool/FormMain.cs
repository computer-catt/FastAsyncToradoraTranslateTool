﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace ToradoraTranslateTool
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
            EnableButtons();

            string version = Application.ProductVersion;
            labelVersion.Text = version.Split("+")[0];//.Substring(0, version.Length - 2); // Convert X.X.X.X to X.X.X
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        // TODO:
        // 1. Extract ISO            ✓
        // 2. Extract .dat           ✓
        // 3. Edit .obj              ✓
        // 4. Repack .dat            ✓
        // 5. Create new seekmap     ✓
        // 6. Create new ISO         ✓

        private void EnableButtons()
        {
            buttonExtractIso.Enabled = true;
            if (File.Exists(Path.Combine(Application.StartupPath, "Data", "Iso", "PSP_GAME", "USRDIR", "resource.dat"))) // If iso already extracted, enable available steps
            {
                buttonExtractGame.Enabled = true;
            }
            if (File.Exists(Path.Combine(Application.StartupPath, "Data", "Txt", "utf16.txt", "utf16.txt")))
            {
                buttonTranslate.Enabled = true;
                buttonRepackGame.Enabled = true;
                buttonRepackIso.Enabled = true;
            }
        }

        private void DisableButtons()
        {
            buttonExtractIso.Enabled = false;
            buttonExtractGame.Enabled = false;
            buttonExtractGame.Enabled = false;
            buttonTranslate.Enabled = false;
            buttonRepackGame.Enabled = false;
            buttonRepackIso.Enabled = false;
        }

        private async void buttonExtractIso_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog openFileDialog = new())
                {
                    openFileDialog.Filter = "Toradora ISO (*.iso) | *.iso";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        ChangeStatus(true);
                        DisableButtons();
                        
                        IsoProgress.Value = 0;
                        buttonExtractIso.Visible = false;
                        
                        await Task.Run(() => IsoTools.ExtractIso(openFileDialog.FileName, progress => IsoProgress.Value = progress));
                        
                        buttonExtractIso.Visible = true;
                        IsoProgress.Value = 0;
                        
                        ChangeStatus(false);
                        EnableButtons();

                        MessageBox.Show("Iso extraction completed", "ToradoraTranslateTool", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                ChangeStatus(false);
                EnableButtons();
                MessageBox.Show("Error!" + Environment.NewLine + ex, "ToradoraTranslateTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void buttonExtractGame_Click(object sender, EventArgs e) {
            Stopwatch stopwatch = new ();
            stopwatch.Start();
            try {
                ChangeStatus(true);
                DisableButtons();

                /*// Resources
                await Task.Run(() => DatTools.ExtractDat(Path.Combine(Application.StartupPath, "Data", "Iso", "PSP_GAME", "USRDIR", "resource.dat")));
                await Task.Run(() => ObjTools.ProcessObjGz(Path.Combine(Application.StartupPath, "Data", "DatWorker", "resource")));

                // First
                await Task.Run(() => DatTools.ExtractDat(Path.Combine(Application.StartupPath, "Data", "Iso", "PSP_GAME", "USRDIR", "first.dat")));
                await Task.Run(() => ObjTools.ProcessTxtGz(Path.Combine(Application.StartupPath, "Data", "DatWorker", "first")));
                await Task.Run(() => ObjTools.ProcessSeekmap(Path.Combine(Application.StartupPath, "Data", "DatWorker", "first")));*/

                await Task.WhenAll(
                    Task.Run(() => { // resource
                        DatTools.ExtractDat(Path.Combine(Application.StartupPath, "Data", "Iso", "PSP_GAME", "USRDIR", "resource.dat")).Wait();
                        ObjTools.ProcessObjGz(Path.Combine(Application.StartupPath, "Data", "DatWorker", "resource"));
                    }),
                    Task.Run(() => { // first
                        DatTools.ExtractDat(Path.Combine(Application.StartupPath, "Data", "Iso", "PSP_GAME", "USRDIR", "first.dat")).Wait();
                        ObjTools.ProcessTxtGz(Path.Combine(Application.StartupPath, "Data", "DatWorker", "first"));
                        ObjTools.ProcessSeekmap(Path.Combine(Application.StartupPath, "Data", "DatWorker", "first"));
                    })
                );

                ChangeStatus(false);
                EnableButtons();

                stopwatch.Stop();
                MessageBox.Show($"Game files extraction completed in {stopwatch.ElapsedMilliseconds} ms", "ToradoraTranslateTool", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex) {
                ChangeStatus(false);
                EnableButtons();
                stopwatch.Stop();
                MessageBox.Show($"Error! in {stopwatch.ElapsedMilliseconds} ms" + Environment.NewLine + ex, "ToradoraTranslateTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally {
                stopwatch.Stop();
            }
        }

        private void buttonTranslate_Click(object sender, EventArgs e)
        {
            FormTranslation myForm = new();
            myForm.Show();
        }

        private async void buttonRepackGame_Click(object sender, EventArgs e)
        {
            try
            {
                ChangeStatus(true);
                DisableButtons();

                await Task.Run(() => ObjTools.RepackObj(itemDebugMode.Checked));
                await Task.Run(ObjTools.RepackTxt);
                await Task.Run(() => DatTools.RepackDat(Path.Combine(Application.StartupPath, "Data", "DatWorker", "resource.dat-LstOrder.lst")));
                await Task.Run(() => ObjTools.RepackSeekmap(Path.Combine(Application.StartupPath, "Data", "DatWorker", "resource.dat"), Path.Combine(Application.StartupPath, "Data", "DatWorker", "first")));
                await Task.Run(() => DatTools.RepackDat(Path.Combine(Application.StartupPath, "Data", "DatWorker", "first.dat-LstOrder.lst")));
                await Task.Run(() => File.Copy(Path.Combine(Application.StartupPath, "Data", "DatWorker", "resource.dat"), Path.Combine(Application.StartupPath, "Data", "Iso", "PSP_GAME", "USRDIR", "resource.dat"), true));
                await Task.Run(() => File.Copy(Path.Combine(Application.StartupPath, "Data", "DatWorker", "first.dat"), Path.Combine(Application.StartupPath, "Data", "Iso", "PSP_GAME", "USRDIR", "first.dat"), true));

                ChangeStatus(false);
                EnableButtons();

                MessageBox.Show("Game files repacking completed", "ToradoraTranslateTool", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ChangeStatus(false);
                EnableButtons();
                MessageBox.Show("Error!" + Environment.NewLine + ex.ToString(), "ToradoraTranslateTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void buttonRepackIso_Click(object sender, EventArgs e)
        {
            try
            {
                ChangeStatus(true);
                DisableButtons();

                await Task.Run(() => IsoTools.RepackIso(Path.Combine(Application.StartupPath, "Data", "Iso")));

                ChangeStatus(false);
                EnableButtons();
            }
            catch (Exception ex)
            {
                ChangeStatus(false);
                EnableButtons();
                MessageBox.Show("Error!" + Environment.NewLine + ex.ToString(), "ToradoraTranslateTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ChangeStatus(bool isWorking)
        {
            if (isWorking)
            {
                labelWork.Text = "Working";
                timerWork.Enabled = true;
            }
            else
            {
                timerWork.Enabled = false;
                labelWork.Text = "Ready";
            }
        }

        private void timerWork_Tick(object sender, EventArgs e)
        {
            if (labelWork.Text != "Working...")
                labelWork.Text += ".";
            else
                labelWork.Text = "Working";
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (timerWork.Enabled)
            {
                e.Cancel = true;
                MessageBox.Show("Wait until the work is done!", "ToradoraTranslateTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonExtractIsoHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This stage will extract selected ISO file to the \\Data\\Iso\\ folder", "ToradoraTranslateTool", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void buttonExtractGameHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This stage will extract and process .dat files from ISO." + Environment.NewLine + "It'll take ~9 minutes 46 seconds on the SSD", "ToradoraTranslateTool", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void buttonTranslateHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show("At this stage you will be able to translate the game text, including menus and settings", "ToradoraTranslateTool", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void buttonRepackGameHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This stage will inject translation and repack all game files." + Environment.NewLine +
                "It'll take ~5-10 minutes on the SSD." + Environment.NewLine +
                "You can enable debug mode by right-clicking on the repack button. In this mode you will be able to teleport to any level, and much more",
                "ToradoraTranslateTool", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void buttonRepackIsoHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This stage will repack ISO and save it in the program folder", "ToradoraTranslateTool", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}