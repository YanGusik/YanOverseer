using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using YanOverseer.BLL.Interfaces;
using YanOverseer.DAL.Models;

namespace YanOverseer.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class RequireUserTypeAttribute : CheckBaseAttribute
    {
        private readonly UserType[] _userTypes;
        private readonly UserTypeCheckMode _checkMode;
        private readonly IGuildSettingsService _serverSettingsService;
        private GuildSettings _serverSettings;

        public RequireUserTypeAttribute(UserTypeCheckMode checkMode, params UserType[] userTypes)
        {
            _userTypes = userTypes;
            _checkMode = checkMode;
            _serverSettingsService = Program.Container.Resolve<IGuildSettingsService>();
        }

        public override async Task<bool> CanExecute(CommandContext ctx, bool help)
        {
            if (ctx.Guild == null || ctx.Member == null)
            {
                return false;
            }

            _serverSettings = await _serverSettingsService.GetGuildSettingsByIdAsync(ctx.Guild.Id);

            var userRealType = GetUserType(ctx.Member);

            return _checkMode switch
            {
                UserTypeCheckMode.Any => _userTypes.Any(x => x == userRealType),
                UserTypeCheckMode.None => _userTypes.All(x => x != userRealType),
                UserTypeCheckMode.MineOrParentAny => (_userTypes.Any(x => x == userRealType) == true ||
                                                      _userTypes.Where(x => x != userRealType)
                                                          .Any(userType => userRealType == userType - 1)),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public UserType GetUserType(DiscordMember user)
        {

            if (user.Roles.Any(discordRole => (discordRole.Permissions & Permissions.Administrator) != 0))
                return UserType.Admin;

            if (_serverSettings != null)
            {
                if (user.Roles.Any(discordRole => discordRole.Name == _serverSettings.ModeratorRoleName))
                    return UserType.Moderator;
            }

            return UserType.Other;
        }
    }
}