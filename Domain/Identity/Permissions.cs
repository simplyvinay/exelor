using System;
using System.ComponentModel.DataAnnotations;

namespace ApiStarter.Domain.Identity
{
    public enum Permissions : int
    {
        [Display(GroupName = "Users", Name = "Read", Description = "Can read company users")]
        ReadUsers = 10,
        [Display(GroupName = "Users", Name = "Read", Description = "Can edit company users")]
        EditUsers = 10,

        [Display(GroupName = "Super User", Name = "SuperUser", Description = "Has access to everything")]
        SuperUser = Int32.MaxValue, 
    }
}