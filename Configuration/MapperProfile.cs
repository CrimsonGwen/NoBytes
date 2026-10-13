

namespace Restaurant.Configuration
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateAccountViewModel, CustomUser>().ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
