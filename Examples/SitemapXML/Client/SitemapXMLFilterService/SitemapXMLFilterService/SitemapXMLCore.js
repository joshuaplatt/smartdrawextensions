SitemapXMLFilter = {};

SitemapXMLFilter.Globals = {
    tablesInput: null,
    hyperlinksInput: null,
    lexicographicOrderInput: null,
    
    tabObjs: [],
    tabStates: {
        LocalFile: 0,
        Web: 1
    },
    
    maxBlockSize: 65000,
    
    currentState: 1,
    
    treeIteration: 0,
    
    //Modify the following values at your own risk
    maxLocalFileSize: 1 << 24, //16 MB
    maxLocalExclusionListNodeCount: 10000,
    maxServerFileSize: 1 << 21,
    maxServerNodeCount: 600,
    
    documentLoaded: false,
    isOnEdge: navigator.userAgent.indexOf("Edge") != -1
};

SitemapXMLFilter.Submit = function() {
    if(!SitemapXMLFilter.Globals.documentLoaded) {
        return;
    }
    
    switch(SitemapXMLFilter.Globals.currentState) {
        case SitemapXMLFilter.Globals.tabStates.LocalFile:
            SitemapXMLFilter.Local.LocalController.Submit();
            break;
        case SitemapXMLFilter.Globals.tabStates.Web:
            SitemapXMLFilter.Web.WebController.Submit();
            break;
        default:
            break;
    }
};

SitemapXMLFilter.Exclude = function() {
    if(!SitemapXMLFilter.Globals.documentLoaded) {
        return;
    }
    
    switch(SitemapXMLFilter.Globals.currentState) {
        case SitemapXMLFilter.Globals.tabStates.LocalFile:
            SitemapXMLFilter.Local.LocalController.Exclude();
            break;
        case SitemapXMLFilter.Globals.tabStates.Web:
            SitemapXMLFilter.Web.WebController.Exclude();
            break;
        default:
            break;
    }
};

SitemapXMLFilter.SetTab = function(tabNum) {
    if(!SitemapXMLFilter.Globals.documentLoaded) {
        return;
    }
    
    SitemapXMLFilter.Globals.currentState = tabNum;
    
    for(var i = 0; i < SitemapXMLFilter.Globals.tabObjs.length; i++) {
        if(i == tabNum) {
            SitemapXMLFilter.Globals.tabObjs[i].SetTabVisibility(true);
        }
        else {
            SitemapXMLFilter.Globals.tabObjs[i].SetTabVisibility(false);
        }
    }
};

SitemapXMLFilter.SelectFile = function() {
    if(!SitemapXMLFilter.Globals.documentLoaded) {
        return;
    }
    
    var files = SitemapXMLFilter.Local.LocalInterface.GetFiles();
    
    if(files && (files.length > 0)) {
        SitemapXMLFilter.Local.LocalInterface.SelectFile();
        SitemapXMLFilter.SetTab(SitemapXMLFilter.Globals.tabStates.LocalFile);
    }
    else {
        SitemapXMLFilter.ClearFile();
    }
};

SitemapXMLFilter.ClearFile = function() {
    if(!SitemapXMLFilter.Globals.documentLoaded) {
        return;
    }
    
    SitemapXMLFilter.Local.LocalInterface.ClearFile();
    SitemapXMLFilter.SetTab(SitemapXMLFilter.Globals.tabStates.Web);
};

/** @param {KeyboardEvent} e */
SitemapXMLFilter.UrlInputKeyDownEvent = function(e) {
    if(!SitemapXMLFilter.Globals.documentLoaded) {
        return;
    }
    
    if(((e.keyCode == 0xA) || (e.keyCode == 0xD)) &&
        (SitemapXMLFilter.Globals.currentState == SitemapXMLFilter.Globals.tabStates.Web)) {
        
        SitemapXMLFilter.Web.WebController.Exclude();
    }
};

SitemapXMLFilter.ChangeUrlInput = function() {
    if(!SitemapXMLFilter.Globals.documentLoaded) {
        return;
    }
    
    SitemapXMLFilter.Web.WebController.UpdateUrlInput();
};

SitemapXMLFilter.CheckAll = function() {
    if(!SitemapXMLFilter.Globals.documentLoaded) {
        return;
    }
    
    switch(SitemapXMLFilter.Globals.currentState) {
        case SitemapXMLFilter.Globals.tabStates.LocalFile:
            SitemapXMLFilter.Local.LocalController.CheckAll();
            break;
        case SitemapXMLFilter.Globals.tabStates.Web:
            SitemapXMLFilter.Web.WebController.CheckAll();
            break;
        default:
            break;
    }
};

SitemapXMLFilter.UncheckAll = function() {
    if(!SitemapXMLFilter.Globals.documentLoaded) {
        return;
    }
    
    switch(SitemapXMLFilter.Globals.currentState) {
        case SitemapXMLFilter.Globals.tabStates.LocalFile:
            SitemapXMLFilter.Local.LocalController.UncheckAll();
            break;
        case SitemapXMLFilter.Globals.tabStates.Web:
            SitemapXMLFilter.Web.WebController.UncheckAll();
            break;
        default:
            break;
    }
};

SitemapXMLFilter.MakeGuid = function() {
    return "aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaaa".replace(/a/g, function ()
    {
        var rand = Math.random() * 16;
        rand = Math.floor(rand); //make sure we have a whole number
        
        return rand.toString(16);
    });
};

$(document).ready(function() {
    SitemapXMLFilter.Globals.tabObjs.push(SitemapXMLFilter.Local.LocalInterface);
    SitemapXMLFilter.Globals.tabObjs.push(SitemapXMLFilter.Web.WebInterface);
    
    SitemapXMLFilter.Local.LocalController.Init();
    SitemapXMLFilter.Web.WebController.Init();
    
    SitemapXMLFilter.Globals.documentLoaded = true;
});