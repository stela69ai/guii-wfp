//using SimpleCrypt.Functions.Helpers;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Diagnostics;
//using System.Drawing;
//using System.IO;
//using System.Linq;
//using System.Security.Cryptography;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

//namespace SimpleCrypt.Views
//{
//    public partial class Menu : Form
//    {
//        private string rsaPrivateKey = "";
//        private string rsaPublicKey = "";

//        public Menu()
//        {
//            InitializeComponent();
//            FileLogs.SetLogViewer(log_textbox);
//            extentions_comboBox.SelectedItem = "(All Files)";

//            // Set better default sizes
//            this.encryption_password_textbox.Size = new Size(200, 25);
//        }

//        private void Menu_Load(object sender, EventArgs e)
//        {
//            item_file_path_list_box.DragEnter += item_file_path_list_box_DragEnter;
//            item_file_path_list_box.DragDrop += item_file_path_list_box_DragDrop;

//            // Load settings
//            save_password_checkbox.Checked = Properties.Settings.Default.password_checkbox_setting;
//            delete_orginal_files_checkbox.Checked = Properties.Settings.Default.delete_orginal_files_setting;
//            follow_sub_folders_checkbox.Checked = Properties.Settings.Default.follow_sub_folders_setting;

//            if (save_password_checkbox.Checked)
//            {
//                encryption_password_textbox.Text = Properties.Settings.Default.encryption_password_saved;
//            }
//        }

//        private void save_password_checkbox_CheckedChanged(object sender, EventArgs e)
//        {
//            Properties.Settings.Default.password_checkbox_setting = save_password_checkbox.Checked;
//            if (save_password_checkbox.Checked)
//            {
//                Properties.Settings.Default.encryption_password_saved = encryption_password_textbox.Text;
//            }
//            else
//            {
//                Properties.Settings.Default.encryption_password_saved = "";
//                encryption_password_textbox.Text = "";
//            }
//            Properties.Settings.Default.Save();
//        }

//        private void encryption_password_textbox_TextChanged(object sender, EventArgs e)
//        {
//            if (save_password_checkbox.Checked)
//            {
//                Properties.Settings.Default.encryption_password_saved = encryption_password_textbox.Text;
//                Properties.Settings.Default.Save();
//            }
//        }

//        private void delete_orginal_files_checkbox_CheckedChanged(object sender, EventArgs e)
//        {
//            Properties.Settings.Default.delete_orginal_files_setting = delete_orginal_files_checkbox.Checked;
//            Properties.Settings.Default.Save();
//        }

//        private void follow_sub_folders_checkbox_CheckedChanged(object sender, EventArgs e)
//        {
//            Properties.Settings.Default.follow_sub_folders_setting = follow_sub_folders_checkbox.Checked;
//            Properties.Settings.Default.Save();
//        }

//        private void item_file_path_list_box_DragDrop(object sender, DragEventArgs e)
//        {
//            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
//            foreach (string file in files)
//            {
//                if (Directory.Exists(file) || File.Exists(file))
//                {
//                    if (!item_file_path_list_box.Items.Contains(file))
//                        item_file_path_list_box.Items.Add(file);
//                }
//            }
//        }

//        private void item_file_path_list_box_DragEnter(object sender, DragEventArgs e)
//        {
//            if (e.Data.GetDataPresent(DataFormats.FileDrop))
//                e.Effect = DragDropEffects.Copy;
//        }

//        private void remove_item_button_Click(object sender, EventArgs e)
//        {
//            if (item_file_path_list_box.SelectedIndex != -1)
//            {
//                item_file_path_list_box.Items.RemoveAt(item_file_path_list_box.SelectedIndex);
//            }
//        }

//        private void generate_rsa_keys_button_Click(object sender, EventArgs e)
//        {
//            try
//            {
//                // Generate RSA keys
//                rsaPrivateKey = SimpleCrypt.Functions.Encryption.RSA.RSA.GenerateKeyPair();
//                rsaPublicKey = SimpleCrypt.Functions.Encryption.RSA.RSA.GetPublicKey(rsaPrivateKey);

