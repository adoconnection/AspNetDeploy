import { connect } from 'react-redux';
import { componentDidMount } from 'react-lifecycle-decorators';
import { Switch, Route } from 'react-router-dom';

let Sources = () => (
    <Switch>
        <Route exact path="/App/Sources" render={ () =>
            <div>
                No sources yet
            </div>
        } />
    </Switch>
);

Sources = componentDidMount(
    (props) => {
    }
)(Sources);

export default connect(
    (state) => {
        return state;
    }
)(Sources);



