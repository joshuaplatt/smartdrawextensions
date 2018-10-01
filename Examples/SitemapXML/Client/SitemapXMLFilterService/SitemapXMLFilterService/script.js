var SitemapXMLFilter = {
    Web: {},
    LF: {},
    
    tablesInput: null,
    hyperlinksInput: null,
    lexicographicOrderInput: null,
    
    tabObjs: [],
    tabStates: {
        LocalFile: 0,
        Web: 1
    },
    
    maxBlockSize: 65000,
    
    currentState: 1,
    
    treeIteration: 0,
    
    //Modify the following values at your own risk
    maxLocalFileSize: 1 << 24, //16 MB
    maxLocalExclusionListNodeCount: 10000,
    maxServerFileSize: 1 << 21,
    maxServerNodeCount: 600,
    
    documentLoaded: false,
    isOnEdge: navigator.userAgent.indexOf("Edge") != -1
};

SitemapXMLFilter.Web = {
    isSubmitting: false,
    
    //submitBtn: null, --temp--
    urlInput: null,
    outputText: null,
    //exSubmitBtn: null, --temp--
    recentUrlsList: null,
    excludeBtn: null,
    checkAllBtn: null,
    uncheckAllBtn: null,
    
    treeOutput: null,
    treeOutputParent: null,
    treeFeedbackText: null,
    loading: null,
    isExcluding: false,
    
    ExcludeUrl: null
};

SitemapXMLFilter.Web.SitemapExclusion = function() {
    this.tree = new SitemapXMLFilter.ExclusionTree();
    this.tree.Init("webTreeOutput");
    
    this.tree.JstreeReadyCallback = function() {
        SitemapXMLFilter.Web.SetLoadingState(false);
        SitemapXMLFilter.Web.checkAllBtn.disabled = false;
        SitemapXMLFilter.Web.checkAllBtn.classList.remove("plugin-button-disabled");
        SitemapXMLFilter.Web.uncheckAllBtn.disabled = false;
        SitemapXMLFilter.Web.uncheckAllBtn.classList.remove("plugin-button-disabled");
        SitemapXMLFilter.Web.isExcluding = true;
    };
    
    this.tree.UpdateCheckedState = function(size, nodes) {
        SitemapXMLFilter.Web.treeFeedbackText.innerText = "Size of sitemap: " +
            (size >> 10) + "KB / " + (SitemapXMLFilter.maxServerFileSize >> 10) +
            "KB\nNodes: " + nodes + " / " + SitemapXMLFilter.maxServerNodeCount;
        
        if(nodes > SitemapXMLFilter.maxServerNodeCount) {
            SitemapXMLFilter.Web.treeFeedbackText.innerText += " (Will be truncated on submission)";
        }
    };
    
    this.sitemapData = null;
    
    this.amountSent = 0;
    this.size = 0;
    this.guid = "";
    
    this.SubmitSitemap = function(sitemap) {
        if(!SitemapXMLFilter.Web.isSubmitting) {
            SitemapXMLFilter.Web.SetSubmitting(true);
            var shortArray = new Uint16Array(sitemap.length);
            this.guid = SitemapXMLFilter.MakeGuid();
            this.amountSent = 0;
            this.size = shortArray.byteLength;
            
            for(var i = 0; i < sitemap.length; i++) {
                shortArray[i] = sitemap.charCodeAt(i);
            }
            
            this.sitemapData = new Uint8Array(shortArray.buffer);
            this.SubmitBlock();
        }
    };
    
    this.SubmitBlock = function() {
        var amountToSend = this.size - this.amountSent;
        var data;
        
        if(amountToSend > SitemapXMLFilter.maxBlockSize) {
            amountToSend = SitemapXMLFilter.maxBlockSize;
        }
        
        data = this.sitemapData.subarray(this.amountSent, this.amountSent + amountToSend);
        this.amountSent += amountToSend;
        
        var request = {
            type: "POST",
            url: "filter/sitemap-upload-block?uploadID=" + this.guid,
            data: data,
            contentType: "text/plain",
            success: function(data, status) {
                if(this.amountSent >= this.size) {
                    this.SubmitFinal();
                }
                else {
                    this.SubmitBlock();
                }
            }.bind(this),
            error: function(data, status) {
                SitemapXMLFilter.Web.outputText.innerText = "Server returned with error code: " + 
                    ((data.responseJSON) ? data.responseJSON : data.responseText);
                SitemapXMLFilter.Web.SetSubmitting(false);
            },
            processData: false
        };
        
        $.ajax(request);
    };
    
    this.SubmitFinal = function() {
        SitemapXMLFilter.Web.outputText.innerText = "Submitted, please wait...";
        
        var input = {
            tables: SitemapXMLFilter.tablesInput.checked,
            hyperlinks: SitemapXMLFilter.hyperlinksInput.checked,
            lexicographicOrder: SitemapXMLFilter.lexicographicOrderInput.checked
        };
        
        var request = {
            type: "POST",
            url: "filter/sitemap-upload-final?uploadID=" + this.guid,
            data: JSON.stringify(input),
            contentType: "text/json",
            success: function(data, status) {
                SDAPI.VS.VisualScriptDone(data, function(data, error) {    //SDone
                    SDAPI.VS.CloseDialog();
                });
            },
            error: function(data, status) {
                SitemapXMLFilter.Web.outputText.innerText = "Server returned with error code: " + 
                    ((data.responseJSON) ? data.responseJSON : data.responseText);
                SitemapXMLFilter.Web.SetSubmitting(false);
            }
        };
        
        $.ajax(request);
    };
};

