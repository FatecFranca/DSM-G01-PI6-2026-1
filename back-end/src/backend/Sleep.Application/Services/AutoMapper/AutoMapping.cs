using AutoMapper;
using Sleep.Communication.Requests.Sleep;
using Sleep.Communication.Requests.User;
using Sleep.Communication.Responses.Sleep;
using Sleep.Domain.Entities;
using Sleep.Domain.Utils.Page;

namespace Sleep.Application.Services.AutoMapper
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            RequestToDomain();
        }
        public void RequestToDomain()
        {
            CreateMap<RequestRegisterUserJson, User>()
                .ForMember(dest => dest.Password, opt => opt.Ignore());
            CreateMap<RequestCreateSleepRecord, SleepRecord>();

            CreateMap<SleepRecord, ShortSleepRecord>()
                .ForMember(dest => dest.RecordDate, opt => opt.MapFrom(src => src.RecordDate))
                .ForMember(dest => dest.DurationInHours, opt => opt.MapFrom(src => src.DurationHours))
                .ForMember(dest => dest.SleepRecordId, opt => opt.MapFrom(src => src.Id));

            CreateMap<PagedList<SleepRecord>, PagedList<ShortSleepRecord>>()
                .ConvertUsing((src, _, context) =>
                    new PagedList<ShortSleepRecord>(
                        context.Mapper.Map<List<ShortSleepRecord>>(src.Items),
                        src.Page,
                        src.PageSize,
                        src.TotalCount
                    ));
        }
    }
}
