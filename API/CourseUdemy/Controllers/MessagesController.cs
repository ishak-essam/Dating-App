using AutoMapper;
using CourseUdemy.Data;
using CourseUdemy.DTOs;
using CourseUdemy.Entity;
using CourseUdemy.Extensions;
using CourseUdemy.Helpers;
using CourseUdemy.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CourseUdemy.Controllers
{
    public class MessagesController : BaseAPIController
    {
        public IUser _user { get; }
        public IMessageRepo _messageRepo { get; }
        public IMapper _mapper { get; }
        public MessagesController(IUser user,IMessageRepo messageRepo ,IMapper mapper)
        {
            _user = user;
            _messageRepo = messageRepo;
            _mapper = mapper;
        }
        [HttpPost]
        public async Task<ActionResult<MessageDTO>> CreateMessage(CreateMessageDTO createMessageDTO )
        {
            var username=User.GetUsername();
            if ( username == createMessageDTO.RecipientUsername.ToLower () ) BadRequest ("U can't send to Ur Self");
            var sender =await _user.GetUserByUserNameAsync(username);
            var recipient=await _user.GetUserByUserNameAsync(createMessageDTO.RecipientUsername);
            if ( recipient == null ) NotFound ();
            var message=new Message
            {
                Sender=sender,
                Recipient=recipient,
                SenderUsername=sender.UserName,
                RecipientUsername=recipient.UserName,
                Content=createMessageDTO.Content,
            };
            _messageRepo.AddMessage (message);
            if ( await _messageRepo.SaveChangeAsync () ) return Ok (_mapper.Map<MessageDTO>(message));
            return BadRequest ("failed to send message");
        }
        [HttpGet]
        public async Task<ActionResult<PagedList<MessageDTO>>> GetMessagesForUsers ( [FromQuery] MessageParams messageParams) {
            var usernme=User.GetUsername();
            messageParams.Username=User.GetUsername();
            var messages=await _messageRepo.GetMessageForUserAsync(messageParams);
            Response.AddPaginationHeader (new PagintionHelper (messages.CurrentPage, messages.PageSize,messages.TotalCount,messages.TotalPage));
            return messages;
        }
        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessageThread (string username ) {
        var currentUsername=User.GetUsername();
            return Ok (await _messageRepo.GetMessageThread (currentUsername, username));
        }
        [HttpDelete ("{id}")]
        public async Task<ActionResult> DeleteMessage ( int id )
        {
            var username=User.GetUsername();
            var message=await _messageRepo.GetMessageAsync(id);
            if ( message.SenderUsername != username && message.RecipientUsername != username )
                return Unauthorized ();
            if ( message.SenderUsername == username)message.SenderDeleteed = true;
            if ( message.RecipientUsername == username)message.RecipientDeleteed = true;
            if ( message.RecipientUsername == username)message.RecipientDeleteed = true;
            if ( message.SenderDeleteed && message.RecipientDeleteed ) _messageRepo.DeleteMessage (message);
            if ( await _messageRepo.SaveChangeAsync () ) return Ok ();
            return BadRequest ("Error When Delete Message");

        }
    }
}
