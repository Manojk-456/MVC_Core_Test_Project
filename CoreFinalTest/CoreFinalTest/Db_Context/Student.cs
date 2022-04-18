using System;
using System.Collections.Generic;

#nullable disable

namespace CoreFinalTest.Db_Context
{
    public partial class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Password { get; set; }
    }
}
