var ClassDiagramFilter = {};

var ClassDiagramFilter = {
    tabObjs: [],
    acceptedExtensionArray: [".adb", ".ads", ".Ada", ".asp", ".asa", ".aspx", ".bet",
        ".cbl", ".cob", ".CBL", ".COB", ".d", ".di", ".e", ".fal", ".ftd", ".f", ".for",
        ".ftn", ".f03", ".f08", ".f15", ".go", ".js", ".jsx", ".cl", ".clisp", ".el",
        ".l", ".lisp", ".lsp", ".lua", ".p", ".pp", ".pas", ".pl", ".pm", ".ph", ".plx",
        ".perl", ".p6", ".pm6", ".pl6", ".php", ".php4", ".php5", ".php7", ".phtml",
        ".py", ".pyx", ".pxd", ".pxi", ".scons", ".r", ".R", ".s", ".q", ".rb", ".ruby",
        ".rs", ".tcl", ".tk", ".wish", ".exp", ".tex", ".cs", ".c++", ".cc", ".cp",
        ".cpp", ".cxx", ".h", ".h++", ".hh", ".hpp", ".hxx", ".m", ".mm", ".inl", ".java"],
    maxBlockSize: 65000,
    submitBtn: null,
    divBelowAuthentication: null,

    tab: 1,
    documentLoaded: false
};


//ClassDiagramFilter.Globals = {
//    /** @type {Array<{SetTabVisibility(visible: boolean): void}>} */
//    tabObjs: [],
//    maxBlockSize: 65000,
//    tab: 1,
//    documentLoaded: false,
//
//    acceptedExtensionArray: [".adb", ".ads", ".Ada", ".asp", ".asa", ".aspx", ".bet",
//        ".cbl", ".cob", ".CBL", ".COB", ".d", ".di", ".e", ".fal", ".ftd", ".f", ".for",
//        ".ftn", ".f03", ".f08", ".f15", ".go", ".js", ".jsx", ".cl", ".clisp", ".el",
//        ".l", ".lisp", ".lsp", ".lua", ".p", ".pp", ".pas", ".pl", ".pm", ".ph", ".plx",
//        ".perl", ".p6", ".pm6", ".pl6", ".php", ".php4", ".php5", ".php7", ".phtml",
//        ".py", ".pyx", ".pxd", ".pxi", ".scons", ".r", ".R", ".s", ".q", ".rb", ".ruby",
//        ".rs", ".tcl", ".tk", ".wish", ".exp", ".tex", ".cs", ".c++", ".cc", ".cp",
//        ".cpp", ".cxx", ".h", ".h++", ".hh", ".hpp", ".hxx", ".m", ".mm", ".inl", ".java"]
//};

//ClassDiagramFilter.DOMElements = {
//    /** @type {HTMLButtonElement} */
//    submitBtn: null,
//    /** @type {HTMLDivElement} */
//    divBelowAuthentication: null
//};

ClassDiagramFilter.GH = {};

ClassDiagramFilter.GH = {
    apiLocation: "https://api.github.com/",
    appID: "<Your App ID>",
    apiCallback: "",
    apiCallbackLoc: "githubauthenticated.html",
    userKey: "",

    tabDiv: null,

    userInput: null,
    repoInput: null,
    branchInput: null,
    listFilesBtn: null,
    excludeClassBtn: null,
    output: null,

    treeOutputDiv: null,
    classExcludeOutputDiv: null,
    classExcludeCheckAllBtn: null,
    classExcludeUncheckAllBtn: null,

    treeOutputOuterDiv: null,
    treeLoading: null,
    classExclusionTreeOuterDiv: null,
    classExLoading: null,

    isSubmitting: false,
    isBypassingListFiles: false,
    isExcludingClasses: false,

    authentication: null,
    classExclude: null,
    isAuthenticated: false,

    inputHistory: null
};



ClassDiagramFilter.LF = {};

ClassDiagramFilter.LF = {
    isSubmitting: false,
    output: null,

    fileList: null,
    excludeClassBtn: null,
    fileNameText: null,

    isExcludingClasses: false,
    classExcludeOutputDiv: null,
    classExclude: null,

    classExclusionTreeOuterDiv: null,
    classExLoading: null,

    classExcludeCheckAllBtn: null,
    classExcludeUncheckAllBtn: null,

    tabDiv: null
};



