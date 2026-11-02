using LearningPlatform.StudentService.DTOs;

namespace LearningPlatform.StudentService.Services
{
    public class ChatIntegrationService : IChatIntegrationService
    {
        private readonly IHttpClientFactory _factory;
        private readonly ILogger<ChatIntegrationService> _logger;

        public ChatIntegrationService(IHttpClientFactory factory, ILogger<ChatIntegrationService> logger)
        {
            _factory = factory;
            _logger = logger;
        }

        public async Task<List<ConversationDto>> GetConversationsAsync(string studentId)
        {
            try
            {
                var client = _factory.CreateClient("ChatService");
                var sessions = await client.GetFromJsonAsync<List<ChatSessionResponse>>(
                    $"api/chat/sessions?studentId={Uri.EscapeDataString(studentId)}");
                return sessions?.Select(ToConversation).ToList() ?? new List<ConversationDto>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not reach ChatService for conversations");
                return new List<ConversationDto>();
            }
        }

        public async Task<List<ChatMessageDto>> GetMessagesAsync(int chatSessionId)
        {
            try
            {
                var client = _factory.CreateClient("ChatService");
                var result = await client.GetFromJsonAsync<List<ChatMessageDto>>(
                    $"api/chat/history/{chatSessionId}");
                return result ?? new List<ChatMessageDto>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not reach ChatService for messages in session {SessionId}", chatSessionId);
                return new List<ChatMessageDto>();
            }
        }

        public async Task SendMessageAsync(int chatSessionId, string content)
        {
            try
            {
                var client = _factory.CreateClient("ChatService");
                var payload = new { chatSessionId, senderRole = "Student", content };
                var response = await client.PostAsJsonAsync("api/chat/send-message", payload);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not reach ChatService to send message");
                throw new Exception("Chat service is currently unavailable");
            }
        }

        public async Task<ConversationDto> StartConversationAsync(string studentId, string teacherId)
        {
            try
            {
                var client = _factory.CreateClient("ChatService");
                var payload = new { studentId, teacherId };
                var response = await client.PostAsJsonAsync("api/chat/request-session", payload);
                response.EnsureSuccessStatusCode();
                var session = await response.Content.ReadFromJsonAsync<ChatSessionResponse>();
                return session is null ? new ConversationDto() : ToConversation(session);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not reach ChatService to start conversation");
                throw new Exception("Chat service is currently unavailable");
            }
        }

        private static ConversationDto ToConversation(ChatSessionResponse session) => new()
        {
            ChatSessionId = session.Id,
            TeacherId = session.TeacherId,
            LastMessageAt = session.CreatedAt
        };

        // Shape returned by chat-service (api/chat/request-session, api/chat/sessions)
        private sealed record ChatSessionResponse(int Id, string TeacherId, string StudentId, DateTime CreatedAt);
    }
}
