import { connect } from 'react-redux';
import { componentDidMount } from 'react-lifecycle-decorators';
import { Switch, Route } from 'react-router-dom';

import { sourceControls } from './../../../data';

let SourcesAdd = ({ sourceControlTypes }) => (
    <div className="container">
        <h1>
            Add source
        </h1>
    </div>
);

SourcesAdd = componentDidMount(
    (props) => {
    }
)(SourcesAdd);

const mapStateToProps = (state) => {
    return {
        sourceControlTypes: state[sourceControls.constants.NAME].sourceControlTypes
    }
};

export default connect(mapStateToProps)(SourcesAdd);
