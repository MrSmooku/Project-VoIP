﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedClasses {
    public class UserLogin {
        public string Name { get; set; }
        public string SecondName { get; set; }
        public string Login { get; set; }
        public string Description { get; set; }
        public int Id { get; set; }
        public Guid SessionID { get; set; }
        public List<UserInfo> Friends { get; set; }

        public static UserLogin Convert(User user) {
            return new UserLogin {
                Name = user.Name,
                SecondName = user.SecondName,
                Login = user.Login,
                Description = user.Description,
                Id = user.Id,
                SessionID = user.SessionID
            };
        }
    }
}
