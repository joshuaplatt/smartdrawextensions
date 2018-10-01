SitemapXMLFilter.Local.LocalInterface = (function() {
    function CLocalInterface() {
        var self = this;
        /** @type {SitemapXMLFilter.Local.LocalController} */
        var Controller = null;
        
        /** @type {HTMLParagraphElement} */
        var outputText = null;
        /** @type {HTMLInputElement} */
        var fileInput = null;
        /** @type {HTMLParagraphElement} */
        var fileNameText = null;
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
        
        /** @type {HTMLButtonElement} */
        var importCancelBtn = null;
        
        /** @type {HTMLInputElement} */
        var tablesInput = null;
        /** @type {HTMLInputElement} */
        var hyperlinksInput = null;
        /** @type {HTMLInputElement} */
        var lexicographicOrderInput = null;
        
        this.GetFiles = function() {
            return fileInput.files;
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
            return "lfTreeOutput";
        };
        
        this.SetTabVisibility = function(visible) {
            fileNameText.hidden = !visible;
            excludeBtn.hidden = !visible;
            importCancelBtn.hidden = !visible;
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
        
        this.SetLoadingState = function(loading) {
            treeOutput.hidden = loading;
            
            if(loading) {
                loadingDiv.classList.remove("loader-hidden");
            }
            else {
                loadingDiv.classList.add("loader-hidden");
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
        
        this.SetSubmitting = function(submitting) {
            
        };
        
        this.SelectFile = function() {
            excludeBtn.hidden = false;
            excludeBtn.classList.add("plugin-button-ready");
            
            if(fileInput.files.length) {
                fileNameText.innerText = fileInput.files[0].name;
            }
            else {
                fileNameText.innerText = "";
            }
        };
        
        this.ClearFile = function() {
            excludeBtn.hidden = true;
            fileNameText.innerText = "";
            
            fileInput.value = "";
            
            if(fileInput.value) {
                fileInput.type = "url";
                fileInput.type = "file";
            }
        };
        
        this.SetTreeFeedbackText = function(text) {
            treeFeedbackText.innerText = text;
        };
        
        //Setup function to select file and clear file in interface -
        //Rig up to core
        //Also, look at the stuff for exclusion, probably not rigged up quite right.
        //Rig up hecking and unhecking all -
        //Possibly merge Web and Local stuff?
        //(and rig up the enter key) -
        //and auto focus
        //yup -
        
        this.Init = function() {
            Controller = SitemapXMLFilter.Local.LocalController;
            
            outputText = document.getElementById("lfOutputText");
            fileInput = document.getElementById("lfFile");
            fileNameText = document.getElementById("lfFileName");
            excludeBtn = document.getElementById("lfExclude");
            checkAllBtn = document.getElementById("lfCheckAll");
            uncheckAllBtn = document.getElementById("lfUncheckAll");
            
            treeOutput = document.getElementById("lfTreeOutput");
            treeOutputParent = document.getElementById("lfTreeOutputParent");
            treeFeedbackText = document.getElementById("lfTreeFeedbackText");
            loadingDiv = document.getElementById("lfLoading");
            
            importCancelBtn = document.getElementById("lfImportCancel");
            
            hyperlinksInput = document.getElementById("gHyperlinks");
            tablesInput = document.getElementById("gTables");
            lexicographicOrderInput = document.getElementById("gLexicographicOrder");
        };
    }
    
    return new CLocalInterface();
})();