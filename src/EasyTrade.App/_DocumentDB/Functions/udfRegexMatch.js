/// <reference path="../../typings/globals/documentdb-server/index.d.ts" />
function udfRegexMatch(input, regExPattern) {
    return input !== null
        && input.match(regExPattern);
}
