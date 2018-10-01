SitemapXMLFilter.Web.WebService = (function() {
    function CWebService() {
        var self = this;
        
        this.RetrieveSitemap = function(url, onSuccess, onFail) {
            var request = {
                type: "POST",
                url: "filter/sitemap-retrieval/",
                data: url,
                
                success: function(data, status, x) {
                    onSuccess && onSuccess(data);
                },
                
                fail: function(data, status, x) {
                    onFail && onFail(data);
                }
            };
            
            $.ajax(request);
        };
        
        this.SitemapConvert = function(url, onSuccess, onFail, settings) {
            var booleanSettings = ((typeof settings == "object") && settings) ? settings : {}; 
            var params = {};
            
            params.url = url;
            params.tables = booleanSettings.tables ? true : false;
            params.hyperlinks = booleanSettings.hyperlinks ? true : false;
            params.lexicographicOrder = booleanSettings.lexicographicOrder ? true : false;
            
            var request = {
                type: "POST",
                url: "filter/sitemapconvert",
                data: JSON.stringify(params),
                contentType: "text/json",
                
                success: function(data, status) {
                    onSuccess && onSuccess(data);
                },
                
                error: function(data, status) {
                    onFail && onFail(data);
                }
            };
            
            $.ajax(request);
        };
        
        this.SitemapUpload = function(data, onSuccess, onFail, onStep, settings) {
            SitemapXMLFilter.Utils.SitemapService.SitemapUpload(data, onSuccess,
                onFail, onStep, settings);
        };
        
        this.Init = function() {
            
        };
    }
    
    return new CWebService();
})();