using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Dear_Doctor.Models
{
    public class PrescribedItem : INotifyPropertyChanged
    {
        private string _medicineName = string.Empty;
        private string _genericName = string.Empty;
        private string _dose = string.Empty;
        private string _duration = string.Empty;
        private string _instructions = string.Empty;

        public string MedicineName
        {
            get => _medicineName;
            set { _medicineName = value; OnPropertyChanged(); }
        }

        public string GenericName
        {
            get => _genericName;
            set { _genericName = value; OnPropertyChanged(); }
        }

        public string Dose
        {
            get => _dose;
            set { _dose = value; OnPropertyChanged(); }
        }

        public string Duration
        {
            get => _duration;
            set { _duration = value; OnPropertyChanged(); }
        }

        public string Instructions
        {
            get => _instructions;
            set { _instructions = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Prescription : INotifyPropertyChanged
    {
        private string _patientName = string.Empty;
        private string _age = string.Empty;
        private string _gender = "Male";
        private DateTime _date = DateTime.Today;
        private string _bp = string.Empty;
        private string _pr = string.Empty;
        private string _chiefComplaints = string.Empty;
        private string _diagnosis = string.Empty;
        private string _advice = string.Empty;
        private string _complication = string.Empty;
        private ObservableCollection<PrescribedItem> _medicines = new();

        public string PatientName
        {
            get => _patientName;
            set { _patientName = value; OnPropertyChanged(); }
        }

        public string Age
        {
            get => _age;
            set { _age = value; OnPropertyChanged(); }
        }

        public string Gender
        {
            get => _gender;
            set { _gender = value; OnPropertyChanged(); }
        }

        public DateTime Date
        {
            get => _date;
            set { _date = value; OnPropertyChanged(); }
        }

        public string BP
        {
            get => _bp;
            set { _bp = value; OnPropertyChanged(); }
        }

        public string PR
        {
            get => _pr;
            set { _pr = value; OnPropertyChanged(); }
        }

        public string ChiefComplaints
        {
            get => _chiefComplaints;
            set { _chiefComplaints = value; OnPropertyChanged(); }
        }

        public string Diagnosis
        {
            get => _diagnosis;
            set { _diagnosis = value; OnPropertyChanged(); }
        }

        public string Advice
        {
            get => _advice;
            set { _advice = value; OnPropertyChanged(); }
        }

        public string Complication
        {
            get => _complication;
            set { _complication = value; OnPropertyChanged(); }
        }

        public ObservableCollection<PrescribedItem> Medicines
        {
            get => _medicines;
            set { _medicines = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
