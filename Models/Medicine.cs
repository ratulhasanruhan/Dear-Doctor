using System;

namespace Dear_Doctor.Models
{
    public class Medicine
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string GenericName { get; set; } = string.Empty;
        public string DefaultDose { get; set; } = string.Empty;       // e.g., "1+0+1"
        public string DefaultDuration { get; set; } = string.Empty;   // e.g., "7 days"
        public string DefaultInstructions { get; set; } = string.Empty; // e.g., "After meal"
    }
}
