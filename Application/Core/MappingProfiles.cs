using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Domain;
namespace Application.Core
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles() {
            CreateMap<Activity, Activity>();
        }
    }
}
