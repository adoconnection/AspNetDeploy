import {connect} from 'react-redux';
import {componentDidMount} from 'react-lifecycle-decorators';

let SourceControlList = () => (
    <div>this is source control list</div>
);

SourceControlList = componentDidMount(
    (props) => {
    }
)(SourceControlList);

export default connect(
    (state) => {
        return state;
    }
)(SourceControlList);


