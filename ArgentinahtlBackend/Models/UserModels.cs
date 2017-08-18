using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Web.Security;
using ArgentinahtlMVC.Models.Services;
using System;

namespace ArgentinahtlMVC.Models
{
    #region Models

    [TypeConverter(typeof(PascalCaseWordSplittingEnumConverter))]
    public enum UserProfile
    {
        None = 0,
        Operator = 1,
        AdministratorClient = 10,
        Administrator = 100,
        Superadmin = 255
    }

    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [ValidatePasswordLength]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LogInModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class UserModel
    {
        [Display(Name = "User ID")]
        public Guid? UserId { get; set; }

        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "User description")]
        public string UserDescription { get; set; }

        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "User profile")]
        public UserProfile Profile { get; set; }

        [Required]
        [Display(Name = "Enabled")]
        public bool Enabled { get; set; }
    }

    public class UserListModel
    {
        public IEnumerable<UserModel> Users { get; set; }
    }

    #endregion

    #region Services

    public interface IMembershipService
    {
        int MinPasswordLength { get; }

        bool ValidateUser(string userName, string password);
        MembershipCreateStatus CreateUser(string userName, string password, string email, UserProfile profile);
        bool ChangePassword(string userName, string oldPassword, string newPassword);
    }

    public interface IFormsAuthenticationService
    {
        void SignIn(string userName, bool createPersistentCookie);
        void SignOut();
    }

    #endregion
}