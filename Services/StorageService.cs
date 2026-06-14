using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Dear_Doctor.Models;

namespace Dear_Doctor.Services
{
    public class StorageService
    {
        private static readonly string AppDataFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
            "DearDoctor"
        );
        private static readonly string MedicinesPath = Path.Combine(AppDataFolder, "medicines.json");

        public StorageService()
        {
            if (!Directory.Exists(AppDataFolder))
            {
                Directory.CreateDirectory(AppDataFolder);
            }
        }

        public List<Medicine> LoadMedicines()
        {
            if (!File.Exists(MedicinesPath))
            {
                var demo = GetDefaultMedicines();
                SaveMedicines(demo);
                return demo;
            }
            try
            {
                var json = File.ReadAllText(MedicinesPath);
                return JsonSerializer.Deserialize<List<Medicine>>(json) ?? GetDefaultMedicines();
            }
            catch (Exception)
            {
                return GetDefaultMedicines();
            }
        }

        public void SaveMedicines(List<Medicine> medicines)
        {
            try
            {
                var json = JsonSerializer.Serialize(medicines, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(MedicinesPath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to save medicines: {ex.Message}");
            }
        }

        private List<Medicine> GetDefaultMedicines()
        {
            return new List<Medicine>
            {
                new() { Name = "Tab. Paracetamol 500mg", GenericName = "Paracetamol", DefaultDose = "1+0+1", DefaultDuration = "3 days", DefaultInstructions = "After meal" },
                new() { Name = "Cap. Amoxicillin 500mg", GenericName = "Amoxicillin", DefaultDose = "1+1+1", DefaultDuration = "7 days", DefaultInstructions = "After meal" },
                new() { Name = "Tab. Omeprazole 20mg", GenericName = "Omeprazole", DefaultDose = "1+0+0", DefaultDuration = "14 days", DefaultInstructions = "30 mins before meal" },
                new() { Name = "Tab. Metformin 500mg", GenericName = "Metformin", DefaultDose = "0+1+0", DefaultDuration = "Continue", DefaultInstructions = "With meal" },
                new() { Name = "Tab. Atorvastatin 10mg", GenericName = "Atorvastatin", DefaultDose = "0+0+1", DefaultDuration = "Continue", DefaultInstructions = "Before sleep" },
                new() { Name = "Syp. Antacid 100ml", GenericName = "Antacid", DefaultDose = "2 tsp", DefaultDuration = "7 days", DefaultInstructions = "After meal when needed" },
                new() { Name = "Tab. Cetirizine 10mg", GenericName = "Cetirizine", DefaultDose = "0+0+1", DefaultDuration = "5 days", DefaultInstructions = "After meal" }
            };
        }
    }
}
