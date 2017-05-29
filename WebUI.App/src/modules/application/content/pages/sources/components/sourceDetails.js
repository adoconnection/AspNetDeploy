import { connect } from 'react-redux';
import { componentDidMount } from 'react-lifecycle-decorators';
import { Switch, Route } from 'react-router-dom';

import signalr from './../../../../../signalr';

import { sourceControlsDetails } from './../../../data';
import { sourceControlVersions } from './../../../data';

import localization from './../../../../localization';
import { tr } from './../../../../localization/constants';

let SourceDetails = ({ match, sourceControlsDetails, sourceControlVersions, locale }) => (
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
                    <hr/>
                    {sourceControlVersions.isLoading
                    ? <img className="list-element-loading" src="/Resources/Layout/Images/vs-loading-colored.gif"/>
                    : sourceControlVersions.data.versions ? sourceControlVersions.data.versions.map((v) => {
                        return <div key={v.id}>
                                 <h3>
                                     {v.Name ? v.Name : "No name"}
                                 </h3>
                                 <div className="row">
                                    <div className="col-sm-7">
                                        <dl className="dl-horizontal">
                                            <dt>URL</dt>
                                            <dd>
                                                {v.url}
                                            </dd>
                                            <dt>Revision</dt>
                                            <dd></dd>
                                        </dl>
                                    </div>
                                 </div>
                             </div>
                        }) : null
                    }
                </div>
            }
            
        } />
    </Switch>
);

SourceDetails = componentDidMount(
    (props) => {
        props.dispatch(sourceControlsDetails.actions.prepareLoading());
        props.dispatch(signalr.actions.send({name: signalr.commands.SOURCE_CONTROLS_DETAILS, data: {id: props.match.params.id}}));
        props.dispatch(sourceControlVersions.actions.prepareLoading());
        props.dispatch(signalr.actions.send({name: signalr.commands.SOURCE_CONTROL_VERSIONS_LIST, data: {sourceControlId: props.match.params.id}}));
    }
)(SourceDetails);

const mapStateToProps = (state) => {
    return {
        sourceControlsDetails: state[sourceControlsDetails.constants.NAME],
        sourceControlVersions: state[sourceControlVersions.constants.NAME],
        locale: state[localization.constants.NAME].shortName
    }
};

export default connect(mapStateToProps)(SourceDetails);

