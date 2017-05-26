import { connect } from 'react-redux';
import { componentDidMount } from 'react-lifecycle-decorators';
import { Switch, Route, Link } from 'react-router-dom';

import { sourceControls } from './../../../data';

let ListSources = ({ match, sourceControls }) => (
    <Switch>
        <Route exact path={match.url} render={ () =>
            <div className="container">
                { sourceControls.map( (sc) => {
                        return <h1 key={sc.id}>
                            <Link to={"/App/Sources/Details/" + sc.id}>{sc.name}</Link>
                            <small>{sc.type}</small>
                        </h1>
                })}
            </div>
        } />
    </Switch>
);

ListSources = componentDidMount(
    (props) => {
    }
)(ListSources);

const mapStateToProps = (state) => {
    return {
        sourceControls: state[sourceControls.constants.NAME]
    }
};

export default connect(mapStateToProps)(ListSources);
