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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHubContext<PresenceHub> _presensehub;

        public MessageHub(IUnitOfWork unitOfWork,IMapper mapper ,IHubContext<PresenceHub> Presensehub)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _presensehub = Presensehub;
        }
        public override async Task OnConnectedAsync ( )
        {
            var htttpContext=Context.GetHttpContext();
            var otherUser =htttpContext.Request.Query["user"];
            var groupName=GetGroupName(Context.User.GetUsername(),otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId,groupName);
            var group =await AddToGroup (groupName);
            await Clients.Group (groupName).SendAsync ("UpdatedGroup", group);

            var messages =await _unitOfWork.messageRepo.GetMessageThread(Context.User.GetUsername(),otherUser);

            if ( _unitOfWork.HasChanges () ) await _unitOfWork.Compelete ();
            await Clients.Caller.SendAsync ("RecivedMessageThread",messages);

        }
        public override async Task OnDisconnectedAsync ( Exception? exception )
        {
            var group= await RemoveFromMessageGroup ();
            await Clients.Group(group.Name).SendAsync("UpdatedGroup");
            await base.OnDisconnectedAsync (exception);
        }
       
        public async Task SendMessage (CreateMessageDTO createMessageDTO ) {
            var username=Context.User.GetUsername();
            if ( username == createMessageDTO.RecipientUsername.ToLower () ) 
                throw new HubException ("U can't message Ur Self");
            var sender =await _unitOfWork.user.GetUserByUserNameAsync(username);
            var recipient=await _unitOfWork.user.GetUserByUserNameAsync(createMessageDTO.RecipientUsername);
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
            var group=await _unitOfWork.messageRepo.GetMessageGroup(grouname);
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

            _unitOfWork.messageRepo.AddMessage (message);
            if ( await _unitOfWork.Compelete () )
            {
                await Clients.Group (grouname).SendAsync ("NewMessage", _mapper.Map<MessageDTO> (message));
            }
        }

        private string GetGroupName ( string caller, string other )
        {
            var stringCompare=string.CompareOrdinal( caller, other )<0;
            return stringCompare ? $"{other}-{caller}" : $"{other}-{caller}";
        }

        private async Task<Group> AddToGroup (string GroupName) {
            var group =await _unitOfWork.messageRepo.GetMessageGroup(GroupName);
            var ConnectionId= new Connection(Context.ConnectionId,Context.User.GetUsername());
            if(group == null )
            {
                group = new Group (GroupName);
                _unitOfWork.messageRepo.addGroup (group);
            }
            group.connections.Add (ConnectionId);
            if ( await _unitOfWork.Compelete() ) return group ;
            throw new HubException ("Failed to add to group");
        }
        private async Task<Group> RemoveFromMessageGroup ( ) {
            var group= await _unitOfWork.messageRepo.GetGroupForConnection(Context.ConnectionId);
            var connection=group.connections.FirstOrDefault(x=>x.ConnectionId==Context.ConnectionId);
            _unitOfWork.messageRepo.removeConnections (connection);
            
            if( await _unitOfWork.Compelete () )  return group;
            throw new HubException ("Failed to remove from group");
        }
    }
}
