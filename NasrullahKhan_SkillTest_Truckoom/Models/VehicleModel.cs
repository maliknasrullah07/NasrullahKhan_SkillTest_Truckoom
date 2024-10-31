namespace NasrullahKhan_SkillTest_Truckoom
{
    public class VehicleModel
    {
        public Nullable<int> VehicleID { get; set; }
        public int Make { get; set; }
        public int Model { get; set; }
        public int Year { get; set; }
        public string? ChasisNumber { get; set; }
        public string? LicensePlate { get; set; }
        public bool IsActive { get; set; }
        public bool IsCancel { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
