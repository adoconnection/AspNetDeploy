import { connect } from 'react-redux';
import { componentWillMount } from 'react-lifecycle-decorators';
import { Switch, Route } from 'react-router-dom';

import signalr from './../../../../../signalr';

import { sourceControlsDetails } from './../../../data';

import localization from './../../../../localization';
import { tr } from './../../../../localization/constants';

let SourceDetails = ({ match, sourceControlsDetails, locale }) => (
    <Switch>
        <Route exact path={match.url} render={ () =>
            {
                return sourceControlsDetails.isLoading
                ? <img className="pageLoading" src="/Resources/Layout/Images/vs-loading-colored.gif"/>
                : <div className="container">
                    <h1>
                        <div className="row">
                            <div className="col-md-10">
                                {sourceControlsDetails.data.name}
                            </div>
                            <div className="col-md-2 text-right">
                                <button className="btn btn-default btn-lg">
                                    {tr[locale]["sources_edit"]}
                                </button>
                            </div>
                        </div>
                    </h1>
                    <hr/>
                    {sourceControlsDetails.data.type == 'Svn'
                    ? <dl className="dl-horizontal">
                        <dt>{tr[locale]["sources_url"]}</dt>
                        <dd>
                            {sourceControlsDetails.data.url}
                        </dd>
                        <dt>{tr[locale]["sources_login"]}</dt>
                        <dd>
                            {sourceControlsDetails.data.login}
                        </dd>
                    </dl>
                    : null}
                </div>
            }
            
        } />
    </Switch>
);

SourceDetails = componentWillMount(
    (props) => {
        props.dispatch(sourceControlsDetails.actions.prepareLoading());
        props.dispatch(signalr.actions.send({name: signalr.commands.SOURCE_CONTROLS_DETAILS, data: {id: props.match.params.id}}));
    }
)(SourceDetails);

const mapStateToProps = (state) => {
    return {
        sourceControlsDetails: state[sourceControlsDetails.constants.NAME],
        locale: state[localization.constants.NAME].shortName
    }
};

export default connect(mapStateToProps)(SourceDetails);

