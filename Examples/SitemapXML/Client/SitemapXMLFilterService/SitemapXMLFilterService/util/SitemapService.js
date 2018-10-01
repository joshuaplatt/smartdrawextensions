SitemapXMLFilter.Utils.SitemapService = (function() {
    function CSitemapService() {
        this.SitemapUpload = function(data, onSuccess, onFail, onStep, settings) {
            var params = ((typeof settings == "object") && settings) ? settings : {};
            var submission = {
                blob: new Uint8Array(data.buffer),
                tables: (params.tables == true),
                hyperlinks: (params.hyperlinks == true),
                lexicographicOrder: (params.lexicographicOrder == true),
                onSuccess: onSuccess,
                onFail: onFail,
                onStep: onStep,
                sent: 0,
                uploadId: SitemapXMLFilter.MakeGuid()
            };
            
            SitemapUploadBlock(submission);
        };
        
        var SitemapUploadBlock = function(submission) {
            var amountToSend = submission.blob.byteLength - submission.sent;
            
            if(amountToSend > SitemapXMLFilter.Globals.maxBlockSize) {
                amountToSend = SitemapXMLFilter.Globals.maxBlockSize;
            }
            
            var request = {
                type: "POST",
                url: "filter/sitemap-upload-block?uploadID=" + submission.uploadId,
                data: submission.blob.subarray(submission.sent,
                    submission.sent + amountToSend),
                contentType: "text/plain",
                processData: false,
                
                success: function(data, status) {
                    submission.onStep && submission.onStep(submission.sent,
                        submission.blob.byteLength);
                    
                    if(submission.sent < submission.blob.byteLength) {
                        SitemapUploadBlock(submission);
                    }
                    else {
                        SitemapUploadFinal(submission);
                    }
                },
                
                error: function(data, status) {
                    submission.onFail && submission.onFail(data);
                }
            };
            
            submission.sent += amountToSend;
            
            $.ajax(request);
        };
        
        var SitemapUploadFinal = function(submission) {
            var params = {
                tables: submission.tables,
                hyperlinks: submission.hyperlinks,
                lexicographicOrder: submission.lexicographicOrder
            };
            
            var request = {
                type: "POST",
                url: "filter/sitemap-upload-final?uploadID=" + submission.uploadId,
                data: JSON.stringify(params),
                contentType: "text/json",
                
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
    
    return new CSitemapService();
})();