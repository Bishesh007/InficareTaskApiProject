namespace InficareTaskProject.ViewModels
{
    public class RolePermissionViewModel
    {
        public string RoleName { get; set; }
        public List<PermissionViewModel> PermissionList { get; set; }
    }

    public class RolePermissionsViewModel
    {
        public string Id { get; set; }
        public string RoleName { get; set; }
        public List<PermissionViewModel> PermissionList { get; set; }
    }
}