ClassDiagramFilter.GH.GithubImporter = function() {
    this.tree = null;
    this.user = "";
    this.repo = "";
    this.branch = "";
    this.guid = "";

    this.exclusionBlob = null;
    this.amountSent = 0;

    this.GetRepositoryTree = function(user, repo, commit) {

    };

    this.BypassListSendSubmission = function(rtree) {
        var outStr = "";


        this.amountSent = 0;
        this.guid = MakeGuid();
        this.exclusionBlob = new Uint16Array(outStr.length);

        for(var i = 0; i < outStr.length; i++) {
            this.exclusionBlob[i] = outStr.charCodeAt(i);
        }

        ClassDiagramFilter.GH.SetIsExcludingClasses(true);

        this.SendExclusionBlock();
    };

    this.BypassListFiles = function(url) {
        ClassDiagramFilter.GH.isBypassingListFiles = false;

        var request = {
            type: "GET",
            url: url + "?recursive=1",
            success: function(data, status, x) {
                this.BypassListSendSubmission(data.tree);
            }.bind(this),
            error: function(data, status, x) {
                console.log("test BypassListFiles error");
                ClassDiagramFilter.GH.SetClassExLoadingState(false);
            }.bind(this)
        };

        if(ClassDiagramFilter.GH.userKey.length != 0) {
            request.url += "&access_token=" + ClassDiagramFilter.GH.userKey;
        }

        $.ajax(request);
    };

    this.GetRepositoryTreeUrl = function(url) {
        var request = {
            type: "GET",
            url: url + "?recursive=1",
            success: function(data, status, x) {
                console.log("test GetRepositoryTreeUrl success");
                if(this.tree) {
                    this.tree.Destroy();
                }

                this.tree = new ClassDiagramFilter.GH.RepositoryTree(data.tree);
                ClassDiagramFilter.GH.SetSubmitting(false);
            }.bind(this),
            error: function(data, status, x) {
                console.log("test GetRepositoryTreeUrl error");
                ClassDiagramFilter.GH.SetTreeLoadingState(false);
            }.bind(this)
        };

        if(ClassDiagramFilter.GH.userKey.length != 0) {
            request.url += "&access_token=" + ClassDiagramFilter.GH.userKey;
        }

        $.ajax(request);
    };

    this.GetRepositoryTreeByFirstCommit = function(user, repo, branch) {
        var url = ClassDiagramFilter.GH.apiLocation + "repos/" + user + "/" + repo + "/branches/" + branch;
        this.user = user;
        this.repo = repo;
        this.branch = branch;

        if(ClassDiagramFilter.GH.userKey.length != 0) {
            url += "?access_token=" + ClassDiagramFilter.GH.userKey;
        }

        if(ClassDiagramFilter.GH.isBypassingListFiles) {
            ClassDiagramFilter.GH.SetClassExLoadingState(true);
        }
        else {
            ClassDiagramFilter.GH.SetTreeLoadingState(true);
        }

        var request = {
            type: "GET",
            url: url,
            success: function(data, status, x) {
                console.log("test GetMostRecentCommit success");

                if(ClassDiagramFilter.GH.authentication) {
                    ClassDiagramFilter.GH.authentication.AddToInputHistory(
                        this.user, this.repo, this.branch);
                }

                if(!ClassDiagramFilter.GH.isBypassingListFiles) {
                    this.GetRepositoryTreeUrl(data.commit.commit.tree.url);
                }
                else {
                    this.BypassListFiles(url);
                }
            }.bind(this),
            error: function(data, status, x) {
                console.log("test GetMostRecentCommit error");
                ClassDiagramFilter.GH.output.innerText = "Error: Could not access repository";
                ClassDiagramFilter.GH.isBypassingListFiles = false;

                if(ClassDiagramFilter.GH.isBypassingListFiles) {
                    ClassDiagramFilter.GH.SetClassExLoadingState(false);
                }
                else {
                    ClassDiagramFilter.GH.SetTreeLoadingState(false);
                }
            }.bind(this)
        };

        $.ajax(request);
    };

    this.SubmitRequest = function() {
        var exclusionStr = "";
        this.amountSent = 0;
        this.guid = MakeGuid();

        if(ClassDiagramFilter.GH.isExcludingClasses) {
            ClassDiagramFilter.GH.SetClassExLoadingState(true);
        }

        this.tree.GenerateExclusionList();
        exclusionStr = this.tree.exclusionList.join("\n");
        this.exclusionBlob = new Uint16Array(exclusionStr.length);

        for(var i = 0; i < exclusionStr.length; i++) {
            this.exclusionBlob[i] = exclusionStr.charCodeAt(i);
        }

        this.SendExclusionBlock();
    };

    this.SendExclusionBlock = function() {
        var amountToSend = this.exclusionBlob.length - this.amountSent;
        if((amountToSend * 2) > ClassDiagramFilter.maxBlockSize) {
            amountToSend = ClassDiagramFilter.maxBlockSize >> 1;
        }

        var request = {
            type: "POST",
            url: "filter/ctags-github-block?uploadID=" + this.guid,
            data: this.exclusionBlob.subarray(this.amountSent, this.amountSent + amountToSend),
            contentType: "text/plain",
            success: function(data, status) {
                if(this.amountSent >= this.exclusionBlob.length) {
                    this.SendFinalRequest();
                }
                else {
                    this.SendExclusionBlock();
                }
            }.bind(this),
            error: function(data, status) {
                ClassDiagramFilter.GH.output.innerText = "Server returned with error code: " +
                    ((!data.responseJSON) ? data.responseText : data.responseJSON);

                ClassDiagramFilter.GH.SetIsExcludingClasses(false);
                ClassDiagramFilter.GH.SetSubmitting(false);
                ClassDiagramFilter.GH.SetClassExLoadingState(false);
            }.bind(this),
            processData: false
        };

        this.amountSent += amountToSend;
        $.ajax(request);
    };

    this.SendFinalRequest = function() {
        var request = {
            type: "POST",
            url: "filter/ctags-github-final?uploadID=" + this.guid +
                "&user=" + this.user + "&repo=" + this.repo + "&branch=" + this.branch +
                "&userID=" + ClassDiagramFilter.GH.userKey,
            success: function(data, status) {
                if(!ClassDiagramFilter.GH.isExcludingClasses) {
                    ClassDiagramFilter.GH.output.innerText = "Success!  Please wait while you are redirected.";
                    ClassDiagramFilter.GH.output.innerText += "\n" + data;
                    SDAPI.VS.VisualScriptDone(data, function(data, error) {    //SDone
                        SDAPI.VS.CloseDialog();
                        ClassDiagramFilter.GH.output.innerText = "This shouldn't display";
                    });
                }
                else {
                    ClassDiagramFilter.GH.SetSubmitting(false);

                    if(ClassDiagramFilter.GH.classExclude) {
                        ClassDiagramFilter.GH.classExclude.Destroy();
                    }

                    ClassDiagramFilter.GH.classExclude = new ClassDiagramFilter.ClassExclusion.ClassExclusionTree();
                    ClassDiagramFilter.GH.classExclude.init(JSON.parse(data), "ghClassExclusionTree",
                    ClassDiagramFilter.GH.SetClassExLoadingState,
                    ClassDiagramFilter.GH.classExcludeCheckAllBtn, ClassDiagramFilter.GH.classExcludeUncheckAllBtn);
                }
            },
            error: function(data, status) {
                ClassDiagramFilter.GH.output.innerText = "Server returned with error code: " +
                    ((!data.responseJSON) ? data.responseText : data.responseJSON);

                ClassDiagramFilter.GH.SetIsExcludingClasses(false);
                ClassDiagramFilter.GH.SetSubmitting(false);
                ClassDiagramFilter.GH.SetClassExLoadingState(false);
            }
        };

        $.ajax(request);
    };
};

