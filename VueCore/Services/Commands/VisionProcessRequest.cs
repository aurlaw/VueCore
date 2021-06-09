using System.IO;
using MediatR;
using VueCore.Models;

namespace VueCore.Services.Commands
{
    public record VisionProcessRequest(
        string FileName,
        string ContentType,
        Stream DataStream
    ): IRequest<VisionResult>;
}