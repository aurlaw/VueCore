using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Scripting.JavaScript.Services;
using VueCore.Events;
using VueCore.Models;
using VueCore.Models.Activities;
using VueCore.Models.Domain;

namespace VueCore.Definitions.JavaScript
{
    public class WorkflowDefinitionProvider : TypeDefinitionProvider
    {
        public override ValueTask<IEnumerable<Type>> CollectTypesAsync(TypeDefinitionContext context, CancellationToken cancellationToken = default)
        {
            var types = new[]{typeof(User), typeof(RegistrationModel), typeof(DocumentFile), typeof(Document), typeof(FileModel), typeof(NewDocumentReceived)};
            return new ValueTask<IEnumerable<Type>>(types);
        }
    }
}