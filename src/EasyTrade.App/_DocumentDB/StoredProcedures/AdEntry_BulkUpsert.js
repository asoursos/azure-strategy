function adEntryBulkUpsert(docs) {
    var collection = getContext().getCollection();
    var collectionLink = collection.getSelfLink();

    // The count of imported docs, also used as current doc index.
    var count = 0;

    // Validate input.
    if (!docs) 
        throw new Error("The array is undefined or null.");

    var docsLength = docs.length;
    if (docsLength === 0) {
        getContext().getResponse().setBody(0);
    }

    // Call the create API to create a document.
    tryCreate(docs[count], callback);

    // Note that there are 2 exit conditions:
    // 1) The createDocument request was not accepted. 
    //    In this case the callback will not be called, we just call setBody and we are done.
    // 2) The callback was called docs.length times.
    //    In this case all documents were created and we don’t need to call tryCreate anymore. Just call setBody and we are done.
    function tryCreate(doc, callback) {
        var isAccepted = collection.createDocument(collectionLink, doc, callback);

        // If the request was accepted, callback will be called.
        // Otherwise report current count back to the client, 
        // which will call the script again with remaining set of docs.
        if (!isAccepted) getContext().getResponse().setBody(count);
    }

    function tryUpsert(doc, callback) {
        var isFilterAccepted = collection.filter(function (d) {
            return d.id === doc.id;
        }, function (err, feed, options) {
            if (err)
                throw err;

            if (!feed || !feed.length) {
                tryCreate(doc, callback);
            }
            else {
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
                var documentLink = existingDocument.getSelfLink();

                var isAccepted = collection.replaceDocument(documentLink, doc, callback);

                // If the request was accepted, callback will be called.
                // Otherwise report current count back to the client, 
                // which will call the script again with remaining set of docs.
                if (!isAccepted) 
                    getContext().getResponse().setBody(count);
            }
        });

        // If the request was accepted, callback will be called.
        // Otherwise report current count back to the client, 
        // which will call the script again with remaining set of docs.
        if (!isFilterAccepted) 
            getContext().getResponse().setBody(count);
    }

    // This is called when collection.createDocument is done in order to process the result.
    function callback(err, doc, options) {
        if (err) throw err;

        // One more document has been inserted, increment the count.
        count++;

        if (count >= docsLength) {
            // If we created all documents, we are done. Just set the response.
            getContext().getResponse().setBody(count);
        } else {
            // Create next document.
            tryUpsert(docs[count], callback);
        }
    }

    function getChangeSet(existingDocument, doc) {
        var changeSet = null;

        if (existingDocument['Title'] !== doc['Title']) {
            changeSet = changeSet || {};
            changeSet['Title'] = existingDocument['Title'];
        }

        if (existingDocument['Description'] !== doc['Description']) {
            changeSet = changeSet || {};
            changeSet['Description'] = existingDocument['Description'];
        }

        if (existingDocument['Price'] !== doc['Price']) {
            changeSet = changeSet || {};
            changeSet['Price'] = existingDocument['Price'];
        }

        return changeSet;
    }
}