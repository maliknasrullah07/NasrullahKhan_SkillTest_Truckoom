using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NasrullahKhan_SkillTest_PenaltyCalculation.Class;
using System.Data;
using System.Data.SqlClient;

namespace NasrullahKhan_SkillTest_Truckoom.Controllers
{
    
    public class VehicleMaintenanceController : Controller
    {
        GeneralClass db = new GeneralClass();
        public IActionResult Index()
        {
            ViewBag.Vehicle = LoadVehicle();
            return View();
        }

        [HttpGet]
        public List<SelectListItem> LoadVehicle()
        {
            List<SelectListItem> lst = new List<SelectListItem>();
            lst.Add(new SelectListItem { Value = "0", Text = "--SELECT--" });

            try
            {
                SqlCommand sqlComm = new SqlCommand();
                sqlComm.CommandText = "SELECT VehicleID, CONCAT(MakeName, ' | ', ModelName, ' | ', Year, ' | ', LicensePlate) AS VehicleName \r\nFROM Vehicles A INNER JOIN MAKE B ON A.Make = B.MAKEID\r\nINNER JOIN MODELS C ON A.Model = C.MODELID AND B.MAKEID = C.MAKEID";
                DataTable dt = db.GetDataTable(sqlComm);

                foreach (DataRow dr in dt.Rows)
                {
                    lst.Add(new SelectListItem { Value = dr["VehicleID"].ToString(), Text = dr["VehicleName"].ToString().ToUpper() });
                }

                dt.Dispose();
            }
            catch (Exception ex)
            {

            }
            return lst;
        }

        [HttpGet]
        public IActionResult GetMaintenanceRecord()
        {
            try
            {
                SqlCommand sqlcomm = new SqlCommand();
                sqlcomm.CommandText = "SELECT MaintenanceID,MakeName,ModelName,Year,LicensePlate,MaintenanceType,FORMAT(MaintenanceDate,'dd-MMM-yyyy') as MaintenanceDate,\r\nDescription,Notes FROM MaintenanceActivities A INNER JOIN Vehicles B ON A.VehicleID = B.VehicleID\r\nINNER JOIN MAKE C ON B.Make = C.MAKEID\r\nINNER JOIN MODELS D ON B.Model = D.MODELID AND C.MAKEID = D.MAKEID";
                DataTable dt = db.GetDataTable(sqlcomm);
                if (dt.Rows.Count == 0)
                {
                    return Json(new { success = true, data = new List<object>() });
                }

                var records = dt.AsEnumerable().Select(row => new
                {
                    MaintenanceID = row.Field<int>("MaintenanceID"),
                    Make = row.Field<string>("MakeName"),
                    Model = row.Field<string>("ModelName"),
                    Year = row.Field<int>("Year"),
                    LicensePlate = row.Field<string>("LicensePlate"),
                    MaintenanceType = row.Field<string>("MaintenanceType"),
                    MaintenanceDate = row.Field<string>("MaintenanceDate"),
                    Description = row.Field<string>("Description"),
                    Notes = row.Field<string>("Notes"),
                }).ToList();

                return Json(new { success = true, data = records });
            }
            catch (Exception ex) {
                return StatusCode(500, new { success = false, message = "An error occurred", error = ex.Message });
            }

        }


        [HttpPost]
        public IActionResult SaveVehicleMaintenance(VehicleMaintenanceModel vmodel, bool IsEdit)
        {
            try
            {
                string validateData = isValidate(vmodel);
                if (validateData == null)
                {
                    if (IsEdit)
                    {
                        if (UpdateData(vmodel))
                        {
                            return Json(new { success = true, message = "Vehicle Maintenance created successfully." });
                        }
                    }
                    else
                    {
                        if (InsertData(vmodel))
                        {
                            return Json(new { success = true, message = "Vehicle Maintenance created successfully." });
                        }
                    }
                }
                else
                {
                    return StatusCode(500, validateData.ToString());
                }
                return StatusCode(500, "An error occurred while creating the vehicle.");
            }
            catch (Exception ex)
            {
                throw new Exception("Create() " + ex.Message);

            }
        }

