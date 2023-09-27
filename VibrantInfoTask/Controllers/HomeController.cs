using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VibrantInfoTask.Data;
using VibrantInfoTask.Models;
using Microsoft.AspNetCore.Hosting;

namespace VibrantInfoTask.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        DbContext _dbContext = new();
        [Obsolete]
        private IHostingEnvironment Environment;

        [Obsolete]
        public HomeController(ILogger<HomeController> logger, IHostingEnvironment _environment)
        {
            Environment = _environment;
            _logger = logger;
        }
        public IActionResult Index()
        {
            ViewData["ProfilePhoto"] = HttpContext.Session.GetString("ProfilePhoto");
            ViewData["UserName"] = HttpContext.Session.GetString("UserName");
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> GetAddressList()
        {
            var response = Json(new
            {
                draw = 0,
                recordsTotal = 0,
                recordsFiltered = 0,
                data = string.Empty
            });

            try
            {
                #region Initialize Input parameter of SP
                var draw = Convert.ToInt32(Request.Form["draw"]);

                var request = new UserList();
                request.SortCol = Convert.ToInt32(Request.Form["order[0][column]"]);
                request.SortDir = Request.Form["order[0][dir]"];
                request.PageIndex = Convert.ToInt32(Request.Form["start"]);
                if (request.PageIndex == 0)
                    request.PageIndex = 1;

                request.PageSize = Convert.ToInt32(Request.Form["length"]);

                string Search = null;
                if (!string.IsNullOrEmpty(Request.Form["search"]))
                    Search = Convert.ToString(Request.Form["search"]).Trim();

                if (!string.IsNullOrEmpty(Search))
                    request.Keyword = Search;

                #endregion

                var result = _dbContext.GetList(request);
                var data = result.Item1;

                var userList = new List<UserList>();
                if (data.Rows.Count > 0)
                {
                    for (int i = 0; i < data.Rows.Count; i++)
                    {
                        var rowdata = new UserList();
                        rowdata.Id = Convert.ToInt32(data.Rows[i]["Id"]);
                        rowdata.FirstName = Convert.ToString(data.Rows[i]["FirstName"]);
                        rowdata.LastName = Convert.ToString(data.Rows[i]["LastName"]);
                        rowdata.Email = Convert.ToString(data.Rows[i]["Email"]);
                        rowdata.PhoneNumber = Convert.ToString(data.Rows[i]["PhoneNumber"]);
                        rowdata.Address = Convert.ToString(data.Rows[i]["Address"]);
                        rowdata.IsBlock = Convert.ToBoolean(data.Rows[i]["IsBlock"]);
                        rowdata.IsActive = Convert.ToBoolean(data.Rows[i]["IsActive"]);
                        userList.Add(rowdata);
                    }
                }

                #region Return JSON
                response = Json(new
                {
                    draw = draw,
                    recordsTotal = result.Item2,
                    recordsFiltered = data.Rows.Count,
                    data = userList
                });

                #endregion
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Internal server error";
            }
            return response;
        }

        #region Add/Update
        public IActionResult AddNew(int Id)
        {
            var objUser = new User();
            if (Id > 0)
            {
                string _query = "select * from Addressbook where Id='" + Id + "'";
                var data = _dbContext.Getdata(_query);

                if (data != null && data.Rows.Count > 0)
                {
                    objUser.Id = Convert.ToInt32(Id);
                    objUser.FirstName = Convert.ToString(data.Rows[0]["FirstName"]);
                    objUser.LastName = Convert.ToString(data.Rows[0]["LastName"]);
                    objUser.Email = Convert.ToString(data.Rows[0]["Email"]);
                    objUser.Password = Convert.ToString(data.Rows[0]["Password"]);
                    objUser.PhoneNumber = Convert.ToString(data.Rows[0]["PhoneNumber"]);
                    objUser.ProfilePhoto = Convert.ToString(data.Rows[0]["ProfilePhoto"]);
                    objUser.BloodGroup = Convert.ToString(data.Rows[0]["BloodGroup"]);
                    objUser.Gender = Convert.ToInt32(data.Rows[0]["Gender"]);
                    objUser.DateOfBirth = Convert.ToDateTime(data.Rows[0]["DateOfBirth"]);
                    objUser.Address = Convert.ToString(data.Rows[0]["Address"]);
                    objUser.IsBlock = Convert.ToBoolean(data.Rows[0]["IsBlock"]);
                    objUser.IsActive = Convert.ToBoolean(data.Rows[0]["IsActive"]);

                }
                else
                {
                    TempData["Message"] = "Invalid User Id.";
                }
            }
            return View(objUser);
        }
        [HttpPost]
        public IActionResult AddNew(User obj)
        {
            try
            {
                if (obj.Id > 0)
                    obj.OperationType = "UPDATE";
                else
                    obj.OperationType = "INSERT";

                if (obj.Photo != null)
                {
                    obj.ProfilePhoto = obj.Photo.FileName;
                }
                
                var result = _dbContext.INSERT_UPDATE_DELETE(obj);

                if (result > 0)
                {
                    if (obj.OperationType == "UPDATE")
                    {
                        try
                        {
                            if (obj.Photo != null)
                            {
                                string path = Path.Combine(this.Environment.WebRootPath, "Uploads/" + result);
                                if (!Directory.Exists(path))
                                {
                                    Directory.CreateDirectory(path);
                                }
                                string fileName = Path.GetFileName(obj.Photo.FileName);
                                using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                                {
                                    obj.Photo.CopyTo(stream);
                                }
                            }

                        }
                        catch (Exception)
                        {

                        }
                        TempData["Message"] = "User Updated successfully.";
                    }
                    else
                    {
                        try
                        {
                            if (obj.Photo != null)
                            {
                                string path = Path.Combine(this.Environment.WebRootPath, "Uploads/" + result);
                                if (!Directory.Exists(path))
                                {
                                    Directory.CreateDirectory(path);
                                }
                                string fileName = Path.GetFileName(obj.Photo.FileName);
                                using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                                {
                                    obj.Photo.CopyTo(stream);
                                }
                            }

                        }
                        catch (Exception)
                        {

                        }
                        TempData["Message"] = "User Added successfully.";
                    }
                }
                else
                {
                    TempData["Message"] = "Operation failed please try again.";
                    return RedirectToAction("AddNew", "Home");
                }
            }
            catch (Exception)
            {
                TempData["Message"] = "Internal server error";
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public JsonResult Delete(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    if (Convert.ToInt32(id) > 0)
                    {
                        var obj = new User();
                        obj.OperationType = "DELETE";
                        obj.Id = Convert.ToInt32(id);
                        var result = _dbContext.INSERT_UPDATE_DELETE(obj);

                        if (result > 0)
                        {
                            TempData["Message"] = "User deleted successfully";
                        }
                        else
                        {
                            TempData["Message"] = "User delete failed";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Internal server error";
            }
            return Json(new { message = "Record Deleted" });
        }
        [HttpGet]
        public IActionResult ChangeStatus(string id, int Status)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    if (Convert.ToInt32(id) > 0)
                    {
                        var obj = new User();
                        obj.OperationType = "ChangeStatus";
                        obj.IsActive = Convert.ToBoolean(Status);
                        obj.Id = Convert.ToInt32(id);
                        var result = _dbContext.INSERT_UPDATE_DELETE(obj);

                        if (result > 0)
                        {
                            TempData["Message"] = "Status changed successfully";
                        }
                        else
                        {
                            TempData["Message"] = "Status not changeg";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Internal server error";
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult BlockOrUnblock(string id, int Status)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    if (Convert.ToInt32(id) > 0)
                    {
                        var obj = new User();
                        obj.OperationType = "BlockOrUnblock";
                        obj.IsBlock = Convert.ToBoolean(Status);
                        obj.Id = Convert.ToInt32(id);
                        var result = _dbContext.INSERT_UPDATE_DELETE(obj);

                        if (result > 0)
                        {
                            TempData["Message"] = "Status changed successfully";
                        }
                        else
                        {
                            TempData["Message"] = "Status not changeg";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Internal server error";
            }
            return RedirectToAction("Index", "Home");
        }
        #endregion Add/Update

        #region ChangePassword
        public IActionResult ChangePassword()
        {
            ViewData["ProfilePhoto"] = HttpContext.Session.GetString("ProfilePhoto");
            ViewData["UserName"] = HttpContext.Session.GetString("UserName");

            var obj = new ChangePasswordViewModel();
            obj.UserId = Convert.ToString(HttpContext.Session.GetInt32("Id"));
            return View(obj);
        }
        [HttpPost]
        public JsonResult CheckOldPassword(string OldPassword)
        {
            var Password = HttpContext.Session.GetString("Password");
            var res = Password == OldPassword;
            return Json(res);
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel obj)
        {
            try
            {
                string _query = "select * from Addressbook where Id='" + obj.UserId + "'";
                var user = _dbContext.Getdata(_query);

                if (user != null && user.Rows.Count > 0)
                {
                    var objUser = new User();
                    objUser.Id = Convert.ToInt32(obj.UserId);
                    objUser.Password = obj.NewPassword;
                    objUser.OperationType = "ResetPassword";
                    objUser.DateOfBirth = null;

                    var result = _dbContext.INSERT_UPDATE_DELETE(objUser);

                    if (result > 0)
                    {
                        TempData["Message"] = "Password Changed successfully.";
                    }
                    else
                    {
                        TempData["Message"] = "Password Change failed";
                    }
                }
                else
                {
                    TempData["Message"] = "User not found.";
                }
            }
            catch (Exception e)
            {
                TempData["Message"] = "Internal server error";
            }
            return RedirectToAction("Login", "Account");
        }
        #endregion ChangePassword
    }
}
