using DocuSign.MyBusiness.Domain.Admin.Models;
using DocuSign.MyBusiness.Domain.Admin.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace DocuSign.MyBusiness.Domain.Admin.Services
{
    public class SessionSettingsRepository : ISettingsRepository
    {
        private const string SettingSessionKey = "SettingSessionKey";

        IHttpContextAccessor _httpContextAccessor;

        public SessionSettingsRepository(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Settings Get()
        {
            var sessionValue = _httpContextAccessor.HttpContext.Session.GetString(SettingSessionKey);
            return sessionValue == null ? new Settings() : JsonConvert.DeserializeObject<Settings>(sessionValue);
        }

        public Settings Save(Settings model)
        {
            _httpContextAccessor.HttpContext.Session.SetString(SettingSessionKey, JsonConvert.SerializeObject(model));
            return model;
        }
    }
}
