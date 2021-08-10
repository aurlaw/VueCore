using MediatR;
using VueCore.Models.Domain;

namespace VueCore.Events
{
    /// <summary>
    /// Published when a new document was uploaded into the system.
    /// </summary>
    public record NewDocumentReceived(Document Document, FileModel File) : INotification;
}