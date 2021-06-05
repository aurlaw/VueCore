using MediatR;
using Microsoft.AspNetCore.Http;
using VueCore.Models;

namespace VueCore.Services.Commands
{

    public record MediaPruneRequest(
        MediaJob Job
    ) : IRequest;

}