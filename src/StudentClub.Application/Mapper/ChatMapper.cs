using StudentClub.Application.DTOs.Chat;
using StudentClub.Domain.Entities.Realtime;

namespace StudentClub.Application.Mapper
{
    /// <summary>
    /// Mapper cho Chat entities và DTOs
    /// </summary>
    public  class ChatMapper
    {

        /// <summary>
        /// Map ChatMessage entity sang ChatMessageResponseDto
        /// </summary>
        public static ChatMessageResponseDto ToChatMessageDto(ChatMessage message)
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
                IsDeleted = message.IsDeleted
            };
        }

        /// <summary>
        /// Map CreateChatMessageRequestDto sang ChatMessage entity
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

        /// <summary>
        /// Map danh sách ChatMessage sang danh sách ChatMessageResponseDto
        /// </summary>
        public static List<ChatMessageResponseDto> ToChatMessageDtoList(List<ChatMessage> messages)
        {
            return messages.Select(ToChatMessageDto).ToList();
        }

        /// <summary>
        /// Map ChatConversation entity sang ChatConversationResponseDto
        /// </summary>
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

        /// <summary>
        /// Map danh sách ChatConversation sang danh sách ChatConversationResponseDto
        /// </summary>
        public static List<ChatConversationResponseDto> ToChatConversationDtoList(List<ChatConversation> conversations)
        {
            return conversations.Select(ToChatConversationDto).ToList();
        }

        /// <summary>
        /// Map UnreadCount sang UnreadCountResponseDto
        /// </summary>
        public static UnreadCountResponseDto ToUnreadCountDto(int unreadCount)
        {
            return new UnreadCountResponseDto
            {
                UnreadCount = unreadCount
            };
        }
    }
}