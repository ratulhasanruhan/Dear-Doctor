using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Dear_Doctor.Views
{
    public partial class SettingsView : UserControl
    {
        private const string StartupAppName = "DearDoctor";
        private const string RunKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        private static readonly string AppDataFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
            "DearDoctor"
        );
        private static readonly string SettingsPath = Path.Combine(AppDataFolder, "settings.json");

        public class AppSettings
        {
            public bool RunOnStartup { get; set; } = true;
            public bool IsInitialized { get; set; } = false;
        }

        public SettingsView()
        {
            InitializeComponent();
            CheckStartupStatus();
        }

        public static void InitializeStartupSetting()
        {
            try
            {
                if (!Directory.Exists(AppDataFolder))
                {
                    Directory.CreateDirectory(AppDataFolder);
                }

                AppSettings settings;
                if (File.Exists(SettingsPath))
                {
                    string json = File.ReadAllText(SettingsPath);
                    settings = System.Text.Json.JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
                }
                else
                {
                    // First run: default to run on startup (true)
                    settings = new AppSettings { RunOnStartup = true, IsInitialized = true };
                    string json = System.Text.Json.JsonSerializer.Serialize(settings, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(SettingsPath, json);
                }

                // Apply to registry
                using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(RunKeyPath, true))
                {
                    if (key != null)
                    {
                        if (settings.RunOnStartup)
                        {
                            string? appPath = Environment.ProcessPath;
                            if (!string.IsNullOrEmpty(appPath))
                            {
                                key.SetValue(StartupAppName, $"\"{appPath}\"");
                            }
                        }
                        else
                        {
                            key.DeleteValue(StartupAppName, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to initialize startup settings: {ex.Message}");
            }
        }

        private void CheckStartupStatus()
        {
            try
            {
                if (!Directory.Exists(AppDataFolder))
                {
                    Directory.CreateDirectory(AppDataFolder);
                }

                AppSettings settings;
                if (File.Exists(SettingsPath))
                {
                    string json = File.ReadAllText(SettingsPath);
                    settings = System.Text.Json.JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
                }
                else
                {
                    settings = new AppSettings { RunOnStartup = true, IsInitialized = true };
                    SaveSettings(settings);
                }

                SetRunOnStartup(settings.RunOnStartup, saveSettings: false);

                // Apply to CheckBox
                StartupCheckBox.Checked -= StartupCheckBox_Checked;
                StartupCheckBox.Unchecked -= StartupCheckBox_Unchecked;
                
                StartupCheckBox.IsChecked = settings.RunOnStartup;
                
                StartupCheckBox.Checked += StartupCheckBox_Checked;
                StartupCheckBox.Unchecked += StartupCheckBox_Unchecked;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load startup settings: {ex.Message}");
            }
        }

        private void SaveSettings(AppSettings settings)
        {
            try
            {
                string json = System.Text.Json.JsonSerializer.Serialize(settings, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SettingsPath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to save settings: {ex.Message}");
            }
        }

        private void StartupCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SetRunOnStartup(true, saveSettings: true);
        }

        private void StartupCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SetRunOnStartup(false, saveSettings: true);
        }

        private void SetRunOnStartup(bool runOnStartup, bool saveSettings)
        {
            try
            {
                using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(RunKeyPath, true))
                {
                    if (key != null)
                    {
                        if (runOnStartup)
                        {
                            string? appPath = Environment.ProcessPath;
                            if (!string.IsNullOrEmpty(appPath))
                            {
                                key.SetValue(StartupAppName, $"\"{appPath}\"");
                            }
                        }
                        else
                        {
                            key.DeleteValue(StartupAppName, false);
                        }
                    }
                }

                if (saveSettings)
                {
                    var settings = new AppSettings { RunOnStartup = runOnStartup, IsInitialized = true };
                    SaveSettings(settings);
                }
            }
            catch (Exception ex)
            {
                if (saveSettings)
                {
                    MessageBox.Show($"Could not update startup setting: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
