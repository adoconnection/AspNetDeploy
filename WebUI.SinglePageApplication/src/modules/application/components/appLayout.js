import {connect} from 'react-redux';
import {componentDidMount} from 'react-lifecycle-decorators';

let AppLayout = ({children}) => (
    <div>{children}</div>
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

