using AutoMapper;
using CourseUdemy.Data;
using CourseUdemy.DTOs;
using CourseUdemy.Entity;
using CourseUdemy.Extensions;
using CourseUdemy.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace CourseUdemy.SignalR
{
    [Authorize]
    public class MessageHub:Hub
    {
        private readonly IMessageRepo _MessageRepo;
        private readonly IUser _user;
        private readonly IMapper _mapper;
        private readonly IHubContext<PresenceHub> _presensehub;

        public MessageHub(IMessageRepo messageRepo , IUser user ,IMapper mapper ,IHubContext<PresenceHub> Presensehub)
        {
            _MessageRepo = messageRepo;
            _user = user;
            _mapper = mapper;
            _presensehub = Presensehub;
        }
        public override async Task OnConnectedAsync ( )
        {
            var htttpContext=Context.GetHttpContext();
            var otherUser =htttpContext.Request.Query["user"];
            var groupName=GetGroupName(Context.User.GetUsername(),otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId,groupName);
            await AddToGroup (groupName);
            var messages =await _MessageRepo.GetMessageThread(Context.User.GetUsername(),otherUser);
            await Clients.Group (groupName).SendAsync ("RecivedMessageThread",messages);

        }
        public override async Task OnDisconnectedAsync ( Exception? exception )
        {
            await RemoveFromMessageGroup ();
            await base.OnDisconnectedAsync (exception);
        }
       
        public async Task SendMessage (CreateMessageDTO createMessageDTO ) {
            var username=Context.User.GetUsername();
            if ( username == createMessageDTO.RecipientUsername.ToLower () ) 
                throw new HubException ("U can't message Ur Self");
            var sender =await _user.GetUserByUserNameAsync(username);
            var recipient=await _user.GetUserByUserNameAsync(createMessageDTO.RecipientUsername);
            if ( recipient == null )
                throw new HubException ("Not Found User");
            var message=new Message
            {
                Sender=sender,
                Recipient=recipient,
                SenderUsername=sender.UserName,
                RecipientUsername=recipient.UserName,
                Content=createMessageDTO.Content,
            };
            var grouname=GetGroupName(sender.UserName,recipient.UserName);
            var group=await _MessageRepo.GetMessageGroup(grouname);
            if ( group.connections.Any (x => x.Username == recipient.UserName) )
            {
                message.DateRead = DateTime.UtcNow;
            }
            else {
                var connectitons=await presenceTracer.GetConnectionForUser(recipient.UserName);
                if ( connectitons != null )
                    await _presensehub.Clients.Clients (connectitons).SendAsync("NewMessageRecived",
                        new { username=sender.UserName, KnownAs = sender.KnownAs});
            }
            ;

            _MessageRepo.AddMessage (message);
            if ( await _MessageRepo.SaveChangeAsync () )
            {
                await Clients.Group (grouname).SendAsync ("NewMessage", _mapper.Map<MessageDTO> (message));
            }
        }

        private string GetGroupName ( string caller, string other )
        {
            var stringCompare=string.CompareOrdinal( caller, other )<0;
            return stringCompare ? $"{other}-{caller}" : $"{other}-{caller}";
        }

        private async Task<bool> AddToGroup (string GroupName) {
            var group =await _MessageRepo.GetMessageGroup(GroupName);
            var ConnectionId= new Connection(Context.ConnectionId,Context.User.GetUsername());
            if(group == null )
            {
                group = new Group (GroupName);
                _MessageRepo.addGroup (group);
            }
            group.connections.Add (ConnectionId);
            return await _MessageRepo.SaveChangeAsync ();
        }
        private async Task RemoveFromMessageGroup ( ) {
            var connection= await _MessageRepo.GetConnection(Context.ConnectionId);
            _MessageRepo.removeConnections (connection);
            await _MessageRepo.SaveChangeAsync ();
        }
    }
}
