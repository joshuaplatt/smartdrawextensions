SitemapXMLFilter.Local = {};

SitemapXMLFilter.Local.LocalController = (function() {
    function CLocalController() {
        var self = this;
        /** @type {SitemapXMLFilter.Local.LocalService} */
        var Service = null;
        /** @type {SitemapXMLFilter.Local.LocalInterface} */
        var Interface = null;
        
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
            Interface.SetLoadingState(true);
            Interface.SetReadyState(false);
            
            var onFail = function(data) {
                Interface.SetLoadingState(false);
                Interface.SetReadyState(true);
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
            
            var onSuccess = function(str) {
                urlExclusion.SortLexicographically = Interface.GetSortChecked();
                
                try {
                    urlExclusion.CreateWithXML($.parseXML(str));
                }
                catch(e) {
                    console.log("Error generating exclusion: " + e.toString());
                    Interface.SetLoadingState(false);
                    self.SetIsExcluding(false);
                    self.SetSubmitting(false);
                    return;
                }
            };
            
            readFileInput(onSuccess, onFail);
        };
        
        this.Submit = function() {
            if(isExcluding) {
                this.ExSubmit();
            }
            else {
                this.FileSubmit();
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
        
        this.FileSubmit = function() {
            if(this.isSubmitting) {
                return;
            }
            
            this.SetSubmitting(true);
            
            var options = this.GetOptionsObject();
            
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
            
            var onFileLoad = function(str) {
                var data = new Uint16Array(str.length);
                
                for(var i = 0; i < str.length; i++) {
                    data[i] = str.charCodeAt(i);
                }
                
                Service.SitemapUpload(data, onSuccess, onFail, onStep, options);
            };
            
            console.log("Submitted, please wait...");
            readFileInput(onFileLoad, onFail);
        };
        
        var readFileInput = function(onSuccess, onFail) {
            var files = Interface.GetFiles();
            var fr;
            
            if(files.length == 0) {
                onFail && onFail("Please input file.");
                return;
            }
            
            fr = new FileReader();
            fr.readAsText(files[0]);
            
            fr.onload = function() {
                onSuccess && onSuccess(this.result);
            };
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
        
        this.Init = function() {
            Service = SitemapXMLFilter.Local.LocalService;
            Interface = SitemapXMLFilter.Local.LocalInterface;
            
            Service.Init();
            Interface.Init();
            
            urlExclusion = new SitemapXMLFilter.Utils.ExclusionTree();
            urlExclusion.Init(Interface.GetTreeId());
            urlExclusion.JstreeReadyCallback = excludeReadyCallback;
            urlExclusion.UpdateCheckedState = excludeUpdateCheckedState;
        };
    }
    
    return new CLocalController();
})();