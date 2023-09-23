﻿using AutoMapper;
using CourseUdemy.DTOs;
using CourseUdemy.Entity;
using CourseUdemy.Excetions;

namespace CourseUdemy.Helpers
{
    public class AutoMapper :Profile
    {
        public AutoMapper ( )
        {
            CreateMap<User, MemberDTO> ().ForMember (dist => dist.PhotoUrl, opt => opt.MapFrom (src => src.Photos.FirstOrDefault (x => x.IsMain).Url)).ForMember (dest => dest.age, opt => opt.MapFrom (ele => ele.DateofBirth.CalcAgetion ()));
            CreateMap<photo, PhotoDTO> ();
            CreateMap<UpdateMemberDTO, User> ();
            CreateMap<registerDTO, User> ();
            CreateMap<Message, MessageDTO> ()
                .ForMember (d => d.SenderPhotoUrl, o => o.MapFrom (s => s.Sender.Photos.FirstOrDefault (x => x.IsMain).Url))
                .ForMember (d => d.RecipientPhotoUrl, o => o.MapFrom (s => s.Recipient.Photos.FirstOrDefault (x => x.IsMain).Url));

            CreateMap<DateTime, DateTime> ().ConvertUsing (d => DateTime.SpecifyKind (d, DateTimeKind.Utc));
            CreateMap<DateTime?, DateTime?> ().ConvertUsing (d =>d.HasValue? DateTime.SpecifyKind (d.Value, DateTimeKind.Utc):null);
        }
    }
}
