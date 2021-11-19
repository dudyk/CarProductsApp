using CarProduct.Client.Models;
using CefSharp;
using CefSharp.OffScreen;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CarProduct.Client
{
    public class CarsClient : ICarsClient
    {
        private readonly string _carsUrl;
        private readonly string _userName;
        private readonly string _password;

        private int _currentPage = 1;
        private const string ExitKey = "exitKey";
        private const string DirectoryPath = "App_Data";

        public CarsClient(string carsUrl, string userName, string password)
        {
            _carsUrl = carsUrl;
            _userName = userName;
            _password = password;
        }

        public async Task<IEnumerable<ProductsPageSnapshot>> GetProductsPage(GetProductsPageRequest request)
        {
            var settings = new CefSettings { CachePath = Path.Combine(DirectoryPath, "Cache") };

            var success = await Cef.InitializeAsync(settings, true, null);
            if (!success)
                throw new Exception("Unable to initialize CEF, check the log file.");

            var browser = new ChromiumWebBrowser(_carsUrl);

            var initialLoadResponse = await browser.WaitForInitialLoadAsync();
            if (!initialLoadResponse.Success)
                throw new Exception($"Page load failed with ErrorCode:{initialLoadResponse.ErrorCode}, HttpStatusCode:{initialLoadResponse.HttpStatusCode}");

            var actions = new ConcurrentDictionary<string, Func<Task<string>>>();
            actions.TryAdd(nameof(CheckSignedInCookie), () => CheckSignedInCookie(browser));
            actions.TryAdd(nameof(HasSignInLinkAndClick), () => HasSignInLinkAndClick(browser));
            actions.TryAdd(nameof(HasSignInFormThenSetAndSubmit), () => HasSignInFormThenSetAndSubmit(browser));
            actions.TryAdd(nameof(HasSearchFormThenSetAndSubmit), () => HasSearchFormThenSetAndSubmit(browser, request));
            actions.TryAdd(nameof(HasChangePageLinkScript), () => HasChangePageLinkScript(browser, request.PageCount));

            var nextActionName = nameof(HasSignInLinkAndClick);
            browser.LoadingStateChanged += async (_, args) =>
            {
                if (args.IsLoading) return;

                if (nextActionName is ExitKey) Cef.Shutdown();

                actions.TryGetValue(nextActionName, out var action);
                if (action is null) return;

                var actionResult = await action.Invoke();
                if (actionResult is null) return;
                
                nextActionName = actionResult;
            };

            return null;
        }

        public async Task<ProductSnapshot> GetProduct(string vehicleId)
        {
            throw new NotImplementedException();
        }

        private async Task<string> CheckSignedInCookie(IWebBrowser browser)
        {
            var cookieManager = browser.GetCookieManager();
            var cookies = await cookieManager.VisitAllCookiesAsync();
            var carsLeggedInCookieValue = cookies
                .FirstOrDefault(r => r.Name
                    .Equals("CARS_logged_in", StringComparison.InvariantCultureIgnoreCase))
                ?.Value;

            var isSignedIn = !string.IsNullOrEmpty(carsLeggedInCookieValue)
                             && Convert.ToBoolean(carsLeggedInCookieValue);

            return isSignedIn
                ? nameof(HasSearchFormThenSetAndSubmit)
                : nameof(HasSignInFormThenSetAndSubmit);
        }

        private async Task<string> HasSignInLinkAndClick(IWebBrowser browser)
        {
            var result = await browser.EvaluateScriptAsync(CarScriptHelper.GetHasSignInLinkScript());

            if (result.Success && Convert.ToBoolean(result.Result))
            {
                await browser.EvaluateScriptAsync(CarScriptHelper.GetClickSignInLinkScript());
                return nameof(HasSignInFormThenSetAndSubmit);
            }

            return null;
        }

        private async Task<string> HasSignInFormThenSetAndSubmit(IWebBrowser browser)
        {
            var result = await browser.EvaluateScriptAsync(CarScriptHelper.GetHasSignInFormScript());

            if (result.Success && Convert.ToBoolean(result.Result))
            {
                await browser.EvaluateScriptAsync(CarScriptHelper.GetSetSignInFormAndSubmitScript(_userName, _password));
                return nameof(HasSearchFormThenSetAndSubmit);
            }

            return null;
        }

        private async Task<string> HasSearchFormThenSetAndSubmit(IWebBrowser browser, GetProductsPageRequest request)
        {
            var result = await browser.EvaluateScriptAsync(CarScriptHelper.GetHasSearchFormScript());

            if (result.Success && Convert.ToBoolean(result.Result))
            {
                var script = CarScriptHelper.GetSetSearchFormAndSubmitScript(
                    request.StockType, request.Make, request.Model, request.Price, request.DistanceMiles, request.Zip);
                await browser.EvaluateScriptAsync(script);

                return nameof(HasChangePageLinkScript);
            }

            return null;
        }

        private async Task<string> HasChangePageLinkScript(ChromiumWebBrowser browser, int pageCount)
        {
            if (pageCount == _currentPage) return ExitKey;

            var filePath = $"screenShot-Page-{_currentPage}";
            await TakeScreenShot(browser, filePath);

            var result = await browser.EvaluateScriptAsync(CarScriptHelper.GetHasChangePageLinkScript(_currentPage));

            // TODO get all products vehicleIds

            if (result.Success && Convert.ToBoolean(result.Result))
            {
                _currentPage++;

                await browser.EvaluateScriptAsync(CarScriptHelper.GetChangePageScript(_currentPage));
                return nameof(HasChangePageLinkScript);
            }

            return null;
        }

        private static async Task TakeScreenShot(ChromiumWebBrowser browser, string fileName)
        {
            var result = await browser.ScreenshotAsync();

            if (!Directory.Exists(DirectoryPath))
                Directory.CreateDirectory(DirectoryPath);

            var filePath = Path.Combine(DirectoryPath, $"{fileName}.png");
            result.Save(filePath);

            result.Dispose();
        }
    }
}