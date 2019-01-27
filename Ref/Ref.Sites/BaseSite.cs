using OpenQA.Selenium.Chrome;
using Ref.Shared.Providers;

namespace Ref.Sites
{
    public abstract class BaseSite
    {
        protected readonly IAppProvider _appProvider;
        protected readonly ChromeOptions _options;
        protected readonly ChromeDriverService _service;

        protected readonly int DriverTimeSpan = 120;

        public BaseSite(
            IAppProvider appProvider)
        {
            _appProvider = appProvider;

            _options = new ChromeOptions();

            _options.AddArgument("--headless");
            _options.AddArgument("--no-sandbox");

            _service = ChromeDriverService.CreateDefaultService(_appProvider.BinPath());

            _service.SuppressInitialDiagnosticInformation = false;
            _service.HideCommandPromptWindow = true;
        }
    }
}