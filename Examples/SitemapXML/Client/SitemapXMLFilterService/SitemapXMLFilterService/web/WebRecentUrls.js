SitemapXMLFilter.Web.WebRecentUrls = function() {
    var self = this;
    /** @type {SitemapXMLFilter.Web.WebController} */
    var Controller = null;
    
    var maxUrls = 10;
    
    var recentUrls = [];
    
    this.AddUrl = function(url, cb) {
        Controller.QuerySettings(function(settings) {
            recentUrls = [url];
            var oldRecent = [];
            
            if(settings.recentUrls && ((typeof settings.recentUrls) == "object") &&
                (Array.isArray(settings.recentUrls))) {
                oldRecent = settings.recentUrls;
            }
            
            for(var i = 0; i < oldRecent.length; i++) {
                if(oldRecent[i] != url) {
                    recentUrls.push(oldRecent[i]);
                }
            }
            
            (recentUrls.length > maxUrls) && recentUrls.shift();
            
            settings.recentUrls = recentUrls;
            Controller.SetSettings(settings, function() {
                cb && cb();
            });
            
            setDatalist(settings);
        });
    };
    
    var setDatalist = function(settings) {
        if(settings.recentUrls && ((typeof settings.recentUrls) == "object") &&
            (Array.isArray(settings.recentUrls))) {
            SitemapXMLFilter.Web.WebInterface.SetSelectionOptions(settings.recentUrls);
        }
    };
    
    this.Init = function() {
        Controller = SitemapXMLFilter.Web.WebController;
        
        Controller.QuerySettings(function(settings) {
            setDatalist(settings);
        });
    };
};