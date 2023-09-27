using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using VibrantInfoTask.Data;
using VibrantInfoTask.Models;
using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace VibrantInfoTask.Controllers
{

    public class AccountController : Controller
    {
        DbContext _dbContext = new();

        [Obsolete]
        private IHostingEnvironment Environment;

        [Obsolete]
        public AccountController(IHostingEnvironment _environment)
        {
            Environment = _environment;
        }
        #region Login
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginViewModel obj)
        {
            try
            {
                string _query = "select * from Addressbook where Email='" + obj.Email + "' and Password='" + obj.Password + "'";
                var result = _dbContext.Getdata(_query);
                if (result != null && result.Rows.Count > 0)
                {
                    HttpContext.Session.SetInt32("Id", Convert.ToInt32(result.Rows[0]["Id"]));
                    var UserName = Convert.ToString(result.Rows[0]["FirstName"]) + " " + Convert.ToString(result.Rows[0]["LastName"]);
                    HttpContext.Session.SetString("UserName", UserName);
                    HttpContext.Session.SetString("Email", Convert.ToString(result.Rows[0]["Email"]));
                    HttpContext.Session.SetString("Password", Convert.ToString(result.Rows[0]["Password"]));
                    string FileName = Convert.ToString(result.Rows[0]["ProfilePhoto"]);
                    HttpContext.Session.SetString("ProfilePhoto", "/Uploads/" + Convert.ToInt32(result.Rows[0]["Id"]) +"/"+ FileName);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["Message"] = "User not found";
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception)
            {
                TempData["Message"] = "Internal server error";
            }
            return RedirectToAction("Login", "Account");
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }

        #endregion Login

        #region Signup
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SignUp(User obj)
        {
            try
            {
                obj.OperationType = "INSERT";
                obj.IsActive = true;
                obj.IsBlock = false;
                if (obj.Photo != null)
                {
                    obj.ProfilePhoto = obj.Photo.FileName;
                }
                var result = _dbContext.INSERT_UPDATE_DELETE(obj);

                if (result > 0)
                {
                    try
                    {
                        if (obj.ProfilePhoto != null)
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

                    TempData["Message"] = "Registration completed successfully.";
                }
                else
                {
                    TempData["Message"] = "Registration failed please try again.";
                    return RedirectToAction("SignUp", "Account");
                }
            }
            catch (Exception)
            {
                TempData["Message"] = "Internal server error";
            }
            return RedirectToAction("Login", "Account");
        }


        [HttpPost]
        public JsonResult IsEmailExist(string email, int Id)
        {
            string _query = "select * from Addressbook where Email='" + email + "' and Id !=" + Id;
            var result = _dbContext.Getdata(_query);
            var res = result.Rows.Count > 0;
            return Json(!res);
        }

        [HttpPost]
        public JsonResult IsPhoneNumberExist(string PhoneNumber, int Id)
        {
            string _query = "select * from Addressbook where PhoneNumber='" + PhoneNumber + "' and Id !=" + Id;
            var result = _dbContext.Getdata(_query);
            var res = result.Rows.Count > 0;
            return Json(!res);
        }

        #endregion Signup

        #region ForgotPassword
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(LoginViewModel obj)
        {
            try
            {
                string _query = "select * from Addressbook where Email='" + obj.Email + "'";
                var result = _dbContext.Getdata(_query);

                if (result != null && result.Rows.Count > 0)
                {
                    var resetLink = "<a href='" + Url.Action("ResetPassword", "Account", new { userId = result.Rows[0]["Id"] }, "https") + "'>Reset Password</a>";
                    var userEmail = result.Rows[0]["Email"].ToString();

                    var emailService = new EmailService();
                    await emailService.SendForgotPasswordEmailAsync(userEmail, resetLink);

                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    TempData["Message"] = "Please enter valid email address";
                }
            }
            catch (Exception e)
            {
                TempData["Message"] = "Internal server error";
            }
            return RedirectToAction("ForgotPassword", "Account");
        }

        #region ResetPassword
        public IActionResult ResetPassword(string UserId)
        {
            var obj = new ResetPasswordViewModel();
            obj.UserId = UserId;
            return View(obj);
        }
        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordViewModel obj)
        {
            try
            {
                string _query = "select * from Addressbook where Id='" + obj.UserId + "'";
                var user = _dbContext.Getdata(_query);

                if (user != null && user.Rows.Count > 0)
                {
                    var objUser = new User();
                    objUser.Id = Convert.ToInt32(obj.UserId);
                    objUser.Password = obj.Password;
                    objUser.OperationType = "ResetPassword";
                    objUser.DateOfBirth = null;

                    var result = _dbContext.INSERT_UPDATE_DELETE(objUser);

                    if (result > 0)
                    {
                        TempData["Message"] = "Password reset successful";
                    }
                    else
                    {
                        TempData["Message"] = "Password reset failed";
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
            return RedirectToAction("Login");
        }
        #endregion ResetPassword

        #endregion ForgotPassword

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}