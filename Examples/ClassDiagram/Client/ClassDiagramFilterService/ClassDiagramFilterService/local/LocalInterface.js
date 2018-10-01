ClassDiagramFilter.Local.LocalInterface = (function() {
    function CLocalInterface() {
        var self = this;
        /** @type {ClassDiagramFilter.Local.LocalController} */
        var Controller = null;
        
        /** @type {HTMLDivElement} */
        var tabDiv = null;
        /** @type {HTMLParagraphElement} */
        var output = null;
        
        /** @type {HTMLInputElement} */
        var fileList = null;
        /** @type {HTMLButtonElement} */
        var excludeClassBtn = null;
        /** @type {HTMLParagraphElement} */
        var fileNameText = null;
        
        /** @type {HTMLDivElement} */
        var classExcludeOutputDiv = null;
        /** @type {HTMLDivElement} */
        var classExclusionTreeOuterDiv = null;
        /** @type {HTMLDivElement} */
        var classExLoading = null;
        
        /** @type {HTMLButtonElement} */
        var classExcludeCheckAllBtn = null;
        /** @type {HTMLButtonElement} */
        var classExcludeUncheckAllBtn = null;
        
        /** @type {HTMLInputElement} */
        var hideMethodsOption = null;
        /** @type {HTMLInputElement} */
        var hidePropertiesOption = null;
        
        this.SetTabVisibility = function(visible) {
            tabDiv.hidden = !visible;
            excludeClassBtn.hidden = !visible;
            classExcludeCheckAllBtn.hidden = !visible;
            classExcludeUncheckAllBtn.hidden = !visible;
            classExclusionTreeOuterDiv.hidden = !visible;
        };
        
        this.GetFiles = function() {
            if(fileList.files && (fileList.files.length > 0)) {
                return fileList.files;
            }
            
            return null;
        };
        
        this.SetClassExlusionLoadingState = function(loading) {
            classExcludeOutputDiv.hidden = loading;
            
            if(loading) {
                classExLoading.classList.remove("loader-hidden");
            }
            else {
                classExLoading.classList.add("loader-hidden");
            }
        };
        
        this.ChangeFileText = function() {
            if(fileList.files && fileList.files.length) {
                Controller.isExcludingClasses = false;
                excludeClassBtn.disabled = false;
                excludeClassBtn.classList.remove("plugin-button-disabled");
                classExcludeOutputDiv.hidden = true;
                
                var str = "";
                for(var i = 0; i < fileList.files.length; i++) {
                    if(i) {
                        str += ", ";
                    }
                    str += fileList.files[i].name;
                }
                
                fileNameText.innerText = str;
            }
            else {
                excludeClassBtn.disabled = true;
                excludeClassBtn.classList.add("plugin-button-disabled");
                fileNameText.innerText = "";
            }
        };
        
        this.SetSubmitting = function(submitting) {
            classExcludeCheckAllBtn.disabled = submitting;
            classExcludeUncheckAllBtn.disabled = submitting;
            fileList.disabled = submitting;
            excludeClassBtn.disabled = submitting || Controller.isExcludingClasses;
            
            if(submitting) {
                classExcludeCheckAllBtn.classList.add("plugin-button-disabled");
                classExcludeUncheckAllBtn.classList.add("plugin-button-disabled");
                fileList.classList.add("plugin-button-disabled");
            }
            else {
                fileList.classList.remove("plugin-button-disabled");
                classExcludeCheckAllBtn.classList.remove("plugin-button-disabled");
                classExcludeUncheckAllBtn.classList.remove("plugin-button-disabled");
            }
            
            if(submitting || Controller.isExcludingClasses) {
                excludeClassBtn.classList.add("plugin-button-disabled");
            }
            else {
                excludeClassBtn.classList.remove("plugin-button-disabled");
            }
        };
        
        this.GetIsHidingMethods = function() {
            return hideMethodsOption.checked;
        };
        
        this.GetIsHidingProperties = function() {
            return hidePropertiesOption.checked;
        };
        
        this.Init = function() {
            Controller = ClassDiagramFilter.Local.LocalController;
            
            tabDiv = document.getElementById("filesTab");
            output = document.getElementById("lfOutputText");
            
            fileList = document.getElementById("lfFiles");
            excludeClassBtn = document.getElementById("lfClassExclude");
            fileNameText = document.getElementById("lfFileName");
            
            classExcludeOutputDiv = document.getElementById("lfClassExclusionTree");
            classExclusionTreeOuterDiv = document.getElementById("lfClassExclusionTreeOuter");
            classExLoading = document.getElementById("lfClassExLoading");
            
            classExcludeCheckAllBtn = document.getElementById("lfClassExcludeCheckAll");
            classExcludeUncheckAllBtn = document.getElementById("lfClassExcludeUncheckAll");
            
            hideMethodsOption = document.getElementById("gHideMethods");
            hidePropertiesOption = document.getElementById("gHideProperties");
        };
    }
    
    return new CLocalInterface();
})();