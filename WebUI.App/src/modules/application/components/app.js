import { connect } from 'react-redux';
import { componentDidMount } from 'react-lifecycle-decorators';

import { TopPanel } from '../navigation/components';
import Main from './Main';

let App = () => (
    <div>
        <TopPanel />
        <Main />
    </div>
);

App = componentDidMount(
    (props) => {
    }
)(App);

export default connect(
    (state) => {
        return state;
    }
)(App);

