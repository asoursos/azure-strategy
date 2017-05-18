interface ILatencyAppProps {
    greeting?: string;
}

interface ILatencyAppState {
    latency?: number;
    error?: string;
}

class LatencyApp extends React.Component<ILatencyAppProps, ILatencyAppState> {
    /**
     *
     */
    constructor(props: ILatencyAppProps) {
        super(props || { greeting: 'Latency here...' });
        this.state = {};
        this.checkLatency = this.checkLatency.bind(this);
        this.checkLatency();
    }

    private checkLatency() {
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
    }

    public render(): JSX.Element {
        var hasError = !!this.state.error;

        return (
            <div className="panel panel-info">
                <div className="panel-heading">
                    <h3 className="panel-title">Document DB Latency from Web Server location</h3>
                </div>
                {!hasError && <div className="panel-body">Current latency is: <h1 className="name">{this.state.latency || '-'} ms</h1></div>}
                {hasError && <div className="panel-body">Current latency is: <h1 className="name">[Failed to check latency, retrying...]</h1></div>}
            </div>
        );
    }
}