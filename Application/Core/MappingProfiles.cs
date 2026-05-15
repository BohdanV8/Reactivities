using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Domain;
using Application.Activities.DTOs;
using Application.Activities.Entities;
using Application.Profiles.Entities;
namespace Application.Core
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles() {
            //CreateMap<Activity, Activity>();
            CreateMap<CreateActivityEntity, Activity>();
            CreateMap<EditActivityEntity, Activity>();
            CreateMap<Activity, ActivityEntity>()
                .ForMember(dest => dest.HostId, opt => opt.MapFrom(src => (src.Attendees != null && src.Attendees.Any(x => x.IsHost)) ? src.Attendees.FirstOrDefault(x => x.IsHost).User.Id : null))
                .ForMember(dest => dest.HostDisplayName, opt => opt.MapFrom(src => (src.Attendees != null && src.Attendees.Any(x => x.IsHost)) ? src.Attendees.FirstOrDefault(x => x.IsHost).User.DisplayName : null));
            CreateMap<ActivityAttendee, UserProfile>().
                ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.User.DisplayName)).
                ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.User.ImageUrl)).
                ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.User.Bio)).
                ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.User.Id));
            CreateMap<User, UserProfile>();
            CreateMap<Comment, CommentEntity>().
                ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.User.DisplayName)).
                ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.User.ImageUrl)).
                ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id));
            CreateMap<EditProfileEntity, User>();
        }
    }
}
