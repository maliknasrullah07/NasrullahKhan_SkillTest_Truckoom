namespace NasrullahKhan_SkillTest_Truckoom
{
    public class VehicleMaintenanceModel
    {
            public Nullable<int> MaintenanceID { get; set; } 
            public int VehicleID { get; set; } 
            public string MaintenanceType { get; set; }
            public double MaintenanceCost { get; set; }
            public DateTime MaintenanceDate { get; set; }
            public string Description { get; set; } 
            public string Notes { get; set; }
            public bool IsCompleted { get; set; }
            public DateTime CreatedDate { get; set; } = DateTime.Now;
        
    }
}
