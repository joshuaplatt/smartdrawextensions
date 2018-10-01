ClassDiagramFilter.Github.GithubService = (function() {
    function CGithubService() {
        /** @type {CGithubService} */
        var self = this;
        /** @type {ClassDiagramFilter.Github.GithubController} */
        var Controller = null;
        /** @type {ClassDiagramFilter.Github.GithubInterface} */
        var Interface = null;
        
        var apiLocation = "https://api.github.com/";
        
        this.GetRepositoryLatestCommit = function(user, repo, branch,
                onSuccess, onFail) {
            var url = apiLocation + "repos/" +
                user + "/" + repo + "/branches/" + branch;
            
            var request = {
                type: "GET",
                url: url,
                
                success: function(data, status, x) {
                    onSuccess && onSuccess(data);
                },
                
                error: function(data, status, x) {
                    onFail && onFail(data);
                }
            };
            
            if(Controller.userKey) {
                request.url += "?access_token=" + Controller.userKey;
            }
            
            $.ajax(request);
        };
        
        this.ListRepositoryTree = function(branchUrl, onSuccess, onFail) {
            var request = {
                type: "GET",
                url: branchUrl + "?recursive=1",
                
                success: function(data, status) {
                    onSuccess && onSuccess(data);
                },
                
                error: function(data, status) {
                    onFail && onFail(data);
                }
            };
            
            if(Controller.userKey) {
                request.url += "&access_token=" + Controller.userKey;
            }
            
            $.ajax(request);
        };
        
        this.GetUserRateLimit = function(userId, onSuccess) {
            var url = apiLocation + "rate_limit?access_token=" + userId;
            
            var request = {
                type: "GET",
                url: url,
                success: function(data, status) {
                    onSuccess && onSuccess(data);
                }
            };
            
            $.ajax(request);
        };
        
        this.SubmitToService = function(exclusionBuffer, onSuccess, onFail, onStep, hideMethods, hideProperties) {
            var submission = {
                blob: new Uint8Array(exclusionBuffer.buffer),
                hideMethods: hideMethods ? "1" : "0",
                hideProperties: hideProperties ? "1" : "0",
                onSuccess: onSuccess,
                onFail: onFail,
                onStep: onStep,
                sent: 0,
                uploadInfo: Interface.GetParamsInput(),
                uploadId: MakeGuid()
            };
            
            var userKey = Controller.userKey;
            
            if(!userKey) {
                onFail && onFail("Authentication error.");
            }
            
            SubmitExclusionBlock(submission, userKey);
        };
        
        var SubmitExclusionBlock = function(submission, userKey) {
            var amountToSend = submission.blob.byteLength - submission.sent;
            
            if(amountToSend > ClassDiagramFilter.Globals.maxBlockSize) {
                amountToSend = ClassDiagramFilter.Globals.maxBlockSize;
            }
            
            var request = {
                type: "POST",
                url: "filter/ctags-github-block?uploadID=" + submission.uploadId,
                data: submission.blob.subarray(submission.sent,
                    submission.sent + amountToSend),
                contentType: "text/plain",
                processData: false,
                
                success: function(data, status) {
                    submission.onStep && submission.onStep(submission.sent,
                        submission.blob.byteLength);
                    
                    if(submission.sent < submission.blob.byteLength) {
                        SubmitExclusionBlock(submission, userKey);
                    }
                    else {
                        SubmitExclusionFinal(submission, userKey);
                    }
                },
                
                error: function(data, status) {
                    submission.onFail && submission.onFail(data);
                }
            };
            
            submission.sent += amountToSend;
            
            $.ajax(request);
        };
        
        var SubmitExclusionFinal = function(submission, userKey) {
            var request = {
                type: "POST",
                url: "filter/ctags-github-final?uploadID=" + submission.uploadId +
                    "&user=" + submission.uploadInfo.user +
                    "&repo=" + submission.uploadInfo.repo +
                    "&branch=" + submission.uploadInfo.branch +
                    "&accessToken=" + userKey +
                    "&hideMethods=" + submission.hideMethods +
                    "&hideProperties=" + submission.hideProperties,
                
                success: function(data, status) {
                    submission.onSuccess && submission.onSuccess(data);
                },
                
                error: function(data, status) {
                    submission.onFail && submission.onFail(data);
                }
            };
            
            $.ajax(request);
        };
        
        this.Init = function() {
            Controller = ClassDiagramFilter.Github.GithubController;
            Interface = ClassDiagramFilter.Github.GithubInterface;
        };
    }
    
    return new CGithubService();
})();
