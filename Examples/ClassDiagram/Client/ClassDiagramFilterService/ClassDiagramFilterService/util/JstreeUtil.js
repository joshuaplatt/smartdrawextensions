ClassDiagramFilter.Utils.JstreeUtil = function() {
    var self = this;
    
    var id = "";
    var jqtree = null;
    var jstree = null;
    
    /** 
     * Set this callback to register a function for when Jstree completes
     *   construction.
     * @type {function(): void} 
     */
    this.JstreeReadyCallback = null;
    
    /**
     * 
     * @param {{}} data 
     */
    this.CreateTree = function(data) {
        if(jstree) {
            this.Destroy();
        }
        
        jqtree = $("#" + id);
        
        jqtree.on("ready.jstree", function() {
            if(self.JstreeReadyCallback) {
                self.JstreeReadyCallback();
            }
            
            if(ClassDiagramFilter.Utils.IsOnEdge) {
                jstree.refresh();
            }
        });
        
        jstree = jqtree.jstree(data).jstree();
    };
    
    this.GetModel = function() {
        if(!jstree || !jstree._model || !jstree._model.data) {
            return null;
        }
        
        return jstree._model.data;
    };
    
    this.GetUndetermined = function() {
        return jstree ? jstree.get_undetermined() : null;
    };
    
    this.GetChecked = function() {
        return jstree ? jstree.get_checked() : null;
    };
    
    this.CheckAll = function() {
        jstree && jstree.check_all();
    };
    
    this.UncheckAll = function() {
        jstree && jstree.uncheck_all();
    };
    
    this.Destroy = function() {
        if(jstree) {
            jstree.destroy();
            jstree = null;
        }
    };
    
    this.Init = function(divId) {
        id = divId;
    };
};
