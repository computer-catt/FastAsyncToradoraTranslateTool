using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

// the only way
// ReSharper disable AsyncVoidMethod

// i dont plan to localize
// ReSharper disable LocalizableElement

namespace ToradoraTranslateTool;

public partial class FormMain : Form {
    private readonly string dataDir = Path.Combine(Application.StartupPath, "Data");
    public FormMain() {
        InitializeComponent();
        EnableButtons();

        string version = Application.ProductVersion;
        labelVersion.Text = version.Split("+")[0]; //.Substring(0, version.Length - 2); // Convert X.X.X.X to X.X.X
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    // TODO:
    // 1. Extract ISO            ✓
    // 2. Extract .dat           ✓
    // 3. Edit .obj              ✓
    // 4. Repack .dat            ✓
    // 5. Create new seekmap     ✓
    // 6. Create new ISO         ✓

    private void EnableButtons() {
        buttonExtractIso.Enabled = true;
        if (File.Exists(Path.Combine(dataDir, "Iso", "PSP_GAME", "USRDIR", "resource.dat"))) { // If iso already extracted, enable available steps
            buttonExtractGame.Enabled = true;
            buttonStartGame.Enabled = true;
        }

        if (File.Exists(Path.Combine(dataDir, "Txt", "utf16.txt", "utf16.txt"))) {
            buttonDeleteGenRes.Visible = true;
            buttonExtractGame.Enabled = true;
            
            buttonTranslate.Enabled = true;
            buttonRepackGame.Enabled = true;
            buttonExportGame.Enabled = true;
        }
    }

    private void DisableButtons() {
        buttonExtractIso.Enabled = false;
        buttonExtractGame.Enabled = false;
        buttonExtractGame.Enabled = false;
        buttonDeleteGenRes.Visible = false;
        buttonTranslate.Enabled = false;
        buttonRepackGame.Enabled = false;
        buttonStartGame.Enabled = false;
        buttonExportGame.Enabled = false;
    }

    private async void buttonExtractIso_Click(object sender, EventArgs e) {
        try {
            using OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = "Toradora ISO (*.iso) | *.iso";
            if (openFileDialog.ShowDialog() != DialogResult.OK) return;

            ChangeStatus(true);
            DisableButtons();

            IsoProgress.Value = 0;
            buttonExtractIso.Visible = false;

            await Task.Run(() => IsoTools.ExtractIso(openFileDialog.FileName, progress => IsoProgress.Value = progress));
            MessageBox.Show("Iso extraction completed", "ToradoraTranslateTool", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex) {
            PrintStackTrace(ex);
            MessageBox.Show("Error!" + Environment.NewLine + ex, "ToradoraTranslateTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
            throw;
        }
        finally {
            ChangeStatus(false);
            EnableButtons();
            buttonExtractIso.Visible = true;
            IsoProgress.Value = 0;
        }
    }

    private async void buttonExtractGame_Click(object sender, EventArgs e) {
        Stopwatch stopwatch = new();
        stopwatch.Start();
        try {
            ChangeStatus(true);
            DisableButtons();

            /*// Resources
            await Task.Run(() => DatTools.ExtractDat(Path.Combine(dataDir, "Iso", "PSP_GAME", "USRDIR", "resource.dat")));
            await Task.Run(() => ObjTools.ProcessObjGz(Path.Combine(dataDir, "Extracted", "resource")));

            // First
            await Task.Run(() => DatTools.ExtractDat(Path.Combine(dataDir, "Iso", "PSP_GAME", "USRDIR", "first.dat")));
            await Task.Run(() => ObjTools.ProcessTxtGz(Path.Combine(dataDir, "Extracted", "first")));
            await Task.Run(() => ObjTools.ProcessSeekmap(Path.Combine(dataDir, "Extracted", "first")));*/

            await Task.WhenAll(
                Task.Run(() => { // resource
                    DatTools.ExtractDat(Path.Combine(dataDir, "Iso", "PSP_GAME", "USRDIR", "resource.dat")).Wait();
                    ObjTools.ProcessObjGz(Path.Combine(dataDir, "Extracted", "resource")).Wait();
                }),
                Task.Run(() => { // first
                    DatTools.ExtractDat(Path.Combine(dataDir, "Iso", "PSP_GAME", "USRDIR", "first.dat")).Wait();
                    ObjTools.ProcessTxtGz(Path.Combine(dataDir, "Extracted", "first")).Wait();
                    ObjTools.ProcessSeekmap(Path.Combine(dataDir, "Extracted", "first")).Wait();
                })
            );

            MessageBox.Show($"Game files extraction completed in {stopwatch.ElapsedMilliseconds} ms", "ToradoraTranslateTool", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception ex) {
            PrintStackTrace(ex);
            MessageBox.Show($"Error! in {stopwatch.ElapsedMilliseconds} ms" + Environment.NewLine + ex, "ToradoraTranslateTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
            throw;
        }
        finally {
            ChangeStatus(false);
            EnableButtons();
            stopwatch.Stop();
        }
    }

    private async void ButtonDeleteGenResClick(object sender, EventArgs e) {
        if (MessageBox.Show("This is an effective factory reset.\nAre you sure you want to continue?", "toradoraTranslateTool", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
        
        Stopwatch stopwatch = new();
        stopwatch.Start();
        try {
            ChangeStatus(true);
            DisableButtons();

            bool repacked = File.Exists(Path.Combine(dataDir, "Extracted", "-"));
            Task[] tasks = [
                Task.Run(() => Directory.Delete(Path.Combine(dataDir, "Extracted"), true)),
                Task.Run(() => Directory.Delete(Path.Combine(dataDir, "Obj"), true)),
                Task.Run(() => Directory.Delete(Path.Combine(dataDir, "Txt"), true)),
                Task.CompletedTask
            ];
            if (repacked) tasks[3] = Task.Run(() => Directory.Delete(Path.Combine(dataDir, "Iso"), true));
            
            await Task.WhenAll(tasks);
            MessageBox.Show($"Resource deletion completed in {stopwatch.ElapsedMilliseconds} ms", "ToradoraTranslateTool", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex) {
            PrintStackTrace(ex);
            MessageBox.Show($"Error! in {stopwatch.ElapsedMilliseconds} ms" + Environment.NewLine + ex, "ToradoraTranslateTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
            throw;
        }
        finally {
            stopwatch.Stop();
            ChangeStatus(false);
            EnableButtons();
        }
    }
    
    private void buttonTranslate_Click(object sender, EventArgs e) {
        FormTranslation myForm = new();
        myForm.Show();
    }

    private async void buttonRepackGame_Click(object sender, EventArgs e) {
        Stopwatch watch = new();
        watch.Start();
        try {
            ChangeStatus(true);
            DisableButtons();

            await Task.Run(() => ObjTools.RepackObjs(itemDebugMode.Checked));
            await Task.Run(ObjTools.RepackTxts);
            await Task.Run(() => DatTools.RepackDat(Path.Combine(dataDir, "Extracted", "resource.dat-LstOrder.lst")));
            await Task.Run(() => ObjTools.RepackSeekmap(Path.Combine(dataDir, "Extracted", "resource.dat"), Path.Combine(dataDir, "Extracted", "first")));
            await Task.Run(() => DatTools.RepackDat(Path.Combine(dataDir, "Extracted", "first.dat-LstOrder.lst")));
            File.Create(Path.Combine(dataDir, "Extracted", "-"));
            await Task.Run(() => File.Copy(Path.Combine(dataDir, "Extracted", "resource.dat"), Path.Combine(dataDir, "Iso", "PSP_GAME", "USRDIR", "resource.dat"), true));
            await Task.Run(() => File.Copy(Path.Combine(dataDir, "Extracted", "first.dat"), Path.Combine(dataDir, "Iso", "PSP_GAME", "USRDIR", "first.dat"), true));

            MessageBox.Show($"Game files repacking completed in {watch.ElapsedMilliseconds} ms", "ToradoraTranslateTool", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex) {
            PrintStackTrace(ex);
            MessageBox.Show($"Error in {watch.ElapsedMilliseconds} ms!{Environment.NewLine}{ex}", "ToradoraTranslateTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
            throw;
        }
        finally {
            watch.Stop();
            ChangeStatus(false);
            EnableButtons();
        }
    }

    private async void ButtonExportGame_Click(object sender, EventArgs e) {
        Stopwatch watch = new();
        try {
            ChangeStatus(true);
            DisableButtons();
            
            ExtractProgress.Value = 0;
            buttonExportGame.Visible = false;
            
            SaveFileDialog saveFileDialog = new() {
                Title = "Save File",
                Filter = "ISO files (*.iso)|*.iso|ZIP files (*.zip)|*.zip|All files (*.*)|*.*"
            };

            string isoPath = Path.Combine(dataDir, "Iso");
            //string isoFolder = Path.Combine(dataDir, "OutIso");
            
            if (saveFileDialog.ShowDialog() == DialogResult.OK) {
                string selectedPath = saveFileDialog.FileName;
                watch.Start();
                
                Action<float> callback = progress => ExtractProgress.Value = (int)progress;
                
                if (Path.GetExtension(selectedPath).Contains("iso", StringComparison.CurrentCultureIgnoreCase))
                    await Task.Run(() => IsoTools.RepackIso(isoPath, selectedPath, callback));
                else await IsoTools.RepackZip(isoPath, selectedPath, callback);
            }
            /*else {
                using FolderBrowserDialog folderBrowserDialog = new();
                folderBrowserDialog.Description = "Or pick a folder instead";
                if (folderBrowserDialog.ShowDialog() != DialogResult.OK) return;
                string selectedFolder = folderBrowserDialog.SelectedPath;
                if (!string.IsNullOrEmpty(selectedFolder)) return;
                if (Directory.EnumerateFileSystemEntries(selectedFolder).Any())
                    if (MessageBox.Show("This folder isn't empty\nAre you sure you want to replace all folder contents with the game files?", "ToradoraTranslateTool", MessageBoxButtons.YesNo) != DialogResult.Yes) {
                        DirectoryInfo directoryInfo = new(selectedFolder);
                        foreach (FileInfo file in directoryInfo.GetFiles()) file.Delete();
                        foreach (DirectoryInfo dir in directoryInfo.GetDirectories()) dir.Delete(true);
                    }

                if (!Directory.Exists(selectedFolder)) 
                    Directory.CreateDirectory(selectedFolder);

                // copy the directory and be done idfk
            }*/
            MessageBox.Show($"Game export completed in {watch.ElapsedMilliseconds} ms", "ToradoraTranslateTool", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex) {
            PrintStackTrace(ex);
            MessageBox.Show("Error!" + Environment.NewLine + ex, "ToradoraTranslateTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
            throw;
        }
        finally {
            watch.Stop();
            ChangeStatus(false);
            EnableButtons();
            ExtractProgress.Value = 0;
            buttonExportGame.Visible = true;
        }
    }

    private void buttonStartGame_Click(object sender, EventArgs e) {
        Stopwatch watch = new();
        watch.Start();
        try {
            ChangeStatus(true);
            DisableButtons();
            string filename = "Data/StartGame.conf";
            
            if (!File.Exists(filename) || new FileInfo(filename).Length == 0) {
                File.Create(filename);
                string errorText = $"Not configured!\n{Path.GetFullPath(filename)}";
                buttonStartGameHelp_Click(sender, e);
                throw new DataException(errorText);
            }

            string[] fileContents = File.ReadAllLines(filename);
            using Process process = new();
            
            string args = "";
            if (fileContents.Length == 2)
                args = fileContents[1];
            
            process.StartInfo = new()
            {
                FileName = fileContents[0],
                Arguments = args + " " + Path.Combine(dataDir, "Iso")
            };
            process.Start();
            process.WaitForExit();
            //MessageBox.Show($"Game files repacking completed in {watch.ElapsedMilliseconds} ms", "ToradoraTranslateTool", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex) {
            PrintStackTrace(ex);
            MessageBox.Show($"Error in {watch.ElapsedMilliseconds} ms!{Environment.NewLine}{ex}", "ToradoraTranslateTool", MessageBoxButtons.OK, MessageBoxIcon.Error);
            throw;
        }
        finally {
            watch.Stop();
            ChangeStatus(false);
            EnableButtons();
        }
    }
    
    // ReSharper disable once AssignmentInConditionalExpression
    private void ChangeStatus(bool isWorking) => labelWork.Text = (timerWork.Enabled = isWorking) ? "Working" : "Ready";

    private void timerWork_Tick(object sender, EventArgs e) {
        if (labelWork.Text == "Working...")
            labelWork.Text = "Working";
        else labelWork.Text += ".";
    }

    private void FormMain_FormClosing(object sender, FormClosingEventArgs e) {
        if (!timerWork.Enabled) return;
        if (MessageBox.Show("There's an ongoing task\nAre you sure you want to close the app?", "ToradoraTranslateTool", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No) // for some reason those are reversed
            e.Cancel = true;
    }

    private void buttonExtractIsoHelp_Click(object sender, EventArgs e) {
        MessageBox.Show("This stage will extract selected ISO file to the \\Data\\Iso\\ folder", "ToradoraTranslateTool", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void buttonExtractGameHelp_Click(object sender, EventArgs e) {
        MessageBox.Show("This stage will extract and process .dat files from ISO." + Environment.NewLine + "It'll take ~40 seconds depending on the CPU", "ToradoraTranslateTool", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void buttonTranslateHelp_Click(object sender, EventArgs e) {
        MessageBox.Show("At this stage you will be able to translate the game text, including menus and settings", "ToradoraTranslateTool", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void buttonRepackGameHelp_Click(object sender, EventArgs e) {
        MessageBox.Show("" +
            "This stage will inject translation and repack all game files." + Environment.NewLine +
            "It'll take ~5-10 seconds depending on the SSD." + Environment.NewLine +
            "You can enable debug mode by right-clicking on the repack button." + Environment.NewLine +
            "In this mode you will be able to teleport to any level, and much more",
            "ToradoraTranslateTool", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void buttonStartGameHelp_Click(object sender, EventArgs e) {
        MessageBox.Show("" +
            "This isn't necessarily a stage" + Environment.NewLine +
             "Just a utility button that helps you test your changes after packing the game" + Environment.NewLine +
             "You can configure this process with the StartGame.conf" + Environment.NewLine + 
             "Put the path of the executable on the first line and the args on the second" + Environment.NewLine +
             "The path of the Iso folder will be appended to the arguments" + Environment.NewLine +
             "Example:" + Environment.NewLine +
             "{executable}" + Environment.NewLine +
             "{arguments}" + Environment.NewLine,
            "ToradoraTranslateTool", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
    
    private void buttonRepackIsoHelp_Click(object sender, EventArgs e) {
        MessageBox.Show("This stage will repack ISO and save it in the program folder", "ToradoraTranslateTool", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
    
    protected override void WndProc(ref Message m) {
        base.WndProc(ref m);
        if (m.Msg == WmNchittest)
            m.Result = HtCaption;
    }

    private const int WmNchittest = 0x84;
    //private const int HT_CLIENT = 0x1;
    private const int HtCaption = 0x2;
    
    private void Exit_click(object sender, EventArgs e) => Application.Exit();

    private static void PrintStackTrace(Exception e) => _ = Console.Error.WriteLineAsync(e.ToString());
}