import { connect } from 'react-redux';
import { componentDidMount } from 'react-lifecycle-decorators';
import {Switch, Route} from 'react-router-dom';

import Welcome from './Welcome';
import Sources from './../content/sources/components/Sources';

let Main = () => (
    <Switch>
        <Route exact path="/App" component={Welcome} />
        <Route path="/App/Sources" conponent={Sources} />
    </Switch>
);

Main = componentDidMount(
    (props) => {
    }
)(Main);

export default connect(
    (state) => {
        return state;
    }
)(Main);


