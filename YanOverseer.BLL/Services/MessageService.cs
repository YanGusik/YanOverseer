using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YanOverseer.BLL.Interfaces;
using YanOverseer.DAL;
using YanOverseer.DAL.Models;

namespace YanOverseer.BLL.Services
{
    public class MessageService : IMessageService
    {
        private readonly MainContext _db;
        private readonly IProfileService _profileService;

        public MessageService(MainContext db, IProfileService profileService)
        {
            _db = db;
            _profileService = profileService;
        }

        public async Task<Message> GetMessageByIdAsync(ulong id)
        {
            return await _db.Messages.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task CreateNewMessageAsync(ulong id, string content, DateTimeOffset timestamp, ulong profileId)
        {
            var message = new Message
            {
                Id = id,
                Content = content,
                Timestamp = timestamp,
                ProfileId = profileId
            };
            _db.Messages.Add(message);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateMessageAsync(ulong id, string content)
        {
            var message = await GetMessageByIdAsync(id);

            if (message == null) return;

            message.Content = content;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteMessageAsync(ulong id)
        {
            var message = await GetMessageByIdAsync(id);
            if (message == null) return;
            
            _db.Messages.Remove(message);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteLimitMessagesAsync(ulong profileId, int limit)
        {
            var profile = await _profileService.GetProfileByIdAsync(profileId);

            if (profile == null) return;

            var allMessageCount = _db.Messages.Count(x => x.ProfileId == profile.Id);

            var countToDelete = allMessageCount - limit;

            var messages = _db.Messages.Where(x => x.ProfileId == profile.Id).OrderBy(x => x.Timestamp).Take(countToDelete);

            _db.Messages.RemoveRange(messages);
        }
    }
}