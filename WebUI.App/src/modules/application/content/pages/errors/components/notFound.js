import { connect } from 'react-redux';
import { componentDidMount } from 'react-lifecycle-decorators';

let NotFound = () => (
    <div>
        404 not found
    </div>
);

NotFound = componentDidMount(
    (props) => {
    }
)(NotFound);

export default connect(
    (state) => {
        return state;
    }
)(NotFound);




