var isSubmitting = false;
var output = null;
var submitbtn = null;

let maxBlockSize = 65000;

function CtagsUploader() {
    this.files = [];
    this.buffs = [];
    this.names = [];
    this.lengths = [];
    
    this.data = null;
    this.guid = MakeGuid();
    
    this.totalLength = 0;
    this.amountSent = 0;
}

CtagsUploader.prototype.Upload = function() {
    if(!files || (this.files.length <= 0)) {
        output.innerText = "Please select files for uploading.";
        return;
    }
    
    var reader = new FileReader();
    reader.onloadend = this.HandleRead.bind(this);
    reader.readAsArrayBuffer(this.files[0]);
};

CtagsUploader.prototype.HandleRead = function(e) {
    this.names.push(this.files[this.buffs.length].name);
    this.buffs.push(new Uint8Array(e.target.result));
    this.lengths.push(e.target.result.byteLength);
    
    if(this.buffs.length < this.files.length) {
        var reader = new FileReader();
        reader.onloadend = this.HandleRead.bind(this);
        reader.readAsArrayBuffer(this.files[this.buffs.length]);
    }
    else {
        this.InitializeTransfer();
    }
};

CtagsUploader.prototype.InitializeTransfer = function() {
    this.CreateData();
    var dataLength = (this.totalLength > maxBlockSize) ? maxBlockSize : this.totalLength;
    var content = new Blob(this.data.subarray(0, dataLength));
    
    var request = {
        type: "POST",
        url: "../../filter/ctags-block-init?uploadID=" + this.guid,
        data: this.data.subarray(0, dataLength),
        contentType: "text/plain",
        success: function(data, status) {
            if(this.amountSent >= this.totalLength) {
                this.FinalizeBlobTransfer();
            }
            else {
                this.SendAdditionalBlob();
            }
        }.bind(this),
        error: function(data, status) {
            output.innerText = "Server returned with error code: " + (data.responseText != "") ? data.responseText : data.statusText;
            isSubmitting = false;
            submitbtn.disabled = false;
        }.bind(this),
        processData: false
    };
    
    this.amountSent += dataLength;
    $.ajax(request);
};

CtagsUploader.prototype.SendAdditionalBlob = function() {
    var dataLength = ((this.totalLength - this.amountSent) > maxBlockSize) ? maxBlockSize : 
        this.totalLength - this.amountSent;
    var content = new Blob(this.data.subarray(this.amountSent, dataLength + this.amountSent));
    
    var request = {
        type: "POST",
        url: "../../filter/ctags-block-add?uploadID=" + this.guid,
        data: this.data.subarray(this.amountSent, dataLength + this.amountSent),
        contentType: "text/plain",
        success: function(data, status) {
            if(this.amountSent >= this.totalLength) {
                this.FinalizeBlobTransfer();
            }
            else {
                this.SendAdditionalBlob();
            }
        }.bind(this),
        error: function(data, status) {
            output.innerText = "Server returned with error code: " + (data.responseText != "") ? data.responseText : data.statusText;
            isSubmitting = false;
            submitbtn.disabled = false;
        }.bind(this),
        processData: false
    };
    
    this.amountSent += dataLength;
    $.ajax(request);
};

CtagsUploader.prototype.FinalizeBlobTransfer = function() {
    var request = {
        type: "POST",
        url: "../../filter/ctags-block-final?uploadID=" + this.guid,
        success: function(data, status) {
            output.innerText = "Success!  Please wait while you are redirected.";
            output.innerText += "\n" + data;
            SDAPI.SDONPlugin.SDONDone(data, function(data, error) {    //SDone
                output.innerText = "This shouldn't display";
            });
        },
        error: function(data, status) {
            output.innerText = "Server returned with error code: " + (data.responseText != "") ? data.responseText : data.statusText;
            isSubmitting = false;
            submitbtn.disabled = false;
        }
    };
    
    $.ajax(request);
};

CtagsUploader.prototype.CreateData = function() {
    var headerString = this.files.length.toString() + "\t" + this.names.join("\t") + "\n" +
        this.lengths.join("\t") + "\n";
    var headerArray = new Uint16Array(headerString.length);
    var position = 0;
    var i = 0;
    this.totalLength = headerArray.byteLength;
    
    for(i = 0; i < this.lengths.length; i++) {
        this.totalLength += this.lengths[i];
    }
    
    for(i = 0; i < headerArray.length; i++) {
        headerArray[i] = headerString.charCodeAt(i);
    }
    
    this.data = new Uint8Array(this.totalLength);
    this.data.set(headerArray);
    position = headerArray.byteLength;
    
    for(i = 0; i < headerArray.length; i++) {
        this.data[(i * 2) + 1] = (headerArray[i] & 0xFF00) >> 8;
        this.data[i * 2] = headerArray[i] & 0xFF;
    }
    
    for(i = 0; i < this.buffs.length; i++) {
        this.data.set(this.buffs[i], position);
        position += this.buffs[i].byteLength;
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

function Submit() {
    if(!isSubmitting) {
        isSubmitting = true;
        submitbtn.disabled = true;
        
        var uploader = new CtagsUploader();
        uploader.files = document.getElementById("files").files;
        uploader.Upload();
    }
}

$(document).ready(function() {
    output = document.getElementById("outputText");
    submitbtn = document.getElementById("submitFiles");
});