ClassDiagramFilter.GH.RepositoryTree = function(rtree) {
    this.jstree = null;
    this.pathList = [];
    this.exclusionList = [];

    this.undetermined = [];
    this.model = [];

    this.jstreeStruct = {
        core: {
            data: [
                {
                    text: "repository",
                    state: { opened: true },
                    children: []
                }
            ]
        },
        plugins: [
            "checkbox"
        ]
    };

    this.init = function(rtree) {
        for(var i = 0; i < rtree.length; i++) {
            this.InsertNode(rtree[i]);
        }

        var jqtree = $("#ghTreeOutput").jstree(this.jstreeStruct);
        jqtree.on("ready.jstree", function() {
            ClassDiagramFilter.GH.SetTreeLoadingState(false);

            if(ClassDiagramFilter.GH.checkAllBtn) {
                checkAllBtn.disabled = false;
                checkAllBtn.classList.remove("plugin-button-disabled");
            }
            if(ClassDiagramFilter.GH.uncheckAllBtn) {
                uncheckAllBtn.disabled = false;
                uncheckAllBtn.classList.remove("plugin-button-disabled");
            }
        });

        this.jstree = jqtree.jstree();
    };

    this.Destroy = function() {
        if(this.jstree) {
            this.jstree.destroy();
        }
    };

    this.InsertNode = function(node) {
        var strSplit = node.path.split("/");
        var currNode = this.jstreeStruct.core.data[0];
        var tempNode = currNode;
        var i;

        for(i = 0; i < strSplit.length; i++) {
            currNode = tempNode;
            tempNode = this.FindNode(strSplit[i], currNode);

            if(!currNode.children) {
                currNode.children = [];
            }
            if(tempNode == null) {
                currNode.children.push({text: strSplit[i]});
                tempNode = currNode.children[currNode.children.length - 1];
            }
        }

        if(node.type == "tree") {
            return;
        }

        if((i >= 1) && tempNode && (strSplit[0] != "docs")) {
            var tempExt = strSplit[i - 1].split(".");

            if((tempExt.length > 1) && (ClassDiagramFilter.acceptedExtensionArray.indexOf("." +
                    tempExt[tempExt.length - 1]) != -1)) {
                tempNode.state = {selected: true};
            }
        }
    };

    this.FindNode = function(name, searchIndex) {
        if(!searchIndex.children) {
            return null;
        }

        for(var i = 0; i < searchIndex.children.length; i++) {
            if(searchIndex.children[i].text == name) {
                return searchIndex.children[i];
            }
        }

        return null;
    };

    this.GenerateExclusionList = function() {
        this.exclusionList = [];
        this.undetermined = this.jstree.get_undetermined();
        this.model = this.jstree._model.data;

        this.GenerateExclusionFromNode("#");
    };

    this.GenerateExclusionFromNode = function(nodeID) {
        var node = this.model[nodeID];
        var child = null;

        if(!node || !node.children) {
            return;
        }

        for(var i = 0; i < node.children.length; i++) {
            child = this.model[node.children[i]];

            if(child && !child.state.selected) {
                if(this.undetermined.indexOf(child.id) != -1) {
                    this.GenerateExclusionFromNode(node.children[i]);
                }
                else {
                    this.GenerateExclusionEntry(child);
                }
            }
        }
    };

    this.GenerateExclusionEntry = function(node) {
        var entryName = node.text;
        var parents = node.parents;

        for(var i = 0; i < parents.length; i++) {
            if(parents[i] == "#") {
                break;
            }

            entryName = this.model[parents[i]].text + "/" + entryName;
        }

        this.exclusionList.push(entryName);
    };

    this.init(rtree);
};

ClassDiagramFilter.GH.RepositoryTreeBypass = function() {
    this.user = "";
    this.repo = "";
    this.branch = "";
    this.guid = "";

    this.exclusionBlob = null;
    this.amountSent = 0;


};

ClassDiagramFilter.GH.Authentication = function() {
    this.state = MakeGuid().replace(/-/g, "_");
    this.userid = "";
    this.interval = null;
    //--temp-- implement local settings so that it can be synched locally
    //  even if you are not properly hooked up
    this.localSettings = {};

    this.BeginAuthentication = function() {
        var url = "https://www.github.com/login/oauth/authorize" +
            "?client_id=" + ClassDiagramFilter.GH.appID + "&state=" + this.state +
            "&redirect_uri=" + encodeURI(ClassDiagramFilter.GH.apiCallback);
        var popup = window.open('', null, "width=800,height=600");

        if(!popup) {
            ClassDiagramFilter.GH.output.innerText = "Could not open window.  " +
                "If a popup blocker is present, please disable it for the filter's site.";
            return;
        }
        else {
            popup.location = url;
        }

        window.addEventListener("message", function(e) {
            if(e.origin != (window.location.origin)) {
                return;
            }

            e.source.postMessage(true, e.origin);

            var wasHidden = ClassDiagramFilter.GH.tabDiv.hidden;

            ClassDiagramFilter.GH.userKey = e.data;
            ClassDiagramFilter.GH.SetTabVisibility(false);

            ClassDiagramFilter.GH.tabDiv = document.getElementById("githubTab");
            ClassDiagramFilter.GH.output = document.getElementById("ghOutput");
            ClassDiagramFilter.GH.isAuthenticated = true;
            ClassDiagramFilter.GH.SetTabVisibility(!wasHidden);

            SDAPI.VS.GetUserSettings(function(e, response, err) {
                var settings = response ? response.Payload : null;

                if(err) {
                    ClassDiagramFilter.GH.output.innerText = "Error getting settings: " + err;
                    return;
                }

                var newSettings = settings ? JSON.parse(settings) : {};
                newSettings.userID = e.data;

                SDAPI.VS.SetUserSettings(JSON.stringify(newSettings), function(success, err) {
                    if(!success) {
                        ClassDiagramFilter.GH.output.innerText = "Error setting settings: " + err;
                    }
                });
            }.bind(this, e));
        });
    };

    this.VerifySavedAuthentication = function() {
        SDAPI.VS.GetUserSettings(function(response, err) {
            var settings = response ? response.Payload : null;

            if(!err && settings) {
                var s = JSON.parse(settings);

                if(!s || !s.userID) {
                    return;
                }

                var url = ClassDiagramFilter.GH.apiLocation +
                    "rate_limit?access_token=" + s.userID;

                var request = {
                    type: "GET",
                    url: url,
                    success: function(settings, data, status) {
                        if(data && (typeof data == "object") && data.rate &&
                            data.rate.limit && (data.rate.limit > 60)) {

                            var wasHidden = ClassDiagramFilter.GH.tabDiv.hidden;

                            ClassDiagramFilter.GH.userKey = settings.userID;
                            ClassDiagramFilter.GH.SetTabVisibility(false);

                            ClassDiagramFilter.GH.tabDiv = document.getElementById("githubTab");
                            ClassDiagramFilter.GH.output = document.getElementById("ghOutput");
                            ClassDiagramFilter.GH.isAuthenticated = true;
                            ClassDiagramFilter.GH.SetTabVisibility(!wasHidden);

                            this.SetInputHistory(settings);
                        }
                    }.bind(this, s)
                };

                $.ajax(request);
            }
        }.bind(this));
    };

    this.SetInputHistory = function(settings) {
        var history = settings.history;

        if(!history || !Array.isArray(history)) {
            return;
        }

        var options = "<option>Select a repository from history...</option>";
        var value;
        var users = [];
        var hadUser;
        var i, j;

        for(i = 0; i < history.length; i++) {
            if(!history[i].user || !history[i].repo || !history[i].branch ||
                (typeof history[i].user != "string")) {
                continue;
            }

            hadUser = false;

            for(j = 0; j < users.length; j++) {
                if(users[j].name.toLowerCase() == history[i].user.toLowerCase()) {
                    users[j].repos.push({repo: history[i].repo, branch: history[i].branch});
                    hadUser = true;
                    break;
                }
            }

            if(!hadUser) {
                users.push({
                    name: history[i].user,
                    repos: [{repo: history[i].repo, branch: history[i].branch}]
                });
            }
        }

        if(users.length == 0) {
            return;
        }

        for(i = 0; i < users.length; i++) {
            options += "\n<optgroup label=\"" + users[i].name + "\">";

            for(j = 0; j < users[i].repos.length; j++) {
                value = encodeURI(users[i].name) + " " +
                    encodeURI(users[i].repos[j].repo) + " " +
                    encodeURI(users[i].repos[j].branch);

                options += "\n<option value=\"" + value + "\">" +
                    users[i].repos[j].repo + " (" + users[i].repos[j].branch +
                    ")</option>";
            }

            options += "\n</optgroup>";
        }

        ClassDiagramFilter.GH.inputHistory.innerHTML = options;
    };

    this.AddToInputHistory = function(user, repo, branch) {
        SDAPI.VS.GetUserSettings(function(input, response, err) {
            var settings = response ? response.Payload : null;

            if(err) {
                return;
            }

            var newSettings = settings ? JSON.parse(settings) : {};
            if(!newSettings.history || !Array.isArray(newSettings.history)) {
                newSettings.history = [input];
            }
            else {
                var hasSetting = false;

                for(var i = 0; i < newSettings.history.length; i++) {
                    //A whole ton of comparisons to check equivalence of history entries
                    if((typeof newSettings.history[i].user == "string") &&
                        (typeof newSettings.history[i].repo == "string") &&
                        (typeof newSettings.history[i].branch == "string") &&
                        (newSettings.history[i].user.toLowerCase() == input.user.toLowerCase()) &&
                        (newSettings.history[i].repo.toLowerCase() == input.repo.toLowerCase()) &&
                        (newSettings.history[i].branch.toLowerCase() == input.branch.toLowerCase())) {

                        hasSetting = true;
                        break;
                    }
                }

                if(!hasSetting) {
                    newSettings.history.unshift(input);

                    if(newSettings.history.length > 10) {
                        newSettings.history.splice(10, newSettings.history.length - 10);
                    }
                }
            }

            this.SetInputHistory(newSettings);

            SDAPI.VS.SetUserSettings(JSON.stringify(newSettings), function(success, err) {
                //either succeeded or failed to set settings
            });
        }.bind(this, {user: user, repo: repo, branch: branch}));
    };
};

