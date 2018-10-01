var ClassDiagramFilter = {};

ClassDiagramFilter.Globals = {
    /** @type {Array<{SetTabVisibility(visible: boolean): void}>} */
    tabObjs: [],
    tab: 1,
    tabEnums: {
        Local: 0,
        Github: 1
    },

    acceptedExtensionArray: [".adb", ".ads", ".Ada", ".asp", ".asa", ".aspx", ".bet",
        ".cbl", ".cob", ".CBL", ".COB", ".d", ".di", ".e", ".fal", ".ftd", ".f", ".for",
        ".ftn", ".f03", ".f08", ".f15", ".go", ".js", ".jsx", ".cl", ".clisp", ".el",
        ".l", ".lisp", ".lsp", ".lua", ".p", ".pp", ".pas", ".pl", ".pm", ".ph", ".plx",
        ".perl", ".p6", ".pm6", ".pl6", ".php", ".php4", ".php5", ".php7", ".phtml",
        ".py", ".pyx", ".pxd", ".pxi", ".scons", ".r", ".R", ".s", ".q", ".rb", ".ruby",
        ".rs", ".tcl", ".tk", ".wish", ".exp", ".tex", ".cs", ".c++", ".cc", ".cp",
        ".cpp", ".cxx", ".h", ".h++", ".hh", ".hpp", ".hxx", ".m", ".mm", ".inl", ".java"],

    maxBlockSize: 65000,

    documentLoaded: false
};


ClassDiagramFilter.ExcludeByClass = function() {
    if(!ClassDiagramFilter.Globals.documentLoaded) {
        return;
    }

};

ClassDiagramFilter.Submit = function() {
    if(!ClassDiagramFilter.Globals.documentLoaded) {
        return;
    }

    switch(ClassDiagramFilter.Globals.tab) {
        case ClassDiagramFilter.Globals.tabEnums.Github:
            ClassDiagramFilter.Github.GithubController.Submit();
            break;
        case ClassDiagramFilter.Globals.tabEnums.Local:
            ClassDiagramFilter.Local.LocalController.Submit();
            break;
        default:
            break;
    }
};

ClassDiagramFilter.SetTab = function(tabNum) {
    if(!ClassDiagramFilter.Globals.documentLoaded) {
        return;
    }

    for(var i = 0; i < ClassDiagramFilter.Globals.tabObjs.length; i++) {
        if(i == tabNum) {
            ClassDiagramFilter.Globals.tabObjs[i].SetTabVisibility(true);
        }
        else {
            ClassDiagramFilter.Globals.tabObjs[i].SetTabVisibility(false);
        }
    }

    ClassDiagramFilter.Globals.tab = tabNum;
};

ClassDiagramFilter.SetAppIdByEnvironment = function() {
    switch("SDENVIRONMENT") {
       case "PRD":
            ClassDiagramFilter.Github.GithubController.appId = "<Your App ID>";
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
    //ClassDiagramFilter.submitBtn = document.getElementById("SubmitBtn"); --temp--

    ClassDiagramFilter.Globals.tabObjs.push(ClassDiagramFilter.Local.LocalInterface);
    ClassDiagramFilter.Globals.tabObjs.push(ClassDiagramFilter.Github.GithubInterface);

    //Github
    ClassDiagramFilter.Github.GithubController.Init();

    //Local
    ClassDiagramFilter.Local.LocalController.Init();

    ClassDiagramFilter.SetAppIdByEnvironment();

    ClassDiagramFilter.Globals.documentLoaded = true;
});