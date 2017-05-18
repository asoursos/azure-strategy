/// <reference path="../../node_modules/@types/react/index.d.ts" />

interface ISearchBoxProps {
    isBusy?: boolean;
    prompt?: string;
    onSearch?: Function;
    onInputChanged?: Function;
}

interface ISearchBoxState {
    query?: string,
    includeInactive?: boolean,
    hasReviews?: boolean
}

class SearchBox extends React.Component<ISearchBoxProps, ISearchBoxState> {

    constructor(props: ISearchBoxProps) {
        super(props)
        this.state = { query: '', includeInactive: false, hasReviews: false };
    }  

    private onSearchRequested(e: React.MouseEvent<HTMLButtonElement>) {
        console.log('SEARCH REQUESTED;');
        if (this.props.onSearch) {
            this.props.onSearch(this.state.query, this.state.includeInactive, this.state.hasReviews);
        }
    }

    private handleInputChange(e: React.ChangeEvent<HTMLInputElement>) {
        this.setState({ query: e.target.value });
        console.log('QUERY Changed;')
    }

    private handleCheckboxChange(e: React.ChangeEvent<HTMLInputElement>) {
        this.setState({ includeInactive: e.target.checked });
    }

    private handleEnterKey(e: React.KeyboardEvent<HTMLInputElement>) {
        if (e.which === 13) {
            this.onSearchRequested(null);
        }
    }

    private handleHasReviewsClick(e: React.MouseEvent<HTMLAnchorElement>) {
        this.setState({ hasReviews: !this.state.hasReviews });
    }

    public render(): JSX.Element {

        const labelStyle: React.CSSProperties = {
            'color': '#f3f3f3',
            'userSelect': 'none'
        };

        return (
            <div className="search-wrap">
                <div className="input-group input-group-lg">
                    <input type="text" className="form-control search-box" autoFocus
                        value={this.state.query} 
                        onKeyUp={this.handleEnterKey.bind(this)}
                        onChange={this.handleInputChange.bind(this)} 
                        placeholder={this.props.prompt} />
                    <div className="input-group-btn">
                        <button className="btn btn-default" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false" type="button"><span className="glyphicon glyphicon-cog"></span></button>
                        <ul className="dropdown-menu">
                            <li><a id="miHasReviews" href="#" onClick={this.handleHasReviewsClick.bind(this)}><span className={classNames('glyphicon', {'glyphicon-check' : this.state.hasReviews, 'glyphicon-unchecked': !this.state.hasReviews })}></span>&nbsp;Χρήστες με reviews</a></li>
                        </ul>
                        <button className="btn btn-default" type="button" onClick={this.onSearchRequested.bind(this)}><span className="glyphicon glyphicon-search"></span> Go!</button>
                    </div>
                </div>
                <div className="search-spinner-wrap" style={{display: this.props.isBusy ? 'block' : 'none'}}>
                    <div className="search-spinner">
                        <div className="color"></div>
                        <div className="white"></div>
                    </div>
                </div>
                <div className="checkbox">
                    <label style={labelStyle}><input type="checkbox" name="includeInactive"
                        checked={this.state.includeInactive}
                        onChange={this.handleCheckboxChange.bind(this)} /> Να συμπεριληφθούν οι μη-ενεργές</label>
                </div>
            </div>
        );
    }
}