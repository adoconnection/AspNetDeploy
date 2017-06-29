import { connect } from 'react-redux';
import { componentDidMount } from 'react-lifecycle-decorators';
import { Link } from 'react-router-dom';

import signalr from './../../../../../signalr';
import { sourceControls } from './../../../data';

let ListSourcesItem = ({ sourceControl }) => (
    <div>
        <h1 key={sourceControl.id}>
            <Link to={"/App/Sources/Details/" + sourceControl.id}>{sourceControl.name}</Link>
            <small>{sourceControl.type}</small>
        </h1>
        <hr/>
        <div className="row text-center">
            {
                sourceControl.isVersionsLoading
                ? <img className="list-element-loading" src="/Resources/Layout/Images/vs-loading-colored.gif"/>
                : sourceControl.versions ? sourceControl.versions.map((v) => {
                    return <div className="col-sm-6" key={v.id}>
                             <h3>
                                 {v.Name ? v.Name : "No name"}
                             </h3>
                         </div>
                }) : null
            }
        </div>
    </div>
);

ListSourcesItem = componentDidMount(
    (props) => {
        props.dispatch(sourceControls.actions.prepareLoadingVersions(props.sourceControl.id));
        props.dispatch(signalr.actions.send({name: signalr.commands.SOURCE_CONTROL_VERSIONS_LIST, data: {sourceControlId: props.sourceControl.id}}));
    }
)(ListSourcesItem);

const mapStateToProps = (state) => {
    return state;
};

export default connect(mapStateToProps)(ListSourcesItem);

