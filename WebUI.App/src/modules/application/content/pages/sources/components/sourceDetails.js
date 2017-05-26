import { connect } from 'react-redux';
import { componentWillMount } from 'react-lifecycle-decorators';
import { Switch, Route } from 'react-router-dom';

import signalr from './../../../../../signalr';

import { sourceControlsDetails } from './../../../data';

let SourceDetails = ({ match, sourceControlsDetails }) => (
    <Switch>
        <Route exact path={match.url} render={ () =>
            <div className="container">
                <h1>
                    <div className="row">
                        <div className="col-md-10">
                            {sourceControlsDetails.name}
                        </div>
                        <div className="col-md-2 text-right">
                            <button className="btn btn-default btn-lg">
                                Configure
                            </button>
                        </div>
                    </div>
                </h1>
                <hr/>
                {sourceControlsDetails.type == 'Svn'
                ? <dl className="dl-horizontal">
                    <dt>URL</dt>
                    <dd>
                        {sourceControlsDetails.url}
                    </dd>
                    <dt>Login</dt>
                    <dd>
                        {sourceControlsDetails.login}
                    </dd>
                </dl>
                : null}
            </div>
        } />
    </Switch>
);

SourceDetails = componentWillMount(
    (props) => {
        props.dispatch(signalr.actions.send({name: signalr.commands.SOURCE_CONTROLS_DETAILS, data: {id: props.match.params.id}}));
    }
)(SourceDetails);

const mapStateToProps = (state) => {
    return {
        sourceControlsDetails: state[sourceControlsDetails.constants.NAME]
    }
};

export default connect(mapStateToProps)(SourceDetails);

