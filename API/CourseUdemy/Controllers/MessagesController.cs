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
        private readonly IUnitOfWork _unitOfWork;

        public IMapper _mapper { get; }
        public MessagesController( IMapper mapper ,IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        [HttpPost]
        public async Task<ActionResult<MessageDTO>> CreateMessage(CreateMessageDTO createMessageDTO )
        {
            var username=User.GetUsername();
            if ( username == createMessageDTO.RecipientUsername.ToLower () ) BadRequest ("U can't send to Ur Self");
            var sender =await _unitOfWork.user.GetUserByUserNameAsync(username);
            var recipient=await _unitOfWork.user.GetUserByUserNameAsync(createMessageDTO.RecipientUsername);
            if ( recipient == null ) NotFound ();
            var message=new Message
            {
                Sender=sender,
                Recipient=recipient,
                SenderUsername=sender.UserName,
                RecipientUsername=recipient.UserName,
                Content=createMessageDTO.Content,
            };
            _unitOfWork.messageRepo.AddMessage (message);
            if ( await _unitOfWork.Compelete () ) return Ok (_mapper.Map<MessageDTO>(message));
            return BadRequest ("failed to send message");
        }
        [HttpGet]
        public async Task<ActionResult<PagedList<MessageDTO>>> GetMessagesForUsers ( [FromQuery] MessageParams messageParams) {
            var usernme=User.GetUsername();
            messageParams.Username=User.GetUsername();
            var messages=await  _unitOfWork.messageRepo.GetMessageForUserAsync(messageParams);
            Response.AddPaginationHeader (new PagintionHelper (messages.CurrentPage, messages.PageSize,messages.TotalCount,messages.TotalPage));
            return messages;
        }
      
        [HttpDelete ("{id}")]
        public async Task<ActionResult> DeleteMessage ( int id )
        {
            var username=User.GetUsername();
            var message=await  _unitOfWork.messageRepo.GetMessageAsync(id);
            if ( message.SenderUsername != username && message.RecipientUsername != username )
                return Unauthorized ();
            if ( message.SenderUsername == username)message.SenderDeleteed = true;
            if ( message.RecipientUsername == username)message.RecipientDeleteed = true;
            if ( message.RecipientUsername == username)message.RecipientDeleteed = true;
            if ( message.SenderDeleteed && message.RecipientDeleteed ) _unitOfWork.messageRepo.DeleteMessage (message);
            if ( await _unitOfWork.Compelete () ) return Ok ();
            return BadRequest ("Error When Delete Message");

        }
    }
}
