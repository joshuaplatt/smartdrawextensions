/**Root namespace for SmartDraw APIs.*/
if (window.SDAPI == null) SDAPI = {};

/**Object for sending messages back and forth between iframes hosted on different domains.*/
SDAPI.CrossWindowPluginMessage = function ()
{
    /**The context of the plugin issuing the message.*/
    this.ContextID = null;
    /**The GUID of this request.*/
    this.MessageID = SDAPI.Utils.MakeGuid();
    /**Number. The API being used. A value from SDAPI.APIType.*/
    this.APIType = SDAPI.APIType.None;
    /**Number. The command of the API being issed.*/
    this.Command = 0;
    /**Anything. Any data to transport from one window to another.*/
    this.Payload = null;
    /**Boolean. Whether or not the command issued has been successfully completed by the parent to the iframe.*/
    this.Success = false;
    /**String. An error message sent by the parent iframe informing the child iframe that the operation failed and why.*/
    this.ErrorMessage = null;
};

/**Enum describing which API is being used to send cross-winow messages.*/
SDAPI.APIType =
{
    /**API not selected.*/
    None: 0,
    /**Using the SDON Plugin API.*/
    SDON: 1
};

/**Enum describing the commands that can be issed when using the SDON API.*/
SDAPI.SDONCommands =
{
    None: 0,
    GetUserSettings: 1,
    SetUserSettings: 2,
    SDONDone: 3,
    CloseDialog: 4,
    ShowSDONLoading: 5,
    HideSDONLoading: 6,

};

/**Constants table for values that are immutable.*/
SDAPI.Constants = {};
SDAPI.Constants.QS_Origin = "sdorigin";
SDAPI.Constants.QS_ContextID = "sdcxtid";

/**Namespace container for utility functions.*/
SDAPI.Utils = {};

/**Gets the SmartDraw host based on the current enviornment URL.
@method GetHost*/
SDAPI.Utils.GetHost = function ()
{
    var protocol = window.location.href.match(/^https:/) ? "https://" : "http://";

    // Must return the actual host origin when the app is served up from a non-SmartDraw server
    var retVal = protocol + window.location.host;
    return retVal;
};

/**Gets an object with a property for each key-value pair in the query string.
@method GetQueryStringVariables*/
SDAPI.Utils.GetQueryStringVariables = function (url)
{
    var vars = {};
    var hash = null;

    if (url == null) url = window.location.href;
    var queryString = url.slice(url.indexOf('?') + 1);
    if (queryString == null) return vars;

    var hashIndex = queryString.indexOf("#");
    if (hashIndex !== -1) queryString = queryString.slice(0, hashIndex);

    var hashes = queryString.split('&');

    for (var i = 0; i < hashes.length; i++)
    {
        hash = hashes[i].split('=');
        if (hash.length === 1) continue;

        vars[hash[0]] = hash[1];
    }

    return vars;
};

/**Gets the origin of the parent iframe using a value passed in the query string to the child iframe.
@method GetOriginFromQueryString
@param {Object} queryStringParams: Optional. The query string parameters as produced by SDAPI.Utils.GetQueryStringVariables()*/
SDAPI.Utils.GetOriginFromQueryString = function (queryStringVars)
{
    if (queryStringVars == null) queryStringVars = SDAPI.Utils.GetQueryStringVariables();
    if (queryStringVars == null) return null;

    var origin = queryStringVars[SDAPI.Constants.QS_Origin];
    if (origin == null) return null;

    return decodeURIComponent(origin);
};

/**Gets the PluginID from the query string to the child iframe.
@method GetPluginIDFromQueryString
@param {Object} queryStringParams: Optional. The query string parameters as produced by SDAPI.Utils.GetQueryStringVariables()*/
SDAPI.Utils.GetContextIDFromQueryString = function (queryStringVars)
{
    if (queryStringVars == null) queryStringVars = SDAPI.Utils.GetQueryStringVariables();
    if (queryStringVars == null) return null;

    var contextID = queryStringVars[SDAPI.Constants.QS_ContextID];
    if (contextID == null) return null;

    return decodeURIComponent(contextID);
};

/**Generates a GUID.
@method MakeGuid*/
SDAPI.Utils.MakeGuid = function ()
{
    return "aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaaa".replace(/a/g, function ()
    {
        var rand = Math.random() * 16;
        rand = Math.floor(rand); //make sure we have a whole number

        return rand.toString(16);
    });
};