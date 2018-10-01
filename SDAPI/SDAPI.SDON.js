/**Controller for interacting with the app's plugin interface via cross-origin iframe communication.*/
SDAPI.SDON = (function ()
{
    //we run a self-executing function to return the object defined in the constructor below so that there is only one instance of this
    //controller in the consumer's code. Otherwise they could instantiate another copy of the controller and cause issues.

    var SDONExtensionController = function ()
    {
        var _self = this;
        var _messageCallbackSet = false;
        var _queryStringVariables = SDAPI.Utils.GetQueryStringVariables();
        var _origin = SDAPI.Utils.GetOriginFromQueryString(_queryStringVariables);
        var _contextID = SDAPI.Utils.GetContextIDFromQueryString(_queryStringVariables);
        var _callbackStack = [];

        /**Gets the settings string for the current user. Will fall back to the default settings for the plugin if the user has no settings set with the plugin.
        @method GetUserSettings
        @param {Function} callback: A callback function that is fired when the data arrives from the parent iframe. Receives a SDAPI.CrossWindowPluginMessage as a parameter where the Payload property is a string of
        user settings data if the operation succeeded or null if the operation failed.*/
        this.GetUserSettings = function (callback)
        {
            var message = new SDAPI.CrossWindowPluginMessage();
            message.ContextID = _contextID;
            message.APIType = SDAPI.APIType.SDON;
            message.Command = SDAPI.SDONCommands.GetUserSettings;

            sendMessage(message, callback);
        };

        /**Sets the settings for a user with the plugin. Any value passed in is turned into a string.
        @method SetUserSettings
        @param {Anything} settings: The user settings to store for the current user.
        @param {Function} callback: A callback function that is fired when the call to set the user's settings completes. Receives a SDAPI.CrossWindowPluginMessage as a parameter where the Payload property is a boolean
        that is set to true if the operation succeeded or false if it did not succeed.*/
        this.SetUserSettings = function (settings, callback)
        {
            var message = new SDAPI.CrossWindowPluginMessage();
            message.ContextID = _contextID;
            message.APIType = SDAPI.APIType.SDON;
            message.Command = SDAPI.SDONCommands.SetUserSettings;
            message.Payload = settings;

            sendMessage(message, callback);
        };

        /**Calls to the parent iframe indicating that the work to get a SDON string or Blob has been completed and is passed back to the parent iframe to be stored as a document or injected into the current document. 
        If the SDON operation failed for any reason, a message is sent back with an error message. If the SDON operation is successful, the child iframe is closed.
        @method SDONDone
        @param {Anything} sdon: An SDON object, string, or blob representing the data to turn into a diagram.
        @param {Function} callback: A callback function that is fired when the parent iframe signals that is has received the SDON. Receives a SDAPI.CrossWindowPluginMessage as a parameter where the Payload property is a boolean
        that is set to true if the operation succeeded or false if it did not succeed.*/
        this.SDONDone = function (sdon, callback)
        {
            var message = new SDAPI.CrossWindowPluginMessage();
            message.ContextID = _contextID;
            message.APIType = SDAPI.APIType.SDON;
            message.Command = SDAPI.SDONCommands.SDONDone;
            message.Payload = sdon;

            sendMessage(message, callback);
        };

        /**Calls to the parent iframe and tells it to cose the iframe and modal.
        @method CloseDialog*/
        this.CloseDialog = function ()
        {
            var message = new SDAPI.CrossWindowPluginMessage();
            message.ContextID = _contextID;
            message.APIType = SDAPI.APIType.SDON;
            message.Command = SDAPI.SDONCommands.CloseDialog;

            sendMessage(message);
        };

        /**Calls to the parent iframe and tells it to display a loading animation over the hosted iframe, meant to be used when a user's plugin is building SDON and the maker of the plugin
        wants a loading animation on the hosting iframe modal.
        @method ShowSDONLoading*/
        this.ShowSDONLoading = function ()
        {
            var message = new SDAPI.CrossWindowPluginMessage();
            message.ContextID = _contextID;
            message.APIType = SDAPI.APIType.SDON;
            message.Command = SDAPI.SDONCommands.ShowSDONLoading;

            sendMessage(message);
        };

        /**Calls to the parent iframe and tells it to hide the loading animation over the hosted iframe, meant to be used when a user's plugin is building SDON and the maker of the plugin
        wants to hide the loading animation on the hosting iframe modal invoked by ShowSDONLoading.
        @method ShowSDONLoading*/
        this.HideSDONLoading = function ()
        {
            var message = new SDAPI.CrossWindowPluginMessage();
            message.ContextID = _contextID;
            message.APIType = SDAPI.APIType.SDON;
            message.Command = SDAPI.SDONCommands.HideSDONLoading;

            sendMessage(message);
        };

        /**Event handler for receiving messages from a parent iframe.
        @method onMessage
        @param {Object} eventArgs: The browser's event args.*/
        var onMessage = function (eventArgs)
        {
            var data = eventArgs.data;
            if (data == null) return; //no data, can't do anything           
            if (data.APIType !== SDAPI.APIType.SDON) return; //message not intended for SDON, ignore

            callCallback(data);
        };

        /**Sends a message to the parent iframe.
        @method sendMessage
        @param {Object} apiMessage: An intance of SDAPI.CrossWindowPluginMessage to send to the parent iframe.*/
        var sendMessage = function (apiMessage, callback)
        {
            if (apiMessage == null) return false;
            if (_messageCallbackSet === false)
            {
                _messageCallbackSet = true;
                window.addEventListener("message", onMessage);
            }

            if (typeof callback === "function")
            {
                var callbackContainer = new CallbackContainer();
                callbackContainer.Request = apiMessage;
                callbackContainer.Callback = callback;

                _callbackStack.push(callbackContainer);
            }

            var failed = false;

            try
            {
                if (window.parent != null)
                {
                    window.parent.postMessage(apiMessage, _origin);
                }
                else
                {
                    failed = true;
                    apiMessage.Complete = true;
                    apiMessage.ErrorMessage = "SDAPI Error: No parent window to send message to.";
                }
            }
            catch (ex)
            {
                failed = true;
                apiMessage.Complete = true;
                apiMessage.ErrorMessage = "SDAPI Error: Error sending message to parent window. " + ex.message;
            }
            finally
            {
                if (failed === true)
                {
                    try
                    {
                        callCallback(apiMessage);
                    }
                    catch (ex)
                    {
                        console.log("SDAPI Error: " + JSON.stringify(ex));
                    }
                }
            }
        }

        /**Manages calling the callbacks associated with cross window messages.
        @method callCallback
        @param {Object} request: An instance of SDAPI.CrossWindowPluginMessage that was handed back to the child iframe by the hosting modal.*/
        var callCallback = function (request)
        {
            if (request == null) return;
            if (request.Success === false && request.ErrorMessage != null) console.log(request.ErrorMessage);

            var callbackToCall = null;

            //get all the matching messages (there should only be just one, but we get them all in just in case something didnt work properly)
            var matchingMessages = _callbackStack.filter(function (callbackEntry) { return (callbackEntry.Request != null && callbackEntry.Request.MessageID === request.MessageID) });
            var numMatches = matchingMessages.length;
            for (var x = 0; x < numMatches; x++)
            {
                var curMatch = matchingMessages[x];

                //set the callback from the first message with a callback function
                if (callbackToCall == null && typeof curMatch.Callback === "function") callbackToCall = curMatch.Callback;

                //then remove the callback entry from the list regardless if it had a callback function
                var index = _callbackStack.indexOf(curMatch);
                if (index != -1) _callbackStack.splice(index, 1);
            }

            if (callbackToCall == null) return; //no callback, nothing to do

            //we get and remove the callback to call from the callback stack before calling it so it is guaranteed to be removed from the list, even if the callback function called has an error
            try
            {
                callbackToCall(request);
            }
            catch (ex)
            {
                console.log("SDAPI Error: " + JSON.stringify(ex));
            }
        };

        /**Object for holding a callback and the request it belongs to.*/
        var CallbackContainer = function ()
        {
            /**The cross-window request.*/
            this.Request = null;
            /**The callback function to call when the parent ifram returns the message.*/
            this.Callback = function (apiMessage) { };
        };
    };

    return new SDONExtensionController();

})();

