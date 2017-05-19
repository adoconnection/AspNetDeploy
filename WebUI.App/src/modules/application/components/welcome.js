import { connect } from 'react-redux';
import { componentDidMount } from 'react-lifecycle-decorators';

let Welcome = () => (
    <div>
        Some welcome information
    </div>
);

Welcome = componentDidMount(
    (props) => {
    }
)(Welcome);

export default connect(
    (state) => {
        return state;
    }
)(Welcome);



