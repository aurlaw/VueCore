using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using VueCore.Models;

namespace VueCore.Components
{
    [ViewComponent(Name = "Version")]
    public class VersionViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(bool show)
        {
            var version =  Assembly.GetEntryAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
// ; GetType().Assembly.GetName().Version.ToString();
            return View(new VersionModel{Show= show, Version = version});
        }
    }
}
