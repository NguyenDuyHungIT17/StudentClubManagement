using StudentClub.Application.DTOs.Chat;
using StudentClub.Domain.Entities.Realtime;

namespace StudentClub.Application.Mapper
{
    public  class ChatMapper
    {
        /// <summary>
        /// Map ChatMessage entity sang DTO
        /// </summary>
        public static ChatMessageResponseDto ToChatMessageDto(
            ChatMessage message,
            string? fromUserName,
            string? toUserName,
            string? clubName)
        {
            return new ChatMessageResponseDto
            {
                MessageId = message.MessageId,
                SenderId = message.SenderId,
                MessageType = message.MessageType,
                Status = message.Status,
                Content = message.Content,
                ClubId = message.ClubId,
                RecipientId = message.RecipientId,
                CreatedAt = message.CreatedAt,
                IsDeleted = message.IsDeleted,
                FromUserName = fromUserName,
                ToUserName = toUserName,
                ClubName = clubName
            };
        }

        /// <summary>
        /// Map request -> entity
        /// </summary>
        public static ChatMessage ToEntity(CreateChatMessageRequestDto dto, int senderId)
        {
            return new ChatMessage
            {
                SenderId = senderId,
                MessageType = (Domain.Enums.ChatMessageType)dto.MessageType,
                Status = Domain.Enums.ChatMessageStatus.Sent,
                Content = dto.Content,
                ClubId = dto.ClubId,
                RecipientId = dto.RecipientId,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };
        }

        public static ChatConversationResponseDto ToChatConversationDto(ChatConversation conversation)
        {
            return new ChatConversationResponseDto
            {
                ConversationId = conversation.ConversationId,
                User1Id = conversation.User1Id,
                User2Id = conversation.User2Id,
                LastMessageContent = null,
                LastMessageTime = conversation.LastMessageTime,
                IsActive = conversation.IsActive,
                CreatedAt = conversation.CreatedAt,
                UpdatedAt = conversation.UpdatedAt
            };
        }

        public static List<ChatConversationResponseDto> ToChatConversationDtoList(List<ChatConversation> conversations)
        {
            return conversations.Select(ToChatConversationDto).ToList();
        }

        public static UnreadCountResponseDto ToUnreadCountDto(int unreadCount)
        {
            return new UnreadCountResponseDto
            {
                UnreadCount = unreadCount
            };
        }
    }
}