

namespace Restaurant.Configuration
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateAccountViewModel, CustomUser>();
            CreateMap<ReservationViewModel, Reservatie>()
                .ForMember(dest => dest.Datum, opt => opt.MapFrom(src => src.Date))
                .ForMember(dest => dest.AantalPersonen, opt => opt.MapFrom(src => src.Persons))
                .ForMember(dest => dest.TijdSlotId, opt => opt.MapFrom(src => src.TijdSlotId))
                .ForMember(dest => dest.Opmerking, opt => opt.MapFrom(src => src.SpecialRequests));
                // KlantId wordt in de controller gezet via UserManager
                //.ForMember(dest => dest.KlantId, opt => Ignore());
        }
    }
}
