using DocuSign.MyBusiness.Domain.Admin.Models;

namespace DocuSign.MyBusiness.Domain.Admin.Services.Interfaces
{
    public interface ISettingsRepository
    {
        public Settings Get();

        public Settings Save(Settings model);
    }
}
