using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace VibrantInfoTask.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please Enter First Name.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please Enter Last Name.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please Select Date Of Birth.")]
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Please Enter Email address.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [Remote("IsEmailExist", "Account", HttpMethod = "POST", ErrorMessage = "Email address already exists.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please Enter Password.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please Enter Phone Number.")]
        [Remote("IsPhoneNumberExist", "Account", HttpMethod = "POST", ErrorMessage = "Phone Number already exists.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please Select Gender.")]
        public int Gender { get; set; }
        public string BloodGroup { get; set; }

        public string ProfilePhoto { get; set; }
        public IFormFile Photo { get; set; }

        [Required(ErrorMessage = "Please Enter Address.")]
        public string Address { get; set; }
        public string OperationType { get; set; }
        public bool IsActive { get; set; }
        public bool IsBlock { get; set; }
    }
    public class UserList
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int SortCol { get; set; }
        public string SortDir { get; set; }
        public string Keyword { get; set; }
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public int Gender { get; set; }
        public string BloodGroup { get; set; }
        public string ProfilePhoto { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }
        public bool IsBlock { get; set; }
    }
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please Enter Email address.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please Enter Password.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
    public class ResetPasswordViewModel
    {
        public string UserId { get; set; }

        [Required(ErrorMessage = "Please Enter Password.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please Enter Confirm Password.")]
        [Compare("NewPassword")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
    public class ChangePasswordViewModel
    {
        public string UserId { get; set; }

        [Required(ErrorMessage = "Please Enter Current Password.")]
        [DataType(DataType.Password)]
        [Remote("CheckOldPassword", "Home", HttpMethod = "POST", ErrorMessage = "Incorrect Current password.")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Please Enter New Password.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Please Enter Confirm Password.")]
        [Compare("NewPassword", ErrorMessage = "New Password and Confirm Password not matched.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
