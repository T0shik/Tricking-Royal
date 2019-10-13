﻿using TrickingRoyal.Database;
using Battles.Application.ViewModels;
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Battles.Enums;

namespace Battles.Application.Services.Users.Commands
{
    public class UpdateUserCommand : IRequest<BaseResponse>
    {
        [Required] [StringLength(15)] public string DisplayName { get; set; }

        [Required] public int Skill { get; set; }

        [StringLength(255)] public string Information { get; set; }

        [StringLength(30)] public string City { get; set; }
        [StringLength(30)] public string Country { get; set; }
        [StringLength(40)] public string Gym { get; set; }

        [StringLength(100)] public string Instagram { get; set; }
        [StringLength(100)] public string Facebook { get; set; }
        [StringLength(100)] public string Youtube { get; set; }

        public string RealUserId { get; set; }

        public UpdateUserCommand AttachUserId(string id)
        {
            RealUserId = id;
            return this;
        }
    }

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, BaseResponse>
    {
        private readonly AppDbContext _ctx;

        public UpdateUserCommandHandler(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<BaseResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = _ctx.UserInformation
                .FirstOrDefault(x => x.Id == request.RealUserId);

            if (user == null)
                return new BaseResponse("User not found", false);

            user.DisplayName = request.DisplayName;
            user.Skill = (Skill) request.Skill;
            user.City = request.City;
            user.Country = request.Country;
            user.Gym = request.Gym;
            user.Information = request.Information;
            user.Instagram = request.Instagram;
            user.Facebook = request.Facebook;
            user.Youtube = request.Youtube;
            //Todo: allow more configuration (email notifications etc...).

            if (!_ctx.ChangeTracker.HasChanges())
                return new BaseResponse("Profile saved.", true);
            
            var saved = await _ctx.SaveChangesAsync(cancellationToken) > 0;

            return saved
                ? new BaseResponse("Profile updated.", true)
                : new BaseResponse("Failed to update profile", false);
        }
    }
}