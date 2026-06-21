using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Dear_Doctor.Controls;
using Dear_Doctor.Models;
using Dear_Doctor.Services;

namespace Dear_Doctor.Views
{
    public partial class PrescriptionView : UserControl
    {
        private readonly StorageService _storageService;
        public Prescription ActivePrescription { get; set; }

        public static readonly DependencyProperty AllMedicinesProperty =
            DependencyProperty.Register(nameof(AllMedicines), typeof(List<Medicine>), typeof(PrescriptionView),
                new PropertyMetadata(null));

        public List<Medicine> AllMedicines
        {
            get => (List<Medicine>)GetValue(AllMedicinesProperty);
            set => SetValue(AllMedicinesProperty, value);
        }

        public PrescriptionView()
        {
            InitializeComponent();
            _storageService = new StorageService();
            ActivePrescription = new Prescription();
            
            // Start with one empty medicine row
            var initialItem = new PrescribedItem();
            ActivePrescription.Medicines.Add(initialItem);
            
            DataContext = ActivePrescription;
            LoadSuggestions();

            ActivePrescription.Medicines.CollectionChanged += Medicines_CollectionChanged;
            foreach (var item in ActivePrescription.Medicines)
            {
                item.PropertyChanged += Item_PropertyChanged;
            }

            // Refresh suggestions every time view is loaded/restored
            this.Loaded += (s, e) => LoadSuggestions();
        }

