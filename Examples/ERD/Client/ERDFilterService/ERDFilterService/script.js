//Custom polyfill, mostly for IE
if(!Array.prototype.includes) {
    Object.defineProperty(Array.prototype, "includes", {
        value: function(searchElement, fromIndex) {
            return this.indexOf(searchElement, fromIndex) != -1;
        }
    });
}

var ERDFilter = {
    documentLoaded: false,
    isOnEdge: navigator.userAgent.indexOf("Edge") != -1
};

ERDFilter.LF = {
    isSubmitting: false,
    exclusion: null,
    outputText: null,
    fileInput: null,
    submitBtn: null,
    fileNameText: null,
    excludeTableBtn: null,
    checkAllBtn: null,
    uncheckAllBtn: null,
    
    showColumns: null,
    showTypes: null,
    showTypesLbl: null,
    
    treeOutputDiv: null,
    treeOutputOuterDiv: null,
    loading: null
};

ERDFilter.LF.ExclusionList = function() {
    this.file = null;
    this.tables = [];
    this.foreignKeys = [];
    this.jstree = null;
    this.parser = null;
    
    this.fileEl = null;
    
    this.GenerateList = function(data) {
        this.file = data;
        
        var jqtree = null;
        var initJSTree = {
            checkbox: {
                
            },
            core: {
                data: []
            },
            plugins: [
                "checkbox"
            ]
        };
        
        var index;
        var i;
        
        this.parser = new ERDFilter.LF.CSVParser(this.file);
        this.parser.Parse();
        
        for(i = 1; i < this.parser.lines.length; i++) {
            if(this.parser.lines[i].length >= 2) {
                index = this.tables.indexOf(this.parser.lines[i][2]);
                
                if(index == -1) {
                    this.tables.push(this.parser.lines[i][2]);
                    
                    if((this.parser.lines[i].length >= 7) &&
                        (this.parser.lines[i][7] == "FOREIGN KEY")) {
                        this.foreignKeys.push(1);
                    }
                    else {
                        this.foreignKeys.push(0);
                    }
                }
                else if((this.parser.lines[i].length >= 7) &&
                    (this.parser.lines[i][7] == "FOREIGN KEY")) {
                    this.foreignKeys[index]++;
                }
            }
        }
        
        for(i = 0; i < this.tables.length; i++) {
            initJSTree.core.data.push({
                text: this.tables[i] + " (" + this.foreignKeys[i] + ")", 
                state: {selected: true}
            });
        }
        
        var localJstree = null;
        
        jqtree = $("#treeOutput").jstree(initJSTree);
        jqtree.on("ready.jstree", function() {
            ERDFilter.LF.treeOutputDiv.hidden = false;
            ERDFilter.LF.loading.classList.add("loader-hidden");
            
            if(ERDFilter.isOnEdge) {
                localJstree.refresh();
            }
        });
        
        jqtree.on("select_node.jstree", ERDFilter.ClearError);
        jqtree.on("deselect_node.jstree", ERDFilter.ClearError);
        
        this.jstree = jqtree.jstree();
        localJstree = this.jstree;
        
        ERDFilter.LF.checkAllBtn.disabled = false;
        ERDFilter.LF.uncheckAllBtn.disabled = false;
        ERDFilter.LF.checkAllBtn.classList.remove("plugin-button-disabled");
        ERDFilter.LF.uncheckAllBtn.classList.remove("plugin-button-disabled");
    };
    
    this.CreateData = function() {
        if(this.parser.lines.length < 1) {
            return "";
        }
        
        var finalData = this.file.substring(this.parser.lineStartPositions[0], 
            this.parser.lineStartPositions[1]);
        var i = 1;
        this.tables = [];
        var text = "";
        var startIndex = 0;
        
        for(var node in this.jstree._model.data) {
            if(this.jstree._model.data[node].state.selected) {
                text = this.jstree._model.data[node].text;
                startIndex = text.lastIndexOf("(") - 1;
                
                if(startIndex != -2) {
                    this.tables.push(text.substr(0, startIndex));
                }
            }
        }
        
        for(i = 1; i < this.parser.lines.length; i++) {
            if((this.parser.lines[i].length <= 2) || this.tables.includes(this.parser.lines[i][2])) {
                finalData += this.file.substring(this.parser.lineStartPositions[i], 
                    this.parser.lineStartPositions[i + 1]);
            }
        }
        
        return finalData;
    };
};