//                // Save private key to file automatically
//                string privateKeyPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "rsa_private_key.txt");
//                File.WriteAllText(privateKeyPath, rsaPrivateKey);

//                // Save public key to file automatically
//                string publicKeyPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "rsa_public_key.txt");
//                File.WriteAllText(publicKeyPath, rsaPublicKey);

//                // Show success message with file locations
//                string message = $"RSA Key Pair generated successfully!\n\n" +
//                               $"Private Key saved to: {privateKeyPath}\n" +
//                               $"Public Key saved to: {publicKeyPath}\n\n" +
//                               $"Private Key (first 100 chars):\n{rsaPrivateKey.Substring(0, Math.Min(100, rsaPrivateKey.Length))}...\n\n" +
//                               $"Public Key (first 100 chars):\n{rsaPublicKey.Substring(0, Math.Min(100, rsaPublicKey.Length))}...\n\n" +
//                               $"Do you want to load the private key into the password field for decryption?";

//                var result = MessageBox.Show(message, "RSA Keys Generated",
//                    MessageBoxButtons.YesNo, MessageBoxIcon.Information);

//                if (result == DialogResult.Yes)
//                {
//                    encryption_password_textbox.Text = rsaPrivateKey;
//                    FileLogs.Log("RSA private key loaded into password field.");
//                }

//                FileLogs.Log($"RSA keys generated and saved to desktop. Private: {privateKeyPath}, Public: {publicKeyPath}");
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Error generating RSA keys: {ex.Message}", "Error",
//                    MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        private void load_private_key_button_Click(object sender, EventArgs e)
//        {
//            using (OpenFileDialog openFileDialog = new OpenFileDialog())
//            {
//                openFileDialog.Filter = "Text Files|*.txt|All Files|*.*";
//                openFileDialog.Title = "Load RSA Private Key";
//                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

//                if (openFileDialog.ShowDialog() == DialogResult.OK)
//                {
//                    try
//                    {
//                        string privateKey = File.ReadAllText(openFileDialog.FileName);
//                        encryption_password_textbox.Text = privateKey;
//                        FileLogs.Log($"Private key loaded from: {openFileDialog.FileName}");
//                        MessageBox.Show("Private key loaded successfully!", "Success",
//                            MessageBoxButtons.OK, MessageBoxIcon.Information);
//                    }
//                    catch (Exception ex)
//                    {
//                        MessageBox.Show($"Error loading private key: {ex.Message}", "Error",
//                            MessageBoxButtons.OK, MessageBoxIcon.Error);
//                    }
//                }
//            }
//        }

//        private void load_public_key_button_Click(object sender, EventArgs e)
//        {
//            using (OpenFileDialog openFileDialog = new OpenFileDialog())
//            {
//                openFileDialog.Filter = "Text Files|*.txt|All Files|*.*";
//                openFileDialog.Title = "Load RSA Public Key";
//                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

//                if (openFileDialog.ShowDialog() == DialogResult.OK)
//                {
//                    try
//                    {
//                        string publicKey = File.ReadAllText(openFileDialog.FileName);
//                        encryption_password_textbox.Text = publicKey;
//                        FileLogs.Log($"Public key loaded from: {openFileDialog.FileName}");
//                        MessageBox.Show("Public key loaded successfully!", "Success",
//                            MessageBoxButtons.OK, MessageBoxIcon.Information);
//                    }
//                    catch (Exception ex)
//                    {
//                        MessageBox.Show($"Error loading public key: {ex.Message}", "Error",
//                            MessageBoxButtons.OK, MessageBoxIcon.Error);
//                    }
//                }
//            }
//        }

//        private async void encrypt_button_Click(object sender, EventArgs e)
//        {
//            if (string.IsNullOrEmpty(encryption_password_textbox.Text))
//            {
//                MessageBox.Show("Please enter an encryption password/key", "Error",
//                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                return;
//            }

