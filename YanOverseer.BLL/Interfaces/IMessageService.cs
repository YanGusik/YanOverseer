using System;
using System.Threading.Tasks;
using YanOverseer.DAL.Models;

namespace YanOverseer.BLL.Interfaces
{
    public interface IMessageService
    {
        Task<Message> GetMessageByIdAsync(ulong id);
        Task CreateNewMessageAsync(ulong id, string content, DateTimeOffset timestamp, ulong profileId);
        Task DeleteMessageAsync(ulong id);
        Task UpdateMessageAsync(ulong id, string content);
        Task DeleteLimitMessagesAsync(ulong profileId, int limit);
    }
}