ERDFilter.LF.CSVParser = function(data) {
    this.data = data;
    
    this.lines = [];
    this.lineStartPositions = [0];
    
    this.currLine = [];
    this.currStr = "";
    this.position = 0;
    
    this.isInQuotes = false;
    this.mayBeDbQuote = false;
    
    this.Parse = function() {
        for(this.position = 0; this.position < this.data.length; this.position++) {
            if(this.isInQuotes) {
                this.parseInQuotes();
            }
            else {
                this.parseOutsideQuotes();
            }
        }
        
        if((this.currLine.length > 0) || (this.currStr.length > 0)) {
            this.currLine.push(this.currStr);
            this.lines.push(this.currLine);
            this.lineStartPositions.push(this.position);
        }
    };
    
    this.parseInQuotes = function() {
        switch(this.data[this.position]) {
            case '"':
                this.isInQuotes = false;
                this.mayBeDbQuote = true;
                break;
            default:
                this.currStr += this.data[this.position];
                break;
        }
    };
    
    this.parseOutsideQuotes = function() {
        switch(this.data[this.position]) {
            case '"':
                if(this.mayBeDbQuote) {
                    this.currStr += '"';
                }
                
                this.isInQuotes = true;
                break;
            case '\r':
                this.position++;
                if((this.position >= this.data.length) || (this.data[this.position] != "\n")) {
                    return;
                }
            case '\n':
                this.currLine.push(this.currStr);
                this.lines.push(this.currLine);
                this.lineStartPositions.push(this.position + 1)
                this.currLine = [];
                this.currStr = "";
                break;
            case ',':
                this.currLine.push(this.currStr);
                this.currStr = "";
                break;
            default:
                this.currStr += this.data[this.position];
                break;
        }
        
        this.mayBeDbQuote = false;
    };
};

ERDFilter.LF.SendPostToServer = function(data) {
    if(data.length > 65000) {   //Max size
        ERDFilter.OutputError("Database CSV too large: " + data.length.toString() +
            "/65000" + "\nExclude some tables to reduce diagram size.");
        ERDFilter.LF.isSubmitting = false;
        ERDFilter.LF.submitBtn.disabled = false;
        ERDFilter.LF.submitBtn.classList.remove("plugin-button-disabled");
        
        return;
    }
    
    var showColumnsState = ERDFilter.LF.showColumns.checked;
    var showTypesState = ERDFilter.LF.showTypes.checked;
    
    var showColumns = showColumnsState ? "1" : "0";
    var showTypes = (showTypesState && showColumnsState) ? "1" : "0";
    
    var request = {
        type: "POST",
        url: "filter/erd?showColumns=" + showColumns + "&showTypes=" + showTypes,
        data: data,
        contentType: "text/plain",
        success: function(data, status) {
            SDAPI.VS.VisualScriptDone(data, function(data, error) {    //SDone
                SDAPI.VS.CloseDialog();
            });
        },
        error: function(data, status) {
            ERDFilter.OutputError("Server returned with error code: " +
                ((data.responseText) ? data.responseText : data.statusText));
            ERDFilter.LF.isSubmitting = false;
            ERDFilter.LF.submitBtn.disabled = false;
            ERDFilter.LF.submitBtn.classList.remove("plugin-button-disabled");
        }
    };
    
    $.ajax(request);
};

ERDFilter.LF.ExcludeTables = function() {
    if(!ERDFilter.documentLoaded) {
        return;
    }
    
    if((ERDFilter.LF.fileInput.files) && (ERDFilter.LF.fileInput.files.length == 1)) {
        var fileReader = new FileReader();
        
        ERDFilter.LF.treeOutputDiv.hidden = true;
        ERDFilter.LF.loading.classList.remove("loader-hidden");
        ERDFilter.LF.excludeTableBtn.classList.remove("plugin-button-ready");
        
        fileReader.readAsText(ERDFilter.LF.fileInput.files[0]);
        
        fileReader.onloadend = function(file, e) {
            var data = this.result;
            
            if(ERDFilter.LF.exclusion && ERDFilter.LF.exclusion.jstree) {
                ERDFilter.LF.exclusion.jstree.destroy();
            }
            
            ERDFilter.LF.exclusion = new ERDFilter.LF.ExclusionList();
            ERDFilter.LF.exclusion.fileEl = file;
            ERDFilter.LF.exclusion.GenerateList(data);
            
            ERDFilter.ClearError();
        }.bind(fileReader, ERDFilter.LF.fileInput.files[0]);
    }
};

