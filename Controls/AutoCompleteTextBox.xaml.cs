using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Dear_Doctor.Models;

namespace Dear_Doctor.Controls
{
    public partial class AutoCompleteTextBox : UserControl
    {
        private bool _isUpdatingText = false;

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(AutoCompleteTextBox),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTextChangedCallback));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty SuggestionsProperty =
            DependencyProperty.Register(nameof(Suggestions), typeof(IEnumerable<Medicine>), typeof(AutoCompleteTextBox),
                new PropertyMetadata(null));

        public IEnumerable<Medicine> Suggestions
        {
            get => (IEnumerable<Medicine>)GetValue(SuggestionsProperty);
            set => SetValue(SuggestionsProperty, value);
        }

        public event EventHandler<MedicineSelectedEventArgs>? MedicineSelected;

        public AutoCompleteTextBox()
        {
            InitializeComponent();
        }

        private static void OnTextChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (AutoCompleteTextBox)d;
            if (!control._isUpdatingText)
            {
                control.InputTextBox.Text = (string)e.NewValue ?? string.Empty;
            }
        }

        private void InputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isUpdatingText) return;

            string query = InputTextBox.Text;
            _isUpdatingText = true;
            Text = query;
            _isUpdatingText = false;

            if (Suggestions == null || string.IsNullOrWhiteSpace(query))
            {
                SuggestionsPopup.IsOpen = false;
                return;
            }

            var filtered = Suggestions
                .Where(m => (m.Name != null && m.Name.Contains(query, StringComparison.OrdinalIgnoreCase)) || 
                            (m.GenericName != null && m.GenericName.Contains(query, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            if (filtered.Any())
            {
                SuggestionsListBox.ItemsSource = filtered;
                SuggestionsPopup.IsOpen = true;
            }
            else
            {
                SuggestionsPopup.IsOpen = false;
            }
        }

        private void InputTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down && SuggestionsPopup.IsOpen)
            {
                SuggestionsListBox.Focus();
                if (SuggestionsListBox.Items.Count > 0)
                {
                    SuggestionsListBox.SelectedIndex = 0;
                    var item = SuggestionsListBox.ItemContainerGenerator.ContainerFromIndex(0) as ListBoxItem;
                    item?.Focus();
                }
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                SuggestionsPopup.IsOpen = false;
                e.Handled = true;
            }
        }

        private void SuggestionsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SuggestionsPopup.IsOpen && SuggestionsListBox.SelectedItem is Medicine selected)
            {
                SelectMedicine(selected);
            }
        }

        private void SelectMedicine(Medicine medicine)
        {
            _isUpdatingText = true;
            InputTextBox.Text = medicine.Name;
            Text = medicine.Name;
            _isUpdatingText = false;
            SuggestionsPopup.IsOpen = false;

            MedicineSelected?.Invoke(this, new MedicineSelectedEventArgs(medicine));
            
            InputTextBox.Focus();
            InputTextBox.CaretIndex = InputTextBox.Text.Length;
        }

        private void SuggestionsListBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Space)
            {
                if (SuggestionsListBox.SelectedItem is Medicine selected)
                {
                    SelectMedicine(selected);
                }
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                SuggestionsPopup.IsOpen = false;
                InputTextBox.Focus();
                e.Handled = true;
            }
        }

        private void InputTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!InputTextBox.IsFocused && !SuggestionsListBox.IsFocused)
                {
                    SuggestionsPopup.IsOpen = false;
                }
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        public void FocusInput()
        {
            InputTextBox.Focus();
            InputTextBox.CaretIndex = InputTextBox.Text.Length;
        }
    }

    public class MedicineSelectedEventArgs : EventArgs
    {
        public Medicine Medicine { get; }
        public MedicineSelectedEventArgs(Medicine medicine)
        {
            Medicine = medicine;
        }
    }
}
