import { connect } from 'react-redux';
import { componentDidMount } from 'react-lifecycle-decorators';
import { Switch, Route, Redirect } from 'react-router-dom';

import { sourceControls } from './../../../data';
import SourcesAdd from './sourcesAdd';

let Sources = ({ match, sourceControls }) => (
    <Switch>
        <Route exact path={match.url} render={ () =>
            sourceControls.length != 0
                ? null
                : <Redirect to={{
                    pathname: match.url + '/Add'
                }}/>
        } />
        <Route path={match.url + '/Add'} component={SourcesAdd} />
    </Switch>
);

Sources = componentDidMount(
    (props) => {
    }
)(Sources);
    
const mapStateToProps = (state) => {
    return {
        sourceControls: state[sourceControls.constants.NAME].sourceControls
    }
};

export default connect(mapStateToProps)(Sources);