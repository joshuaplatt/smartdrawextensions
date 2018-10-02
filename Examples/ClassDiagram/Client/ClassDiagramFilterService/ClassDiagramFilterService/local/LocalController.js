ClassDiagramFilter.Local = {};

ClassDiagramFilter.Local.LocalController = (function() {
    function CLocalController() {
        var self = this;
        /** @type {ClassDiagramFilter.Local.LocalInterface} */
        var Interface = null;
        /** @type {ClassDiagramFilter.Local.LocalService} */
        var Service = null;
        
        var isSubmitting = false;
        
        /** @type {ClassDiagramFilter.Utils.ClassExclusionUtil} */
        var classExclusion = null;
        
        this.isExcludingClasses = false;
        
        this.SetSubmitting = function(submitting) {
            isSubmitting = submitting;
            Interface.SetSubmitting(submitting);
        };
        
        this.ChangeFiles = function() {
            if(!ClassDiagramFilter.Globals.documentLoaded) {
                return;
            }
            
            Interface.ChangeFileText();
        };
        
        this.SetChecked = function(checked) {
            if(checked) {
                classExclusion && classExclusion.CheckAll();
            }
            else {
                classExclusion && classExclusion.UncheckAll();
            }
        };
        
        this.ExcludeByClass = function() {
            if(!ClassDiagramFilter.Globals.documentLoaded || isSubmitting) {
                return;
            }
            
            this.SetSubmitting(true);
            this.isExcludingClasses = true;
            Interface.SetClassExlusionLoadingState(true);
            
            var onSuccess = function(data) {
                try {
                    var sdon = JSON.parse(data);
                    classExclusion.BuildTreeFromSDON(sdon);
                }
                catch(e) {
                    onFail(e.message);
                }
            };
            
            var onStep = function(sent, total) {
                console.log("Upload progress: " +
                    ((total > 0) ? (sent / total * 100) : 100) + "%");
            };
            
            var onFail = function(data) {
                Interface.SetClassExlusionLoadingState(false);
                self.SetSubmitting(false);
                self.isExcludingClasses = false;
                
                var errstr = "Unknown error.";
                
                if(data) {
                    if(typeof data == "string") {
                        errstr = data;
                    }
                    if(data.responseJSON) {
                        errstr = data.responseJSON;
                    }
                    else if(data.responseText) {
                        errstr = data.responseText;
                    }
                }
                
                console.log("Error: " + errstr);
            };
            
            var onFileLoadSuccess = function(names, buffs) {
                Service.SubmitToService(names, buffs,
                    onSuccess, onFail, onStep, false, false);
            };
            
            var onFileLoadStep = function(loaded, total) {
                console.log("Read files: " + loaded + "/" + total);
            };
            
            LoadFiles(onFileLoadSuccess, onFail, onFileLoadStep);
        };
        
        this.Submit = function() {
            if(!ClassDiagramFilter.Globals.documentLoaded || isSubmitting) {
                return;
            }
            
            this.SetSubmitting(true);
            
            if(!this.isExcludingClasses) {
                SubmitFromFiles();
            }
            else {
                SubmitFromExclusion();
            }
        };
        
        var SubmitFromFiles = function() {
            var onSuccess = function(data) {
                SDAPI.SDON.SDONDone(data, function(data, err) {
                    SDAPI.SDON.CloseDialog();
                });
            };
            
            var onStep = function(sent, total) {
                console.log("Upload progress: " +
                    ((total > 0) ? (sent / total * 100) : 100) + "%");
            };
            
            var onFail = function(data) {
                self.SetSubmitting(false);
                console.log("Error when submitting: " + data);
            };
            
            var onFileLoadSuccess = function(names, buffs) {
                Service.SubmitToService(names, buffs, onSuccess, onFail, onStep,
                    Interface.GetIsHidingMethods(), Interface.GetIsHidingProperties());
            };
            
            var onFileLoadStep = function(loaded, total) {
                console.log("Read files: " + loaded + "/" + total);
            };
            
            LoadFiles(onFileLoadSuccess, onFail, onFileLoadStep);
        };
        
        var SubmitFromExclusion = function() {
            var sdon = classExclusion.GenerateSDONFromTree(Interface.GetIsHidingMethods(),
                Interface.GetIsHidingProperties());
            
            SDAPI.SDON.SDONDone(sdon, function(data, err) {
                SDAPI.SDON.CloseDialog();
            });
        };
        
        var LoadFiles = function(onSuccess, onFail, onStep) {
            var files = Interface.GetFiles();
            
            if(!files) {
                onFail && onFail("No files found.");
                return;
            }
            
            var filesLoaded = 0;
            var filesExpected = files.length;
            var fileBuffs = new Array(files.length);
            var fileNames = new Array(files.length);
            var fr = new FileReader();
            
            /** @param {FileReaderProgressEvent} e */
            var onFileLoad = function(e) {
                if(e.target.error) {
                    onFail && onFail("File error: " + e.target.error.message);
                    return;
                }
                
                fileBuffs[filesLoaded] = new Uint8Array(e.target.result);
                fileNames[filesLoaded] = files[filesLoaded].name;
                filesLoaded++;
                
                onStep && onStep(filesLoaded, filesExpected);
                
                if(filesLoaded < filesExpected) {
                    fr.readAsArrayBuffer(files[filesLoaded]);
                }
                else {
                    onSuccess && onSuccess(fileNames, fileBuffs);
                }
            };
            
            fr.onloadend = onFileLoad;
            fr.readAsArrayBuffer(files[0]);
        };
        
        var ClassExclusionOnReadyCallback = function() {
            Interface.SetClassExlusionLoadingState(false);
            self.SetSubmitting(false);
        };
        
        this.Init = function() {
            ClassDiagramFilter.Local.LocalInterface.Init();
            Interface = ClassDiagramFilter.Local.LocalInterface;
            Service = ClassDiagramFilter.Local.LocalService;
            
            classExclusion = new ClassDiagramFilter.Utils.ClassExclusionUtil();
            classExclusion.Init("lfClassExclusionTree", ClassExclusionOnReadyCallback);
        };
    }
    
    return new CLocalController();
})();
