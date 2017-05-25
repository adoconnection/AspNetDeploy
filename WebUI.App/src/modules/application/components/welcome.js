import { connect } from 'react-redux';
import { componentDidMount } from 'react-lifecycle-decorators';

let Welcome = () => (
    <div className="container">
        <h1>
            Welcome page
        </h1>
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