//            if (!IsAnyEncryptionMethodSelected())
//            {
//                MessageBox.Show("Please select at least one encryption method", "Error",
//                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                return;
//            }

//            await ProcessFilesAsync(true); // true for encryption
//        }

//        private async void decrypt_button_Click(object sender, EventArgs e)
//        {
//            if (string.IsNullOrEmpty(encryption_password_textbox.Text))
//            {
//                MessageBox.Show("Please enter a decryption password/key", "Error",
//                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                return;
//            }

//            if (!IsAnyEncryptionMethodSelected())
//            {
//                MessageBox.Show("Please select at least one decryption method", "Error",
//                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                return;
//            }

//            await ProcessFilesAsync(false); // false for decryption
//        }

//        private bool IsAnyEncryptionMethodSelected()
//        {
//            return aes_checkbox.Checked || TripleDES_checkbox.Checked || DES_checkbox.Checked ||
//                   chacha20_checkbox.Checked || blowfish_checkbox.Checked || rc4_checkbox.Checked ||
//                   rsa_checkbox.Checked;
//        }

//        private async Task ProcessFilesAsync(bool isEncryption)
//        {
//            var paths = item_file_path_list_box.Items.Cast<string>().ToList();
//            if (paths.Count == 0)
//            {
//                MessageBox.Show("Please add files or folders to process", "Error",
//                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                return;
//            }

//            var methods = GetSelectedMethods();
//            string operation = isEncryption ? "Encryption" : "Decryption";

//            foreach (string method in methods)
//            {
//                FileLogs.Log($"{method} {operation} Started.");
//                int count = 0;

//                foreach (string path in paths)
//                {
//                    count += await ProcessPathAsync(path, method, isEncryption);
//                }

//                FileLogs.Log($"Finished: {count} File(s) {(isEncryption ? "Encrypted" : "Decrypted")} with {method}.");
//            }
//        }

//        private List<string> GetSelectedMethods()
//        {
//            var methods = new List<string>();
//            if (aes_checkbox.Checked) methods.Add("AES");
//            if (DES_checkbox.Checked) methods.Add("DES");
//            if (TripleDES_checkbox.Checked) methods.Add("3DES");
//            if (chacha20_checkbox.Checked) methods.Add("CHACHA20");
//            if (blowfish_checkbox.Checked) methods.Add("BLOWFISH");
//            if (rc4_checkbox.Checked) methods.Add("RC4");
//            if (rsa_checkbox.Checked) methods.Add("RSA");
//            return methods;
//        }

//        private async Task<int> ProcessPathAsync(string path, string method, bool isEncryption)
//        {
//            int count = 0;
//            string password = encryption_password_textbox.Text;

//            try
//            {
//                if (File.Exists(path))
//                {
//                    if (isEncryption)
//                    {
//                        if (path.CheckExtension(extentions_comboBox.Text.ParseExtensions()))
//                        {
//                            await path.EncryptFileAsync(password, method);
//                            FileLogs.Log(path + " Encrypted.");
//                            count++;
//                            if (delete_orginal_files_checkbox.Checked)
//                                File.Delete(path);
//                        }
//                    }
//                    else
//                    {
//                        string expectedExtension = GetExpectedExtension(method);
//                        if (path.EndsWith(expectedExtension))
//                        {
//                            await path.DecryptFileAsync(password, method);
//                            FileLogs.Log(path + " Decrypted.");
//                            count++;
//                            if (delete_orginal_files_checkbox.Checked)
//                                File.Delete(path);
//                        }
//                    }
//                }
//                else if (Directory.Exists(path))
//                {
//                    var followSubDirs = follow_sub_folders_checkbox.Checked;
//                    var allfiles = path.GetFolderFilesPaths(followSubDirs);

