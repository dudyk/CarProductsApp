namespace CarProduct.Client
{
    public static class CarScriptHelper
    {
        private const string HasSignInLinkScriptTemplate = @"
            function hasSignInLink() {
                return document.querySelector('a.header-signin') !== null
            }
            hasSignInLink();";

        private const string ClickSignInLinkScriptTemplate = @"
            function clickSignInLink() {
                document.querySelector('a.header-signin').click();
            }
            clickSignInLink();";

        private const string HasSigInFormScriptTemplate = @"
            function hasSignInForm() {
                return document.querySelector('form input#email') !== null
                        && document.querySelector('form input#password') !== null;
            }
            hasSignInForm();";

        private const string SetSignInFormAndSubmitScriptTemplate = @"
            function setSignInFormAndSubmit() {{
                document.querySelector('input#email').value = '{0}';
                document.querySelector('input#password').value = '{1}';
                document.querySelector('form.session-form').submit();
            }}
            setSignInFormAndSubmit();";

        private const string HasSearchFormScriptTemplate = @"
            function hasSearchForm() {
                return document.querySelector('form.search-form') !== null;
            }
            hasSearchForm();";

        private const string SetSearchFormAndSubmitScriptTemplate = @"
            function setSearchFormAndSubmit() {{
                document.querySelector('form select#make-model-search-stocktype').value = '{0}';
                document.querySelector('form select#makes').value = '{1}';
                document.querySelector('form select#models').value = '{2}';
                document.querySelector('form select#make-model-max-price').value = '{3}';
                document.querySelector('form select#make-model-maximum-distance').value = '{4}';
                document.querySelector('form input#make-model-zip').value = '{5}';
                document.querySelector('form.search-form').submit();
            }}
            setSearchFormAndSubmit();";
        
        private const string HasChangePageLinkScriptTemplate = @"
            function hasSearchForm() {{
                return document.querySelector('a#pagination-direct-link-{0}') !== null;
            }}
            hasSearchForm();";
        
        private const string ChangePageScriptTemplate = @"
            function hasSearchForm() {{
                return document.querySelector('a#pagination-direct-link-{0}') !== null;
            }}
            hasSearchForm();";

        public static string GetHasSignInLinkScript() => HasSignInLinkScriptTemplate;
        public static string GetClickSignInLinkScript() => ClickSignInLinkScriptTemplate;
        public static string GetHasSignInFormScript() => HasSigInFormScriptTemplate;
        public static string GetSetSignInFormAndSubmitScript(string userName, string password) =>
            string.Format(SetSignInFormAndSubmitScriptTemplate, userName, password);
        public static string GetHasSearchFormScript() => HasSearchFormScriptTemplate;
        public static string GetSetSearchFormAndSubmitScript(string stockType, string make, string model, string price, string distanceMiles, string zip) =>
            string.Format(SetSearchFormAndSubmitScriptTemplate, stockType, make, model, price, distanceMiles, zip);

        public static string GetHasChangePageLinkScript(int pageNumber) => string.Format(HasChangePageLinkScriptTemplate, pageNumber);
        public static string GetChangePageScript(int pageNumber) => string.Format(ChangePageScriptTemplate, pageNumber);
    }
}