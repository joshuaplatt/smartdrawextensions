SitemapXMLFilter.Utils = {};

SitemapXMLFilter.Utils.JstreeState = {
    iteration: 0
};

SitemapXMLFilter.Utils.IsOnEdge = (function() {
    return navigator.userAgent.indexOf("Edge") != -1;
})();