/** @type {ClassDiagramFilter.GH.GithubImporter} */
ClassDiagramFilter.GH.importer = new ClassDiagramFilter.GH.GithubImporter();

ClassDiagramFilter.LF.ClassUploader = function() {
    this.files = [];
    this.buffs = [];
    this.names = [];
    this.lengths = [];

    this.data = null;
    this.guid = MakeGuid();

    this.totalLength = 0;
    this.amountSent = 0;

    this.Upload = function() {
        if(!this.files || (this.files.length <= 0)) {
            ClassDiagramFilter.LF.output.innerText = "Please select files for uploading.";
            return;
        }

        ClassDiagramFilter.LF.SetClassExLoadingState(true);

        var reader = new FileReader();
        reader.onloadend = this.HandleRead.bind(this);
        reader.readAsArrayBuffer(this.files[0]);
    };

    this.HandleRead = function(e) {
        this.names.push(this.files[this.buffs.length].name);
        this.buffs.push(new Uint8Array(e.target.result));
        this.lengths.push(e.target.result.byteLength);

        if(this.buffs.length < this.files.length) {
            var reader = new FileReader();
            reader.onloadend = this.HandleRead.bind(this);
            reader.readAsArrayBuffer(this.files[this.buffs.length]);
        }
        else {
            this.InitializeTransfer();
        }
    };

    this.InitializeTransfer = function() {
        this.CreateData();
        var dataLength = (this.totalLength > ClassDiagramFilter.maxBlockSize) ?
            ClassDiagramFilter.maxBlockSize : this.totalLength;
        var content = new Blob(this.data.subarray(0, dataLength));

        var request = {
            type: "POST",
            url: "filter/ctags-block-init?uploadID=" + this.guid,
            data: this.data.subarray(0, dataLength),
            contentType: "text/plain",
            success: function(data, status) {
                if(this.amountSent >= this.totalLength) {
                    this.FinalizeBlobTransfer();
                }
                else {
                    this.SendAdditionalBlob();
                }
            }.bind(this),
            error: function(data, status) {
                ClassDiagramFilter.LF.output.innerText = "Server returned with error code: " +
                    ((data.responseText) ? data.responseText : data.statusText);

                ClassDiagramFilter.LF.SetIsExcludingClasses(false);
                ClassDiagramFilter.LF.SetSubmitting(false);
                ClassDiagramFilter.LF.SetClassExLoadingState(true);
            }.bind(this),
            processData: false
        };

        this.amountSent += dataLength;
        $.ajax(request);
    };

    this.SendAdditionalBlob = function() {
        var dataLength = ((this.totalLength - this.amountSent) > ClassDiagramFilter.maxBlockSize) ?
            ClassDiagramFilter.maxBlockSize : this.totalLength - this.amountSent;
        var content = new Blob(this.data.subarray(this.amountSent, dataLength + this.amountSent));

        var request = {
            type: "POST",
            url: "filter/ctags-block-add?uploadID=" + this.guid,
            data: this.data.subarray(this.amountSent, dataLength + this.amountSent),
            contentType: "text/plain",
            success: function(data, status) {
                if(this.amountSent >= this.totalLength) {
                    this.FinalizeBlobTransfer();
                }
                else {
                    this.SendAdditionalBlob();
                }
            }.bind(this),
            error: function(data, status) {
                ClassDiagramFilter.LF.output.innerText = "Server returned with error code: " +
                    ((data.responseText) ? data.responseText : data.statusText);

                ClassDiagramFilter.LF.SetIsExcludingClasses(false);
                ClassDiagramFilter.LF.SetSubmitting(false);
                ClassDiagramFilter.LF.SetClassExLoadingState(true);
            }.bind(this),
            processData: false
        };

        this.amountSent += dataLength;
        $.ajax(request);
    };

    this.FinalizeBlobTransfer = function() {
        var request = {
            type: "POST",
            url: "filter/ctags-block-final?uploadID=" + this.guid,
            success: function(data, status) {
                if(!ClassDiagramFilter.LF.isExcludingClasses) {
                    ClassDiagramFilter.LF.output.innerText = "Success!  Please wait while you are redirected.";
                    ClassDiagramFilter.LF.output.innerText += "\n" + data;
                    SDAPI.VS.VisualScriptDone(data, function(data, error) {    //SDone
                        SDAPI.VS.CloseDialog();
                        ClassDiagramFilter.LF.output.innerText = "This shouldn't display";
                    });
                }
                else {
                    ClassDiagramFilter.LF.SetSubmitting(false);

                    if(ClassDiagramFilter.LF.classExclude) {
                        ClassDiagramFilter.LF.classExclude.Destroy();
                    }

                    ClassDiagramFilter.LF.classExclude = new ClassDiagramFilter.ClassExclusion.ClassExclusionTree();
                    ClassDiagramFilter.LF.classExclude.init(JSON.parse(data), "lfClassExclusionTree",
                        ClassDiagramFilter.LF.SetClassExLoadingState,
                        ClassDiagramFilter.LF.classExcludeCheckAllBtn, ClassDiagramFilter.LF.classExcludeUncheckAllBtn);
                }
            },
            error: function(data, status) {
                ClassDiagramFilter.LF.output.innerText = "Server returned with error code: " +
                    ((data.responseText) ? data.responseText : data.statusText);

                ClassDiagramFilter.LF.SetIsExcludingClasses(false);
                ClassDiagramFilter.LF.SetSubmitting(false);
                ClassDiagramFilter.LF.SetClassExLoadingState(true);
            }
        };

        $.ajax(request);
    };

    this.CreateData = function() {
        var headerString = this.files.length.toString() + "\t" + this.names.join("\t") + "\n" +
            this.lengths.join("\t") + "\n";
        this.totalLength = headerString.length << 1;
        var byteOffset = this.totalLength;
        var i = 0;

        for(i = 0; i < this.lengths.length; i++) {
            this.totalLength += this.lengths[i];
        }

        this.data = new Uint8Array(this.totalLength);
        var headerArray = new Uint16Array(this.data.buffer);

        for(i = 0; i < headerString.length; i++) {
            headerArray[i] = headerString.charCodeAt(i);
        }

        for(i = 0; i < this.buffs.length; i++) {
            this.data.set(this.buffs[i], byteOffset);
            byteOffset += this.buffs[i].byteLength;
        }
    };
};

