using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Dear_Doctor.Models;
using Dear_Doctor.Services;

namespace Dear_Doctor.Views
{
    public partial class MedicineView : UserControl
    {
        private readonly StorageService _storageService;
        private List<Medicine> _allMedicines = new();
        private ObservableCollection<Medicine> _displayedMedicines = new();
        private Medicine? _editingMedicine = null;
        private bool _isNew = false;

        public MedicineView()
        {
            InitializeComponent();
            _storageService = new StorageService();
            LoadMedicines();
        }

        private void LoadMedicines()
        {
            _allMedicines = _storageService.LoadMedicines();
            UpdateListView();
        }

        private void UpdateListView()
        {
            string query = SearchBox.Text.Trim();
            List<Medicine> filtered;
            
            if (string.IsNullOrEmpty(query))
            {
                filtered = _allMedicines.OrderBy(m => m.Name).ToList();
            }
            else
            {
                filtered = _allMedicines
                    .Where(m => m.Name.Contains(query, StringComparison.OrdinalIgnoreCase) || 
                                m.GenericName.Contains(query, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(m => m.Name)
                    .ToList();
            }

            _displayedMedicines = new ObservableCollection<Medicine>(filtered);
            MedicinesListView.ItemsSource = _displayedMedicines;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchPlaceholder.Visibility = string.IsNullOrEmpty(SearchBox.Text) ? Visibility.Visible : Visibility.Collapsed;
            UpdateListView();
        }

        private void AddMedicineButton_Click(object sender, RoutedEventArgs e)
        {
            _isNew = true;
            _editingMedicine = new Medicine();
            
            DrawerTitle.Text = "Add Medicine";
            MedicineNameInput.Text = string.Empty;
            GenericNameInput.Text = string.Empty;
            DefaultDoseInput.Text = string.Empty;
            DefaultDurationInput.Text = string.Empty;
            DefaultInstructionsInput.Text = string.Empty;

            ShowDrawer(true);
            MedicineNameInput.Focus();
        }

        private void EditMedicine_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Medicine medicine)
            {
                _isNew = false;
                _editingMedicine = medicine;

                DrawerTitle.Text = "Edit Medicine";
                MedicineNameInput.Text = medicine.Name;
                GenericNameInput.Text = medicine.GenericName;
                DefaultDoseInput.Text = medicine.DefaultDose;
                DefaultDurationInput.Text = medicine.DefaultDuration;
                DefaultInstructionsInput.Text = medicine.DefaultInstructions;

                ShowDrawer(true);
                MedicineNameInput.Focus();
            }
        }

        private void DeleteMedicine_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Medicine medicine)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete '{medicine.Name}'?", 
                    "Confirm Delete", 
                    MessageBoxButton.YesNo, 
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    _allMedicines.Remove(medicine);
                    _storageService.SaveMedicines(_allMedicines);
                    
                    if (_editingMedicine?.Id == medicine.Id)
                    {
                        ShowDrawer(false);
                    }
                    
                    UpdateListView();
                }
            }
        }

        private void SaveMedicineButton_Click(object sender, RoutedEventArgs e)
        {
            string name = MedicineNameInput.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Medicine name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                MedicineNameInput.Focus();
                return;
            }

            if (_editingMedicine == null) return;

            _editingMedicine.Name = name;
            _editingMedicine.GenericName = GenericNameInput.Text.Trim();
            _editingMedicine.DefaultDose = DefaultDoseInput.Text.Trim();
            _editingMedicine.DefaultDuration = DefaultDurationInput.Text.Trim();
            _editingMedicine.DefaultInstructions = DefaultInstructionsInput.Text.Trim();

            if (_isNew)
            {
                _allMedicines.Add(_editingMedicine);
            }

            _storageService.SaveMedicines(_allMedicines);
            ShowDrawer(false);
            UpdateListView();
        }

        private void CancelEditButton_Click(object sender, RoutedEventArgs e)
        {
            ShowDrawer(false);
        }

        private void ShowDrawer(bool show)
        {
            if (show)
            {
                EditColumn.Width = new GridLength(320);
            }
            else
            {
                EditColumn.Width = new GridLength(0);
                _editingMedicine = null;
            }
        }
    }
}
