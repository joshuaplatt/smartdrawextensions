SitemapXMLFilter.Local.LocalService = (function() {
    function CLocalService() {
        this.SitemapUpload = function(data, onSuccess, onFail, onStep, settings) {
            SitemapXMLFilter.Utils.SitemapService.SitemapUpload(data, onSuccess,
                onFail, onStep, settings);
        };
        
        this.Init = function() {
            
        };
    }
    
    return new CLocalService();
})();