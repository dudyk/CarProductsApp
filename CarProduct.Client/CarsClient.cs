using CarProduct.Client.Models;
using CefSharp;
using CefSharp.OffScreen;
using System;
using System.Threading.Tasks;

namespace CarProduct.Client
{
    public class CarsClient : ICarsClient
    {
        /*private readonly CarsClientSettings _carsClientSettings;

        public CarsClient(IOptions<CarsClientSettings> carsClientSettings)
        {
            _carsClientSettings = carsClientSettings.Value;
        }*/



        private readonly string HasSigInLinkScript = @"
            function hasSignInLink() {
                return document.querySelector('a.header-signin') !== null
            }
            hasSignInLink();";

        private readonly string ClickSignInLink = @"
            function clickSignInLink() {
                document.querySelector('a.header-signin').click();
            }
            clickSignInLink();";

        private readonly string HasSigInFormScript = @"
            function hasSignInForm() {
                return document.querySelector('form input#email') !== null
                        && document.querySelector('form input#password') !== null;
            }
            hasSignInForm();";

        private readonly string SetSignInFormAndSubmitScript = @"
            function setSignInFormAndSubmit() {
                document.querySelector('input#email').value = 'johngerson808@gmail.com';
                document.querySelector('input#password').value = 'test8008';
                document.querySelector('form.session-form').submit();
            }
            setSignInFormAndSubmit();";

        private readonly string HasSearchFormScript = @"
            function hasSearchForm() {
                return document.querySelector('form.search-form') !== null;
            }
            hasSearchForm();";

        private readonly string SetSearchFormAndSubmitScript = @"
            function setSearchFormAndSubmit() {
                document.querySelector('form select#make-model-search-stocktype').value = 'used';
                document.querySelector('form select#makes').value = 'tesla';
                document.querySelector('form select#models').value = 'tesla-model_s';
                document.querySelector('form select#make-model-max-price').value = '100000';
                document.querySelector('form select#make-model-maximum-distance').value = 'all';
                document.querySelector('form input#make-model-zip').value = '94596';
                document.querySelector('form.search-form').submit();
            }
            setSearchFormAndSubmit();";

        public Task<ProductsPageSnapshot> GetProductsPage(GetProductsPageRequest scrapeRequest)
        {
            throw new NotImplementedException();
        }

        public async Task<ProductSnapshot> GetProduct(string vehicleId)
        {
            var settings = new CefSettings
            {
                //By default CefSharp will use an in-memory cache, you need to specify a Cache Folder to persist data
                //CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Cache")
            };

            //Perform dependency check to make sure all relevant resources are in our output directory.
            var success = await Cef.InitializeAsync(settings, performDependencyCheck: true, browserProcessHandler: null);

            if (!success)
                throw new Exception("Unable to initialize CEF, check the log file.");

            var waitMainPageLoaded = true;
            var waitLoginPageLoaded = false;
            var waitSearchPageLoaded = false;
            var waitSearchResultPageLoaded = false;

            var browser = new ChromiumWebBrowser("https://www.cars.com/");

            var initialLoadResponse = await browser.WaitForInitialLoadAsync();

            if (!initialLoadResponse.Success)
                throw new Exception($"Page load failed with ErrorCode:{initialLoadResponse.ErrorCode}, HttpStatusCode:{initialLoadResponse.HttpStatusCode}");

            browser.LoadingStateChanged += async (sender, args) =>
            {
                if (!args.IsLoading)
                {
                    if (waitMainPageLoaded)
                    {
                        var result = await browser.EvaluateScriptAsync(HasSigInLinkScript);

                        if (result.Success && Convert.ToBoolean(result.Result))
                        {
                            await browser.EvaluateScriptAsync(ClickSignInLink);
                         
                            waitMainPageLoaded = false;
                            waitLoginPageLoaded = true;
                        }
                    }

                    if (waitLoginPageLoaded)
                    {
                        var result = await browser.EvaluateScriptAsync(HasSigInFormScript);

                        if (result.Success && Convert.ToBoolean(result.Result))
                        {
                            await browser.EvaluateScriptAsync(SetSignInFormAndSubmitScript);

                            waitLoginPageLoaded = false;
                            waitSearchPageLoaded = true;
                        }
                    }

                    if (waitSearchPageLoaded)
                    {
                        var result = await browser.EvaluateScriptAsync(HasSearchFormScript);

                        if (result.Success && Convert.ToBoolean(result.Result))
                        {
                            await browser.EvaluateScriptAsync(SetSearchFormAndSubmitScript);

                            waitSearchPageLoaded = false;
                            waitSearchResultPageLoaded = true;
                        }
                    }

                    if (waitSearchPageLoaded)
                    {
                        // TODO
                    }
                }
            };

            return null;
        }
    }
}