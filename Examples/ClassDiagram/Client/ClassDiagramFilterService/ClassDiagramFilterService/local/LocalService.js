ClassDiagramFilter.Local.LocalService = (function() {
    function CLocalService() {
        var self = this;
        
        this.SubmitToService = function(names, buffs, onSuccess, onFail, onStep, hideMethods, hideProperties) {
            if(names.length != buffs.length) {
                onFail("Error: Lengths of arrays do not match up.");
                return;
            }
            
            var submission = {
                blob: ClassDiagramFilter.Utils.CreateLocalExport(names, buffs),
                hideMethods: hideMethods ? "1" : "0",
                hideProperties: hideProperties ? "1" : "0",
                onSuccess: onSuccess,
                onFail: onFail,
                onStep: onStep,
                sent: 0,
                uploadId: MakeGuid()
            };
            
            SendTransferBlock(submission);
        };
        
        
        var SendTransferBlock = function(submission) {
            var amountToSend = submission.blob.byteLength - submission.sent;
            
            if(amountToSend > ClassDiagramFilter.Globals.maxBlockSize) {
                amountToSend = ClassDiagramFilter.Globals.maxBlockSize;
            }
            
            var request = {
                type: "POST",
                url: "filter/ctags-block-add?uploadID=" + submission.uploadId,
                data: submission.blob.subarray(submission.sent,
                    amountToSend + submission.sent),
                contentType: "text/plain",
                processData: false,
                
                success: function(data, status) {
                    submission.onStep && submission.onStep(submission.sent,
                        submission.blob.byteLength);
                    
                    if(submission.sent < submission.blob.byteLength) {
                        SendTransferBlock(submission);
                    }
                    else {
                        SubmitTransferFinal(submission);
                    }
                },
                
                error: function(data, status) {
                    submission.onFail && submission.onFail(data);
                }
            };
            
            submission.sent += amountToSend;
            $.ajax(request);
        };
        
        var SubmitTransferFinal = function(submission) {
            var request = {
                type: "POST",
                url: "filter/ctags-block-final?uploadID=" + submission.uploadId +
                "&hideMethods=" + submission.hideMethods +
                "&hideProperties=" + submission.hideProperties,
                
                success: function(data, status) {
                    submission.onSuccess && submission.onSuccess(data);
                },
                
                error: function(data, status) {
                    submission.onFail && submission.onFail(data);
                }
            };
            
            $.ajax(request);
        };
    }
    
    return new CLocalService();
})();
