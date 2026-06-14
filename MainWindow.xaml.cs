using System;
using System.Windows;
using System.Windows.Input;
using Dear_Doctor.Views;

namespace Dear_Doctor
{
    public partial class MainWindow : Window
    {
        private PrescriptionView? _prescriptionView;
        private MedicineView? _medicineView;

        public MainWindow()
        {
            InitializeComponent();
            InitializeViews();
        }

        private void InitializeViews()
        {
            _prescriptionView = new PrescriptionView();
            _medicineView = new MedicineView();
            
            // Set default view
            MainContentFrame.Content = _prescriptionView;
        }

        private void NavigationTab_Checked(object sender, RoutedEventArgs e)
        {
            if (MainContentFrame == null) return;

            if (PrescriptionTab.IsChecked == true)
            {
                // Refresh medicine suggestions in the prescription view in case changes were made in the database tab
                _prescriptionView = _prescriptionView ?? new PrescriptionView();
                MainContentFrame.Content = _prescriptionView;
            }
            else if (MedicinesTab.IsChecked == true)
            {
                _medicineView = _medicineView ?? new MedicineView();
                MainContentFrame.Content = _medicineView;
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Handle Global Hotkeys when Prescription Generator is active
            if (MainContentFrame.Content is PrescriptionView rxView)
            {
                // Ctrl + P -> Print
                if (e.Key == Key.P && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    rxView.PrintPrescription();
                    e.Handled = true;
                }
                // Ctrl + N -> New Prescription (Clear)
                else if (e.Key == Key.N && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    rxView.ClearPrescription();
                    e.Handled = true;
                }
            }
        }
    }
}