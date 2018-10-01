SitemapXMLFilter.Web.WebInterface = (function() {
    function CWebInterface() {
        var self = this;
        /** @type {SitemapXMLFilter.Web.WebController} */
        var Controller = null;
        
        /** @type {HTMLInputElement} */
        var urlInput = null;
        /** @type {HTMLParagraphElement} */
        var outputText = null;
        /** @type {HTMLSelectElement} */
        var recentUrlsList = null;
        /** @type {HTMLButtonElement} */
        var excludeBtn = null;
        /** @type {HTMLButtonElement} */
        var checkAllBtn = null;
        /** @type {HTMLButtonElement} */
        var uncheckAllBtn = null;
        
        /** @type {HTMLDivElement} */
        var treeOutput = null;
        /** @type {HTMLDivElement} */
        var treeOutputParent = null;
        /** @type {HTMLParagraphElement} */
        var treeFeedbackText = null;
        /** @type {HTMLDivElement} */
        var loadingDiv = null;
        
        /** @type {HTMLInputElement} */
        var tablesInput = null;
        /** @type {HTMLInputElement} */
        var hyperlinksInput = null;
        /** @type {HTMLInputElement} */
        var lexicographicOrderInput = null;
        
        this.SetSubmitting = function(submitting) {
            
        };
        
        this.SetLoadingState = function(loading) {
            treeOutput.hidden = loading;
            
            if(loading) {
                loadingDiv.classList.remove("loader-hidden");
            }
            else {
                loadingDiv.classList.add("loader-hidden");
            }
        };
        
        this.SetTabVisibility = function(visible) {
            urlInput.hidden = !visible;
            excludeBtn.hidden = !visible;
            treeOutputParent.hidden = !visible;
            checkAllBtn.hidden = !visible;
            uncheckAllBtn.hidden = !visible;
            outputText.hidden = !visible;
            treeFeedbackText.hidden = !visible;
        };
        
        this.SetReadyState = function(ready) {
            if(ready) {
                excludeBtn.classList.add("plugin-button-ready");
            }
            else {
                excludeBtn.classList.remove("plugin-button-ready");
            }
        };
        
        this.SetReadyVisibility = function(ready) {
            excludeBtn.hidden = !ready;
        };
        
        this.SetSelectionOptions = function(options) {
            var child;
            
            while(child = recentUrlsList.lastChild) {
                recentUrlsList.removeChild(child);
            }
            
            for(var i = 0; i < options.length; i++) {
                child = document.createElement("option");
                child.text = options[i];
                recentUrlsList.appendChild(child);
            }
        };
        
        this.SetIsExcluding = function(excluding) {
            checkAllBtn.disabled = !excluding;
            uncheckAllBtn.disabled = !excluding;
            
            if(excluding) {
                checkAllBtn.classList.remove("plugin-button-disabled");
                uncheckAllBtn.classList.remove("plugin-button-disabled");
            }
            else {
                checkAllBtn.classList.add("plugin-button-disabled");
                uncheckAllBtn.classList.add("plugin-button-disabled");
            }
        };
        
        this.SetTreeFeedbackText = function(text) {
            treeFeedbackText.innerText = text;
        };
        
        this.GetUrl = function() {
            return urlInput.value;
        };
        
        this.GetTablesChecked = function() {
            return tablesInput.checked;
        };
        
        this.GetHyperlinksChecked = function() {
            return hyperlinksInput.checked;
        };
        
        this.GetSortChecked = function() {
            return lexicographicOrderInput.checked;
        };
        
        this.GetTreeId = function() {
            return "webTreeOutput";
        };
        
        this.Init = function() {
            Controller = SitemapXMLFilter.Web.WebController;
            
            urlInput = document.getElementById("webUrl");
            outputText = document.getElementById("webOutputText");
            recentUrlsList = document.getElementById("webRecentUrls");
            excludeBtn = document.getElementById("webExclude");
            checkAllBtn = document.getElementById("webCheckAll");
            uncheckAllBtn = document.getElementById("webUncheckAll");
            
            treeOutput = document.getElementById("webTreeOutput");
            treeOutputParent = document.getElementById("webTreeOutputParent");
            treeFeedbackText = document.getElementById("webTreeFeedbackText");
            loadingDiv = document.getElementById("webLoading");
            
            hyperlinksInput = document.getElementById("gHyperlinks");
            tablesInput = document.getElementById("gTables");
            lexicographicOrderInput = document.getElementById("gLexicographicOrder");
        };
    }
    
    return new CWebInterface();
})();