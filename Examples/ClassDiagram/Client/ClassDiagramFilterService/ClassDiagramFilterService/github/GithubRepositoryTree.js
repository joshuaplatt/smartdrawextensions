ClassDiagramFilter.Github.GithubRepositoryTree = function() {
    function GithubRepositoryTreeState() {
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
    }
    
    var tree = new ClassDiagramFilter.Utils.JstreeUtil();
    
    /** @type {GithubRepositoryTreeState} */
    var state = null;
    
    var exclusionList = [];
    
    this.GenerateTree = function(rtree) {
        state = new GithubRepositoryTreeState();
        
        for(var i = 0; i < rtree.length; i++) {
            InsertNode(rtree[i]);
        }
        
        tree.CreateTree(state.jstreeStruct);
    };
    
    this.GenerateExclusionList = function() {
        if(!state) {
            return null;
        }
        
        exclusionList = [];
        state.undetermined = tree.GetUndetermined();
        state.model = tree.GetModel();
        
        GenerateExclusionFromNode("#");
        
        return exclusionList;
    };
    
    this.Destroy = function() {
        tree.Destroy();
    };
    
    this.CheckAll = function() {
        tree.CheckAll();
    };
    
    this.UncheckAll = function() {
        tree.UncheckAll();
    };
    
    var InsertNode = function(node) {
        var strSplit = node.path.split("/");
        var currNode = state.jstreeStruct.core.data[0];
        var tempNode = currNode;
        var i;
        
        for(i = 0; i < strSplit.length; i++) {
            currNode = tempNode;
            tempNode = FindNode(strSplit[i], currNode);
            
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
            
            if((tempExt.length > 1) &&
                (ClassDiagramFilter.Globals.acceptedExtensionArray.indexOf("." + 
                tempExt[tempExt.length - 1]) != -1)) {
                
                tempNode.state = {selected: true};
            }
        }
    };
    
    var FindNode = function(name, searchIndex) {
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
    
    var GenerateExclusionFromNode = function(nodeID) {
        var node = state.model[nodeID];
        var child = null;
        
        if(!node || !node.children) {
            return;
        }
        
        for(var i = 0; i < node.children.length; i++) {
            child = state.model[node.children[i]];
            
            if(child && !child.state.selected) {
                if(state.undetermined.indexOf(child.id) != -1) {
                    GenerateExclusionFromNode(node.children[i]);
                }
                else {
                    GenerateExclusionEntry(child);
                }
            }
        }
    };
    
    var GenerateExclusionEntry = function(node) {
        var entryName = node.text;
        var parents = node.parents;
        
        for(var i = 0; i < parents.length; i++) {
            if(parents[i] == "#") {
                break;
            }
            
            entryName = state.model[parents[i]].text + "/" + entryName;
        }
        
        exclusionList.push(entryName);
    };
    
    this.Init = function(divId, callback) {
        tree.Init(divId);
        
        if(callback) {
            tree.JstreeReadyCallback = callback;
        }
    };
};
