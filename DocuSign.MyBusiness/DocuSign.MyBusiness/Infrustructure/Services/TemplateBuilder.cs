using DocuSign.eSign.Model;
using DocuSign.MyBusiness.Infrustructure.Model;
using DocuSign.MyBusiness.Infrustructure.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.IO;

namespace DocuSign.MyBusiness.Infrustructure.Services
{
    public class LocalTemplateBuilder : ITemplateBuilder
    {
        private readonly string _rootDir;

        private const string _employmentContracttemplatePath = "/Templates/Employment_Contract.json";
        private const string _contractTemplatePath = "/Templates/Sales_Contract.json";
        private const string _termsAndConditionsTemplatePath = "/Templates/T_&_C_Supplement.json";
        private const string _userProfileUpdateTemplatePath = "/Templates/User_Profile_Update.json";
        private const string _customQuoteTemplatePath = "/Templates/ORDER_FORM.pdf";

        public LocalTemplateBuilder(IConfiguration configuration)
        {
            _rootDir = configuration.GetValue<string>(WebHostDefaults.ContentRootKey);
        }

        public EnvelopeTemplate BuildTemplate(string templateName)
        {
            var templatePath = GetTemplatePathByName(templateName);
            using var reader = new StreamReader(_rootDir + templatePath);
            return JsonConvert.DeserializeObject<EnvelopeTemplate>(reader.ReadToEnd());
        }

        public byte[] GetBinaryContent(string templateName)
        {
            switch (templateName)
            {
                case TemplateNames.CustomQuoteTemplateName:
                    var templatePath = GetTemplatePathByName(templateName);
                    return File.ReadAllBytes(_rootDir + templatePath);
                default:
                    throw new ApplicationException("The template is not suported");
            }
        }

        private string GetTemplatePathByName(string templateName)
        {
            switch (templateName)
            {
                case TemplateNames.EmploymentContractTemplateName:
                    return _employmentContracttemplatePath;
                case TemplateNames.ContractTemplateName:
                    return _contractTemplatePath;
                case TemplateNames.TermsAndConditionsTemplateName:
                    return _termsAndConditionsTemplatePath;
                case TemplateNames.UserProfileUpdateTemplateName:
                    return _userProfileUpdateTemplatePath;
                case TemplateNames.CustomQuoteTemplateName:
                    return _customQuoteTemplatePath;
                default:
                    throw new ApplicationException("The template is not suported");
            }
        }
    }
}