ClassDiagramFilter.ClassExclusion = {};

ClassDiagramFilter.ClassExclusion.Globals = {
    iterationID: 0
};

ClassDiagramFilter.ClassExclusion.ClassExclusionList = function() {
    this.dataSource = "";
    this.tree = null;

};

ClassDiagramFilter.ClassExclusion.ClassExclusionTree = function() {
    this.jstree = null;
    this.sdon = {};
    this.finalsdon = {};
    this.iteration = 0;
    this.nodeID = 0;

    this.nodeArray = [];
    this.undetermined = [];
    this.model = {};

    this.jstreeStruct = {
        core: {
            data: [
                {
                    id: "CEid" + ClassDiagramFilter.ClassExclusion.iterationID + "#",
                    text: "Class List",
                    state: { opened: true },
                    children: []
                }
            ]
        },
        plugins: [
            "checkbox"
        ]
    };

    this.init = function(sdon, id, loadFn, checkAllBtn, uncheckAllBtn) {
        if(!(sdon && (typeof sdon.RootShape == "object") && Array.isArray(sdon.RootShape) && (sdon.RootShape.length == 1))) {
            return;
        }

        this.iteration = ClassDiagramFilter.ClassExclusion.iterationID;
        ClassDiagramFilter.ClassExclusion.iterationID++;
        this.nodeID = 0;
        var root = this.jstreeStruct.core.data[0];
        this.sdon = sdon;

        if(sdon.RootShape[0]) {
            this.SearchShapeArrayRowForClass(sdon.RootShape[0], root);
        }

        var tree = $("#" + id).jstree(this.jstreeStruct);
        //todo: implement all items that go in this thing --temp--
        tree.on("ready.jstree", function(loadFn, checkAllBtn, uncheckAllBtn) {
            loadFn(false);

            if(checkAllBtn) {
                checkAllBtn.disabled = false;
                checkAllBtn.classList.remove("plugin-button-disabled");
            }
            if(uncheckAllBtn) {
                uncheckAllBtn.disabled = false;
                uncheckAllBtn.classList.remove("plugin-button-disabled");
            }
        }.bind(this, loadFn, checkAllBtn, uncheckAllBtn));

        this.jstree = tree.jstree();
    };

    this.Destroy = function() {
        if(this.jstree) {
            this.jstree.destroy();
        }
    };

    this.SearchShapeArrayRowForClass = function(shape, parent) {
        if(!shape.ShapeArray || !shape.ShapeArray.Shapes) {
            return;
        }

        var childShape;

        for(var i = 0; i < shape.ShapeArray.Shapes.length; i++) {
            childShape = shape.ShapeArray.Shapes[i];

            this.SearchShapeArrayColumnForClass(childShape, parent, shape);
        }
    };

    this.SearchShapeArrayColumnForClass = function(shape, parent, parentRow) {
        if(!shape || !shape.ShapeArray || !shape.ShapeArray.Shapes) {
            return;
        }

        var childNode;

        if(shape.ShapeArray.Shapes.length == 1) {
            this.InsertNodeToTree(shape.ShapeArray.Shapes[0], parent, parentRow, false);
        }
        else if(shape.ShapeArray.Shapes.length == 2) {
            childNode = this.InsertNodeToTree(shape.ShapeArray.Shapes[0], parent, parentRow, true);

            this.SearchShapeArrayRowForClass(shape.ShapeArray.Shapes[1], childNode, childNode);
        }
    };

    this.InsertNodeToTree = function(shape, parent, parentRow, hasChildren) {
        var node;
        var parentId = this.GetIndexFromID(parent.id);

        if(!shape || !shape.Table) {
            return null;
        }

        if(!shape.Table.Cells && shape.Table.Cell) {
            //checking for use of deprecated "Cell" array.
            shape.Table.Cells = shape.Table.Cell;
        }

        if(!shape.Table.Cells || !(shape.Table.Cells.length > 0)) {
            return null;
        }

        if(!hasChildren) {
            node = {
                id: "CEid" + this.iteration + "-" + this.nodeID,
                text: shape.Table.Cells[0].Label,
                state: { selected: true }
            };
        }
        else {
            node = {
                id: "CEid" + this.iteration + "-" + this.nodeID,
                text: shape.Table.Cells[0].Label,
                state: { opened: true, selected: true },
                children: []
            };
        }

        this.nodeArray.push({Shape: shape, ParentId: parentId, ParentRow: parentRow, checked: false});
        parent.children.push(node);
        shape.filter_intern_id = node.id;
        this.nodeID++;

        return node;
    };

    this.ApplyExclusionToSDON = function() {
        this.undetermined = this.jstree.get_undetermined();
        this.model = this.jstree._model.data;
        var i, id, index, keys;

        for(i = 0; i < this.undetermined.length; i++) {
            index = this.GetIndexFromID(this.undetermined[i]);

            if(index > -1) {
                this.nodeArray[index].checked = true;
            }
        }

        this.CheckAllNodesInTree("#");

        for(i = 1; i < this.nodeArray.length; i++) {
            if(!this.nodeArray[i].checked) {
                this.RemoveEntry(this.nodeArray[i]);
            }
        }

        return JSON.stringify(this.sdon);
    };

    this.CheckAllNodesInTree = function(node) {
        var index;
        var val = this.model[node];

        if((node != "#") && val.state.selected) {
            index = this.GetIndexFromID(node);

            if(index > -1) {
                this.nodeArray[index].checked = true;
            }
        }

        for(var i = 0; i < val.children.length; i++) {
            this.CheckAllNodesInTree(val.children[i]);
        }
    };

    this.RemoveEntry = function(entry) {
        var parent = (entry.ParentId > -1) ? this.nodeArray[entry.ParentId] : null;
        var parentRow = entry.ParentRow;
        var testShape;

        if(!parentRow) {
            return;
        }

        for(var i = 0; i < parentRow.ShapeArray.Shapes.length; i++) {
            testShape = parentRow.ShapeArray.Shapes[i];

            if(testShape.ShapeArray && testShape.ShapeArray.Shapes &&
                ((testShape.ShapeArray.Shapes.length == 1) ||
                (testShape.ShapeArray.Shapes.length == 2))) {

                if(testShape.ShapeArray.Shapes[0].filter_intern_id == entry.Shape.filter_intern_id) {
                    parentRow.ShapeArray.Shapes.splice(i, 1);

                    if(parentRow.ShapeArray.Shapes.length == 0) {
                        this.RemoveEntry(parent);
                    }
                }
            }
        }
    };

    this.GetIndexFromID = function(id) {
        var i = id.indexOf('-');
        if(i == -1) {
            return -1;
        }

        return Number.parseInt(id.substring(i + 1));
    };
};

