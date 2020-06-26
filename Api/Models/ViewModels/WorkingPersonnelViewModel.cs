namespace Api.Models.ViewModels
{
    public class WorkingPersonnelRoleViewModel
    {
        public string Name { set; get; }
        public int RoleId { set; get; }
        public string RoleName { set; get; }
    }
    public class WorkingPersonnelViewModel: WorkingPersonnel
    {
        public string RoleName { set; get; }
    }
}
