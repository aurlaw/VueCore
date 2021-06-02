using MediatR;
using Microsoft.AspNetCore.Http;

namespace VueCore.Services.Commands
{
    public record MediaProcessRequest(
        string GroupId,
        IFormFile File,
        byte[] Data
    ) : IRequest;
}