ClassDiagramFilter.GH.Authenticate = function() {
    if(!ClassDiagramFilter.documentLoaded) {
        return;
    }

    ClassDiagramFilter.GH.authentication = new ClassDiagramFilter.GH.Authentication();
    ClassDiagramFilter.GH.authentication.BeginAuthentication();
};

ClassDiagramFilter.GH.ListFiles = function() {
    if(!ClassDiagramFilter.documentLoaded) {
        return;
    }

    if(!ClassDiagramFilter.GH.isSubmitting) {
        ClassDiagramFilter.GH.SetSubmitting(true);
        ClassDiagramFilter.GH.SetIsExcludingClasses(false);

        ClassDiagramFilter.GH.listFilesBtn.classList.remove("plugin-button-ready");

        var user = ClassDiagramFilter.GH.userInput.value;
        var repo = ClassDiagramFilter.GH.repoInput.value;
        var branch = ClassDiagramFilter.GH.branchInput.value;

        ClassDiagramFilter.GH.importer.GetRepositoryTreeByFirstCommit(user, repo, branch);
    }
};

ClassDiagramFilter.GH.ExcludeByClass = function() {
    if(!ClassDiagramFilter.documentLoaded) {
        return;
    }

    if(!ClassDiagramFilter.GH.isSubmitting && ClassDiagramFilter.GH.userKey) {
        ClassDiagramFilter.GH.SetSubmitting(true);
        ClassDiagramFilter.GH.SetIsExcludingClasses(true);

        ClassDiagramFilter.GH.importer.SubmitRequest();
    }
};

ClassDiagramFilter.GH.SubmitList = function() {
    if(!ClassDiagramFilter.documentLoaded) {
        return;
    }

    if(!ClassDiagramFilter.GH.isSubmitting && ClassDiagramFilter.GH.userKey) {
        ClassDiagramFilter.GH.SetSubmitting(true);

        if(!ClassDiagramFilter.GH.isExcludingClasses) {
            ClassDiagramFilter.GH.importer.SubmitRequest();
        }
        else {
            SDAPI.VS.VisualScriptDone(ClassDiagramFilter.GH.classExclude.ApplyExclusionToSDON(), function(data, error) {    //SDone
                SDAPI.VS.CloseDialog();
            });
        }
    }
};

ClassDiagramFilter.GH.ExcludeByClassBypass = function() {
    if(!ClassDiagramFilter.documentLoaded) {
        return;
    }

    if(!ClassDiagramFilter.GH.isSubmitting && ClassDiagramFilter.GH.userKey) {
        ClassDiagramFilter.GH.SetSubmitting(true);
        ClassDiagramFilter.GH.SetIsExcludingClasses(false);
        ClassDiagramFilter.GH.isBypassingListFiles = true;

        ClassDiagramFilter.GH.listFilesBtn.classList.remove("plugin-button-ready");

        var user = ClassDiagramFilter.GH.userInput.value;
        var repo = ClassDiagramFilter.GH.repoInput.value;
        var branch = ClassDiagramFilter.GH.branchInput.value;

        ClassDiagramFilter.GH.importer.GetRepositoryTreeByFirstCommit(user, repo, branch);
    }
};

/**
 *
 * @param {Event} e
 */
ClassDiagramFilter.GH.SelectFromInputHistory = function(e) {
    if(!ClassDiagramFilter.documentLoaded || !e.target.value) {
        return;
    }

    var valarr = e.target.value.split(" ");

    if(valarr.length != 3) {
        return;
    }

    ClassDiagramFilter.GH.userInput.value = decodeURI(valarr[0]);
    ClassDiagramFilter.GH.repoInput.value = decodeURI(valarr[1]);
    ClassDiagramFilter.GH.branchInput.value = decodeURI(valarr[2]);
};

ClassDiagramFilter.GH.SetSubmitting = function(submitting) {
    ClassDiagramFilter.GH.isSubmitting = submitting;
    ClassDiagramFilter.GH.listFilesBtn.disabled = submitting;
    ClassDiagramFilter.GH.excludeClassBtn.disabled = submitting;
    //ClassDiagramFilter.submitBtn.disabled = submitting; --temp--

    if(submitting) {
        ClassDiagramFilter.GH.listFilesBtn.classList.add("plugin-button-disabled");
        ClassDiagramFilter.GH.excludeClassBtn.classList.add("plugin-button-disabled");
    }
    else {
        ClassDiagramFilter.GH.listFilesBtn.classList.remove("plugin-button-disabled");
        ClassDiagramFilter.GH.excludeClassBtn.classList.remove("plugin-button-disabled");
    }
};

ClassDiagramFilter.GH.SetIsExcludingClasses = function(classes) {
    ClassDiagramFilter.GH.treeOutputOuterDiv.hidden = classes;
    ClassDiagramFilter.GH.classExclusionTreeOuterDiv.hidden = !classes;
    ClassDiagramFilter.GH.isExcludingClasses = classes;
    ClassDiagramFilter.GH.classExcludeCheckAllBtn.disabled = false;
    ClassDiagramFilter.GH.classExcludeUncheckAllBtn.disabled = false;

    ClassDiagramFilter.GH.classExcludeCheckAllBtn.classList.remove("plugin-button-disabled");
    ClassDiagramFilter.GH.classExcludeUncheckAllBtn.classList.remove("plugin-button-disabled");

    //if(classes) {
    //    ClassDiagramFilter.GH.classExcludeCheckAllBtn.classList.remove("plugin-button-disabled");
    //    ClassDiagramFilter.GH.classExcludeUncheckAllBtn.classList.remove("plugin-button-disabled");
    //}
    //else {
    //    ClassDiagramFilter.GH.classExcludeCheckAllBtn.classList.add("plugin-button-disabled");
    //    ClassDiagramFilter.GH.classExcludeUncheckAllBtn.classList.add("plugin-button-disabled");
    //}
};

