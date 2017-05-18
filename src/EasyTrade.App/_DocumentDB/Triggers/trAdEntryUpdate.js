function trAdEntryUpdate() {
    var ctx = getContext();
    var req = ctx.getRequest();
    var doc = __.request.getBody();
    applyChanges(doc);
    function applyChanges(doc) {
        /// <summary>Keeps track of the changes of the AdEntry to be updated.</summary>
        /// <param name="doc">The document.</param>
        var existingDocument = __.filter(function (d) {
            return d["id"] === doc.id;
        }, function (err, feed, options) {
            if (err)
                throw err;
            if (feed && feed.length) {
                var existingDocument = feed[0];
                if (existingDocument) {
                    var changeSet = getChangeSet(existingDocument, doc);
                    if (changeSet) {
                        doc.ChangeSets = doc.ChangeSets || [];
                        doc.ChangeSets.push({
                            TimeStamp: new Date().getTime(),
                            Changes: changeSet
                        });
                    }
                }
            }
            // Replace the document to include the changesets
            req.setBody(doc);
        });
    }
    function getChangeSet(existingDocument, doc) {
        var propertyNames = ['Title', 'Description', 'Price'];
        var changeSet = null;
        propertyNames.forEach(function (propertyName) {
            if (existingDocument[propertyName] !== doc[propertyName]) {
                changeSet = changeSet || {};
                changeSet[propertyName] = existingDocument[propertyName];
            }
        });
        return changeSet;
    }
}
