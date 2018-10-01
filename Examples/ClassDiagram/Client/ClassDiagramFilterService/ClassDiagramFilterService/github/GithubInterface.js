ClassDiagramFilter.Github.GithubInterface = (function() {
    function CGithubInterface() {
        var self = this;
        /** @type {ClassDiagramFilter.Github.GithubController} */
        var Controller = null;
        
        /** @type {HTMLDivElement} */
        var tabDiv = null;
        /** @type {HTMLDivElement} */
        var divBelowAuthentication = null;
        
        /** @type {HTMLInputElement} */
        var userInput = null;
        /** @type {HTMLInputElement} */
        var repoInput = null;
        /** @type {HTMLInputElement} */
        var branchInput = null;
        /** @type {HTMLButtonElement} */
        var listFilesBtn = null;
        /** @type {HTMLButtonElement} */
        var excludeClassBtn = null;
        /** @type {HTMLButtonElement} */
        var excludeFileBtn = null;
        /** @type {HTMLParagraphElement} */
        var output = null;
        
        /** @type {HTMLDivElement} */
        var treeOutputDiv = null;
        /** @type {HTMLDivElement} */
        var classExcludeOutputDiv = null;
        /** @type {HTMLButtonElement} */
        var classExcludeCheckAllBtn = null;
        /** @type {HTMLButtonElement} */
        var classExcludeUncheckAllBtn = null;
        
        /** @type {HTMLDivElement} */
        var treeOutputOuterDiv = null;
        /** @type {HTMLDivElement} */
        var treeLoading = null;
        /** @type {HTMLDivElement} */
        var classExclusionTreeOuterDiv = null;
        /** @type {HTMLDivElement} */
        var classExLoading = null;
        
        /** @type {HTMLInputElement} */
        var hideMethodsOption = null;
        /** @type {HTMLInputElement} */
        var hidePropertiesOption = null;
        
        /** @type {HTMLSelectElement} */
        var inputHistory = null;
        
        var currentTree = 0;
        
        this.ExclusionTrees = {
            Class: 0,
            Repository: 1
        };
        
        /**
         * Function that is recognized from the tabObjs array in SetTab().
         * @param {boolean} visible Whether or not the tab should be visible.
         */
        this.SetTabVisibility = function(visible) {
            tabDiv.hidden = !visible;
            excludeClassBtn.hidden = !visible;
            listFilesBtn.hidden = !visible;
            classExcludeCheckAllBtn.hidden = !visible;
            classExcludeUncheckAllBtn.hidden = !visible;
            
            divBelowAuthentication.hidden = visible &&
                !Controller.isAuthenticated;
            
            treeOutputOuterDiv.hidden = !visible ||
                (currentTree != this.ExclusionTrees.Repository);
            
            classExclusionTreeOuterDiv.hidden = !visible ||
                (currentTree != this.ExclusionTrees.Class);
        };
        
        this.GetParamsInput = function() {
            var params = {
                user: userInput.value,
                repo: repoInput.value,
                branch: branchInput.value
            };
            
            return params;
        };
        
        /**
         * 
         * @param {{user: string, repo: string, branch: string}} params 
         */
        this.SetParamsInput = function(params) {
            userInput.value = params.user;
            repoInput.value = params.repo;
            branchInput.value = params.branch;
        };
        
        this.GetCurrentTree = function() {
            return currentTree;
        };
        
        this.SetSelectionOptions = function(options) {
            var child;
            
            while(child = inputHistory.lastChild) {
                inputHistory.removeChild(child);
            }
            
            for(var i = 0; i < options.length; i++) {
                inputHistory.appendChild(options[i]);
            }
        };
        
        this.HideVerificationTab = function() {
            var wasHidden = tabDiv.hidden;
            
            this.SetTabVisibility(false);
            Controller.isAuthenticated = true;
            tabDiv = document.getElementById("githubTab");
            output = document.getElementById("ghOutput");
            this.SetTabVisibility(!wasHidden);
        };
        
        this.ShowVerificationTab = function() {
            var wasHidden = tabDiv.hidden;
            
            this.SetTabVisibility(false);
            Controller.isAuthenticated = false;
            tabDiv = document.getElementById("githubAuthTab");
            output = document.getElementById("ghAuthOutput");
            this.SetTabVisibility(!wasHidden);
        };
        
        this.SetExclusionTreeFocus = function(tree) {
            if(tree == this.ExclusionTrees.Class) {
                currentTree = tree;
            }
            else if(tree == this.ExclusionTrees.Repository) {
                currentTree = tree;
            }
            else {
                return;
            }
            
            treeOutputOuterDiv.hidden = tabDiv.hidden ||
                (currentTree != this.ExclusionTrees.Repository);
            
            classExclusionTreeOuterDiv.hidden = tabDiv.hidden ||
                (currentTree != this.ExclusionTrees.Class);
            
            excludeClassBtn.hidden = tabDiv.hidden ||
                (currentTree == this.ExclusionTrees.Class);
            
            excludeFileBtn.hidden = tabDiv.hidden ||
                (currentTree != this.ExclusionTrees.Class);
        };
        
        this.SetRepositoryExclusionLoadingState = function(loading) {
            treeOutputDiv.hidden = loading;
            
            if(loading) {
                treeLoading.classList.remove("loader-hidden");
            }
            else {
                treeLoading.classList.add("loader-hidden");
            }
        };
        
        this.SetClassExclusionLoadingState = function(loading) {
            classExcludeOutputDiv.hidden = loading;
            
            if(loading) {
                classExLoading.classList.remove("loader-hidden");
            }
            else {
                classExLoading.classList.add("loader-hidden");
            }
        };
        
        this.SetSubmittting = function(submitting) {
            classExcludeCheckAllBtn.disabled = submitting;
            classExcludeUncheckAllBtn.disabled = submitting;
            listFilesBtn.disabled = submitting;
            excludeClassBtn.disabled = submitting ||
                (currentTree == this.ExclusionTrees.Class);
            excludeFileBtn.disabled = submitting ||
                (currentTree != this.ExclusionTrees.Class);
            
            if(submitting) {
                classExcludeCheckAllBtn.classList.add("plugin-button-disabled");
                classExcludeUncheckAllBtn.classList.add("plugin-button-disabled");
                listFilesBtn.classList.add("plugin-button-disabled");
            }
            else {
                classExcludeCheckAllBtn.classList.remove("plugin-button-disabled");
                classExcludeUncheckAllBtn.classList.remove("plugin-button-disabled");
                listFilesBtn.classList.remove("plugin-button-disabled");
            }
            
            if(submitting || (currentTree == this.ExclusionTrees.Class)) {
                excludeClassBtn.classList.add("plugin-button-disabled");
            }
            else {
                excludeClassBtn.classList.remove("plugin-button-disabled");
            }
            
            if(submitting || (currentTree != this.ExclusionTrees.Class)) {
                excludeFileBtn.classList.add("plugin-button-disabled");
            }
            else {
                excludeFileBtn.classList.remove("plugin-button-disabled");
            }
        };
        
        this.RemoveGlowyButton = function() {
            listFilesBtn.classList.remove("plugin-button-ready");
        };
        
        this.GetIsHidingMethods = function() {
            return hideMethodsOption.checked;
        };
        
        this.GetIsHidingProperties = function() {
            return hidePropertiesOption.checked;
        };
        
        this.Init = function() {
            Controller = ClassDiagramFilter.Github.GithubController;
            
            tabDiv = document.getElementById("githubAuthTab");
            divBelowAuthentication = document.getElementById("gEverythingBelowAuthentication");
            
            userInput = document.getElementById("ghUser");
            repoInput = document.getElementById("ghRepo");
            branchInput = document.getElementById("ghBranch");
            listFilesBtn = document.getElementById("ghListFilesBtn");
            excludeClassBtn = document.getElementById("ghClassExclude");
            excludeFileBtn = document.getElementById("ghFileExclude");
            output = document.getElementById("ghAuthOutput");
            
            treeOutputDiv = document.getElementById("ghTreeOutput");
            classExcludeOutputDiv = document.getElementById("ghClassExclusionTree");
            classExcludeCheckAllBtn = document.getElementById("ghClassExcludeCheckAll");
            classExcludeUncheckAllBtn = document.getElementById("ghClassExcludeUncheckAll");
            
            treeOutputOuterDiv = document.getElementById("ghTreeOutputOuter");
            treeLoading = document.getElementById("ghTreeLoading");
            classExclusionTreeOuterDiv = document.getElementById("ghClassExclusionTreeOuter");
            classExLoading = document.getElementById("ghClassExLoading");
            
            hideMethodsOption = document.getElementById("gHideMethods");
            hidePropertiesOption = document.getElementById("gHideProperties");
            
            inputHistory = document.getElementById("ghInputHistory");
            
            //Detect if IE
            if((window.navigator.userAgent.indexOf("MSIE ") != -1) || 
                (window.navigator.userAgent.indexOf("Trident/") != -1)) {
                    ClassDiagramFilter.SetTab(0);
                    tabDiv.innerHTML = "<p>Warning: IE does not work with Github authentication" +
                        " starting July 2018.</p>";
            }
        };
    }
    
    return new CGithubInterface();
})();
