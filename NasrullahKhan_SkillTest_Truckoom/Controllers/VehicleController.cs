using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;
using System.Data;
using NasrullahKhan_SkillTest_PenaltyCalculation.Class;
using System.Collections.Generic;

namespace NasrullahKhan_SkillTest_Truckoom.Controllers
{
    public class VehicleController : Controller
    {
        GeneralClass db = new GeneralClass();
        public IActionResult Index()
        {
            ViewBag.Make = LoadVehicleMake();

            return View();
        }

        [HttpGet]
        public List<SelectListItem> LoadVehicleMake()
        {
            List<SelectListItem> lst = new List<SelectListItem>();
            lst.Add(new SelectListItem { Value = "0", Text = "--SELECT--" });

            try
            {
                SqlCommand sqlComm = new SqlCommand();
                sqlComm.CommandText = "select * from Make";
                DataTable dt = db.GetDataTable(sqlComm);

                foreach (DataRow dr in dt.Rows)
                {
                    lst.Add(new SelectListItem { Value = dr["MakeID"].ToString(), Text = dr["MakeName"].ToString().ToUpper() });
                }

                dt.Dispose();
            }
            catch (Exception ex)
            {

            }
            return lst;
        }

        [HttpGet]
        public List<SelectListItem> GetModelsByMake(int makeId)
        {
            var lst = new List<SelectListItem>();

            SqlCommand sqlcomm = new SqlCommand();
            sqlcomm.CommandText = "select ModelID,ModelName from Models where MakeId = " + makeId;

            DataTable dt = db.GetDataTable(sqlcomm);

            foreach (DataRow dr in dt.Rows)
            {
                lst.Add(new SelectListItem { Value = dr["ModelID"].ToString(), Text = dr["ModelName"].ToString().ToUpper() });
            }
            dt.Dispose();

            return lst;
        }

        [HttpPost]
        public IActionResult SaveRecord(VehicleModel vmodel, bool IsEdit)
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
                            return StatusCode(201, "Vehicle created successfully.");
                        }
                    }
                    else
                    {
                        if (InsertData(vmodel))
                        {
                            return Json(new { success = true, message = "Vehicle created successfully." });
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

        [HttpGet]
        public IActionResult LoadAllVehicles ()
        {
            try
            {
                SqlCommand sqlcomm = new SqlCommand();
                sqlcomm.CommandText = @"SELECT VehicleID,MakeName,ModelName,Year,ChasisNumber,LicensePlate,case when IsActive = 1 then 'Active' else 'Cancel' end as Status, FORMAT(CreatedDate, 'dd-MMM-yyyy') as Date
                                        FROM Vehicles A INNER JOIN MAKE B ON A.Make = B.MAKEID
                                        INNER JOIN MODELS C ON A.Model = C.MODELID AND B.MAKEID = C.MAKEID";
                DataTable dt = db.GetDataTable(sqlcomm);
                if (dt.Rows.Count == 0)
                {
                    return Json(new { success = true, data = new List<object>() });
                }

                var records = dt.AsEnumerable().Select(row => new
                {
                    VehicleID = row.Field<int>("VehicleID"),
                    MakeName = row.Field<string>("MakeName"),
                    ModelName = row.Field<string>("ModelName"),
                    Year = row.Field<int>("Year"),
                    ChasisNumber = row.Field<string>("ChasisNumber"),
                    LicensePlate = row.Field<string>("LicensePlate"),
                    Status = row.Field<string>("Status"),
                    Date = row.Field<string>("Date"),
                }).ToList();

                return Json(new { success = true, data = records });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred", error = ex.Message });
            }
        }

        [HttpPost]
        private bool InsertData(VehicleModel model)
        {
            try
            {
                SqlCommand sqlcomm = new SqlCommand();
                sqlcomm.CommandText = "INSERT INTO Vehicles(Make,Model,Year,ChasisNumber,LicensePlate,IsActive,IsCancel) " +
                                    "VALUES(@Make,@Model,@Year,@ChasisNumber,@LicensePlate,@IsActive,@IsCancel)";
                sqlcomm.Parameters.AddWithValue("@Make", model.Make);
                sqlcomm.Parameters.AddWithValue("@Model", model.Model);
                sqlcomm.Parameters.AddWithValue("@Year", model.Year);
                sqlcomm.Parameters.AddWithValue("@ChasisNumber", model.ChasisNumber);
                sqlcomm.Parameters.AddWithValue("@LicensePlate", model.LicensePlate);
                sqlcomm.Parameters.AddWithValue("@IsActive", model.IsActive);
                sqlcomm.Parameters.AddWithValue("@IsCancel", model.IsCancel);
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
        private bool UpdateData(VehicleModel model)
        {
            try
            {
                SqlCommand sqlcomm = new SqlCommand();
                sqlcomm.CommandText = @"UPDATE Vehicles SET Make = @Make, Model = @Model, Year = @Year, ChasisNumber = @ChasisNumber, LicensePlate = @LicensePlate,
                                      IsActive = @IsActive, IsCancel = @IsCancel WHERE Id = @Id";

                sqlcomm.Parameters.AddWithValue("@Make", model.Make);
                sqlcomm.Parameters.AddWithValue("@Model", model.Model);
                sqlcomm.Parameters.AddWithValue("@Year", model.Year);
                sqlcomm.Parameters.AddWithValue("@ChasisNumber", model.ChasisNumber);
                sqlcomm.Parameters.AddWithValue("@LicensePlate", model.LicensePlate);
                sqlcomm.Parameters.AddWithValue("@IsActive", model.IsActive);
                sqlcomm.Parameters.AddWithValue("@IsCancel", model.IsCancel);
                sqlcomm.Parameters.AddWithValue("@Id", model.VehicleID);

                db.ExecuteNonQuery(sqlcomm);
                sqlcomm.Parameters.Clear();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("UpdateData() " + ex.Message);
            }
        }

        private string isValidate(VehicleModel model)
        {
            if (model.Make == 0)
            {
                return "Please Select Vehicle Make";
            }
            if (model.Model == 0)
            {
                return "Please Select Vehicle Model";
            }
            if (model.Year == 0)
            {
                return "Please Select Vehicle Year";
            }
            if (model.ChasisNumber.ToString() == "")
            {
                return "Please Select Vehicle Chasis";
            }
            if (model.LicensePlate.ToString() == "")
            {
                return "Please Select Vehicle LicensePlate";
            }
            return null;
        }

        public IActionResult deleteVehicleMaintenance(int id)
        {
            SqlCommand sqlcomm = new SqlCommand();
            sqlcomm.CommandText = "DELETE FROM Vehicles WHERE VehicleID = " + id;
            db.ExecuteNonQuery(sqlcomm);
            return Json(new { success = true, message = "Vehicle deleted successfully." });
        }
    }
}
