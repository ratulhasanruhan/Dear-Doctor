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
                new() { Name = "Tab. Paracetamol 500mg", GenericName = "Paracetamol", DefaultDose = "1+0+1", DefaultDuration = "3 days", DefaultInstructions = "After meal", Category = "General Physician" },
                new() { Name = "Cap. Amoxicillin 500mg", GenericName = "Amoxicillin", DefaultDose = "1+1+1", DefaultDuration = "7 days", DefaultInstructions = "After meal", Category = "General Physician" },
                new() { Name = "Tab. Omeprazole 20mg", GenericName = "Omeprazole", DefaultDose = "1+0+0", DefaultDuration = "14 days", DefaultInstructions = "30 mins before meal", Category = "General Physician" },
                new() { Name = "Tab. Metformin 500mg", GenericName = "Metformin", DefaultDose = "0+1+0", DefaultDuration = "Continue", DefaultInstructions = "With meal", Category = "General Physician" },
                new() { Name = "Tab. Atorvastatin 10mg", GenericName = "Atorvastatin", DefaultDose = "0+0+1", DefaultDuration = "Continue", DefaultInstructions = "Before sleep", Category = "General Physician" },
                new() { Name = "Syp. Antacid 100ml", GenericName = "Antacid", DefaultDose = "2 tsp", DefaultDuration = "7 days", DefaultInstructions = "After meal when needed", Category = "General Physician" },
                new() { Name = "Tab. Cetirizine 10mg", GenericName = "Cetirizine", DefaultDose = "0+0+1", DefaultDuration = "5 days", DefaultInstructions = "After meal", Category = "General Physician" },
                
                // Skin & Hair defaults
                new() { Name = "Lotion Minoxidil 5%", GenericName = "Minoxidil", DefaultDose = "1ml", DefaultDuration = "Continue", DefaultInstructions = "Apply to scalp twice daily", Category = "Skin & Hair" },
                new() { Name = "Tab. Finasteride 1mg", GenericName = "Finasteride", DefaultDose = "1+0+0", DefaultDuration = "Continue", DefaultInstructions = "At the same time daily", Category = "Skin & Hair" },
                new() { Name = "Cream Ketoconazole 2%", GenericName = "Ketoconazole", DefaultDose = "Apply once", DefaultDuration = "14 days", DefaultInstructions = "Apply to affected skin", Category = "Skin & Hair" },
                new() { Name = "Cap. Itraconazole 100mg", GenericName = "Itraconazole", DefaultDose = "0+0+1", DefaultDuration = "7 days", DefaultInstructions = "Immediately after main meal", Category = "Skin & Hair" },
                new() { Name = "Cream Benzoyl Peroxide 5%", GenericName = "Benzoyl Peroxide", DefaultDose = "Apply once", DefaultDuration = "30 days", DefaultInstructions = "Apply at night on acne spots", Category = "Skin & Hair" }
            };
        }
    }
}
