SitemapXMLFilter.Web = {};

SitemapXMLFilter.Web.WebController = (function() {
    function CWebController() {
        var self = this;
        /** @type {SitemapXMLFilter.Web.WebInterface} */
        var Interface = null;
        /** @type {SitemapXMLFilter.Web.WebService} */
        var Service = null;
        
        /** @type {SitemapXMLFilter.Web.WebRecentUrls} */
        var recentUrls = null;
        
        /** @type {SitemapXMLFilter.Utils.ExclusionTree} */
        var urlExclusion = null;
        
        var settings = {};
        
        this.isSubmitting = false;
        
        var isExcluding = false;
        
        this.QuerySettings = function(callback) {
            SDAPI.VS.GetUserSettings(function(response, err) {
                if(response && (typeof response.Payload == "string") &&
                    response.Payload) {
                    settings = JSON.parse(response.Payload);
                }
                
                //Returns regardless of if there is a valid response
                callback && callback(settings);
            });
        };
        
        this.SetSettings = function(newSettings, callback) {
            settings = newSettings;
            
            SDAPI.VS.SetUserSettings(JSON.stringify(newSettings),
                function(success, err) {
                    callback(success);
                });
        };
        
        this.GetOptionsObject = function() {
            var options = {
                tables: Interface.GetTablesChecked(),
                hyperlinks: Interface.GetHyperlinksChecked(),
                lexicographicOrder: Interface.GetSortChecked()
            };
            
            return options;
        };
        
        this.SetSubmitting = function(submitting) {
            this.isSubmitting = submitting;
            Interface.SetSubmitting(submitting);
        };
        
        this.SetIsExcluding = function(excluding) {
            isExcluding = excluding;
            Interface.SetIsExcluding(excluding);
        };
        
        this.UpdateUrlInput = function() {
            var url = Interface.GetUrl();
            var isValid = url ? true : false;
            
            Interface.SetReadyState(isValid);
            Interface.SetReadyVisibility(isValid);
        };
        
        this.CheckAll = function() {
            isExcluding && urlExclusion.CheckAll();
        };
        
        this.UncheckAll = function() {
            isExcluding && urlExclusion.UncheckAll();
        };
        
        this.Exclude = function() {
            if(this.isSubmitting) {
                return;
            }
            
            this.SetSubmitting(true);
            Interface.SetReadyState(false);
            var url = filterUrlInput();
            
            if(url.length <= 0) {
                this.SetSubmitting(false);
                Interface.SetReadyState(true);
                console.log("Please enter URL.");
            }
            
            console.log("Submitted, please wait...");
            Interface.SetLoadingState(true);
            
            var onSuccess = function(data) {
                urlExclusion.SortLexicographically = Interface.GetSortChecked();
                
                try {
                    urlExclusion.CreateWithXML($.parseXML(data));
                }
                catch(e) {
                    console.log("Error generating exclusion: " + e.toString());
                    Interface.SetLoadingState(false);
                    self.SetIsExcluding(false);
                    self.SetSubmitting(false);
                    return;
                }
                
                recentUrls.AddUrl(url);
            };
            
            var onFail = function(data) {
                Interface.SetLoadingState(false);
                self.SetSubmitting(false);
                var errstr = "Unknown error.";
                
                if(data) {
                    if(typeof data == "string") {
                        errstr = data;
                    }
                    if(data.responseJSON) {
                        errstr = data.responseJSON;
                    }
                    else if(data.responseText) {
                        errstr = data.responseText;
                    }
                }
                
                console.log("Error: " + errstr);
            };
            
            Service.RetrieveSitemap(url, onSuccess, onFail);
        };
        
        this.Submit = function() {
            if(isExcluding) {
                this.ExSubmit();
            }
            else {
                this.UrlSubmit();
            }
        };
        
        this.ExSubmit = function() {
            if(this.isSubmitting) {
                return;
            }
            
            this.SetSubmitting(true);
            
            var str = urlExclusion.ExportSitemap();
            var data = new Uint16Array(str.length);
            var options = this.GetOptionsObject();
            
            for(var i = 0; i < str.length; i++) {
                data[i] = str.charCodeAt(i);
            }
            
            var onSuccess = function(data) {
                SDAPI.VS.VisualScriptDone(data, function(data, err) {
                    SDAPI.VS.CloseDialog();
                });
            };
            
            var onFail = function(data) {
                self.SetSubmitting(false);
                
                var errstr = "Unknown error.";
                
                if(data) {
                    if(typeof data == "string") {
                        errstr = data;
                    }
                    if(data.responseJSON) {
                        errstr = data.responseJSON;
                    }
                    else if(data.responseText) {
                        errstr = data.responseText;
                    }
                }
                
                console.log("Error: " + errstr);
            };
            
            var onStep = function(sent, total) {
                console.log(((total > 0) ? (sent / total * 100) : 100) +
                    "% transferred");
            };
            
            console.log("Submitted, please wait...");
            Service.SitemapUpload(data, onSuccess, onFail, onStep, options);
        };
        
        this.UrlSubmit = function() {
            if(this.isSubmitting) {
                return;
            }
            
            this.SetSubmitting(true);
            
            var url = filterUrlInput();
            var options = this.GetOptionsObject();
            
            if(url.length <= 0) {
                console.log("Please enter URL.");
                self.SetSubmitting(false);
            }
            
            var onSuccess = function(data) {
                recentUrls.AddUrl(url, function() {
                    submitData(data);
                });
            };
            
            var submitData = function(data) {
                SDAPI.VS.VisualScriptDone(data, function(data, err) {
                    SDAPI.VS.CloseDialog();
                });
            };
            
            var onFail = function(data) {
                self.SetSubmitting(false);
                
                var errstr = "Unknown error.";
                
                if(data) {
                    if(typeof data == "string") {
                        errstr = data;
                    }
                    if(data.responseJSON) {
                        errstr = data.responseJSON;
                    }
                    else if(data.responseText) {
                        errstr = data.responseText;
                    }
                }
                
                console.log("Error: " + errstr);
            };
            
            console.log("Submitted, please wait...");
            Service.SitemapConvert(url, onSuccess, onFail, options);
        };
        
        var excludeReadyCallback = function() {
            Interface.SetLoadingState(false);
            self.SetIsExcluding(true);
            self.SetSubmitting(false);
        };
        
        var excludeUpdateCheckedState = function(size, nodes) {
            var text = "Size of sitemap: ";
            text += (size >> 10) + "KB / ";
            text += (SitemapXMLFilter.Globals.maxServerFileSize >> 10) + "KB\n";
            text += "Nodes: " + nodes + " / ";
            text += SitemapXMLFilter.Globals.maxServerNodeCount;
            
            if(nodes > SitemapXMLFilter.Globals.maxServerNodeCount) {
                text += " (Will be truncated on submission)";
            }
            
            Interface.SetTreeFeedbackText(text);
        };
        
        var filterUrlInput = function() {
            var ret = Interface.GetUrl();
            
            if(ret.length == 0) {
                return ret;
            }
            
            var proto = ret.indexOf("://");
            var path = ret.indexOf("/", (proto < 0) ? 0 : proto + 3);
            
            if(path < 0) {
                ret += "/sitemap.xml"
            }
            else if(path == (ret.length - 1)) {
                ret += "sitemap.xml";
            }
            
            if(proto < 0) {
                ret = "http://" + ret;
            }
            
            return ret;
        };
        
        this.Init = function() {
            Interface = SitemapXMLFilter.Web.WebInterface;
            Service = SitemapXMLFilter.Web.WebService;
            
            Interface.Init();
            Service.Init();
            
            recentUrls = new SitemapXMLFilter.Web.WebRecentUrls();
            recentUrls.Init();
            
            urlExclusion = new SitemapXMLFilter.Utils.ExclusionTree();
            urlExclusion.Init(Interface.GetTreeId());
            urlExclusion.JstreeReadyCallback = excludeReadyCallback;
            urlExclusion.UpdateCheckedState = excludeUpdateCheckedState;
        };
    }
    
    return new CWebController();
})();