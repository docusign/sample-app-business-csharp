namespace DocuSign.MyBusiness.Controllers.Admin.Model
{
    public class ConnectedUserModel
    {
        public ConnectedUserModel()
        {
        }

        public ConnectedUserModel(string name, string email)
        {
            Name = name;
            Email = email;
        }

        public string Name { get; set; }
        public string Email { get; set; }
        public string AccountName { get; set; }
    }
}
