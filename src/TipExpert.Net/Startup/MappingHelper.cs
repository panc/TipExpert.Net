using AutoMapper;
using MongoDB.Bson;
using TipExpert.Core;
using TipExpert.Net.Models;

namespace TipExpert.Net
{
    public static class MappingHelper
    {
        public static void InitializeMappings()
        {
            Mapper.Initialize(c =>
            {
                c.CreateMap<User, UserDto>();
                c.CreateMap<UserDto, User>();

                c.CreateMap<League, LeagueDto>();
                c.CreateMap<LeagueDto, League>();

                c.CreateMap<Match, MatchDto>();
                c.CreateMap<MatchDto, Match>();

                c.CreateMap<Invitation, InvitationDto>();
                c.CreateMap<InvitationDto, Invitation>();

                c.CreateMap<Game, GameDto>();
                c.CreateMap<GameDto, Game>();
                c.CreateMap<MatchTips, MatchTipsDto>();
                c.CreateMap<MatchTipsDto, MatchTips>();
                c.CreateMap<Tip, TipDto>();
                c.CreateMap<TipDto, Tip>();

                c.CreateMap<PlayerDto, Player>();
                c.CreateMap<Player, PlayerDto>()
                    .ForMember(x => x.name, x => x.MapFrom(p => p.User.Name))
                    .ForMember(x => x.email, x => x.MapFrom(p => p.User.Email));

                c.CreateMap<InvitationDto, Invitation>();
                c.CreateMap<Invitation, InvitationDto>()
                    .ForMember(x => x.name, x => x.MapFrom(p => p.User.Name))
                    .ForMember(x => x.email, x => x.ResolveUsing(p => p.User == null ? p.Email : p.User.Email))
                    .ForMember(x => x.gameName, x => x.MapFrom(p => p.Game.Title));

                c.CreateMap<string, ObjectId>().ConvertUsing(x => x.ToObjectId());
            });
        }
    }
}