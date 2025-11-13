// Profiles/CheckinProfile.cs
using AutoMapper;
using FlowMind.Api.DTOs.Request;
using FlowMind.Api.DTOs.Response;
using FlowMind.Api.Models.Entities;

namespace FlowMind.Api.Profiles
{
    public class CheckinProfile : Profile
    {
        public CheckinProfile()
        {
            CreateMap<CheckinDiario, CheckinResponse>();
            CreateMap<CheckinCreateRequest, CheckinDiario>();
        }
    }
}