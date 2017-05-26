import { connect } from 'react-redux';
import { componentDidMount } from 'react-lifecycle-decorators';
import { Switch, Route, Link } from 'react-router-dom';

import signalr from './../../../../../signalr';

import AddSVNForm from './../../../controls/components/forms/addSVNForm';

let AddSVN = ({ match, dispatch }) => (
    <Switch>
        <Route exact path={ match.url } render={ () =>
            <div className="container">
                <h1>
                    Add SVN source
                </h1>
                <hr/>
                <AddSVNForm onSubmit={ (values) => {
                    values.type = "svn";
                    
                    dispatch(signalr.actions.send({
                        name: signalr.commands.SOURCE_CONTROLS_ADD,
                        data: values
                    }));
                } } />
            </div>
        } />
    </Switch>
);

AddSVN = componentDidMount(
    (props) => {
    }
)(AddSVN);

const mapStateToProps = (state) => {
    return state;
};

export default connect(mapStateToProps)(AddSVN);

