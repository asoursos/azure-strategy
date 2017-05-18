/// <reference path="../../typings/globals/documentdb-server/index.d.ts" />
function uspAdEntryBulkUpsert(docs: Array<any>) {
    // The count of docs to be upserted, also used as current doc index.
    var count = 0;

    // Validate input.
    if (!docs)
        throw new Error("The array is undefined or null.");

    var docsLength = docs.length;
    if (docsLength == 0) {
        __.response.setBody(0);
    }

    // Call the create API to create a document.
    tryUpsert(docs[count], callback);

    // Note that there are 2 exit conditions:
    // 1) The createDocument request was not accepted. 
    //    In this case the callback will not be called, we just call setBody and we are done.
    // 2) The callback was called docs.length times.
    //    In this case all documents were created and we don’t need to call tryCreate anymore. Just call setBody and we are done.
    function tryCreate(doc, callback) {
        var isAccepted = __.createDocument(__.getSelfLink(), doc, callback);

        // If the request was accepted, callback will be called.
        // Otherwise report current count back to the client, 
        // which will call the script again with remaining set of docs.
        if (!isAccepted)
            __.response.setBody(count);
    }

    function tryUpsert(doc, callback) {
        var isFilterAccepted = __.filter(function (d) {
            return d['id'] === doc.id;
        }, function (err: IFeedCallbackError, feed: IDocumentMeta[], options: IFeedCallbackOptions) {
            if (err)
                throw err;

            // If document was not found, we try creating it
            if (!feed || !feed.length) {
                tryCreate(doc, callback);
            }
            else {
                // We keep track of the changes requested before updating the document.
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
                var documentLink = existingDocument._self;
                var isAccepted = __.replaceDocument(existingDocument._self, doc, callback);

                // If the request was accepted, callback will be called.
                // Otherwise report current count back to the client, 
                // which will call the script again with remaining set of docs.
                if (!isAccepted)
                    __.response.setBody(count);
            }
        });

        // If the request was accepted, callback will be called.
        // Otherwise report current count back to the client, 
        // which will call the script again with remaining set of docs.
        if (!isFilterAccepted)
            __.response.setBody(count);
    }

    // This is called when collection.createDocument or collection.replaceDocument is done in order to process the result.
    function callback(err: IRequestCallbackError, doc: any, options: IRequestCallbackOptions) {
        if (err)
            throw err;

        // One more document has been inserted, increment the count.
        count++;

        if (count >= docsLength) {
            // If we have processed all documents, we are done. Just set the response.
            __.response.setBody(count);
        } else {
            // Upsert next document.
            tryUpsert(docs[count], callback);
        }
    }

    function getChangeSet(existingDocument, doc) {
        var changeSet = null;
        var propertiesToTrack = ['Title', 'Description', 'Price'];

        propertiesToTrack.forEach(function (propertyName) {
            if (existingDocument[propertyName] !== doc[propertyName]) {
                changeSet = changeSet || {};
                changeSet[propertyName] = existingDocument[propertyName];
            }                
        });

        return changeSet;
    }
}