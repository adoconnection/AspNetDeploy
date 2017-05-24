import { connect } from 'react-redux';
import { componentDidMount } from 'react-lifecycle-decorators';
import {Switch, Route} from 'react-router-dom';

import Welcome from './Welcome';
import Sources from '../content/pages/sources/components/Sources';
import NotFound from '../content/pages/errors/components/NotFound';

let Main = () => (
    <Switch>
        <Route exact path="/App" component={Welcome} />
        <Route path="/App/Sources" component={Sources} />
        <Route component={NotFound} />
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


