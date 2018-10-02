ClassDiagramFilter.Utils.ClassExclusionUtil = function() {
    function ClassExclusionState() {
        this.nodeId = 0;
        this.iteration = ClassDiagramFilter.Utils.JstreeState.iteration;
        ClassDiagramFilter.Utils.JstreeState.iteration++;
        
        this.sdon = {};
        this.finalsdon = {};
        
        this.nodeArray = [];
        this.undetermined = [];
        
        this.jstreeStruct = {
            core: {
                data: [
                    {
                        id: "CEid" + this.iteration + "#",
                        text: "Class List",
                        state: { opened: true },
                        children: []
                    }
                ]
            },
            plugins: [
                "checkbox"
            ]
        }
    }
    
    var tree = new ClassDiagramFilter.Utils.JstreeUtil();
    
    /** @type {ClassExclusionState} */
    var state = null;
    
    var model = {};
    
    this.BuildTreeFromSDON = function(sdon) {
        if(!(sdon && (typeof sdon.Shape == "object"))) {
            return;
        }
        
        state = new ClassExclusionState();
        var root = state.jstreeStruct.core.data[0];
        state.sdon = sdon;
        
        if(sdon.Shape && sdon.Shape.ShapeContainer && sdon.Shape.ShapeContainer.Shapes) {
            SearchShapesForClass(sdon.Shape.ShapeContainer.Shapes, root);
            //SearchShapeContainerRowForClass(sdon.Shape, root);
        }
        
        tree.CreateTree(state.jstreeStruct);
    };
    
    this.GenerateSDONFromTree = function(hideMethods, hideProperties) {
        if(!state) {
            return "";
        }
        
        state.undetermined = tree.GetUndetermined();
        model = tree.GetModel();
        var i, index;
        
        for(i = 0; i < state.undetermined.length; i++) {
            index = GetIndexFromID(state.undetermined[i]);
            
            if(index > -1) {
                state.nodeArray[index].checked = true;
            }
        }
        
        CheckAllNodesInTree("#", hideMethods, hideProperties);
        
        for(i = 1; i < state.nodeArray.length; i++) {
            if(!state.nodeArray[i].checked) {
                RemoveEntry2(state.nodeArray[i]);
                //RemoveEntry(state.nodeArray[i]);
            }
        }
        
        return JSON.stringify(state.sdon);
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
    
    var SearchShapeContainerRowForClass = function(shape, parent) {
        if(!shape.ShapeContainer || !shape.ShapeContainer.Shapes) {
            return;
        }
        
        var childShape;
        
        for(var i = 0; i < shape.ShapeContainer.Shapes.length; i++) {
            childShape = shape.ShapeContainer.Shapes[i];
            
            //SearchShapeContainerColumnForClass(childShape, parent, shape);
        }
    };
    
    var SearchShapeContainerColumnForClass = function(shape, parent, parentRow) {
        if(!shape || !shape.ShapeContainer || !shape.ShapeContainer.Shapes) {
            return;
        }
        
        var childNode;
        
        if(shape.ShapeContainer.Shapes.length == 1) {
            //InsertNodeToTree(shape.ShapeContainer.Shapes[0], parent, parentRow, false);
        }
        else if(shape.ShapeContainer.Shapes.length == 2) {
            //childNode = InsertNodeToTree(shape.ShapeContainer.Shapes[0], parent, parentRow, true);
            
            //SearchShapeContainerRowForClass(shape.ShapeContainer.Shapes[1], childNode);
        }
    };
    
    var SearchShapesForClass = function(shapes, parent) {
        var childShape;
        var node;
        
        for(var i = 0; i < shapes.length; i++) {
            childShape = shapes[i];
            
            if(childShape.ShapeConnector) {
                node = InsertNodeToTree2(childShape, parent, shapes, true);
                SearchShapesForClass(childShape.ShapeConnector[0].Shapes, node);
            }
            else {
                InsertNodeToTree2(childShape, parent, shapes);
            }
        }
    };
    
    var InsertNodeToTree2 = function(shape, parent, parentShapes, hasChildren) {
        var node;
        var parentId = GetIndexFromID(parent.id);
        
        if(!shape || !shape.Table) {
            return null;
        }
        
        if(!shape.Table.Cells && shape.Table.Cell) {
            //checking for use of deprecated "Cell" array.
            shape.Table.Cells = shape.Table.Cell;
            shape.Table.Cell = null;
        }
        
        if(!shape.Table.Cells || !(shape.Table.Cells.length > 0)) {
            return null;
        }
        
        if(!hasChildren) {
            node = {
                id: "CEid" + state.iteration + "-" + state.nodeId,
                text: shape.Table.Cells[0].Label,
                state: { selected: true }
            };
        }
        else {
            node = {
                id: "CEid" + state.iteration + "-" + state.nodeId,
                text: shape.Table.Cells[0].Label,
                state: { opened: true, selected: true },
                children: []
            };
        }
        
        state.nodeArray.push({
            Shape: shape,
            ParentId: parentId,
            ParentShapes: parentShapes,
            checked: false
        });
        
        parent.children.push(node);
        shape.filter_intern_id = node.id;
        state.nodeId++;
        
        return node;
    };
    
    //old functionality, use InsertNodeToTree2 (remove this)
    var InsertNodeToTree = function(shape, parent, parentRow, hasChildren) {
        var node;
        var parentId = GetIndexFromID(parent.id);
        
        if(!shape || !shape.Table) {
            return null;
        }
        
        if(!shape.Table.Cells && shape.Table.Cell) {
            //checking for use of deprecated "Cell" array.
            shape.Table.Cells = shape.Table.Cell;
        }
        
        if(!shape.Table.Cells || !(shape.Table.Cells.length > 0)) {
            return null;
        }
        
        if(!hasChildren) {
            node = {
                id: "CEid" + state.iteration + "-" + state.nodeId,
                text: shape.Table.Cells[0].Label,
                state: { selected: true }
            };
        }
        else {
            node = {
                id: "CEid" + state.iteration + "-" + state.nodeId,
                text: shape.Table.Cells[0].Label,
                state: { opened: true, selected: true },
                children: []
            };
        }
        
        state.nodeArray.push({
            Shape: shape,
            ParentId: parentId,
            ParentRow: parentRow,
            checked: false
        });
        
        parent.children.push(node);
        shape.filter_intern_id = node.id;
        state.nodeId++;
        
        return node;
    };
    
    var CheckAllNodesInTree = function(node, hideMethods, hideProperties) {
        var index;
        var val = model[node];
        
        if(node != "#") {
            index = GetIndexFromID(node);
            
            if((index > -1) && val.state.selected) {
                state.nodeArray[index].checked = true;
            }
            
            if((index > -1) && (hideMethods || hideProperties)) {
                HideSections(state.nodeArray[index].Shape, hideMethods, hideProperties);
            }
        }
        
        for(var i = 0; i < val.children.length; i++) {
            CheckAllNodesInTree(val.children[i], hideMethods, hideProperties);
        }
    };
    
    var HideSections = function(shape, hideMethods, hideProperties) {
        if(!shape || !shape.Table || !shape.Table.Cells) {
            return;
        }
        
        var index = 0;
        var endIndex;
        var newCells = [];
        var i;
        var isMethods = false, isProperties = false;
        var currCell, auxCell;
        var rowCount = 0;
        
        //if(!shape.Table.Join) {
        //    shape.Table.Join = [];
        //    
        //}
        //else if(shape.Table.Join.length) {
        //    shape.Table.Join = [shape.Table.Join[0]];
        //}
        
        while(index != -1) {
            i = index;
            endIndex = FindNextSection(shape.Table, index + 2);
            index = endIndex;
            endIndex = (endIndex == -1) ? shape.Table.Cells.length : endIndex;
            
            isMethods = (shape.Table.Cells[i].Label == "Methods");
            isProperties = (shape.Table.Cells[i].Label == "Properties");
            
            if((isMethods && hideMethods) || (isProperties && hideProperties)) {
                currCell = shape.Table.Cells[i];
                newCells.push(currCell);
                auxCell = shape.Table.Cells[i + 1];
                newCells.push(auxCell);
                
                rowCount++;
                
                currCell.Row = rowCount;
                auxCell.Row = rowCount;
                auxCell.Note = "";
                
                for(i += 2; i < endIndex; i += 2) {
                    auxCell.Note += shape.Table.Cells[i].Label;
                    
                    if((shape.Table.Cells.length > (i + 1)) &&
                        shape.Table.Cells[i + 1].Note) {
                        auxCell.Note += shape.Table.Cells[i + 1].Note;
                    }
                    
                    auxCell.Note += "\n";
                }
            }
            else {
                //shape.Table.Join.push({
                //    Row: shape.Table.Cells[i].Row,
                //    Column: 1,
                //    N: 1
                //});
                
                for(; i < endIndex; i++) {
                    if(shape.Table.Cells[i].Column == 1) {
                        rowCount++;
                    }
                    
                    shape.Table.Cells[i].Row = rowCount;
                    newCells.push(shape.Table.Cells[i]);
                }
            }
        }
        
        shape.Table.Cells = newCells;
        shape.Table.Rows = rowCount;
    };
    
    var FindNextSection = function(table, index) {
        for(var i = index; i < table.Cells.length; i += 2) {
            if(table.Cells[i].TextUnderline === false) {
                return i; //found section header
            }
        }
        
        return -1;
    };
    
    var RemoveEntry2 = function(entry) {
        var parent = (entry.ParentId > -1) ? state.nodeArray[entry.ParentId] : null;
        var parentShapes = entry.ParentShapes;
        var testShape;
        
        if(!parentShapes) {
            return;
        }
        
        for(var i = 0; i < parentShapes.length; i++) {
            testShape = parentShapes[i];
            
            if(testShape.filter_intern_id == entry.Shape.filter_intern_id) {
                parentShapes.splice(i, 1);
                
                if(parentShapes.length == 0) {
                    RemoveEntry2(parent);
                }
            }
        }
    };
    
    //old functionality, use RemoveEntry2 (remove this)
    var RemoveEntry = function(entry) {
        var parent = (entry.ParentId > -1) ? state.nodeArray[entry.ParentId] : null;
        var parentRow = entry.ParentRow;
        var testShape;
        
        if(!parentRow) {
            return;
        }
        
        for(var i = 0; i < parentRow.ShapeContainer.Shapes.length; i++) {
            testShape = parentRow.ShapeContainer.Shapes[i];
            
            if(testShape.ShapeContainer && testShape.ShapeContainer.Shapes && 
                ((testShape.ShapeContainer.Shapes.length == 1) ||
                (testShape.ShapeContainer.Shapes.length == 2))) {
                
                if(testShape.ShapeContainer.Shapes[0].filter_intern_id == entry.Shape.filter_intern_id) {
                    parentRow.ShapeContainer.Shapes.splice(i, 1);
                    
                    if(parentRow.ShapeContainer.Shapes.length == 0) {
                        //RemoveEntry(parent);
                    }
                }
            }
        }
    };
    
    var GetIndexFromID = function(id) {
        var i = id.indexOf('-');
        if(i == -1) {
            return -1;
        }
        
        return parseInt(id.substring(i + 1));
    };
    
    this.Init = function(divId, callback) {
        tree.Init(divId);
        
        if(callback) {
            tree.JstreeReadyCallback = callback;
        }
    };
};