//                    foreach (var file in allfiles)
//                    {
//                        if (isEncryption)
//                        {
//                            if (file.CheckExtension(extentions_comboBox.Text.ParseExtensions()) && !file.EndsWith(GetExpectedExtension(method)))
//                            {
//                                await file.EncryptFileAsync(password, method);
//                                FileLogs.Log(file + " Encrypted.");
//                                count++;
//                                if (delete_orginal_files_checkbox.Checked)
//                                    File.Delete(file);
//                            }
//                        }
//                        else
//                        {
//                            string expectedExtension = GetExpectedExtension(method);
//                            if (file.RemoveExtension().CheckExtension(extentions_comboBox.Text.ParseExtensions()) && file.EndsWith(expectedExtension))
//                            {
//                                await file.DecryptFileAsync(password, method);
//                                FileLogs.Log(file + " Decrypted.");
//                                count++;
//                                if (delete_orginal_files_checkbox.Checked)
//                                    File.Delete(file);
//                            }
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                FileLogs.Log($"Error processing {path}: {ex.Message}");
//            }

//            return count;
//        }

//        private string GetExpectedExtension(string method)
//        {
//            switch (method.ToUpper())
//            {
//                case "AES": return ".aes";
//                case "DES": return ".des";
//                case "3DES": return ".3des";
//                case "CHACHA20": return ".chacha20";
//                case "BLOWFISH": return ".blowfish";
//                case "RC4": return ".rc4";
//                case "RSA": return ".rsa";
//                default: return "." + method.ToLower();
//            }
//        }

//        private void add_files_button_Click(object sender, EventArgs e)
//        {
//            using (OpenFileDialog fileDialog = new OpenFileDialog())
//            {
//                fileDialog.Title = "Select Files";
//                fileDialog.Multiselect = true;
//                fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

//                if (fileDialog.ShowDialog() == DialogResult.OK)
//                {
//                    foreach (string filePath in fileDialog.FileNames)
//                    {
//                        if (!item_file_path_list_box.Items.Contains(filePath))
//                            item_file_path_list_box.Items.Add(filePath);
//                    }
//                }
//            }
//        }

//        private void add_folder_button_Click(object sender, EventArgs e)
//        {
//            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
//            {
//                folderDialog.Description = "Select Folder";
//                folderDialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

//                if (folderDialog.ShowDialog() == DialogResult.OK)
//                {
//                    string folderPath = folderDialog.SelectedPath;
//                    if (!item_file_path_list_box.Items.Contains(folderPath))
//                        item_file_path_list_box.Items.Add(folderPath);
//                }
//            }
//        }

//        private void save_logs_button_Click(object sender, EventArgs e)
//        {
//            if (string.IsNullOrEmpty(log_textbox.Text))
//            {
//                MessageBox.Show("No logs to save", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
//                return;
//            }

//            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
//            {
//                saveFileDialog.Filter = "Text Files|*.txt|All Files|*.*";
//                saveFileDialog.FileName = $"SimpleCrypt_Logs_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
//                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

//                if (saveFileDialog.ShowDialog() == DialogResult.OK)
//                {
//                    File.WriteAllText(saveFileDialog.FileName, log_textbox.Text);
//                    MessageBox.Show("Logs saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
//                }
//            }
//        }

//        private void creator_label_Click(object sender, EventArgs e)
//        {
//            Process.Start("https://github.com/Nova-Squad");
//        }

//        private void clear_list_button_Click(object sender, EventArgs e)
//        {
//            item_file_path_list_box.Items.Clear();
//        }

//        private void clear_logs_button_Click(object sender, EventArgs e)
//        {
//            log_textbox.Clear();
//        }
//    }
//}


