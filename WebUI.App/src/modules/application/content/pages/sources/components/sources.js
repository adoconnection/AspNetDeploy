import { connect } from 'react-redux';
import { componentWillMount } from 'react-lifecycle-decorators';
import { Switch, Route, Redirect } from 'react-router-dom';

import signalr from './../../../../../signalr';
import { sourceControls } from './../../../data';

import AddSources from './addSources';
import ListSources from './listSources';
import SourceDetails from './sourceDetails';

let Sources = ({ match, sourceControls, dispatch }) => (
    <Switch>
        <Route exact path={match.url} render={ () =>
            {
                return sourceControls.isLoading
                ? <img className="pageLoading" src="/Resources/Layout/Images/vs-loading-colored.gif"/>
                : (sourceControls.data.lenght !=0
                    ?  <Redirect to={{
                        pathname: match.url + '/List'
                    }}/>
                    : <Redirect to={{
                        pathname: match.url + '/Add'
                    }}/>
                );
            }
        }/>
        <Route path={match.url + '/Add'} component={AddSources} />
        <Route path={match.url + '/List'} component={ListSources} />
        <Route path={match.url + '/Details/:id'} component={SourceDetails} />
    </Switch>
);

Sources = componentWillMount(
    (props) => {
        props.dispatch(sourceControls.actions.prepareLoading());
        props.dispatch(signalr.actions.send({name: signalr.commands.SOURCE_CONTROLS_LIST}));
    }
)(Sources);
    
const mapStateToProps = (state) => {
    return {
        sourceControls: state[sourceControls.constants.NAME]
    }
};

export default connect(mapStateToProps)(Sources);