        private void Medicines_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (PrescribedItem item in e.OldItems)
                {
                    item.PropertyChanged -= Item_PropertyChanged;
                }
            }
            if (e.NewItems != null)
            {
                foreach (PrescribedItem item in e.NewItems)
                {
                    item.PropertyChanged += Item_PropertyChanged;
                }
            }
        }

        private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PrescribedItem.MedicineName))
            {
                if (sender is PrescribedItem item)
                {
                    string name = item.MedicineName?.Trim() ?? string.Empty;
                    if (!string.IsNullOrEmpty(name) && AllMedicines != null)
                    {
                        var matchedMed = AllMedicines.FirstOrDefault(m => string.Equals(m.Name, name, StringComparison.OrdinalIgnoreCase));
                        if (matchedMed != null)
                        {
                            item.Category = string.IsNullOrEmpty(matchedMed.Category) ? "General Physician" : matchedMed.Category;
                        }
                    }
                }
            }
        }

        private void LoadSuggestions()
        {
            AllMedicines = _storageService.LoadMedicines();
        }

        private void MedicineInput_MedicineSelected(object sender, MedicineSelectedEventArgs e)
        {
            if (sender is AutoCompleteTextBox autoTextBox)
            {
                var item = autoTextBox.DataContext as PrescribedItem;
                if (item != null)
                {
                    // Populate default suggestions from the selected medicine
                    item.GenericName = e.Medicine.GenericName;
                    item.Dose = e.Medicine.DefaultDose;
                    item.Duration = e.Medicine.DefaultDuration;
                    item.Instructions = e.Medicine.DefaultInstructions;
                    item.Category = string.IsNullOrEmpty(e.Medicine.Category) ? "General Physician" : e.Medicine.Category;

                    // Focus the DoseInput TextBox of the same row
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        var container = MedicinesItemsControl.ItemContainerGenerator.ContainerFromItem(item) as ContentPresenter;
                        if (container != null)
                        {
                            container.ApplyTemplate();
                            var doseInput = FindVisualChild<TextBox>(container, "DoseInput");
                            if (doseInput != null)
                            {
                                doseInput.Focus();
                                doseInput.SelectAll();
                            }
                        }
                    }), System.Windows.Threading.DispatcherPriority.Input);
                }
            }
        }

        private void DoseInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                if (sender is TextBox textBox)
                {
                    var item = textBox.DataContext as PrescribedItem;
                    if (item != null)
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            var container = MedicinesItemsControl.ItemContainerGenerator.ContainerFromItem(item) as ContentPresenter;
                            if (container != null)
                            {
                                var durationInput = FindVisualChild<TextBox>(container, "DurationInput");
                                if (durationInput != null)
                                {
                                    durationInput.Focus();
                                    durationInput.SelectAll();
                                }
                            }
                        }), System.Windows.Threading.DispatcherPriority.Input);
                        e.Handled = true;
                    }
                }
            }
        }

        private void DurationInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                if (sender is TextBox textBox)
                {
                    var item = textBox.DataContext as PrescribedItem;
                    if (item != null)
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            var container = MedicinesItemsControl.ItemContainerGenerator.ContainerFromItem(item) as ContentPresenter;
                            if (container != null)
                            {
                                var instructionsInput = FindVisualChild<TextBox>(container, "InstructionsInput");
                                if (instructionsInput != null)
                                {
                                    instructionsInput.Focus();
                                    instructionsInput.SelectAll();
                                }
                            }
                        }), System.Windows.Threading.DispatcherPriority.Input);
                        e.Handled = true;
                    }
                }
            }
        }

        private void InstructionsInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                if (sender is TextBox textBox)
                {
                    var item = textBox.DataContext as PrescribedItem;
                    if (item != null)
                    {
                        int currentIndex = ActivePrescription.Medicines.IndexOf(item);
                        if (currentIndex == ActivePrescription.Medicines.Count - 1)
                        {
                            // If this is the last row, automatically create a new row
                            var newRow = new PrescribedItem();
                            ActivePrescription.Medicines.Add(newRow);

                            // Focus the AutoCompleteTextBox of the new row after it's rendered
                            Dispatcher.BeginInvoke(new Action(() =>
                            {
                                var container = MedicinesItemsControl.ItemContainerGenerator.ContainerFromItem(newRow) as ContentPresenter;
                                if (container != null)
                                {
                                    container.ApplyTemplate();
                                    var autoComplete = FindVisualChild<AutoCompleteTextBox>(container, "MedicineInput");
                                    autoComplete?.FocusInput();
                                }
                            }), System.Windows.Threading.DispatcherPriority.Input);
                        }
                        else
                        {
                            // Move focus to next row's MedicineInput
                            var nextRow = ActivePrescription.Medicines[currentIndex + 1];
                            Dispatcher.BeginInvoke(new Action(() =>
                            {
                                var container = MedicinesItemsControl.ItemContainerGenerator.ContainerFromItem(nextRow) as ContentPresenter;
                                if (container != null)
                                {
                                    var autoComplete = FindVisualChild<AutoCompleteTextBox>(container, "MedicineInput");
                                    autoComplete?.FocusInput();
                                }
                            }), System.Windows.Threading.DispatcherPriority.Input);
                        }
                        e.Handled = true;
                    }
                }
            }
        }

        private void AddRowButton_Click(object sender, RoutedEventArgs e)
        {
            var newRow = new PrescribedItem();
            ActivePrescription.Medicines.Add(newRow);

            Dispatcher.BeginInvoke(new Action(() =>
            {
                var container = MedicinesItemsControl.ItemContainerGenerator.ContainerFromItem(newRow) as ContentPresenter;
                if (container != null)
                {
                    container.ApplyTemplate();
                    var autoComplete = FindVisualChild<AutoCompleteTextBox>(container, "MedicineInput");
                    autoComplete?.FocusInput();
                }
            }), System.Windows.Threading.DispatcherPriority.Input);
        }

        private void DeleteRowButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is PrescribedItem item)
            {
                ActivePrescription.Medicines.Remove(item);
                if (ActivePrescription.Medicines.Count == 0)
                {
                    ActivePrescription.Medicines.Add(new PrescribedItem());
                }
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ClearPrescription();
        }

        public void ClearPrescription()
        {
            ActivePrescription.PatientName = string.Empty;
            ActivePrescription.Age = string.Empty;
            ActivePrescription.Gender = "Male";
            ActivePrescription.Date = DateTime.Today;
            ActivePrescription.BP = string.Empty;
            ActivePrescription.PR = string.Empty;
            ActivePrescription.ChiefComplaints = string.Empty;
            ActivePrescription.Diagnosis = string.Empty;
            ActivePrescription.Advice = string.Empty;
            ActivePrescription.Complication = string.Empty;
            
            ActivePrescription.Medicines.Clear();
            ActivePrescription.Medicines.Add(new PrescribedItem());
            
            // Set focus back to PatientName input
            PatientNameInput.Focus();
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            PrintPrescription();
        }

        public void PrintPrescription()
        {
            // Clean up any empty rows first so they aren't printed
            var emptyRows = ActivePrescription.Medicines.Where(m => string.IsNullOrWhiteSpace(m.MedicineName)).ToList();
            foreach (var row in emptyRows)
            {
                ActivePrescription.Medicines.Remove(row);
            }

            if (ActivePrescription.Medicines.Count == 0)
            {
                MessageBox.Show("Please add at least one medicine to print.", "No Medicines", MessageBoxButton.OK, MessageBoxImage.Warning);
                ActivePrescription.Medicines.Add(new PrescribedItem());
                return;
            }

            PrintService.Print(ActivePrescription);

            // Re-add an empty row if all were cleared
            if (ActivePrescription.Medicines.Count == 0)
            {
                ActivePrescription.Medicines.Add(new PrescribedItem());
            }
        }

        private T? FindVisualChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T tChild && (string.IsNullOrEmpty(childName) || (child is FrameworkElement fe && fe.Name == childName)))
                {
                    return tChild;
                }
                var childOfChild = FindVisualChild<T>(child, childName);
                if (childOfChild != null)
                {
                    return childOfChild;
                }
            }
            return null;
        }
    }

    // --- CONVERTERS ---
    public class IndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int index)
            {
                return (index + 1).ToString(culture) + ".";
            }
            return "";
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && !string.IsNullOrWhiteSpace(str))
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CategoryFilterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<PrescribedItem> items && parameter is string category)
            {
                return items.Where(item => item.Category == category && !string.IsNullOrWhiteSpace(item.MedicineName)).ToList();
            }
            return new List<PrescribedItem>();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CategoryVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<PrescribedItem> items && parameter is string category)
            {
                bool hasAny = items.Any(item => item.Category == category && !string.IsNullOrWhiteSpace(item.MedicineName));
                return hasAny ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EvenOddBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int index)
            {
                if (index % 2 != 0)
                {
                    return new SolidColorBrush(Color.FromRgb(245, 245, 245));
                }
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
