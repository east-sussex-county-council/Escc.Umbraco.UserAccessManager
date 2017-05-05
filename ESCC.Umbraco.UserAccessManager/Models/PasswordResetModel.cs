﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Escc.Umbraco.UserAccessManager.Models
{
    public class PasswordResetModel
    {
        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).{8,}$", ErrorMessage = "New password does not comply with password policy | Minimum length of 8 and containing at least 1 upper, lower and numeric character (a-z, A-Z, 0-9)")]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword")]
        [Display(Name = "Confirmation Password")]
        public string NewPasswordConfim { get; set; }

        public int UserId { get; set; }

        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        public string UniqueResetId { get; set; }

        public DateTime TimeLimit { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance in activity logs
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("[UserId:{0},EmailAddress:{1}]", UserId, EmailAddress);
        }
    }
}