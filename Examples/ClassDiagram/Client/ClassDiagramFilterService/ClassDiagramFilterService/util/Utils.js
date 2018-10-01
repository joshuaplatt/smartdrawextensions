ClassDiagramFilter.Utils = {};

ClassDiagramFilter.Utils.CreateLocalExport = function(nameArray, buffArray) {
    var data;
    var header;
    var lengths = [];
    var totalLength = 0;
    var headerString;
    var byteOffset;
    var i;
    
    for(i = 0; i < buffArray.length; i++) {
        totalLength += buffArray[i].byteLength;
        lengths.push(buffArray[i].byteLength);
    }
    
    headerString = nameArray.length.toString();
    headerString += "\t" + nameArray.join("\t") + "\n";
    headerString += lengths.join("\t") + "\n";
    
    byteOffset = headerString.length << 1;
    totalLength += byteOffset;
    
    data = new Uint8Array(totalLength);
    header = new Uint16Array(data.buffer, 0, headerString.length);
    
    for(i = 0; i < headerString.length; i++) {
        header[i] = headerString.charCodeAt(i);
    }
    
    for(i = 0; i < buffArray.length; i++) {
        data.set(buffArray[i], byteOffset);
        byteOffset += buffArray[i].byteLength;
    }
    
    return data;
};

ClassDiagramFilter.Utils.JstreeState = {
    iteration: 0
};

ClassDiagramFilter.Utils.IsOnEdge = (function() {
    return navigator.userAgent.indexOf("Edge") != -1;
})();
