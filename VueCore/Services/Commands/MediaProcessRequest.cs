using MediatR;
using Microsoft.AspNetCore.Http;

namespace VueCore.Services.Commands
{
    public record MediaProcessRequest(
        string GroupId,
        string Title,
        IFormFile File,
        byte[] Data
    ) : IRequest;
}