SitemapXMLFilter.Web.RecentURLs = function() {
    this.MaxUrls = 10;
    
    this.recent = [];
    this.userSettings = {};
    
    this.Init = function() {
        SDAPI.VS.GetUserSettings(function(response, error) {
            var settings = response ? response.Payload : null;
            
            if(settings) {
                this.userSettings = JSON.parse(settings);
                
                this.SetDatalist();
            }
        }.bind(this));
    };
    
    this.AddUrl = function(url) {
        var newRecent = [url];
        
        SDAPI.VS.GetUserSettings(function(response, error) {
            var settings = response ? response.Payload : null;
            
            if(settings) {
                this.userSettings = JSON.parse(settings);
            }
            else {
                this.userSettings = {};
            }
            
            for(var i = 0; i < this.recent.length; i++) {
                if(this.recent[i] != url) {
                    newRecent.push(this.recent[i]);
                }
            }
            
            this.recent = (newRecent.length > this.MaxUrls) ?
                newRecent.shift() :
                newRecent;
            
            this.userSettings.recentUrls = this.recent;
            
            SDAPI.VS.SetUserSettings(JSON.stringify(this.userSettings), function(success, error) {
                //maybe not needed
            }.bind(this));
            
            this.SetDatalist();
        }.bind(this));
    };
    
    this.SetDatalist = function() {
        if(this.userSettings.recentUrls &&
            ((typeof this.userSettings.recentUrls) == "object") &&
            (Array.isArray(this.userSettings.recentUrls))) {
            
            var child;
            
            while(child = SitemapXMLFilter.Web.recentUrlsList.lastChild) {
                SitemapXMLFilter.Web.recentUrlsList.removeChild(child);
            }
            
            this.recent = this.userSettings.recentUrls;
            var el;
            
            for(var i = 0; i < this.recent.length; i++) {
                el = document.createElement("option");
                el.value = this.recent[i];
                SitemapXMLFilter.Web.recentUrlsList.appendChild(el);
            }
        }
    };
};

SitemapXMLFilter.LF = {
    isSubmitting: false,
    
    //limitNoticeText: null, --temp--
    //submitBtn: null, --temp--
    //exSubmitBtn: null, --temp--
    outputText: null,
    fileInput: null,
    fileNameText: null,
    excludeBtn: null,
    checkAllBtn: null,
    uncheckAllBtn: null,
    
    treeOutput: null,
    treeOutputParent: null,
    treeFeedbackText: null,
    loading: null,
    isExcluding: false,
    
    importCancelBtn: null
};

