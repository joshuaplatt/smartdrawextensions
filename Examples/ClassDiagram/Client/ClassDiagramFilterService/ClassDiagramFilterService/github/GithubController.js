ClassDiagramFilter.Github = {};

ClassDiagramFilter.Github.GithubController = (function() {
    function CGithubController() {
        var self = this;
        /** @type {ClassDiagramFilter.Github.GithubInterface} */
        var Interface = null;
        /** @type {ClassDiagramFilter.Github.Authentication} */
        var Authentication = null;
        /** @type {ClassDiagramFilter.Github.GithubService} */
        var Service = null;

        var settings = {};

        var apiCallbackRelative = "githubauthenticated.html";
        var apiCallbackLocation = "";

        /** @type {ClassDiagramFilter.Github.GithubRepositoryTree} */
        var repositoryTree = null;
        /** @type {ClassDiagramFilter.Utils.ClassExclusionUtil} */
        var classExclusion = null;

        /** @type {{user: string, repo: string, branch: string}} */
        var currentInput = {};

        var isDisplayingTree = false;
        var classesAreInvalid = true;

        this.isAuthenticated = false;
        this.isSubmitting = false;
        this.userKey = "";

        this.appId = "<Your App ID Here>";

        this.QuerySettings = function(callback) {
            SDAPI.SDON.GetUserSettings(function(response, err) {
                if(response && (typeof response.Payload == "string")) {
                    if(response.Payload) {
                        settings = JSON.parse(response.Payload);
                    }

                    callback && callback(settings);
                }
            });
        };

        this.SetSettings = function(newSettings) {
            settings = newSettings;

            SDAPI.SDON.SetUserSettings(JSON.stringify(newSettings),
                function(success, err) {
                    //either succeeded or failed to set settings
                });
        };

        this.GetAPICallbackLocation = function() {
            if(apiCallbackLocation) {
                return apiCallbackLocation;
            }

            var locSplit = window.location.pathname.split('/');

            if(locSplit.length >= 2) {
                locSplit[0] = window.location.protocol + "//" + window.location.hostname;
                locSplit[locSplit.length - 1] = apiCallbackRelative;
                apiCallbackLocation = locSplit.join('/').toLowerCase();
            }

            return apiCallbackLocation;
        };

        this.SetSubmitting = function(submitting) {
            this.isSubmitting = submitting;
            Interface.SetSubmittting(submitting);
        };

        this.RefreshInputHistorySelection = function() {
            var history = settings.history;

            if(!history || !Array.isArray(history)) {
                return;
            }

            var currOption = document.createElement("option");
            currOption.innerText = "Select a repository from history...";

            var options = [currOption];
            var currOptGroup;
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
                currOptGroup = document.createElement("optgroup");
                currOptGroup.label = users[i].name;

                for(j = 0; j < users[i].repos.length; j++) {
                    currOption = document.createElement("option");
                    currOption.value = encodeURI(users[i].name) + " " +
                        encodeURI(users[i].repos[j].repo) + " " +
                        encodeURI(users[i].repos[j].branch);
                    currOption.innerText = users[i].repos[j].repo + " (" +
                        users[i].repos[j].branch + ")";

                    currOptGroup.appendChild(currOption);
                }

                options.push(currOptGroup);
            }

            Interface.SetSelectionOptions(options);
        };

        this.AddToInputHistory = function(user, repo, branch) {
            this.QuerySettings(function() {
                var input = {user: user, repo: repo, branch: branch};

                if(!settings.history || !Array.isArray(settings.history)) {
                    settings.history = [input];
                }
                else {
                    var hasSetting = false;

                    for(var i = 0; i < settings.history.length; i++) {
                        //A whole ton of comparisons to check equivalence of history entries
                        if((typeof settings.history[i].user == "string") &&
                            (typeof settings.history[i].repo == "string") &&
                            (typeof settings.history[i].branch == "string") &&
                            (settings.history[i].user.toLowerCase() == user.toLowerCase()) &&
                            (settings.history[i].repo.toLowerCase() == repo.toLowerCase()) &&
                            (settings.history[i].branch.toLowerCase() == branch.toLowerCase())) {

                            hasSetting = true;
                            break;
                        }
                    }

                    if(!hasSetting) {
                        settings.history.unshift(input);

                        if(settings.history.length > 10) {
                            settings.history.splice(10, settings.history.length - 10);
                        }
                    }
                }

                self.RefreshInputHistorySelection(settings);
                self.SetSettings(settings);
            });
        };

        this.SelectFromInputHistory = function(e) {
            if(!ClassDiagramFilter.Globals.documentLoaded || !e.target.value) {
                return;
            }

            var valarr = e.target.value.split(" ");

            if(valarr.length != 3) {
                return;
            }

            Interface.SetParamsInput({
                user: decodeURI(valarr[0]),
                repo: decodeURI(valarr[1]),
                branch: decodeURI(valarr[2])
            });
        };

        this.Authenticate = function() {
            if(!ClassDiagramFilter.Globals.documentLoaded) {
                return;
            }

            Authentication.BeginAuthentication();
        };

        this.ListFiles = function() {
            if(!ClassDiagramFilter.Globals.documentLoaded || this.isSubmitting) {
                return;
            }

            this.SetSubmitting(true);
            Interface.SetRepositoryExclusionLoadingState(true);
            Interface.SetExclusionTreeFocus(Interface.ExclusionTrees.Repository);
            isDisplayingTree = true;
            classesAreInvalid = true;
            currentInput = Interface.GetParamsInput();
            Interface.RemoveGlowyButton();

            var onSuccessCommit = function(data) {
                try {
                    Service.ListRepositoryTree(data.commit.commit.tree.url,
                        onSuccessTree, onFail);

                    //Add to input history
                }
                catch(e) {
                    onFail("External error");
                }
            };

            var onSuccessTree = function(data) {
                try {
                    repositoryTree.GenerateTree(data.tree);
                    self.AddToInputHistory(currentInput.user, currentInput.repo,
                        currentInput.branch);
                }
                catch(e) {
                    onFail("Error upon loading tree.");
                }
            };

            var onFail = function(data) {
                Interface.SetRepositoryExclusionLoadingState(false);
                self.SetSubmitting(false);
                isDisplayingTree = false;
            };

            Service.GetRepositoryLatestCommit(currentInput.user, currentInput.repo,
                currentInput.branch, onSuccessCommit, onFail);
        };

        this.ExcludeByFile = function() {
            if(!ClassDiagramFilter.Globals.documentLoaded || this.isSubmitting) {
                return;
            }

            this.SetSubmitting(true);
            Interface.SetRepositoryExclusionLoadingState(true);
            Interface.SetExclusionTreeFocus(Interface.ExclusionTrees.Repository);
            Interface.SetRepositoryExclusionLoadingState(false);
            this.SetSubmitting(false);
        };

        this.ExcludeClassBypass = function() {
            if(!ClassDiagramFilter.Globals.documentLoaded || this.isSubmitting) {
                return;
            }


        };

        this.ExcludeByClass = function() {
            if(!ClassDiagramFilter.Globals.documentLoaded || this.isSubmitting) {
                return;
            }

            this.SetSubmitting(true);
            Interface.SetClassExclusionLoadingState(true);
            Interface.SetExclusionTreeFocus(Interface.ExclusionTrees.Class);

            if(!classesAreInvalid) {
                Interface.SetClassExclusionLoadingState(false);
                this.SetSubmitting(false);
                return;
            }

            isDisplayingTree = true;

            var exclusionTree = repositoryTree.GenerateExclusionList();
            var exclusionBlob = GenerateExclusionBlob(exclusionTree);

            var onSuccess = function(data) {
                try {
                    var sdon = JSON.parse(data);
                    classExclusion.BuildTreeFromSDON(sdon);
                }
                catch(e) {
                    onFail(e.message);
                }
            };

            var onStep = function(sent, total) {
                console.log(((total > 0) ? (sent / total * 100) : 100) + "% transferred");
            };

            var onFail = function(data) {
                Interface.SetClassExclusionLoadingState(false);
                self.SetSubmitting(false);
                isDisplayingTree = false;

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

            Service.SubmitToService(exclusionBlob, onSuccess, onFail, onStep, false, false);
        };

        this.Submit = function() {
            if(!ClassDiagramFilter.Globals.documentLoaded || this.isSubmitting ||
                !isDisplayingTree) {
                return;
            }

            this.SetSubmitting(true);
            var currentTree = Interface.GetCurrentTree();

            if(currentTree == Interface.ExclusionTrees.Repository) {
                SubmitFromRepository();
            }
            else if(currentTree == Interface.ExclusionTrees.Class) {
                SubmitFromClassEx();
            }
            else {
                this.SetSubmitting(false);
            }
        };

        this.Logout = function() {
            Authentication.Logout();
        };

        this.SetChecked = function(checked) {
            if(checked) {
                if(Interface.GetCurrentTree() == Interface.ExclusionTrees.Repository) {
                    repositoryTree.CheckAll();
                }
                else if(Interface.GetCurrentTree() == Interface.ExclusionTrees.Class) {
                    classExclusion.CheckAll();
                }
            }
            else {
                if(Interface.GetCurrentTree() == Interface.ExclusionTrees.Repository) {
                    repositoryTree.UncheckAll();
                }
                else if(Interface.GetCurrentTree() == Interface.ExclusionTrees.Class) {
                    classExclusion.UncheckAll();
                }
            }
        };

        var SubmitFromRepository = function() {
            var exclusionTree = repositoryTree.GenerateExclusionList();
            var exclusionBlob = GenerateExclusionBlob(exclusionTree);

            var onSuccess = function(data) {
                SDAPI.SDON.SDONDone(data, function(data, err) {
                    SDAPI.SDON.CloseDialog();
                });
            };

            var onStep = function(sent, total) {
                console.log(((total > 0) ? (sent / total * 100) : 100) + "% transferred");
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

            Service.SubmitToService(exclusionBlob, onSuccess, onFail, onStep,
                Interface.GetIsHidingMethods(), Interface.GetIsHidingProperties());
        };

        var SubmitFromClassEx = function() {
            var sdon = classExclusion.GenerateSDONFromTree(Interface.GetIsHidingMethods(),
                Interface.GetIsHidingProperties());

            SDAPI.SDON.SDONDone(sdon, function(data, err) {
                SDAPI.SDON.CloseDialog();
            });
        };

        var RepositoryTreeOnReadyCallback = function() {
            Interface.SetRepositoryExclusionLoadingState(false);
            self.SetSubmitting(false);
        };

        var ClassExclusionOnReadyCallback = function() {
            classesAreInvalid = false;
            Interface.SetClassExclusionLoadingState(false);
            self.SetSubmitting(false);
        };

        var GenerateExclusionBlob = function(list) {
            var exclusionStr = list.join("\n");
            var exclusionBlob = new Uint16Array(exclusionStr.length);

            for(var i = 0; i < exclusionStr.length; i++) {
                exclusionBlob[i] = exclusionStr.charCodeAt(i);
            }

            return exclusionBlob;
        };

        this.Init = function() {
            Interface = ClassDiagramFilter.Github.GithubInterface;
            Authentication = ClassDiagramFilter.Github.Authentication;
            Service = ClassDiagramFilter.Github.GithubService;

            repositoryTree = new ClassDiagramFilter.Github.GithubRepositoryTree();
            classExclusion = new ClassDiagramFilter.Utils.ClassExclusionUtil();

            Interface.Init();
            Authentication.Init();
            Service.Init();

            //Initialize callback loc
            var locSplit = window.location.pathname.split('/');

            if(locSplit.length >= 2) {
                locSplit[0] = window.location.protocol + "//" + window.location.hostname;
                locSplit[locSplit.length - 1] = this.apiCallbackLoc;
                this.apiCallback = locSplit.join('/');
            }

            //Attempt auto-authenticate from saved id
            this.QuerySettings(function() {
                self.RefreshInputHistorySelection();
                Authentication.VerifySavedAuthentication(settings);
            });

            //Trees
            repositoryTree.Init("ghTreeOutput", RepositoryTreeOnReadyCallback);
            classExclusion.Init("ghClassExclusionTree", ClassExclusionOnReadyCallback);
        };
    }

    return new CGithubController();
})();