        [HttpPost]
        private bool InsertData(VehicleMaintenanceModel model)
        {
            try
            {
                SqlCommand sqlcomm = new SqlCommand();
                sqlcomm.CommandText = @"INSERT INTO MaintenanceActivities(VehicleID, MaintenanceType, MaintenanceCost, MaintenanceDate, Description, Notes, IsCompleted)
                                        VALUES(@VehicleID, @MaintenanceType, @MaintenanceCost, @MaintenanceDate, @Description, @Notes, @IsCompleted)";
                sqlcomm.Parameters.AddWithValue("@VehicleID", model.VehicleID);
                sqlcomm.Parameters.AddWithValue("@MaintenanceType", model.MaintenanceType);
                sqlcomm.Parameters.AddWithValue("@MaintenanceCost", model.MaintenanceCost);
                sqlcomm.Parameters.AddWithValue("@MaintenanceDate", model.MaintenanceDate);
                sqlcomm.Parameters.AddWithValue("@Description", model.Description);
                sqlcomm.Parameters.AddWithValue("@Notes", model.Notes);
                sqlcomm.Parameters.AddWithValue("@IsCompleted", model.IsCompleted);
                db.ExecuteNonQuery(sqlcomm);
                sqlcomm.Parameters.Clear();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Create() " + ex.Message);
            }
        }

        [HttpPut]
        private bool UpdateData(VehicleMaintenanceModel model)
        {
            try
            {
                SqlCommand sqlcomm = new SqlCommand();
                sqlcomm.CommandText = @"UPDATE MaintenanceActivities SET VehicleID = @VehicleID,MaintenanceType = @MaintenanceType,
                                    MaintenanceCost = @MaintenanceCost,MaintenanceDate = @MaintenanceDate,Description = @Description,Notes = @Notes,IsCompleted = @IsCompleted
                                WHERE MaintenanceID = @MaintenanceID";

                sqlcomm.Parameters.AddWithValue("@VehicleID", model.VehicleID);
                sqlcomm.Parameters.AddWithValue("@MaintenanceType", model.MaintenanceType);
                sqlcomm.Parameters.AddWithValue("@MaintenanceCost", model.MaintenanceCost);
                sqlcomm.Parameters.AddWithValue("@MaintenanceDate", model.MaintenanceDate);
                sqlcomm.Parameters.AddWithValue("@Description", model.Description);
                sqlcomm.Parameters.AddWithValue("@Notes", model.Notes);
                sqlcomm.Parameters.AddWithValue("@IsCompleted", model.IsCompleted);
                sqlcomm.Parameters.AddWithValue("@MaintenanceID", model.MaintenanceID);

                db.ExecuteNonQuery(sqlcomm);
                sqlcomm.Parameters.Clear();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("UpdateData() " + ex.Message);
            }
        }

        private string isValidate(VehicleMaintenanceModel model)
        {
            if (model.VehicleID == 0)
            {
                return "Please Select Vehicle";
            }
            if (model.MaintenanceType == "")
            {
                return "Please Type Maintenance Activity";
            }
            if (model.MaintenanceCost <= 0)
            {
                return "Please Enter Maintenance Cost";
            }
            if (model.Description == "")
            {
                return "Please Type Maintenance Description";
            }
            return null;
        }

        [HttpPost]
        public IActionResult deleteVehicleMaintenance(int MaintenanceID)
        {
            SqlCommand sqlcomm = new SqlCommand();
            sqlcomm.CommandText = "DELETE FROM MaintenanceActivities where MaintenanceID = " + MaintenanceID;
            db.ExecuteNonQuery(sqlcomm);
            return Json(new { success = true, message = "Vehicle Maintenance deleted successfully." });
        }

        public IActionResult GetMaintenanceRecordById(int id)
        {
            SqlCommand sqlcomm = new SqlCommand();
            sqlcomm.CommandText = "select VehicleID,MaintenanceType,MaintenanceDate,IsCompleted,MaintenanceCost,Description,Notes from MaintenanceActivities where MaintenanceID = " + id;
            DataTable dt = db.GetDataTable(sqlcomm);

            if (dt.Rows.Count == 0)
            {
                return Json(new { success = true, data = new List<object>() });
            }

            var records = dt.AsEnumerable().Select(row => new
            {
                VehicleID = row.Field<int>("VehicleID"),
                MaintenanceType = row.Field<string>("MaintenanceType"),
                IsCompleted = row.Field<bool>("IsCompleted"),
                MaintenanceCost = row.Field<decimal>("MaintenanceCost"),
                MaintenanceDate = row.Field<DateTime>("MaintenanceDate"),
                Description = row.Field<string>("Description"),
                Notes = row.Field<string>("Notes"),
            }).ToList();

            return Json(new { success = true, data = records });
        }
    }
}
