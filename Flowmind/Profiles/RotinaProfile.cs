// Profiles/RotinaProfile.cs
using AutoMapper;
using FlowMind.Api.Models.Entities;
using FlowMind.Api.DTOs.Response;

namespace FlowMind.Api.Profiles
{
    public class RotinaProfile : Profile
    {
        public RotinaProfile()
        {
            CreateMap<RotinaDiaria, RotinaResponse>();
        }
    }
}