/// <reference path="../../node_modules/@types/react/index.d.ts" />
var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
var Pager = (function (_super) {
    __extends(Pager, _super);
    /**
     *
     */
    function Pager(props) {
        return _super.call(this, props) || this;
    }
    Pager.prototype.onPageClick = function (pageIndex) {
        var pageNumber = this.props.pageNumber;
        if (pageIndex === 'next') {
            pageNumber = Math.min(this.props.pageNumber + 1, this.getPageCount());
        }
        else if (pageIndex === 'prev') {
            pageNumber = Math.max(this.props.pageNumber - 1, 1);
        }
        else {
            pageNumber = parseInt(pageIndex);
        }
        if (pageNumber == this.props.pageNumber)
            return;
        console.log("Navigated to page:" + pageNumber);
        if (this.props.onPageChanged) {
            this.props.onPageChanged(pageNumber);
        }
    };
    Pager.prototype.range = function (start, count) {
        var numbers = new Array(count);
        for (var i = start; i < start + count; i++) {
            numbers.push(i);
        }
        return numbers;
    };
    Pager.prototype.getPageCount = function () {
        var pageCount = Math.ceil(this.props.totalCount / this.props.pageSize);
        return pageCount;
    };
    Pager.prototype.getPageTokens = function () {
        var pageCount = this.getPageCount();
        if (pageCount <= 10) {
            return this.range(1, pageCount).map(function (v) { return v.toString(); });
        }
        var pageNumber = this.props.pageNumber || 1;
        var pageTokens = new Array();
        pageTokens.push('1');
        for (var i = 2; i < pageCount; i++) {
            if (i <= pageNumber + 1 && i >= pageNumber - 1) {
                pageTokens.push(i.toString());
            }
            else if (pageNumber == 1 && i == 3) {
                pageTokens.push(i.toString());
                pageTokens.push("...");
            }
            else if (pageNumber == pageCount && i == pageCount - 2) {
                pageTokens.push("...");
                pageTokens.push(i.toString());
            }
            else if (i == pageNumber - 2 || i == pageNumber + 2) {
                pageTokens.push("...");
            }
        }
        pageTokens.push(pageCount.toString());
        return pageTokens;
    };
    Pager.prototype.render = function () {
        var _this = this;
        var pageCount = this.getPageCount();
        var pageLinks = this.getPageTokens().map(function (token, idx) {
            var pageElement = token === '...'
                ? React.createElement("span", null, token)
                : React.createElement("a", { href: "#", "data-page": token, onClick: _this.onPageClick.bind(_this, token) }, token);
            return (React.createElement("li", { key: idx, className: parseInt(token) == _this.props.pageNumber ? 'active' : null }, pageElement));
        });
        return (React.createElement("nav", { "aria-label": "..." },
            React.createElement("ul", { className: "pagination" },
                React.createElement("li", { className: this.props.pageNumber === 1 ? 'disabled' : null },
                    React.createElement("a", { href: "#", "data-page": "prev", "aria-label": "Previous", onClick: this.onPageClick.bind(this, 'prev') },
                        React.createElement("span", { "aria-hidden": "true" }, "\u00AB"))),
                pageLinks,
                React.createElement("li", { className: this.props.pageNumber === pageCount ? 'disabled' : null },
                    " ",
                    React.createElement("a", { href: "#", "data-page": "next", "aria-label": "Previous", onClick: this.onPageClick.bind(this, 'next') },
                        React.createElement("span", { "aria-hidden": "true" }, "\u00BB"))))));
    };
    return Pager;
}(React.Component));
/// <reference path="../../node_modules/@types/react/index.d.ts" />
var SearchBox = (function (_super) {
    __extends(SearchBox, _super);
    function SearchBox(props) {
        var _this = _super.call(this, props) || this;
        _this.state = { query: '', includeInactive: false, hasReviews: false };
        return _this;
    }
    SearchBox.prototype.onSearchRequested = function (e) {
        console.log('SEARCH REQUESTED;');
        if (this.props.onSearch) {
            this.props.onSearch(this.state.query, this.state.includeInactive, this.state.hasReviews);
        }
    };
    SearchBox.prototype.handleInputChange = function (e) {
        this.setState({ query: e.target.value });
        console.log('QUERY Changed;');
    };
    SearchBox.prototype.handleCheckboxChange = function (e) {
        this.setState({ includeInactive: e.target.checked });
    };
    SearchBox.prototype.handleEnterKey = function (e) {
        if (e.which === 13) {
            this.onSearchRequested(null);
        }
    };
    SearchBox.prototype.handleHasReviewsClick = function (e) {
        this.setState({ hasReviews: !this.state.hasReviews });
    };
    SearchBox.prototype.render = function () {
        var labelStyle = {
            'color': '#f3f3f3',
            'userSelect': 'none'
        };
        return (React.createElement("div", { className: "search-wrap" },
            React.createElement("div", { className: "input-group input-group-lg" },
                React.createElement("input", { type: "text", className: "form-control search-box", autoFocus: true, value: this.state.query, onKeyUp: this.handleEnterKey.bind(this), onChange: this.handleInputChange.bind(this), placeholder: this.props.prompt }),
                React.createElement("div", { className: "input-group-btn" },
                    React.createElement("button", { className: "btn btn-default", "data-toggle": "dropdown", "aria-haspopup": "true", "aria-expanded": "false", type: "button" },
                        React.createElement("span", { className: "glyphicon glyphicon-cog" })),
                    React.createElement("ul", { className: "dropdown-menu" },
                        React.createElement("li", null,
                            React.createElement("a", { id: "miHasReviews", href: "#", onClick: this.handleHasReviewsClick.bind(this) },
                                React.createElement("span", { className: classNames('glyphicon', { 'glyphicon-check': this.state.hasReviews, 'glyphicon-unchecked': !this.state.hasReviews }) }),
                                "\u00A0\u03A7\u03C1\u03AE\u03C3\u03C4\u03B5\u03C2 \u03BC\u03B5 reviews"))),
                    React.createElement("button", { className: "btn btn-default", type: "button", onClick: this.onSearchRequested.bind(this) },
                        React.createElement("span", { className: "glyphicon glyphicon-search" }),
                        " Go!"))),
            React.createElement("div", { className: "search-spinner-wrap", style: { display: this.props.isBusy ? 'block' : 'none' } },
                React.createElement("div", { className: "search-spinner" },
                    React.createElement("div", { className: "color" }),
                    React.createElement("div", { className: "white" }))),
            React.createElement("div", { className: "checkbox" },
                React.createElement("label", { style: labelStyle },
                    React.createElement("input", { type: "checkbox", name: "includeInactive", checked: this.state.includeInactive, onChange: this.handleCheckboxChange.bind(this) }),
                    " \u039D\u03B1 \u03C3\u03C5\u03BC\u03C0\u03B5\u03C1\u03B9\u03BB\u03B7\u03C6\u03B8\u03BF\u03CD\u03BD \u03BF\u03B9 \u03BC\u03B7-\u03B5\u03BD\u03B5\u03C1\u03B3\u03AD\u03C2"))));
    };
    return SearchBox;
}(React.Component));
var LatencyApp = (function (_super) {
    __extends(LatencyApp, _super);
    /**
     *
     */
    function LatencyApp(props) {
        var _this = _super.call(this, props || { greeting: 'Latency here...' }) || this;
        _this.state = {};
        _this.checkLatency = _this.checkLatency.bind(_this);
        _this.checkLatency();
        return _this;
    }
    LatencyApp.prototype.checkLatency = function () {
        var me = this;
        $.ajax({
            url: '/Home/CheckLatency',
            method: 'POST',
            type: 'json',
        }).done(function (latency) {
            me.setState({ latency: latency, error: null });
        }).fail(function () {
            me.setState({ latency: null, error: 'Failed latency check' });
        }).always(function () {
            // re-check the latency after 5 seconds
            setTimeout(me.checkLatency, 5000);
        });
    };
    LatencyApp.prototype.render = function () {
        var hasError = !!this.state.error;
        return (React.createElement("div", { className: "panel panel-info" },
            React.createElement("div", { className: "panel-heading" },
                React.createElement("h3", { className: "panel-title" }, "Document DB Latency from Web Server location")),
            !hasError && React.createElement("div", { className: "panel-body" },
                "Current latency is: ",
                React.createElement("h1", { className: "name" },
                    this.state.latency || '-',
                    " ms")),
            hasError && React.createElement("div", { className: "panel-body" },
                "Current latency is: ",
                React.createElement("h1", { className: "name" }, "[Failed to check latency, retrying...]"))));
    };
    return LatencyApp;
}(React.Component));
//# sourceMappingURL=app.js.map