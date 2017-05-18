/// <reference path="../../typings/globals/documentdb-server/index.d.ts" />
function udfRegexMatch(input: string, regExPattern: string) {
    return input !== null
        && input.match(regExPattern);
}