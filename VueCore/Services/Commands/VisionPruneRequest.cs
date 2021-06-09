using System.Collections.Generic;
using MediatR;

namespace VueCore.Services.Commands
{
    public record VisionPruneRequest
    (
        IEnumerable<string> FileList
    ): IRequest;
}