SitemapXMLFilter.LF.SitemapFile = function() {
    this.tree = new SitemapXMLFilter.ExclusionTree();
    this.tree.Init("lfTreeOutput");
    
    this.tree.JstreeReadyCallback = function() {
        SitemapXMLFilter.LF.SetLoadingState(false);
        SitemapXMLFilter.LF.outputText.innerText = "";
        SitemapXMLFilter.LF.checkAllBtn.disabled = false;
        SitemapXMLFilter.LF.checkAllBtn.classList.remove("plugin-button-disabled");
        SitemapXMLFilter.LF.uncheckAllBtn.disabled = false;
        SitemapXMLFilter.LF.uncheckAllBtn.classList.remove("plugin-button-disabled");
        SitemapXMLFilter.Web.isExcluding = true;
    };
    
    this.tree.UpdateCheckedState = function(size, nodes) {
        SitemapXMLFilter.LF.treeFeedbackText.innerText = "Size of sitemap: " +
            (size >> 10) + "KB / " + (SitemapXMLFilter.maxServerFileSize >> 10) +
            "KB\nNodes: " + nodes + " / " + SitemapXMLFilter.maxServerNodeCount;
        
        if(nodes > SitemapXMLFilter.maxServerNodeCount) {
            SitemapXMLFilter.LF.treeFeedbackText.innerText += " (Will be truncated on submission)";
        }
    };
    
    this.xml = null;
    
    this.file = null;
    this.sitemapData = null;
    
    this.amountSent = 0;
    this.size = 0;
    this.guid = "";
    
    this.LoadFile = function(file) {
        var fr = new FileReader();
        fr.readAsText(file);
    };
    
    this.SubmitSitemap = function(sitemap) {
        if(!SitemapXMLFilter.LF.isSubmitting) {
            SitemapXMLFilter.LF.SetSubmitting(true);
            var shortArray = new Uint16Array(sitemap.length);
            this.guid = SitemapXMLFilter.MakeGuid();
            this.amountSent = 0;
            this.size = shortArray.byteLength;
            
            for(var i = 0; i < sitemap.length; i++) {
                shortArray[i] = sitemap.charCodeAt(i);
            }
            
            this.sitemapData = new Uint8Array(shortArray.buffer);
            this.SubmitBlock();
        }
    };
    
    this.SubmitBlock = function() {
        var amountToSend = this.size - this.amountSent;
        var data;
        
        if(amountToSend > SitemapXMLFilter.maxBlockSize) {
            amountToSend = SitemapXMLFilter.maxBlockSize;
        }
        
        data = this.sitemapData.subarray(this.amountSent, this.amountSent + amountToSend);
        this.amountSent += amountToSend;
        
        var request = {
            type: "POST",
            url: "filter/sitemap-upload-block?uploadID=" + this.guid,
            data: data,
            contentType: "text/plain",
            success: function(data, status) {
                if(this.amountSent >= this.size) {
                    this.SubmitFinal();
                }
                else {
                    this.SubmitBlock();
                }
            }.bind(this),
            error: function(data, status) {
                SitemapXMLFilter.LF.outputText.innerText = "Server returned with error code: " +
                    ((data.responseJSON) ? data.responseJSON : data.responseText);
                SitemapXMLFilter.LF.SetSubmitting(false);
            },
            processData: false
        };
        
        $.ajax(request);
    };
    
    this.SubmitFinal = function() {
        SitemapXMLFilter.LF.outputText.innerText = "Submitted, please wait...";
        
        var input = {
            tables: SitemapXMLFilter.tablesInput.checked,
            hyperlinks: SitemapXMLFilter.hyperlinksInput.checked,
            lexicographicOrder: SitemapXMLFilter.lexicographicOrderInput.checked
        };
        
        var request = {
            type: "POST",
            url: "filter/sitemap-upload-final?uploadID=" + this.guid,
            data: JSON.stringify(input),
            contentType: "text/json",
            success: function(data, status) {
                SDAPI.VS.VisualScriptDone(data, function(data, error) {    //SDone
                    SDAPI.VS.CloseDialog();
                });
            },
            error: function(data, status) {
                SitemapXMLFilter.LF.outputText.innerText = "Server returned with error code: " +
                    ((data.responseJSON) ? data.responseJSON : data.responseText);
                SitemapXMLFilter.LF.SetSubmitting(false);
            }
        };
        
        $.ajax(request);
    };
    
    this.ReaderCallback = function(reader) {
        
    };
};

