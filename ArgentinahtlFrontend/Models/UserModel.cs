using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CheckArgentina.Models
{

    public class UsersModel
    {
        public string UserUsername { get; set; }
        public string UserName { get; set; }
        public string UserPass { get; set; }
    }

    public class UserListModel
    {
        public List<UsersModel> Users { get; set; }
    }
}