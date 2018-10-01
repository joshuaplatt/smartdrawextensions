SitemapXMLFilter.Utils.ExclusionTree = function() {
    var self = this;
    
    /** @typedef {{text: string, id: string, state: {opened: boolean, selected: boolean}, children: [JstreeTemplateNode]}} JstreeTemplateNode */
    
    function ExclusionTreeState() {
        this.xmlHeader = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
            "<urlset>\n";
        this.xmlFooter = "</urlset>";
        
        /** @type {HTMLElement} */
        this.doc = null;
        
        /** @type {Array<{selected: boolean, parent: number, children: [number]}>} */
        this.nodeList = [];
        /** @type {Array<{url: string, nodes: [number]}>} */
        this.urlList = [];
        
        this.iteration = SitemapXMLFilter.Utils.JstreeState.iteration++;
        this.nodeId = 0;
        
        this.jstreeStruct = {
            core: {
                /** @type {[JstreeTemplateNode]} */
                data: []
            },
            plugins: [
                "checkbox"
            ]
        };
    };
    
    var id = "";
    var jstree = null;
    var jqtree = null;
    
    /** @type {ExclusionTreeState} */
    var state = null;
    
    var exportedString = "";
    
    /** 
     * Set this callback to register a function for when Jstree completes
     *   construction.
     * @type {function(): void}
     */
    this.JstreeReadyCallback = null;
    
    this.SortLexicographically = false;
    
    /**
     * Set this callback to register a function to get live updates for tree
     *   size.  (function(Number byteSize, Number nodeCount))
     * @type {function(Number, Number): void}
     */
    this.UpdateCheckedState = null;
    
    /** @param {XMLDocument} xml */
    this.CreateWithXML = function(xml) {
        if(jstree) {
            this.Destroy();
        }
        
        console.log("start");
        
        state = new ExclusionTreeState();
        state.doc = xml.documentElement;
        exportedString = "";
        
        if(state.doc.nodeName == "urlset") {
            state.xmlHeader = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
                "<urlset";
            
            for(var i = 0; i < state.doc.attributes.length; i++) {
                state.xmlHeader += " " + state.doc.attributes[i].name;
                state.xmlHeader += "=\"" + state.doc.attributes[i].value + "\"";
            }
            
            state.xmlHeader += ">\n";
            
            ParseUrlset();
        }
        else if(state.doc.nodeName == "sitemapindex") {
            throw "Error: Sitemapindexes are not allowed, compile sitemaps " +
                "into the same XML sitemap file.";
        }
        else {
            throw "Error: Invalid sitemap";
        }
        
        console.log("jstree start");
        
        if(this.SortLexicographically && state.jstreeStruct.core.data.length) {
            SortTemplateNodesRecursive(state.jstreeStruct.core.data[0]);
        }
        
        jqtree = $("#" + id);
        
        jqtree.on("ready.jstree", function() {
            console.log("jstree cb");
            
            var dataArr = jstree._model.data;
            for(var node in dataArr) {
                if(dataArr[node].parents.length == 2) {
                    jstree.close_node(node);
                }
            }
            
            var size = UpdateSizeInfo();
            var nodeCount = CountActiveNodes();
            
            self.UpdateCheckedState && self.UpdateCheckedState(size, nodeCount);
            
            console.log("closed nodes");
            if(self.JstreeReadyCallback) {
                self.JstreeReadyCallback();
            }
            
            if(SitemapXMLFilter.isOnEdge) {
                jstree.refresh();
            }
        });
        
        jqtree.on("select_node.jstree", OnSelect);
        jqtree.on("deselect_node.jstree", OnDeselect);
        
        jstree = jqtree.jstree(state.jstreeStruct).jstree();
        
        console.log("jstree end");
    };
    
    this.ExportSitemap = function() {
        var isValid;
        var i, j;
        var url;
        exportedString = state.xmlHeader;
        
        for(i = 0; i < state.urlList.length; i++) {
            isValid = true;
            url = state.urlList[i];
            
            for(j = 0; j < url.nodes.length; j++) {
                if(!state.nodeList[url.nodes[j]].selected) {
                    isValid = false;
                    break;
                }
            }
            
            if(isValid) {
                exportedString += url.url;
            }
        }
        
        exportedString += state.xmlFooter;
        
        return exportedString;
    };
    
    this.CheckAll = function() {
        for(var i = 0; i < state.nodeList.length; i++) {
            state.nodeList[i].selected = true;
        }
        
        var size = UpdateSizeInfo();
        var nodeCount = CountActiveNodes();
        
        jstree && jstree.check_all();
        
        this.UpdateCheckedState && this.UpdateCheckedState(size, nodeCount);
    };
    
    this.UncheckAll = function() {
        for(var i = 0; i < state.nodeList.length; i++) {
            state.nodeList[i].selected = false;
        }
        
        var size = UpdateSizeInfo();
        var nodeCount = CountActiveNodes();
        
        jstree && jstree.uncheck_all();
        
        this.UpdateCheckedState && this.UpdateCheckedState(size, nodeCount);
    };
    
    this.Destroy = function() {
        if(jstree) {
            jstree.destroy();
            jstree = null;
        }
        
        urls = [];
        xmlHeader = "";
    };
    
    var ParseUrlset = function() {
        var child;
        
        for(var i = 0; i < state.doc.childNodes.length; i++) {
            child = state.doc.childNodes[i];
            
            if(child.nodeName == "url") {
                ParseUrl(child);
            }
        }
    };
    
    /** @param {Node} url */
    var ParseUrl = function(url) {
        var child, loc, str;
        
        for(var i = 0; i < url.childNodes.length; i++) {
            child = url.childNodes[i];
            
            if(child.nodeName == "loc") {
                loc = child.firstChild.data;
                break;
            }
        }
        
        if(loc) {
            str = SerializeXML(url) + "\n";
            InsertUrl(loc, str);
        }
    };
    
    /** 
     * @param {String} url 
     * @param {String} serial
     * */
    var InsertUrl = function(url, serial) {
        var urlStartIndex = url.indexOf("://");
        var splitStr = [];
        /** @type {JstreeTemplateNode} */
        var template;
        var i, j;
        var wasFound;
        var parentNodeIndex = 0;
        
        var urlNode = {
            url: serial,
            nodes: [0]
        };
        
        if(urlStartIndex == -1) {
            urlStartIndex = 0;
        }
        else {
            urlStartIndex += 3;
        }
        
        splitStr = url.substring(urlStartIndex).split("/", 64);
        if(splitStr.length == 0) {
            return;
        }
        
        if(state.jstreeStruct.core.data.length == 0) {
            state.jstreeStruct.core.data.push({
                text: splitStr[0],
                children: [],
                id: "SMid" + state.iteration + "#",
                state: {opened: true, selected: true}
            });
            
            state.nodeList.push({
                selected: true,
                parent: -1,
                children: []
            });
            
            state.nodeId++;
        }
        
        template = state.jstreeStruct.core.data[0];
        
        for(i = 1; i < splitStr.length; i++) {
            wasFound = false;
            if(splitStr[i] == "") {
                break;
            }
            
            if(template.children) {
                for(j = 0; j < template.children.length; j++) {
                    if(template.children[j].text == splitStr[i]) {
                        wasFound = true;
                        template = template.children[j];
                        break;
                    }
                }
            }
            
            if(!wasFound) {
                template = InsertNode(splitStr[i], template,
                    parentNodeIndex, (i == 1) ? true : false);
                parentNodeIndex = state.nodeList.length - 1;
            }
            else {
                parentNodeIndex = state.nodeList[parentNodeIndex].children[j];
            }
            
            urlNode.nodes.push(parentNodeIndex);
        }
        
        state.urlList.push(urlNode);
    };
    
    /**
     * 
     * @param {string} urlFragment 
     * @param {{text: string, id: string, state: {selected: boolean}}} parentTemplate 
     * @param {number} parentNodeIndex 
     * @param {boolean} opened
     * @return {JstreeTemplateNode}
     */
    var InsertNode = function(urlFragment, parentTemplate,
        parentNodeIndex, opened) {
        var newTemplate = {
            text: urlFragment,
            id: "SMid" + state.iteration + "-" + state.nodeId,
            state: {selected: true, opened: opened}
        };
        
        var newNode = {
            selected: true,
            parent: parentNodeIndex,
            children: []
        };
        
        var nodeid = state.nodeList.length;
        
        state.nodeId++;
        state.nodeList[parentNodeIndex].children.push(nodeid);
        state.nodeList.push(newNode);
        
        if(!parentTemplate.children) {
            parentTemplate.children = [];
        }
        
        parentTemplate.children.push(newTemplate);
        
        return newTemplate;
    };
    
    /** @param {Node} xml */
    var SerializeXML = function(xml) {
        var str = "";
        
        switch(xml.nodeType) {
            case Node.ELEMENT_NODE:
                str = SerializeElement(xml);
                break;
            case Node.TEXT_NODE:
                str = SerializeText(xml);
                break;
            case Node.CDATA_SECTION_NODE:
                str = "<![CDATA[";
                str += SerializeText(xml);
                str += "]]>";
                break;
            default:
                break;
        }
        
        return str;
    };
    
    /** @param {HTMLElement} el */
    var SerializeElement = function(el) {
        var str = "<" + el.nodeName;
        var i;
        
        for(i = 0; i < el.attributes.length; i++) {
            //if(el.attributes[i].nodeName != "xmlns") {
                str += " " + el.attributes[i].nodeName + "=\"" +
                    el.attributes[i].value + "\"";
            //}
        }
        
        if(el.childNodes && el.childNodes.length) {
            str += ">";
            
            for(i = 0; i < el.childNodes.length; i++) {
                str += SerializeXML(el.childNodes[i]);
            }
            
            str += "</" + el.nodeName + ">";
        }
        else {
            str += "/>";
        }
        
        return str;
    };
    
    /** @param {Text} text */
    var SerializeText = function(text) {
        var str = text.nodeValue;
        return str;
    };
    
    /** @param {JstreeTemplateNode} node */
    var SortTemplateNodesRecursive = function(node) {
        if(node.children) {
            for(var i = 0; i < node.children.length; i++) {
                SortTemplateNodesRecursive(node.children[i]);
            }
            
            node.children.sort(CompareTemplateNodes);
        }
    };
    
    var CompareTemplateNodes = function(x, y) {
        return x.text.localeCompare(y.text);
    };
    
    var GetIndexFromID = function(id) {
        var i = id.indexOf('-');
        if(i == -1) {
            return -1;
        }
        
        return parseInt(id.substring(i + 1));
    };
    
    /** @param {{event: Event, instance, node, selected: []}} info */
    var OnSelect = function(jnode, info) {
        var index = GetIndexFromID(info.node.id);
        var size;
        var nodeCount;
        
        if(index == -1) {
            index = 0;
        }
        
        UpdateSelectionRecursive(index, true);
        size = UpdateSizeInfo();
        nodeCount = CountActiveNodes();
        
        self.UpdateCheckedState && self.UpdateCheckedState(size, nodeCount);
    };
    
    /** @param {{event: Event, instance, node, selected: []}} info */
    var OnDeselect = function(node, info) {
        var index = GetIndexFromID(info.node.id);
        var size;
        var nodeCount;
        
        if(index == -1) {
            index = 0;
        }
        
        UpdateSelectionRecursive(index, false);
        size = UpdateSizeInfo();
        nodeCount = CountActiveNodes();
        
        self.UpdateCheckedState && self.UpdateCheckedState(size, nodeCount);
    };
    
    var UpdateSelectionRecursive = function(nodeIndex, select) {
        if(nodeIndex == -1) {
            return;
        }
        
        var node = state.nodeList[nodeIndex];
        var i;
        
        node.selected = select;
        
        if(select) {
            var parentIndex = node.parent;
            var count = 0;
            
            while(parentIndex != -1) {
                if(!state.nodeList[parentIndex].selected) {
                    state.nodeList[parentIndex].selected = true;
                }
                
                parentIndex = state.nodeList[parentIndex].parent;
                
                if(count++ > 1e6) {
                    throw "Size update calculation error.";
                }
            }
        }
        
        for(i = 0; i < node.children.length; i++) {
            if(state.nodeList[node.children[i]].selected != select) {
                UpdateSelectionRecursive(node.children[i], select);
            }
        }
        
        if(!select && (node.parent != -1)) {
            var parentDisable = true;
            var parentNode = state.nodeList[node.parent];
            
            for(i = 0; i < parentNode.children.length; i++) {
                if(state.nodeList[parentNode.children[i]].selected) {
                    parentDisable = false;
                    break;
                }
            }
            
            if(parentDisable) {
                UpdateSelectionRecursive(node.parent, false);
            }
        }
    };
    
    var UpdateSizeInfo = function() {
        var i, j;
        var isValid;
        var url;
        var size = state.xmlHeader.length + state.xmlFooter.length;
        
        for(i = 0; i < state.urlList.length; i++) {
            isValid = true;
            url = state.urlList[i];
            
            for(j = 0; j < url.nodes.length; j++) {
                if(!state.nodeList[url.nodes[j]].selected) {
                    isValid = false;
                    break;
                }
            }
            
            if(isValid) {
                size += url.url.length;
            }
        }
        
        return size;
    };
    
    var CountActiveNodes = function() {
        var nodeCount = 0;
        
        for(var i = 0; i < state.nodeList.length; i++) {
            if(state.nodeList[i].selected) {
                nodeCount++;
            }
        }
        
        return nodeCount;
    };
    
    this.Init = function(divId) {
        id = divId;
    };
};
