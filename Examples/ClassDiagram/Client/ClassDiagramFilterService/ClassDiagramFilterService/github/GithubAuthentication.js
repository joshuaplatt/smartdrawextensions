ClassDiagramFilter.Github.Authentication = (function() {
    function CAuthentication() {
        var self = this;
        /** @type {ClassDiagramFilter.Github.GithubController} */
        var Controller = ClassDiagramFilter.Github.GithubController;
        /** @type {ClassDiagramFilter.Github.GithubInterface} */
        var Interface = ClassDiagramFilter.Github.GithubInterface;
        /** @type {ClassDiagramFilter.Github.GithubService} */
        var Service = ClassDiagramFilter.Github.GithubService;
        
        this.BeginAuthentication = function() {
            var state = MakeGuid().replace(/-/g, "_");
            
            /** @param {MessageEvent} e */
            var listener = function(e) {
                if((e.origin != window.location.origin) ||
                    (e.source.state != state)) {
                    return;
                }
                
                FinalizeAuthentication(e);
                window.removeEventListener("message", listener);
            };
            
            window.addEventListener("message", listener);
            
            var callbackLocation = encodeURI(Controller.GetAPICallbackLocation());
            var url = "https://www.github.com/login/oauth/authorize" +
                "?client_id=" + Controller.appId + "&state=" + state +
                "&redirect_uri=" + callbackLocation;
            var popup = window.open('', null, "width=800,height=600");
            
            if(!popup) {
                console.log("Could not open window.  " +
                    "If a popup blocker is present, please disable it for the filter's site.");
                return;
            }
            else {
                popup.location = url;
            }
        };
        
        this.VerifySavedAuthentication = function(settings) {
            if(!settings || !settings.userID) {
                return;
            }
            
            Service.GetUserRateLimit(settings.userID, function(data) {
                RestoreFromSavedAuthentication(data, settings);
            });
        };
        
        this.Logout = function() {
            if(Controller.isSubmitting) {
                return;
            }
            
            Controller.SetSubmitting(true);
            
            Controller.QuerySettings(function(settings) {
                if(settings && settings.userID) {
                    settings.userID = undefined;
                    Controller.SetSettings(settings);
                    Interface.ShowVerificationTab();
                    Controller.SetSubmitting(false);
                }
            });
        };
        
        var FinalizeAuthentication = function(e) {
            e.source.postMessage(true, e.origin);
            
            Controller.userKey = e.data;
            Interface.HideVerificationTab();
            
            Controller.QuerySettings(function(s) {
                var settings = s ? s : {};
                settings.userID = e.data;
                
                Controller.SetSettings(settings);
            });
        };
        
        var RestoreFromSavedAuthentication = function(data, settings) {
            if(data && (typeof data == "object") && data.rate &&
                data.rate.limit && (data.rate.limit > 60)) {
                
                Controller.userKey = settings.userID;
                Interface.HideVerificationTab();
            }
        };
        
        this.Init = function() {
            Controller = ClassDiagramFilter.Github.GithubController;
            Interface = ClassDiagramFilter.Github.GithubInterface;
            Service = ClassDiagramFilter.Github.GithubService;
        };
    }
    
    return new CAuthentication();
})();