ClassDiagramFilter.GH.SetTabVisibility = function(visible) {
    ClassDiagramFilter.GH.tabDiv.hidden = !visible;
    ClassDiagramFilter.GH.excludeClassBtn.hidden = !visible;
    ClassDiagramFilter.divBelowAuthentication.hidden = visible && !ClassDiagramFilter.GH.isAuthenticated;
    ClassDiagramFilter.GH.treeOutputOuterDiv.hidden = !visible || ClassDiagramFilter.GH.isExcludingClasses;
    ClassDiagramFilter.GH.listFilesBtn.hidden = !visible;
    ClassDiagramFilter.GH.classExclusionTreeOuterDiv.hidden = !ClassDiagramFilter.GH.isExcludingClasses || !visible;
    ClassDiagramFilter.GH.classExcludeCheckAllBtn.hidden = !visible;
    ClassDiagramFilter.GH.classExcludeUncheckAllBtn.hidden = !visible;
};

ClassDiagramFilter.GH.SetTreeLoadingState = function(loading) {
    ClassDiagramFilter.GH.treeOutputDiv.hidden = loading;

    if(loading) {
        ClassDiagramFilter.GH.treeLoading.classList.remove("loader-hidden");
    }
    else {
        ClassDiagramFilter.GH.treeLoading.classList.add("loader-hidden");
    }
};

ClassDiagramFilter.GH.SetClassExLoadingState = function(loading) {
    ClassDiagramFilter.GH.classExcludeOutputDiv.hidden = loading;

    if(loading) {
        ClassDiagramFilter.GH.classExLoading.classList.remove("loader-hidden");
    }
    else {
        ClassDiagramFilter.GH.classExLoading.classList.add("loader-hidden");
    }
};

ClassDiagramFilter.GH.SetChecked = function(checked) {
    if(ClassDiagramFilter.GH.isExcludingClasses && ClassDiagramFilter.GH.classExclude &&
        ClassDiagramFilter.GH.classExclude.jstree) {
        if(checked) {
            ClassDiagramFilter.GH.classExclude.jstree.check_all();
        }
        else {
            ClassDiagramFilter.GH.classExclude.jstree.uncheck_all();
        }
    }
    else if(!ClassDiagramFilter.GH.isExcludingClasses && ClassDiagramFilter.GH.importer &&
        ClassDiagramFilter.GH.importer.tree && ClassDiagramFilter.GH.importer.tree.jstree) {
        if(checked) {
            ClassDiagramFilter.GH.importer.tree.jstree.check_all();
        }
        else {
            ClassDiagramFilter.GH.importer.tree.jstree.uncheck_all();
        }
    }
};

ClassDiagramFilter.LF.Submit = function() {
    if(!ClassDiagramFilter.LF.isSubmitting) {
        ClassDiagramFilter.LF.SetSubmitting(true);

        if(!ClassDiagramFilter.LF.isExcludingClasses) {
            var uploader = new ClassDiagramFilter.LF.ClassUploader();
            uploader.files = ClassDiagramFilter.LF.fileList.files;
            uploader.Upload();
        }
        else {
            SDAPI.VS.VisualScriptDone(ClassDiagramFilter.LF.classExclude.ApplyExclusionToSDON(), function(data, error) {    //SDone
                SDAPI.VS.CloseDialog();
            });
        }
    }
};

ClassDiagramFilter.LF.SetSubmitting = function(submitting) {
    ClassDiagramFilter.LF.isSubmitting = submitting;
    //ClassDiagramFilter.submitBtn.disabled = submitting; --temp--
};

ClassDiagramFilter.LF.ChangeFiles = function() {
    if(!ClassDiagramFilter.documentLoaded) {
        return;
    }

    ClassDiagramFilter.LF.SetIsExcludingClasses(false);

    if(ClassDiagramFilter.LF.fileList.files && ClassDiagramFilter.LF.fileList.files.length) {
        ClassDiagramFilter.LF.excludeClassBtn.disabled = false;
        ClassDiagramFilter.LF.excludeClassBtn.classList.remove("plugin-button-disabled");

        var str = "";
        for(var i = 0; i < ClassDiagramFilter.LF.fileList.files.length; i++) {
            if(i) {
                str += ", ";
            }
            str += ClassDiagramFilter.LF.fileList.files[i].name;
        }

        if(str) {
            ClassDiagramFilter.LF.fileNameText.innerText = str;
        }
    }
    else {
        ClassDiagramFilter.LF.excludeClassBtn.disabled = true;
        ClassDiagramFilter.LF.excludeClassBtn.classList.add("plugin-button-disabled");
        ClassDiagramFilter.LF.fileNameText.innerText = "";
    }
};

ClassDiagramFilter.LF.ExcludeByClass = function() {
    if(!ClassDiagramFilter.documentLoaded) {
        return;
    }

    if(!ClassDiagramFilter.LF.isSubmitting) {
        ClassDiagramFilter.LF.SetSubmitting(true);
        ClassDiagramFilter.LF.SetIsExcludingClasses(true);

        var uploader = new ClassDiagramFilter.LF.ClassUploader();
        uploader.files = ClassDiagramFilter.LF.fileList.files;
        uploader.Upload();
    }
};

ClassDiagramFilter.LF.SetIsExcludingClasses = function(classes) {
    ClassDiagramFilter.LF.classExcludeOutputDiv.hidden = !classes;
    ClassDiagramFilter.LF.isExcludingClasses = classes;
    ClassDiagramFilter.LF.classExcludeCheckAllBtn.disabled = !classes;
    ClassDiagramFilter.LF.classExcludeUncheckAllBtn.disabled = !classes;

    if(classes) {
        ClassDiagramFilter.LF.classExcludeCheckAllBtn.classList.remove("plugin-button-disabled");
        ClassDiagramFilter.LF.classExcludeUncheckAllBtn.classList.remove("plugin-button-disabled");
    }
    else {
        ClassDiagramFilter.LF.classExcludeCheckAllBtn.classList.add("plugin-button-disabled");
        ClassDiagramFilter.LF.classExcludeUncheckAllBtn.classList.add("plugin-button-disabled");
    }
};

