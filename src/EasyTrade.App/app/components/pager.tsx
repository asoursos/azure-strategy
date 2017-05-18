/// <reference path="../../node_modules/@types/react/index.d.ts" />

interface IPagerProps {
    initialPageNumber?: Number;
    totalCount: number;
    pageSize: number;
    pageNumber: number;
    onPageChanged: Function;    
}

interface IPagerState {
}

class Pager extends React.Component<IPagerProps, IPagerState> {
    /**
     *
     */
    constructor(props: IPagerProps) {
        super(props)
    }

    private onPageClick(pageIndex) {
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
    }

    private range(start, count): Array<number> {

        var numbers = new Array<number>(count);
        for (var i = start; i < start + count; i++) {
            numbers.push(i);
        }

        return numbers;
    }

    private getPageCount(): number {
        var pageCount = Math.ceil(this.props.totalCount / this.props.pageSize);
        return pageCount;
    }

    private getPageTokens() : Array<string> {
        
        var pageCount = this.getPageCount();
        if (pageCount <= 10) {
            return this.range(1, pageCount).map(v => v.toString());
        }

        var pageNumber = this.props.pageNumber || 1;
        var pageTokens = new Array<string>();       
        pageTokens.push('1');

        for (var i = 2; i < pageCount; i++)
        {
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
    }

    public render(): JSX.Element {
        var pageCount = this.getPageCount();
        var pageLinks = this.getPageTokens().map((token, idx) => {
                var pageElement = token === '...'
                    ? <span>{token}</span>
                    : <a href="#" data-page={token} onClick={this.onPageClick.bind(this, token)}>{token}</a>;

                return (<li key={idx} className={parseInt(token) == this.props.pageNumber ? 'active' : null}>{pageElement}</li>);
            }
        );

        return (
            <nav aria-label="...">
                <ul className="pagination">
                    <li className={this.props.pageNumber === 1 ? 'disabled' : null}><a href="#" data-page="prev" aria-label="Previous" onClick={this.onPageClick.bind(this, 'prev')}><span aria-hidden="true">&laquo;</span></a></li>
                    {pageLinks}
                    <li className={this.props.pageNumber === pageCount ? 'disabled' : null}> <a href="#" data-page="next" aria-label="Previous" onClick={this.onPageClick.bind(this, 'next')}><span aria-hidden="true">&raquo;</span></a></li>
                </ul>
            </nav>
        );
    }
}