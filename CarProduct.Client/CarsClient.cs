using CarProduct.Client.Models;
using CefSharp;
using CefSharp.OffScreen;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace CarProduct.Client
{
    public class CarsClient : ICarsClient
    {
        private readonly string _carsUrl;
        private readonly string _userName;
        private readonly string _password;

        private const string ExitKey = "exitKey";
        private const string DirectoryPath = "App_Data";

        public CarsClient(string carsUrl, string userName, string password)
        {
            _carsUrl = carsUrl;
            _userName = userName;
            _password = password;
        }

        public async Task<ProductSnapshot> GetProduct(string vehicleId)
        {
            throw new NotImplementedException();
        }

        public async Task<ProductsPageSnapshots> GetProductsPage(GetProductsPageRequest request)
        {
            var snapshots = new ProductsPageSnapshots();

            var settings = new CefSettings
            {
                //CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DirectoryPath, "Cache")
            };

            var success = await Cef.InitializeAsync(settings, true, null);
            if (!success)
                throw new Exception("Unable to initialize CEF, check the log file.");

            var browser = new ChromiumWebBrowser(_carsUrl);

            var initialLoadResponse = await browser.WaitForInitialLoadAsync();
            if (!initialLoadResponse.Success)
                throw new Exception($"Page load failed with ErrorCode:{initialLoadResponse.ErrorCode}, HttpStatusCode:{initialLoadResponse.HttpStatusCode}");

            var tokenSource = new CancellationTokenSource();
            var actions = new ConcurrentDictionary<string, Func<Task<string>>>();
            actions.TryAdd(nameof(CheckSignedInCookie), () => CheckSignedInCookie(browser));
            actions.TryAdd(nameof(HasSignInLinkAndClick), () => HasSignInLinkAndClick(browser));
            actions.TryAdd(nameof(HasSignInFormThenSetAndSubmit), () => HasSignInFormThenSetAndSubmit(browser));
            actions.TryAdd(nameof(HasSearchFormThenSetAndSubmit), () => HasSearchFormThenSetAndSubmit(browser, request));
            foreach (var page in Enumerable.Range(1, request.PageCount))
                actions.TryAdd($"{nameof(GatherSearchPageData)}[{page}]", () => GatherSearchPageData(browser, page, snapshots, tokenSource));

            var processingTaskSource = new TaskCompletionSource<bool>();
            var processingTask = processingTaskSource.Task;
            await Task.Factory.StartNew(() =>
            {
                var nextActionName = nameof(HasSignInLinkAndClick);
                browser.LoadingStateChanged += async (_, args) =>
                {
                    if (args.IsLoading) return;

                    if (nextActionName is ExitKey)
                    {
                        Cef.Shutdown();
                        processingTaskSource.SetResult(true);

                        // todo resolve exit
                    }

                    actions.TryGetValue(nextActionName, out var action);
                    if (action is null) return;

                    var actionResult = await action.Invoke();
                    if (actionResult is null) return;

                    nextActionName = actionResult;
                };
            });

            await processingTask;
            return snapshots;
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
            var result = await browser.EvaluateScriptAsync(ScriptHelper.GetHasSignInLinkScript());

            if (result.Success && Convert.ToBoolean(result.Result))
            {
                await browser.EvaluateScriptAsync(ScriptHelper.GetClickSignInLinkScript());
                return nameof(HasSignInFormThenSetAndSubmit);
            }

            return null;
        }

        private async Task<string> HasSignInFormThenSetAndSubmit(IWebBrowser browser)
        {
            var result = await browser.EvaluateScriptAsync(ScriptHelper.GetHasSignInFormScript());

            if (result.Success && Convert.ToBoolean(result.Result))
            {
                await browser.EvaluateScriptAsync(ScriptHelper.GetSetSignInFormAndSubmitScript(_userName, _password));
                return nameof(HasSearchFormThenSetAndSubmit);
            }

            return null;
        }

        private async Task<string> HasSearchFormThenSetAndSubmit(IWebBrowser browser, GetProductsPageRequest request)
        {
            var result = await browser.EvaluateScriptAsync(ScriptHelper.GetHasSearchFormScript());

            if (result.Success && Convert.ToBoolean(result.Result))
            {
                var script = ScriptHelper.GetSetSearchFormAndSubmitScript(
                    request.StockType, request.Make, request.Model, request.Price, request.DistanceMiles, request.Zip);
                await browser.EvaluateScriptAsync(script);

                return $"{nameof(GatherSearchPageData)}[{1}]";
            }

            return null;
        }

        private async Task<string> GatherSearchPageData(ChromiumWebBrowser browser, int currentPage, ProductsPageSnapshots snapshots, CancellationTokenSource tokenSource)
        {
            var pageLinkResponse = await browser.EvaluateScriptAsync(ScriptHelper.GetHasChangePageLinkScript(currentPage));
            if (pageLinkResponse.Success && pageLinkResponse.Result is null || tokenSource.Token.IsCancellationRequested)
                return null;

            tokenSource.Cancel();

            var vehiclesResponse = await browser.EvaluateScriptAsync(ScriptHelper.GetGetVehicleUrlsScript());
            if (vehiclesResponse.Success && vehiclesResponse.Result is IEnumerable<dynamic> vehicleIds)
            {
                var filePath = $"ScreenShot-Page-{currentPage}";
                await TakeScreenShot(browser, filePath);

                snapshots.Items.Add(new ProductsPageSnapshot
                {
                    PageNumber = currentPage,
                    ScreenShotFileName = filePath,
                    VehicleIds = vehicleIds
                        .Select(r => new Uri(r).Segments[2].Replace("/", ""))
                });

                await browser.EvaluateScriptAsync(ScriptHelper.GetChangePageScript(currentPage));

                return $"{nameof(GatherSearchPageData)}[{++currentPage}]";
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