SitemapXMLFilter.Web.SetSubmitting = function(submitting) {
    SitemapXMLFilter.Web.isSubmitting = submitting;
    //SitemapXMLFilter.Web.submitBtn.disabled = submitting; --temp--
    
    //if(SitemapXMLFilter.Web.ExcludeUrl.tree.hasTree) {
    //    //SitemapXMLFilter.Web.exSubmitBtn.disabled = submitting; --temp--
    //}
};

SitemapXMLFilter.Web.FilterUrlInput = function() {
    var ret = SitemapXMLFilter.Web.urlInput.value;
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

SitemapXMLFilter.Web.Exclude = function() {
    if(!SitemapXMLFilter.documentLoaded) {
        return;
    }
    
    SitemapXMLFilter.Web.excludeBtn.classList.remove("plugin-button-ready");
    var url = SitemapXMLFilter.Web.FilterUrlInput();
    
    if(url.length > 0) {
        SitemapXMLFilter.Web.outputText.innerText = "Submitted, please wait...";
        SitemapXMLFilter.Web.SetLoadingState(true);
        
        var request = {
            type: "POST",
            url: "filter/sitemap-retrieval/",
            data: url,
            success: function(data, status) {
                try {
                    SitemapXMLFilter.Web.ExcludeUrl.tree.SortLexicographically =
                        SitemapXMLFilter.lexicographicOrderInput.checked;
                    SitemapXMLFilter.Web.ExcludeUrl.tree.CreateWithXML($.parseXML(data));
                    SitemapXMLFilter.Web.ExcludeUrl.tree.hasTree = true;
                    SitemapXMLFilter.Web.RecentURLHandler.AddUrl(url);
                    
                    if(!SitemapXMLFilter.Web.isSubmitting) {
                        //SitemapXMLFilter.Web.exSubmitBtn.disabled = false; --temp--
                    }
                }
                catch(e) {
                    SitemapXMLFilter.Web.outputText.innerText = "Error: " + e.toString();
                    SitemapXMLFilter.Web.SetLoadingState(false);
                    console.dir(e);
                }
            },
            error: function(data, status) {
                SitemapXMLFilter.Web.outputText.innerText = "Server returned with error code: " + 
                    ((data.responseJSON) ? data.responseJSON : data.responseText);
                SitemapXMLFilter.Web.SetLoadingState(false);
            }
        };
        
        $.ajax(request);
    }
    else {
        SitemapXMLFilter.Web.outputText.innerText = "Please enter URL.";
    }
};

SitemapXMLFilter.Web.ExSubmit = function() {
    if(!SitemapXMLFilter.documentLoaded) {
        return;
    }
    
    if(SitemapXMLFilter.Web.ExcludeUrl.tree) {
        var data = SitemapXMLFilter.Web.ExcludeUrl.tree.ExportSitemap();
        
        if(data.length > (1 << 21)) { //2 MB (for upload)
            SitemapXMLFilter.Web.outputText.innerText = "Error: File too large for export (" + (((data.length * 100) >> 20) * 0.01).toFixed(2) + 
                " MB / 2 MB).  Exclude directories to reduce size.";
            return;
        }
        
        SitemapXMLFilter.Web.ExcludeUrl.SubmitSitemap(data);
    }
    else {
        SitemapXMLFilter.Web.outputText.innerText = "Error: No file imported for exclusion";
    }
};

SitemapXMLFilter.Web.SubmitSitemap = function() {
    if(!SitemapXMLFilter.documentLoaded) {
        return;
    }
    
    if(!SitemapXMLFilter.Web.isSubmitting) {
        SitemapXMLFilter.Web.SetSubmitting(true);
        var url = SitemapXMLFilter.Web.FilterUrlInput();
        
        if(url.length > 0) {
            SitemapXMLFilter.Web.outputText.innerText = "Submitted, please wait...";
            
            var input = {
                url: url,
                tables: SitemapXMLFilter.tablesInput.checked,
                hyperlinks: SitemapXMLFilter.hyperlinksInput.checked,
                lexicographicOrder: SitemapXMLFilter.lexicographicOrderInput.checked
            };
            
            var request = {
                type: "POST",
                url: "filter/sitemapconvert/",
                data: JSON.stringify(input),
                contentType: "text/json",
                success: function(url, data, status) {
                    SitemapXMLFilter.Web.RecentURLHandler.AddUrl(url);
                    SDAPI.VS.VisualScriptDone(data, function(data, error) {    //SDone
                        SDAPI.VS.CloseDialog();
                    });
                    
                }.bind(this, url),
                error: function(data, status) {
                    SitemapXMLFilter.Web.outputText.innerText = "Server returned with error code: " + 
                        ((data.responseJSON) ? data.responseJSON : data.responseText);
                    SitemapXMLFilter.Web.SetSubmitting(false);
                }
            };
            
            $.ajax(request);
        }
        else {
            SitemapXMLFilter.Web.outputText.innerText = "Please enter URL.";
            SitemapXMLFilter.Web.SetSubmitting(false);
        }
    }
};

SitemapXMLFilter.Web.ChangeUrlInput = function() {
    if(!SitemapXMLFilter.documentLoaded) {
        return;
    }
    
    if(SitemapXMLFilter.Web.urlInput.value) {
        SitemapXMLFilter.Web.excludeBtn.classList.add("plugin-button-ready");
        SitemapXMLFilter.Web.excludeBtn.hidden = false;
    }
    else {
        SitemapXMLFilter.Web.excludeBtn.hidden = true;
    }
};

SitemapXMLFilter.Web.UrlInputKeyDownEvent = function(e) {
    if((e.keyCode == 0xA) || (e.keyCode == 0xD)) { //LF || CR
        SitemapXMLFilter.Web.Exclude();
    }
};

SitemapXMLFilter.Web.SetTabVisibility = function(visible) {
    SitemapXMLFilter.Web.urlInput.hidden = !visible;
    SitemapXMLFilter.Web.excludeBtn.hidden = !visible;
    SitemapXMLFilter.Web.treeOutputParent.hidden = !visible;
    SitemapXMLFilter.Web.checkAllBtn.hidden = !visible;
    SitemapXMLFilter.Web.uncheckAllBtn.hidden = !visible;
    SitemapXMLFilter.Web.outputText.hidden = !visible;
    SitemapXMLFilter.Web.treeFeedbackText.hidden = !visible;
};

SitemapXMLFilter.Web.SetLoadingState = function(loading) {
    SitemapXMLFilter.Web.treeOutput.hidden = loading;
    
    if(loading) {
        SitemapXMLFilter.Web.loading.classList.remove("loader-hidden");
    }
    else {
        SitemapXMLFilter.Web.loading.classList.add("loader-hidden");
    }
};

SitemapXMLFilter.Web.CheckAll = function() {
    if(SitemapXMLFilter.Web.ExcludeUrl && SitemapXMLFilter.Web.ExcludeUrl.tree) {// &&
        //SitemapXMLFilter.Web.ExcludeUrl.tree.jstree) {
            SitemapXMLFilter.Web.ExcludeUrl.tree.CheckAll();
    }
};

SitemapXMLFilter.Web.UncheckAll = function() {
    if(SitemapXMLFilter.Web.ExcludeUrl && SitemapXMLFilter.Web.ExcludeUrl.tree) {//&&
        //SitemapXMLFilter.Web.ExcludeUrl.tree.jstree) {
            SitemapXMLFilter.Web.ExcludeUrl.tree.UncheckAll();
    }
};

SitemapXMLFilter.LF.SetSubmitting = function(submitting) {
    SitemapXMLFilter.LF.isSubmitting = submitting;
    //SitemapXMLFilter.LF.submitBtn.disabled = submitting; --temp--
    
    //if(SitemapXMLFilter.LF.ExclusionFile.tree.hasTree) {
    //    //SitemapXMLFilter.LF.exSubmitBtn.disabled = submitting; --temp--
    //}
};

SitemapXMLFilter.LF.Exclude = function() {
    if(!SitemapXMLFilter.documentLoaded) {
        return;
    }
    
    SitemapXMLFilter.LF.excludeBtn.classList.remove("plugin-button-ready");
    var files = SitemapXMLFilter.LF.fileInput.files;
    var file;
    
    if(files.length > 0) {
        file = files[0];
        
        if(file.size > SitemapXMLFilter.maxLocalFileSize) { //16 MB (or overridden value)
            SitemapXMLFilter.LF.outputText.innerText = "Error: File too large (>16 MB)";
            return;
        }
        
        //SitemapXMLFilter.LF.exSubmitBtn.disabled = true; --temp--
        //SitemapXMLFilter.LF.limitNoticeText.hidden = true; --temp--
        //SitemapXMLFilter.LF.ExclusionFile.tree.hasTree = false;
        SitemapXMLFilter.LF.SetLoadingState(true);
        
        var fr = new FileReader();
        fr.readAsText(SitemapXMLFilter.LF.fileInput.files[0]);
        
        fr.onloadend = function(file, e) {
            try {
                SitemapXMLFilter.LF.ExclusionFile.tree.SortLexicographically =
                    SitemapXMLFilter.lexicographicOrderInput.checked;
                SitemapXMLFilter.LF.ExclusionFile.tree.CreateWithXML($.parseXML(this.result));
                SitemapXMLFilter.LF.ExclusionFile.tree.hasTree = true;
                
                if(!SitemapXMLFilter.LF.isSubmitting) {
                    //SitemapXMLFilter.LF.exSubmitBtn.disabled = false; --temp--
                }
            }
            catch(e) {
                SitemapXMLFilter.LF.outputText.innerText = "Error: " + e.toString();
                SitemapXMLFilter.LF.SetLoadingState(false);
                console.dir(e);
            }
        };
    }
    else {
        SitemapXMLFilter.LF.outputText.innerText = "Please input file.";
    }
};

SitemapXMLFilter.LF.Submit = function() {
    if(!SitemapXMLFilter.documentLoaded) {
        return;
    }
    
    var files = SitemapXMLFilter.LF.fileInput.files;
    var file;
    
    if(files.length > 0) {
        file = files[0];
        
        if(file.size > (1 << 21)) { //2 MB (for upload)
            SitemapXMLFilter.LF.outputText.innerText = "Error: File too large for export (>2 MB).  " +
                "Exclude directories to reduce size.";
            return;
        }
        
        var fr = new FileReader();
        fr.readAsText(SitemapXMLFilter.LF.fileInput.files[0]);
        
        fr.onloadend = function(file, e) {
            SitemapXMLFilter.LF.ExclusionFile.SubmitSitemap(this.result);
        };
    }
    else {
        SitemapXMLFilter.LF.outputText.innerText = "Please input file.";
    }
};

SitemapXMLFilter.LF.ExSubmit = function() {
    if(!SitemapXMLFilter.documentLoaded) {
        return;
    }
    
    if(SitemapXMLFilter.LF.ExclusionFile.tree) {
        var data = SitemapXMLFilter.LF.ExclusionFile.tree.ExportSitemap();
        
        if(data.length > (1 << 21)) { //2 MB (for upload)
            SitemapXMLFilter.LF.outputText.innerText = "Error: File too large for export (" +
                (((data.length * 100) >> 20) * 0.01).toFixed(2) + 
                " MB / 2 MB).  Exclude directories to reduce size.";
            return;
        }
        
        SitemapXMLFilter.LF.ExclusionFile.SubmitSitemap(data);
    }
    else {
        SitemapXMLFilter.LF.outputText.innerText = "Error: No file imported for exclusion";
    }
};

SitemapXMLFilter.LF.SetTabVisibility = function(visible) {
    SitemapXMLFilter.LF.fileNameText.hidden = !visible;
    SitemapXMLFilter.LF.excludeBtn.hidden = !visible;
    SitemapXMLFilter.LF.importCancelBtn.hidden = !visible;
    SitemapXMLFilter.LF.treeOutputParent.hidden = !visible;
    SitemapXMLFilter.LF.checkAllBtn.hidden = !visible;
    SitemapXMLFilter.LF.uncheckAllBtn.hidden = !visible;
    SitemapXMLFilter.LF.outputText.hidden = !visible;
    SitemapXMLFilter.LF.treeFeedbackText.hidden = !visible;
};

SitemapXMLFilter.LF.SetLoadingState = function(loading) {
    SitemapXMLFilter.LF.treeOutput.hidden = loading;
    
    if(loading) {
        SitemapXMLFilter.LF.loading.classList.remove("loader-hidden");
    }
    else {
        SitemapXMLFilter.LF.loading.classList.add("loader-hidden");
    }
};

SitemapXMLFilter.LF.SelectFile = function() {
    if(!SitemapXMLFilter.documentLoaded) {
        return;
    }
    
    var file = null;
    
    if(SitemapXMLFilter.LF.fileInput.files && 
        (SitemapXMLFilter.LF.fileInput.files.length > 0)) {
        SitemapXMLFilter.LF.excludeBtn.classList.add("plugin-button-ready");
        SitemapXMLFilter.LF.excludeBtn.hidden = false;
        file = SitemapXMLFilter.LF.fileInput.files[0];
        SitemapXMLFilter.LF.fileNameText.innerText = file.name;
        
        SitemapXMLFilter.SetTab(0);
    }
    else {
        SitemapXMLFilter.LF.ClearFile();
    }
};

SitemapXMLFilter.LF.ClearFile = function() {
    if(!SitemapXMLFilter.documentLoaded) {
        return;
    }
    
    SitemapXMLFilter.LF.excludeBtn.hidden = true;
    SitemapXMLFilter.LF.fileNameText.innerText = "";
    SitemapXMLFilter.SetTab(1);
    
    SitemapXMLFilter.LF.fileInput.value = "";
    
    if(SitemapXMLFilter.LF.fileInput.value) {   //Kill it with fire
        SitemapXMLFilter.LF.fileInput.type = "url";
        SitemapXMLFilter.LF.fileInput.type = "file";
    }
};

SitemapXMLFilter.LF.CheckAll = function() {
    if(SitemapXMLFilter.LF.ExclusionFile && SitemapXMLFilter.LF.ExclusionFile.tree &&
        SitemapXMLFilter.LF.ExclusionFile.tree) {
            SitemapXMLFilter.LF.ExclusionFile.tree.CheckAll();
    }
};

SitemapXMLFilter.LF.UncheckAll = function() {
    if(SitemapXMLFilter.LF.ExclusionFile && SitemapXMLFilter.LF.ExclusionFile.tree &&
        SitemapXMLFilter.LF.ExclusionFile.tree) {
            SitemapXMLFilter.LF.ExclusionFile.tree.UncheckAll();
    }
};

SitemapXMLFilter.Submit = function() {
    if(!SitemapXMLFilter.documentLoaded) {
        return;
    }
    
    if(SitemapXMLFilter.currentState == SitemapXMLFilter.tabStates.Web) {
        if(SitemapXMLFilter.Web.isSubmitting) {
            return;
        }
        
        if(SitemapXMLFilter.Web.ExcludeUrl && SitemapXMLFilter.Web.ExcludeUrl.tree &&
            SitemapXMLFilter.Web.isExcluding) {
            SitemapXMLFilter.Web.ExSubmit();
        }
        else {
            SitemapXMLFilter.Web.SubmitSitemap();
        }
    }
    else if(SitemapXMLFilter.currentState == SitemapXMLFilter.tabStates.LocalFile) {
        if(SitemapXMLFilter.LF.isSubmitting) {
            return;
        }
        
        if(SitemapXMLFilter.LF.ExclusionFile && SitemapXMLFilter.LF.ExclusionFile.tree &&
            SitemapXMLFilter.LF.isExcluding) {
            SitemapXMLFilter.LF.ExSubmit();
        }
        else {
            SitemapXMLFilter.LF.Submit();
        }
    }
};

SitemapXMLFilter.SetTab = function(tabNum) {
    if(!SitemapXMLFilter.documentLoaded) {
        return;
    }
    
    SitemapXMLFilter.currentState = tabNum;
    
    for(var i = 0; i < SitemapXMLFilter.tabObjs.length; i++) {
        if(i == tabNum) {
            SitemapXMLFilter.tabObjs[i].SetTabVisibility(true);
        }
        else {
            SitemapXMLFilter.tabObjs[i].SetTabVisibility(false);
        }
    }
};

SitemapXMLFilter.MakeGuid = function() {
    return "aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaaa".replace(/a/g, function ()
    {
        var rand = Math.random() * 16;
        rand = Math.floor(rand); //make sure we have a whole number
        
        return rand.toString(16);
    });
};

$(document).ready(function() {
    //SitemapXMLFilter.tabDivs.push(document.getElementById("filesTab")); --temp--
    //SitemapXMLFilter.tabDivs.push(document.getElementById("webTab")); --temp--
    SitemapXMLFilter.tabObjs.push(SitemapXMLFilter.LF);
    SitemapXMLFilter.tabObjs.push(SitemapXMLFilter.Web);
    
    SitemapXMLFilter.hyperlinksInput = document.getElementById("gHyperlinks");
    SitemapXMLFilter.tablesInput = document.getElementById("gTables");
    SitemapXMLFilter.lexicographicOrderInput = document.getElementById("gLexicographicOrder");
    
    //SitemapXMLFilter.Web.submitBtn = document.getElementById("webSubmit"); --temp--
    SitemapXMLFilter.Web.urlInput = document.getElementById("webUrl");
    SitemapXMLFilter.Web.outputText = document.getElementById("webOutputText");
    //SitemapXMLFilter.Web.exSubmitBtn = document.getElementById("webExSubmit"); --temp--
    SitemapXMLFilter.Web.recentUrlsList = document.getElementById("webRecentUrls");
    SitemapXMLFilter.Web.excludeBtn = document.getElementById("webExclude");
    SitemapXMLFilter.Web.checkAllBtn = document.getElementById("webCheckAll");
    SitemapXMLFilter.Web.uncheckAllBtn = document.getElementById("webUncheckAll");
    SitemapXMLFilter.Web.treeOutput = document.getElementById("webTreeOutput");
    SitemapXMLFilter.Web.treeOutputParent = document.getElementById("webTreeOutputParent");
    SitemapXMLFilter.Web.treeFeedbackText = document.getElementById("webTreeFeedbackText");
    SitemapXMLFilter.Web.loading = document.getElementById("webLoading");
    
    SitemapXMLFilter.LF.outputText = document.getElementById("lfOutputText");
    //SitemapXMLFilter.LF.submitBtn = document.getElementById("lfSubmit"); --temp--
    SitemapXMLFilter.LF.fileInput = document.getElementById("lfFile");
    SitemapXMLFilter.LF.fileNameText = document.getElementById("lfFileName");
    //SitemapXMLFilter.LF.exSubmitBtn = document.getElementById("lfExSubmit"); --temp--
    //SitemapXMLFilter.LF.limitNoticeText = document.getElementById("lfLimitNotice"); --temp--
    SitemapXMLFilter.LF.excludeBtn = document.getElementById("lfExclude");
    SitemapXMLFilter.LF.checkAllBtn = document.getElementById("lfCheckAll");
    SitemapXMLFilter.LF.uncheckAllBtn = document.getElementById("lfUncheckAll");
    SitemapXMLFilter.LF.importCancelBtn = document.getElementById("lfImportCancel");
    SitemapXMLFilter.LF.treeOutput = document.getElementById("lfTreeOutput");
    SitemapXMLFilter.LF.treeOutputParent = document.getElementById("lfTreeOutputParent");
    SitemapXMLFilter.LF.treeFeedbackText = document.getElementById("lfTreeFeedbackText");
    SitemapXMLFilter.LF.loading = document.getElementById("lfLoading");
    
    SitemapXMLFilter.LF.ExclusionFile = new SitemapXMLFilter.LF.SitemapFile();
    SitemapXMLFilter.Web.ExcludeUrl = new SitemapXMLFilter.Web.SitemapExclusion();
    
    SitemapXMLFilter.Web.urlInput.focus();
    
    try {
        SitemapXMLFilter.Web.RecentURLHandler = new SitemapXMLFilter.Web.RecentURLs();
        SitemapXMLFilter.Web.RecentURLHandler.Init();
    }
    finally {
        SitemapXMLFilter.documentLoaded = true;
    }
});