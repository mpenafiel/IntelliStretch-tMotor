using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntelliStretch
{
    public class UserProfile
    {
        public UserProfile() { }
        public UserProfile(string lastName, string firstName, Protocols.Joint joint, string lastLoginTime)
        {
            this.LastName = lastName;
            this.FirstName = firstName;
            this.Joint = joint;
            this.LastLoginTime = lastLoginTime;
        }

        public string LastName { get; set; }
        public string FirstName { get; set; }
        public Protocols.Joint Joint { get; set; }
        public string LastLoginTime { get; set; }
    }
}