ClassDiagramFilter.LF.SetTabVisibility = function(visible) {
    ClassDiagramFilter.LF.tabDiv.hidden = !visible;
    ClassDiagramFilter.LF.excludeClassBtn.hidden = !visible;
    ClassDiagramFilter.LF.classExcludeCheckAllBtn.hidden = !visible;
    ClassDiagramFilter.LF.classExcludeUncheckAllBtn.hidden = !visible;
    ClassDiagramFilter.LF.classExclusionTreeOuterDiv.hidden = !visible;
};

ClassDiagramFilter.LF.SetClassExLoadingState = function(loading) {
    ClassDiagramFilter.LF.classExcludeOutputDiv.hidden = loading;

    if(loading) {
        ClassDiagramFilter.LF.classExLoading.classList.remove("loader-hidden");
    }
    else {
        ClassDiagramFilter.LF.classExLoading.classList.add("loader-hidden");
    }
};

ClassDiagramFilter.LF.SetChecked = function(checked) {
    if(ClassDiagramFilter.LF.classExclude && ClassDiagramFilter.LF.classExclude.jstree) {
        if(checked) {
            ClassDiagramFilter.LF.classExclude.jstree.check_all();
        }
        else {
            ClassDiagramFilter.LF.classExclude.jstree.uncheck_all();
        }
    }
};

ClassDiagramFilter.SetTab = function(tabNum) {
    if(!ClassDiagramFilter.documentLoaded) {
        return;
    }

    for(var i = 0; i < ClassDiagramFilter.tabObjs.length; i++) {
        if(i == tabNum) {
            ClassDiagramFilter.tabObjs[i].SetTabVisibility(true);
        }
        else {
            ClassDiagramFilter.tabObjs[i].SetTabVisibility(false);
        }
    }

    ClassDiagramFilter.tab = tabNum;
};

ClassDiagramFilter.Submit = function() {
    if(!ClassDiagramFilter.documentLoaded) {
        return;
    }

    switch(ClassDiagramFilter.tab) {
        case 0:
            ClassDiagramFilter.LF.Submit();
            break;
        case 1:
            ClassDiagramFilter.GH.SubmitList();
            break;
        default:
            break;
    }
};

ClassDiagramFilter.ExcludeByClass = function() {
    switch(ClassDiagramFilter.tab) {
        case 0:
            ClassDiagramFilter.LF.ExcludeByClass();
            break;
        case 1:
            ClassDiagramFilter.GH.ExcludeByClass();
            break;
        default:
            break;
    }
};

function MakeGuid() {
    return "aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaaa".replace(/a/g, function ()
    {
        var rand = Math.random() * 16;
        rand = Math.floor(rand); //make sure we have a whole number

        return rand.toString(16);
    });
}

$(document).ready(function() {
    ClassDiagramFilter.divBelowAuthentication = document.getElementById("gEverythingBelowAuthentication");

    ClassDiagramFilter.LF.tabDiv = document.getElementById("filesTab");
    ClassDiagramFilter.GH.tabDiv = document.getElementById("githubAuthTab");

    //ClassDiagramFilter.submitBtn = document.getElementById("SubmitBtn"); --temp--

    ClassDiagramFilter.tabObjs.push(ClassDiagramFilter.LF);
    ClassDiagramFilter.tabObjs.push(ClassDiagramFilter.GH);

    //Detect if IE
    if((window.navigator.userAgent.indexOf("MSIE ") != -1) ||
        (window.navigator.userAgent.indexOf("Trident/") != -1)) {
            ClassDiagramFilter.SetTab(0);
            ClassDiagramFilter.GH.tabDiv.innerHTML = "<p>Warning: IE does not work Github authentication" +
                " starting July 2018.</p>";
    }

    //Github
    ClassDiagramFilter.GH.userInput = document.getElementById("ghUser");
    ClassDiagramFilter.GH.repoInput = document.getElementById("ghRepo");
    ClassDiagramFilter.GH.branchInput = document.getElementById("ghBranch");
    ClassDiagramFilter.GH.listFilesBtn = document.getElementById("ghListFilesBtn");
    ClassDiagramFilter.GH.excludeClassBtn = document.getElementById("ghClassExclude");
    ClassDiagramFilter.GH.output = document.getElementById("ghAuthOutput");
    ClassDiagramFilter.GH.treeOutputDiv = document.getElementById("ghTreeOutput");
    ClassDiagramFilter.GH.treeOutputOuterDiv = document.getElementById("ghTreeOutputOuter");
    ClassDiagramFilter.GH.classExcludeOutputDiv = document.getElementById("ghClassExclusionTree");
    ClassDiagramFilter.GH.classExclusionTreeOuterDiv = document.getElementById("ghClassExclusionTreeOuter");
    ClassDiagramFilter.GH.treeLoading = document.getElementById("ghTreeLoading");
    ClassDiagramFilter.GH.classExLoading = document.getElementById("ghClassExLoading");
    ClassDiagramFilter.GH.classExcludeCheckAllBtn = document.getElementById("ghClassExcludeCheckAll");
    ClassDiagramFilter.GH.classExcludeUncheckAllBtn = document.getElementById("ghClassExcludeUncheckAll");
    ClassDiagramFilter.GH.inputHistory = document.getElementById("ghInputHistory");

    //Initialize callback loc
    var locSplit = window.location.pathname.split('/');

    if(locSplit.length >= 2) {
        locSplit[0] = window.location.protocol + "//" + window.location.hostname;
        locSplit[locSplit.length - 1] = ClassDiagramFilter.GH.apiCallbackLoc;
        ClassDiagramFilter.GH.apiCallback = locSplit.join('/');
    }

    //Local files
    ClassDiagramFilter.LF.output = document.getElementById("lfOutputText");
    ClassDiagramFilter.LF.classExcludeOutputDiv = document.getElementById("lfClassExclusionTree");
    ClassDiagramFilter.LF.classExclusionTreeOuterDiv = document.getElementById("lfClassExclusionTreeOuter");
    ClassDiagramFilter.LF.classExLoading = document.getElementById("lfClassExLoading");
    ClassDiagramFilter.LF.fileList = document.getElementById("lfFiles");
    ClassDiagramFilter.LF.excludeClassBtn = document.getElementById("lfClassExclude");
    ClassDiagramFilter.LF.fileNameText = document.getElementById("lfFileName");
    ClassDiagramFilter.LF.classExcludeCheckAllBtn = document.getElementById("lfClassExcludeCheckAll");
    ClassDiagramFilter.LF.classExcludeUncheckAllBtn = document.getElementById("lfClassExcludeUncheckAll");

    switch("SDENVIRONMENT") {
        case "PRD":
            ClassDiagramFilter.GH.appID = "<Your App ID>";
            break;
        default:
            //local
            break;
    }

    try {
        ClassDiagramFilter.GH.authentication = new ClassDiagramFilter.GH.Authentication();
        ClassDiagramFilter.GH.authentication.VerifySavedAuthentication();
    }
    finally {
        ClassDiagramFilter.documentLoaded = true;
    }
});