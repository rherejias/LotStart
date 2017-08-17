using System;

namespace LotStart.ViewModels
{
    public class UserObject
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string Username { get; set; }
        public string LastName { get; set; }
        public string GivenName { get; set; }
        public string EmployeeNbr { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public string Department { get; set; }
        public string DepartmentCode { get; set; }
        public int MyProperty { get; set; }
        public int AddedBy { get; set; }
        public DateTime DateAdded { get; set; }
        public string Source { get; set; }
        public string Type { get; set; }
        public string ThumbnailPhoto { get; set; }
    }
}