ERDFilter.LF.SubmitCSV = function() {
    if(!ERDFilter.documentLoaded) {
        return;
    }
    
    if(!ERDFilter.LF.isSubmitting) {
        ERDFilter.LF.isSubmitting = true;
        ERDFilter.LF.submitBtn.disabled = true;
        ERDFilter.LF.submitBtn.classList.add("plugin-button-disabled");
        
        if((ERDFilter.LF.fileInput.files) && (ERDFilter.LF.fileInput.files.length == 1)) {
            if(ERDFilter.LF.exclusion && (ERDFilter.LF.fileInput.files[0] == ERDFilter.LF.exclusion.fileEl)) {
                ERDFilter.LF.SendPostToServer(ERDFilter.LF.exclusion.CreateData());
            }
            else {
                var fileReader = new FileReader();
                fileReader.readAsText(ERDFilter.LF.fileInput.files[0]);
                
                fileReader.onloadend = function(e) {
                    var data = this.result;
                    ERDFilter.LF.SendPostToServer(data);
                };
            }
        }
        else {
            ERDFilter.LF.isSubmitting = false;
            ERDFilter.LF.submitBtn.disabled = false;
            ERDFilter.LF.submitBtn.classList.remove("plugin-button-disabled");
            ERDFilter.OutputError("Couldn't get file.");
        }
    }
};

ERDFilter.LF.SelectFile = function() {
    if(!ERDFilter.documentLoaded) {
        return;
    }
    
    ERDFilter.ClearError();
    
    if(ERDFilter.LF.fileInput.files && (ERDFilter.LF.fileInput.files.length > 0)) {
        ERDFilter.LF.fileNameText.innerText = ERDFilter.LF.fileInput.files[0].name;
        ERDFilter.LF.excludeTableBtn.classList.add("plugin-button-ready");
        ERDFilter.LF.excludeTableBtn.hidden = false;
    }
    else {
        ERDFilter.LF.fileNameText.innerText = "";
        ERDFilter.LF.excludeTableBtn.hidden = true;
    }
};

ERDFilter.LF.ExclusionCheckAll = function() {
    if(ERDFilter.LF.exclusion && ERDFilter.LF.exclusion.jstree) {
        ERDFilter.LF.exclusion.jstree.check_all();
        ERDFilter.ClearError();
    }
};

ERDFilter.LF.ExclusionUncheckAll = function() {
    if(ERDFilter.LF.exclusion && ERDFilter.LF.exclusion.jstree) {
        ERDFilter.LF.exclusion.jstree.uncheck_all();
        ERDFilter.ClearError();
    }
};

ERDFilter.LF.ShowColumnsChecked = function() {
    if(!ERDFilter.documentLoaded) {
        return;
    }
    
    if(ERDFilter.LF.showColumns.checked) {
        ERDFilter.LF.showTypes.disabled = false;
        ERDFilter.LF.showTypesLbl.classList.remove("disabled-text");
    }
    else {
        ERDFilter.LF.showTypes.disabled = true;
        ERDFilter.LF.showTypesLbl.classList.add("disabled-text");
    }
    //if(!ERDFilter.LF.showColumns.checked && ERDFilter.LF.showTypes.checked) {
    //    ERDFilter.LF.showTypes.checked = false;
    //}
};

ERDFilter.LF.ShowTypesChecked = function() {
    if(!ERDFilter.documentLoaded) {
        return;
    }
    
    //if(!ERDFilter.LF.showColumns.checked && ERDFilter.LF.showTypes.checked) {
    //    ERDFilter.LF.showColumns.checked = true;
    //}
};

ERDFilter.OutputError = function(err) {
    ERDFilter.LF.outputText.innerHTML = "<span style=\"font-weight: bold;\">Error:</span> " + err;
};

ERDFilter.ClearError = function() {
    ERDFilter.LF.outputText.innerHTML = "";
};

$(document).ready(function() {
    ERDFilter.LF.outputText = document.getElementById("outputText");
    ERDFilter.LF.fileInput = document.getElementById("file");
    ERDFilter.LF.submitBtn = document.getElementById("submit");
    ERDFilter.LF.fileNameText = document.getElementById("fileName");
    ERDFilter.LF.excludeTableBtn = document.getElementById("exclude");
    ERDFilter.LF.checkAllBtn = document.getElementById("checkAll");
    ERDFilter.LF.uncheckAllBtn = document.getElementById("uncheckAll");
    
    ERDFilter.LF.showColumns = document.getElementById("showColumns");
    ERDFilter.LF.showTypes = document.getElementById("showTypes");
    ERDFilter.LF.showTypesLbl = document.getElementById("showTypesLbl");
    
    ERDFilter.LF.treeOutputDiv = document.getElementById("treeOutput");
    ERDFilter.LF.treeOutputOuterDiv = document.getElementById("treeOutputOuter");
    ERDFilter.LF.loading = document.getElementById("loading");
    
    ERDFilter.documentLoaded = true;
});

//window.setInterval(function() {
//    //if the time has exceeded an amount w/out the document loading, have some error here
//}, 50);