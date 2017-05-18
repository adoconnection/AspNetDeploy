import {connect} from 'react-redux';
import {componentDidMount} from 'react-lifecycle-decorators';

import {TopPanel} from '../navigation/components';

let AppLayout = ({children}) => (
    <div>
        <TopPanel/>
        <div>{children}</div>
    </div>
);

AppLayout = componentDidMount(
    (props) => {
    }
)(AppLayout);

export default connect(
    (state) => {
        return state;
    }
)(AppLayout);