using SimpleCrypt.Functions.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace SimpleCrypt.Views
{
    public partial class Menu : Form
    {
        public Menu()
        {
            InitializeComponent();
            FileLogs.SetLogViewer(log_textbox);
            extentions_comboBox.SelectedItem = "(All Files)";
        }

        private void Menu_Load(object sender, EventArgs e)
        {
            item_file_path_list_box.DragEnter += item_file_path_list_box_DragEnter;
            item_file_path_list_box.DragDrop += item_file_path_list_box_DragDrop;

            // Load settings
            save_password_checkbox.Checked = Properties.Settings.Default.password_checkbox_setting;
            delete_orginal_files_checkbox.Checked = Properties.Settings.Default.delete_orginal_files_setting;
            follow_sub_folders_checkbox.Checked = Properties.Settings.Default.follow_sub_folders_setting;

            if (save_password_checkbox.Checked)
            {
                encryption_password_textbox.Text = Properties.Settings.Default.encryption_password_saved;
            }
        }

        private void save_password_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.password_checkbox_setting = save_password_checkbox.Checked;
            if (save_password_checkbox.Checked)
            {
                Properties.Settings.Default.encryption_password_saved = encryption_password_textbox.Text;
            }
            else
            {
                Properties.Settings.Default.encryption_password_saved = "";
                encryption_password_textbox.Text = "";
            }
            Properties.Settings.Default.Save();
        }

        private void encryption_password_textbox_TextChanged(object sender, EventArgs e)
        {
            if (save_password_checkbox.Checked)
            {
                Properties.Settings.Default.encryption_password_saved = encryption_password_textbox.Text;
                Properties.Settings.Default.Save();
            }
        }

        private void delete_orginal_files_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.delete_orginal_files_setting = delete_orginal_files_checkbox.Checked;
            Properties.Settings.Default.Save();
        }

        private void follow_sub_folders_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.follow_sub_folders_setting = follow_sub_folders_checkbox.Checked;
            Properties.Settings.Default.Save();
        }

        private void item_file_path_list_box_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {
                if ((Directory.Exists(file) || File.Exists(file)) && !item_file_path_list_box.Items.Contains(file))
                    item_file_path_list_box.Items.Add(file);
            }
        }

        private void item_file_path_list_box_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void remove_item_button_Click(object sender, EventArgs e)
        {
            if (item_file_path_list_box.SelectedIndex != -1)
                item_file_path_list_box.Items.RemoveAt(item_file_path_list_box.SelectedIndex);
        }

        private void load_rsa_key_button_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "RSA Key Files|*.txt;*.xml;*.key|All Files|*.*";
                openFileDialog.Title = "Load RSA Key File";
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string keyContent = File.ReadAllText(openFileDialog.FileName);
                        encryption_password_textbox.Text = keyContent;

                        string keyType = DetectKeyType(keyContent);
                        FileLogs.Log($"{keyType} loaded from: {openFileDialog.FileName}");
                        MessageBox.Show($"{keyType} loaded successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading key file: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private string DetectKeyType(string keyContent)
        {
            if (keyContent.Contains("<RSAKeyValue>"))
            {
                if (keyContent.Contains("<D>") || keyContent.Contains("<P>") || keyContent.Contains("<Q>") ||
                    keyContent.Contains("<DP>") || keyContent.Contains("<DQ>") || keyContent.Contains("<InverseQ>"))
                {
                    return "Private RSA key";
                }
                else
                {
                    return "Public RSA key";
                }
            }
            return "Key file";
        }

        private bool IsRSAKey(string key)
        {
            return key.Contains("<RSAKeyValue>");
        }

        private bool IsPrivateKey(string key)
        {
            return key.Contains("<RSAKeyValue>") &&
                   (key.Contains("<D>") || key.Contains("<P>") || key.Contains("<Q>") ||
                    key.Contains("<DP>") || key.Contains("<DQ>") || key.Contains("<InverseQ>"));
        }

        private bool IsPublicKey(string key)
        {
            return key.Contains("<RSAKeyValue>") && !IsPrivateKey(key);
        }

        private void generate_rsa_keys_button_Click(object sender, EventArgs e)
        {
            try
            {
                // Generate RSA keys
                string privateKey = SimpleCrypt.Functions.Encryption.RSA.RSA.GenerateKeyPair();
                string publicKey = SimpleCrypt.Functions.Encryption.RSA.RSA.GetPublicKey(privateKey);

                // Ask user where to save private key
                using (SaveFileDialog saveDialog = new SaveFileDialog())
                {
                    saveDialog.Filter = "Text Files|*.txt|All Files|*.*";
                    saveDialog.Title = "Save RSA Private Key";
                    saveDialog.FileName = "rsa_private_key.txt";
                    saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Save private key
                        File.WriteAllText(saveDialog.FileName, privateKey);

                        // Save public key in same location with different name
                        string publicKeyPath = Path.Combine(
                            Path.GetDirectoryName(saveDialog.FileName),
                            "rsa_public_key.txt");
                        File.WriteAllText(publicKeyPath, publicKey);

                        string message = $"RSA Key Pair generated and saved successfully!\n\n" +
                                       $"Private Key saved to: {saveDialog.FileName}\n" +
                                       $"Public Key saved to: {publicKeyPath}\n\n" +
                                       $"Do you want to load the private key now?";

                        var result = MessageBox.Show(message, "RSA Keys Generated",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                        if (result == DialogResult.Yes)
                        {
                            encryption_password_textbox.Text = privateKey;
                            FileLogs.Log("RSA private key loaded into password field.");
                        }

                        FileLogs.Log($"RSA keys generated. Private: {saveDialog.FileName}, Public: {publicKeyPath}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating RSA keys: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void encrypt_button_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(encryption_password_textbox.Text))
            {
                MessageBox.Show("Please enter an encryption password/key", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!IsAnyEncryptionMethodSelected())
            {
                MessageBox.Show("Please select at least one encryption method", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check if trying to encrypt with private key
            string key = encryption_password_textbox.Text;
            if (rsa_checkbox.Checked && IsPrivateKey(key))
            {
                MessageBox.Show("Cannot encrypt with a PRIVATE key!\n\nPlease use a PUBLIC key for encryption.",
                    "Invalid Key", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            await ProcessFilesAsync(true);
        }

        private async void decrypt_button_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(encryption_password_textbox.Text))
            {
                MessageBox.Show("Please enter a decryption password/key", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!IsAnyEncryptionMethodSelected())
            {
                MessageBox.Show("Please select at least one decryption method", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check if trying to decrypt with public key
            string key = encryption_password_textbox.Text;
            if (rsa_checkbox.Checked && IsPublicKey(key))
            {
                MessageBox.Show("Cannot decrypt with a PUBLIC key!\n\nPlease use a PRIVATE key for decryption.",
                    "Invalid Key", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            await ProcessFilesAsync(false);
        }

        private bool IsAnyEncryptionMethodSelected()
        {
            return aes_checkbox.Checked || TripleDES_checkbox.Checked || DES_checkbox.Checked ||
                   chacha20_checkbox.Checked || blowfish_checkbox.Checked || rc4_checkbox.Checked ||
                   rsa_checkbox.Checked;
        }

        private async Task ProcessFilesAsync(bool isEncryption)
        {
            var paths = item_file_path_list_box.Items.Cast<string>().ToList();
            if (paths.Count == 0)
            {
                MessageBox.Show("Please add files or folders to process", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var methods = GetSelectedMethods();
            string operation = isEncryption ? "Encryption" : "Decryption";

            // Show key type info for RSA
            if (rsa_checkbox.Checked)
            {
                string key = encryption_password_textbox.Text;
                string keyType = IsPrivateKey(key) ? "Private" : "Public";
                FileLogs.Log($"Using {keyType} RSA key for {operation}");
            }

            foreach (string method in methods)
            {
                FileLogs.Log($"{method} {operation} Started.");
                int count = 0;

                foreach (string path in paths)
                {
                    count += await ProcessPathAsync(path, method, isEncryption);
                }

                FileLogs.Log($"Finished: {count} File(s) {(isEncryption ? "Encrypted" : "Decrypted")} with {method}.");
            }
        }

        private List<string> GetSelectedMethods()
        {
            var methods = new List<string>();
            if (aes_checkbox.Checked) methods.Add("AES");
            if (DES_checkbox.Checked) methods.Add("DES");
            if (TripleDES_checkbox.Checked) methods.Add("3DES");
            if (chacha20_checkbox.Checked) methods.Add("CHACHA20");
            if (blowfish_checkbox.Checked) methods.Add("BLOWFISH");
            if (rc4_checkbox.Checked) methods.Add("RC4");
            if (rsa_checkbox.Checked) methods.Add("RSA");
            return methods;
        }

        private async Task<int> ProcessPathAsync(string path, string method, bool isEncryption)
        {
            int count = 0;
            string password = encryption_password_textbox.Text;

            try
            {
                if (File.Exists(path))
                {
                    if (isEncryption)
                    {
                        if (path.CheckExtension(extentions_comboBox.Text.ParseExtensions()))
                        {
                            await path.EncryptFileAsync(password, method);
                            FileLogs.Log(path + " Encrypted.");
                            count++;
                            if (delete_orginal_files_checkbox.Checked)
                                File.Delete(path);
                        }
                    }
                    else
                    {
                        string expectedExtension = GetExpectedExtension(method);
                        if (path.EndsWith(expectedExtension))
                        {
                            await path.DecryptFileAsync(password, method);
                            FileLogs.Log(path + " Decrypted.");
                            count++;
                            if (delete_orginal_files_checkbox.Checked)
                                File.Delete(path);
                        }
                    }
                }
                else if (Directory.Exists(path))
                {
                    var followSubDirs = follow_sub_folders_checkbox.Checked;
                    var allfiles = path.GetFolderFilesPaths(followSubDirs);

                    foreach (var file in allfiles)
                    {
                        if (isEncryption)
                        {
                            if (file.CheckExtension(extentions_comboBox.Text.ParseExtensions()) && !file.EndsWith(GetExpectedExtension(method)))
                            {
                                await file.EncryptFileAsync(password, method);
                                FileLogs.Log(file + " Encrypted.");
                                count++;
                                if (delete_orginal_files_checkbox.Checked)
                                    File.Delete(file);
                            }
                        }
                        else
                        {
                            string expectedExtension = GetExpectedExtension(method);
                            if (file.RemoveExtension().CheckExtension(extentions_comboBox.Text.ParseExtensions()) && file.EndsWith(expectedExtension))
                            {
                                await file.DecryptFileAsync(password, method);
                                FileLogs.Log(file + " Decrypted.");
                                count++;
                                if (delete_orginal_files_checkbox.Checked)
                                    File.Delete(file);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FileLogs.Log($"Error processing {path}: {ex.Message}");
            }

            return count;
        }

        private string GetExpectedExtension(string method)
        {
            switch (method.ToUpper())
            {
                case "AES": return ".aes";
                case "DES": return ".des";
                case "3DES": return ".3des";
                case "CHACHA20": return ".chacha20";
                case "BLOWFISH": return ".blowfish";
                case "RC4": return ".rc4";
                case "RSA": return ".rsa";
                default: return "." + method.ToLower();
            }
        }

        private void add_files_button_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog fileDialog = new OpenFileDialog())
            {
                fileDialog.Title = "Select Files";
                fileDialog.Multiselect = true;
                fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (string filePath in fileDialog.FileNames)
                    {
                        if (!item_file_path_list_box.Items.Contains(filePath))
                            item_file_path_list_box.Items.Add(filePath);
                    }
                }
            }
        }

        private void add_folder_button_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select Folder";
                folderDialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string folderPath = folderDialog.SelectedPath;
                    if (!item_file_path_list_box.Items.Contains(folderPath))
                        item_file_path_list_box.Items.Add(folderPath);
                }
            }
        }

        private void save_logs_button_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(log_textbox.Text))
            {
                MessageBox.Show("No logs to save", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Text Files|*.txt|All Files|*.*";
                saveFileDialog.FileName = $"SimpleCrypt_Logs_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(saveFileDialog.FileName, log_textbox.Text);
                    MessageBox.Show("Logs saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void creator_label_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/Nova-Squad");
        }

        private void clear_list_button_Click(object sender, EventArgs e)
        {
            item_file_path_list_box.Items.Clear();
        }

        private void clear_logs_button_Click(object sender, EventArgs e)
        {
            log_textbox.Clear();
        }
    }
}