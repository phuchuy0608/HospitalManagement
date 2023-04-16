using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospitalManagement.Helpers
{
    public class CustomErrorDescriber: IdentityErrorDescriber
    {
        public override IdentityError DuplicateUserName(string userName)
        {
            
            var error = base.DuplicateUserName(userName);
            error.Description = "Email đã tồn tại!";
            return error;
        }

    }
}
