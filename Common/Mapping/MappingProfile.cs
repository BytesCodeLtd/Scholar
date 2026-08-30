using AutoMapper;
using Scholar.Models;
using Scholar.Models.ViewModels;

namespace Scholar.Common.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            ConfigureTestSettings();
        }

        private void ConfigureTestSettings()
        {
            CreateMap<TestSettings, TestSettingsViewModel>()
                .ForMember(d => d.InstituteName, o => o.Ignore())
                .ForMember(d => d.LogoUrl, o => o.Ignore())
                .ForMember(d => d.InstituteAddress, o => o.Ignore())
                .ForMember(d => d.CanSwitchInstitute, o => o.Ignore())
                .ForMember(d => d.Institutes, o => o.Ignore());

            CreateMap<TestSettingsViewModel, TestSettings>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.InstituteId, o => o.Ignore())
                .ForMember(d => d.Institute, o => o.Ignore())
                .ForMember(d => d.CreatedAt, o => o.Ignore())
                .ForMember(d => d.UpdatedAt, o => o.Ignore())
                .ForMember(d => d.IsActive, o => o.Ignore());
        }
    }
}
